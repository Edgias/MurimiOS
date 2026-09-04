using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static partial class RefactorTool
{
    static void VerifyDomain()
    {
        var hashes = JsonSerializer.Deserialize<Dictionary<string,string>>(File.ReadAllText(Full("output/refactoring/domain-completion/hashes.json")))!;
        var migrations = hashes.Where(p => p.Key.Contains("/Migrations/")).ToArray();
        foreach (var pair in migrations)
            if (!File.Exists(Full(pair.Key)) || Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Full(pair.Key)))) != pair.Value)
                throw new InvalidOperationException("Migration changed: " + pair.Key);
        using var zip = ZipFile.OpenRead(Full("output/refactoring/domain-completion/sources.zip"));
        var moves = JsonSerializer.Deserialize<List<DeclarationMove>>(File.ReadAllText(Full("output/refactoring/domain-completion/moves.json")))!;
        foreach (var move in moves)
        {
            using var reader = new StreamReader(zip.GetEntry(move.Source)!.Open());
            var original = TopTypes(Parse(reader.ReadToEnd())).Single(t => TypeName(t) == move.Name);
            var current = TopTypes(Parse(File.ReadAllText(Full(move.Target)))).Single(t => TypeName(t) == move.Name);
            string Tokens(SyntaxNode node) => string.Join(" ",node.DescendantTokens().Select(t => t.Text));
            if (Tokens(original) != Tokens(current)) throw new InvalidOperationException("Declaration changed: " + move.Target);
        }
        Console.WriteLine($"Verified {moves.Count} unchanged declarations and {migrations.Length} byte-identical migrations.");
    }

    sealed record DeclarationMove(string Source, string Target, string Name);
    static void RelocateDeclarations(List<DeclarationMove> changes)
    {
        var allTypes = Sources().SelectMany(f => TopTypes(Parse(File.ReadAllText(f))).Select(t => (Ns: Namespace(t), Name: TypeName(t)))).Distinct().ToArray();
        var map = new Dictionary<string,string>();
        foreach (var group in changes.GroupBy(c => c.Source))
        {
            var root = Parse(File.ReadAllText(Full(group.Key)));
            var ns = root.Members.OfType<BaseNamespaceDeclarationSyntax>().Single();
            var selected = group.Select(c => c.Name).ToHashSet();
            var remaining = ns.Members.Where(m => !selected.Contains(TypeName(m))).ToArray();
            foreach (var change in group)
            {
                var member = ns.Members.Single(m => TypeName(m) == change.Name);
                var newRoot = root.ReplaceNode(ns, ns.WithMembers(SyntaxFactory.SingletonList(member)));
                Directory.CreateDirectory(Path.GetDirectoryName(Full(change.Target))!);
                if (File.Exists(Full(change.Target))) throw new InvalidOperationException("Target exists: " + change.Target);
                if (remaining.Length == 0 && change == group.First()) Move(group.Key,change.Target);
                File.WriteAllText(Full(change.Target),newRoot.ToFullString());
                map[Namespace(member)+"."+change.Name] = NewNamespace(change.Target)+"."+change.Name;
            }
            if (remaining.Length > 0) File.WriteAllText(Full(group.Key),root.ReplaceNode(ns,ns.WithMembers(SyntaxFactory.List(remaining))).ToFullString());
        }
        foreach (string file in Sources().ToArray())
        {
            var move = changes.FirstOrDefault(c => c.Target == Relative(file));
            string before = File.ReadAllText(file);
            string after = RewriteNamespaces(before, move == null ? null : NewNamespace(move.Target), map, allTypes);
            // Former siblings also need the relocated namespace even without an explicit using.
            var parsed = Parse(after);
            string originalNs = Parse(before).Members.OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString() ?? "";
            var additional = allTypes.Where(t => t.Ns == originalNs && map.ContainsKey(t.Ns+"."+t.Name))
                .Select(t => map[t.Ns+"."+t.Name]).Select(t => t[..t.LastIndexOf('.')]).Distinct()
                .Where(n => n != (move == null ? originalNs : NewNamespace(move.Target)))
                .Where(n => !parsed.Usings.Any(u => u.Name?.ToString() == n));
            after = string.Join("",additional.Select(n => "using " + n + ";\n")) + after;
            if (move?.Target.StartsWith("src/Application/") == true)
            {
                // A port carries no adapter implementation dependency. Retain only imports
                // needed by its contract and move any referenced DTOs explicitly with it.
                var clean = Parse(after);
                after = clean.RemoveNodes(clean.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u => u.Name?.ToString().StartsWith("Edgias.MurimiOS.Infrastructure") == true || u.Name?.ToString().StartsWith("Minio") == true),SyntaxRemoveOptions.KeepNoTrivia)!.ToFullString();
            }
            if (before != after) File.WriteAllText(file,after);
        }
        DeduplicateImports();
    }

    static void ApplicationPorts()
    {
        Backup("application-ports");
        var moves = new List<DeclarationMove>();
        void Add(string source,string folder,params string[] names) { foreach (string name in names) moves.Add(new(source,folder+"/"+name+".cs",name)); }
        Add("Infrastructure/Platform/Messaging/OutboxMessage.cs","src/Domain/OutboxMessage","OutboxMessage");
        Add("Infrastructure/Platform/Sync/SyncChange.cs","src/Domain/SyncClient","SyncChange");
        Add("Infrastructure/Platform/Sync/SyncCommandLog.cs","src/Domain/SyncClient","SyncCommandLog");
        Add("Infrastructure/Platform/SupplyNetwork/GrantedSupplyProjections.cs","src/Application/Common/Abstractions","GrantedHarvestLotProjection","GrantedTraceLinkProjection");
        Add("API/Access/RequestActorContext.cs","src/Application/Common/Abstractions","IRequestActorContext");
        Add("API/Access/OrganisationScopeAccess.cs","src/Application/Common/Abstractions","ResolvedOrganisationScope");
        Add("Infrastructure/Platform/Media/LocalMediaStorage.cs","src/Application/Common/Abstractions","IMediaStorage","MediaInspection","MediaOffsetMismatchException","MediaChunkSizeException");
        Add("Infrastructure/Platform/Media/MediaMalwareScanning.cs","src/Application/Common/Abstractions","IMediaMalwareScanner","MediaScanVerdict","MediaScanResult");
        RelocateDeclarations(moves);
        File.WriteAllText(Full("output/refactoring/application-ports/moves.json"),JsonSerializer.Serialize(moves,Json));
        CreateDbPort("Infrastructure/Platform/PlatformDbContext.cs","IPlatformDbContext");
        CreateDbPort("Infrastructure/Data/MurimiOSDbContext.cs","ILegacyDbContext");
        Console.WriteLine($"Relocated {moves.Count} contracts and persistence records; created database ports.");
    }

    static void CreateDbPort(string path,string interfaceName)
    {
        var root = Parse(File.ReadAllText(Full(path)));
        var type = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
        string imports = string.Join("\n",root.Usings.Where(u => !u.Name!.ToString().Contains("Infrastructure")).Select(u=>u.ToString()));
        if (interfaceName == "ILegacyDbContext")
            imports += "\n" + string.Join("\n",Parse(File.ReadAllText(Full("API/GlobalUsings.cs"))).Usings.Where(u => !u.Name!.ToString().Contains("Infrastructure")).Select(u=>u.WithGlobalKeyword(default).ToString()));
        string properties = string.Join("\n",type.Members.OfType<PropertyDeclarationSyntax>().Where(p=>p.Type.ToString().StartsWith("DbSet<")).Select(p=>$"    {p.Type} {p.Identifier} {{ get; }}"));
        string content = imports + "\nusing Microsoft.EntityFrameworkCore;\nusing Microsoft.EntityFrameworkCore.Infrastructure;\nusing Microsoft.EntityFrameworkCore.ChangeTracking;\nnamespace EdgIQ.MurimiOS.Application.Common.Abstractions;\n\n"
            + $"public interface {interfaceName}\n{{\n{properties}\n"
            + "    DatabaseFacade Database { get; }\n    ChangeTracker ChangeTracker { get; }\n    DbSet<TEntity> Set<TEntity>() where TEntity : class;\n    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;\n    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);\n}\n";
        File.WriteAllText(Full("src/Application/Common/Abstractions/"+interfaceName+".cs"),content);
        var newType = type.WithBaseList(type.BaseList!.AddTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("EdgIQ.MurimiOS.Application.Common.Abstractions."+interfaceName))));
        File.WriteAllText(Full(path),root.ReplaceNode(type,newType).ToFullString());
        DeduplicateImports();
    }

    static void ApplicationServices()
    {
        Backup("application-services");
        var moves = new List<DeclarationMove>();
        void Add(string file,string folder,Func<MemberDeclarationSyntax,bool>? filter = null)
        {
            moves.AddRange(TopTypes(Parse(File.ReadAllText(Full(file)))).Where(t=>filter?.Invoke(t) ?? true)
                .Select(t=>new DeclarationMove(file,folder+"/"+TypeName(t)+".cs",TypeName(t))));
        }
        Add("Infrastructure/Platform/Integrations/ProviderAdapterContracts.cs","src/Application/Common/Abstractions",t=>TypeName(t)!="ProviderJson");
        Add("Infrastructure/Platform/Compliance/ComplianceStandardPacks.cs","src/Application/Common/Compliance");
        Add("Infrastructure/Platform/Compliance/ComplianceEvaluationEngine.cs","src/Application/Common/Compliance");
        Add("Infrastructure/Platform/Finance/EnterpriseConsolidationEngine.cs","src/Application/Common/Finance");
        Add("Infrastructure/Platform/Imports/GeospatialImportParser.cs","src/Application/Common/Imports");
        Add("Infrastructure/Platform/Imports/MasterDataCsv.cs","src/Application/Common/Imports");
        Add("Infrastructure/Platform/SupplyNetwork/SupplierCompletion.cs","src/Application/Common/SupplyNetwork");
        Add("Infrastructure/Platform/Traceability/EpcisInterop.cs","src/Application/Common/Traceability");
        Add("Infrastructure/Platform/Observability/PlatformDiagnostics.cs","src/Application/Common/Observability");
        Add("API/Reporting/ProductionReportPdfBuilder.cs","src/Application/Common/Reporting");
        Add("Infrastructure/Platform/Enterprise/TenantDatabaseRouting.cs","src/Application/Common/Abstractions",t=>TypeName(t)=="TenantDatabaseRoutingScope");
        Add("Infrastructure/Platform/Livestock/IcarAdeExchangeAdapter.cs","src/Application/Common/Abstractions",t=>t is RecordDeclarationSyntax);
        Add("Infrastructure/Platform/Livestock/ScotEidMovementAdapter.cs","src/Application/Common/Abstractions",t=>t is RecordDeclarationSyntax);
        Add("Infrastructure/Platform/Compliance/ComplianceAuditPackGenerator.cs","src/Application/Common/Abstractions",t=>TypeName(t) is "ComplianceAuditPackOptions" or "ComplianceAuditPackArtifact");
        Add("Infrastructure/Platform/Security/RequestQuotaStore.cs","src/Application/Common/Abstractions",t=>TypeName(t)=="RequestQuotaDecision");
        RelocateDeclarations(moves);
        foreach (var move in moves)
        {
            string text=File.ReadAllText(Full(move.Target));
            text=Regex.Replace(text,@"\bPlatformDbContext\b","IPlatformDbContext");
            if(text.Contains("IPlatformDbContext")&&!text.Contains("using EdgIQ.MurimiOS.Application.Common.Abstractions;"))
                text="using EdgIQ.MurimiOS.Application.Common.Abstractions;\n"+text;
            File.WriteAllText(Full(move.Target),text);
        }
        CreateServicePort("Infrastructure/Platform/Livestock/IcarAdeExchangeAdapter.cs","IcarAdeExchangeAdapter");
        CreateServicePort("Infrastructure/Platform/Livestock/ScotEidMovementAdapter.cs","ScotEidMovementAdapter");
        CreateServicePort("Infrastructure/Platform/Compliance/ComplianceAuditPackGenerator.cs","ComplianceAuditPackGenerator");
        CreateServicePort("Infrastructure/Platform/Compliance/ComplianceAuditPackGenerator.cs","ComplianceAuditPackSignatureService");
        CreateServicePort("Infrastructure/Platform/Security/RequestQuotaStore.cs","RequestQuotaStore");
        CreateServicePort("Infrastructure/Platform/Enterprise/TenantDatabaseRouting.cs","TenantDatabaseRegistry",["DatabaseKeys","Contains","ControlDatabaseKey"]);
        CreateServicePort("Infrastructure/Platform/Enterprise/TenantDatabaseRouting.cs","TenantDatabaseRouteResolver",["Invalidate","IsWriteBlockedAsync"]);
        File.WriteAllText(Full("output/refactoring/application-services/moves.json"),JsonSerializer.Serialize(moves,Json));
        DeduplicateImports();
        Console.WriteLine($"Relocated {moves.Count} application utility/contract declarations and introduced seven adapter ports.");
    }

    static void CreateServicePort(string path,string className,string[]? members = null)
    {
        var root=Parse(File.ReadAllText(Full(path)));
        var type=root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single(t=>t.Identifier.ValueText==className);
        var declarations=new List<MemberDeclarationSyntax>();
        foreach(var member in type.Members.Where(m=>m.Modifiers.Any(SyntaxKind.PublicKeyword)&&!m.Modifiers.Any(SyntaxKind.StaticKeyword)))
        {
            string name=member switch { MethodDeclarationSyntax m=>m.Identifier.ValueText,PropertyDeclarationSyntax p=>p.Identifier.ValueText,_=>""};
            if(members!=null&&!members.Contains(name))continue;
            if(member is MethodDeclarationSyntax method)
                declarations.Add(method.WithModifiers(default).WithBody(null).WithExpressionBody(null).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            if(member is PropertyDeclarationSyntax property)
                declarations.Add(property.WithModifiers(default).WithExpressionBody(null).WithInitializer(null).WithSemicolonToken(default)
                    .WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.SingletonList(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))))));
        }
        string imports=string.Join("\n",root.Usings.Where(u=>!u.Name!.ToString().StartsWith("Edgias.MurimiOS.Infrastructure")&&!u.Name.ToString().StartsWith("Npgsql")).Select(u=>u.ToString()));
        var iface=SyntaxFactory.InterfaceDeclaration("I"+className).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword)).WithMembers(SyntaxFactory.List(declarations));
        File.WriteAllText(Full("src/Application/Common/Abstractions/I"+className+".cs"),imports+"\nnamespace EdgIQ.MurimiOS.Application.Common.Abstractions;\n"+iface.NormalizeWhitespace().ToFullString()+"\n");
        var baseList=type.BaseList??SyntaxFactory.BaseList();
        var changed=type.WithBaseList(baseList.AddTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("EdgIQ.MurimiOS.Application.Common.Abstractions.I"+className))));
        File.WriteAllText(Full(path),root.ReplaceNode(type,changed).ToFullString());
    }
}
