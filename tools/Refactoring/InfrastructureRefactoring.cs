using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static partial class RefactorTool
{
    static void RegroupInfrastructure(bool resume=false)
    {
        if(!resume)
        {
            Backup("infrastructure");
            SplitPlatformConfigurations();
        }
        var moves=new Dictionary<string,string>();
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("Infrastructure/")).ToArray())
        {
            string rel=Relative(file),tail=rel["Infrastructure/".Length..];
            tail=tail.Replace("Platform/PlatformDbContext.cs","Data/PlatformDbContext.cs")
                .Replace("Platform/DesignTimePlatformDbContextFactory.cs","Data/DesignTimePlatformDbContextFactory.cs")
                .Replace("Platform/TenantConnectionInterceptor.cs","Data/Interceptors/TenantConnectionInterceptor.cs")
                .Replace("Platform/Auditing/","Data/Interceptors/")
                .Replace("Platform/Media/","Storage/").Replace("Platform/Messaging/","BackgroundJobs/Messaging/")
                .Replace("Platform/Integrations/","Integrations/").Replace("Platform/Enterprise/","Data/Enterprise/")
                .Replace("Platform/Security/","Identity/").Replace("Platform/Observability/","Observability/")
                .Replace("Platform/Analytics/","BackgroundJobs/Analytics/").Replace("Platform/Livestock/","Integrations/Livestock/")
                .Replace("Platform/Compliance/","Storage/Compliance/").Replace("Services/","BusinessServices/");
            if(tail=="Dependencies.cs")tail="DependencyInjection/Dependencies.cs";
            if(tail.StartsWith("Data/Config/")&&tail.Count(c=>c=='/')==2)
            {
                string name=Path.GetFileNameWithoutExtension(tail).Replace("Config","");
                string owner=name=="BaseEntity"?"Common":name=="Bin"?"Bins":name;
                tail="Data/Config/"+owner+"/"+Path.GetFileName(tail);
            }
            moves[rel]="src/Infrastructure/"+tail;
        }
        moves["API/Access/OrganisationScopeAccess.cs"]="src/Infrastructure/Identity/OrganisationScopeAccess.cs";
        moves["API/Platform/IntegrationCredentialProtector.cs"]="src/Infrastructure/Identity/IntegrationCredentialProtector.cs";
        MoveSourceFiles(moves);
        Move("Infrastructure/Infrastructure.csproj","src/Infrastructure/Infrastructure.csproj");
        string project=Full("src/Infrastructure/Infrastructure.csproj");
        File.WriteAllText(project,File.ReadAllText(project).Replace("..\\src\\Application\\Application.csproj", "..\\Application\\Application.csproj")
            .Replace("</Project>","  <ItemGroup>\n    <Compile Include=\"..\\..\\Infrastructure\\Platform\\Migrations\\**\\*.cs\" Link=\"Data\\Migrations\\%(RecursiveDir)%(Filename)%(Extension)\" />\n  </ItemGroup>\n</Project>"));
        // Historical migration source stays physically and byte-for-byte unchanged.
        File.WriteAllText(Full("src/Infrastructure/MigrationContextAlias.cs"),"global using PlatformDbContext = EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;\n");
        foreach(string file in Directory.EnumerateFiles(Root,"*.csproj",SearchOption.AllDirectories).Where(f=>!Regex.IsMatch(Relative(f),@"/(bin|obj)/",RegexOptions.IgnoreCase)))
        {
            if(file==project)continue;
            string before=File.ReadAllText(file);
            string after=before.Replace("..\\..\\Infrastructure\\Infrastructure.csproj","..\\..\\src\\Infrastructure\\Infrastructure.csproj")
                .Replace("..\\Infrastructure\\Infrastructure.csproj","..\\src\\Infrastructure\\Infrastructure.csproj");
            if(after!=before)File.WriteAllText(file,after);
        }
        string solution=Full("MurimiOS.sln");
        File.WriteAllText(solution,File.ReadAllText(solution).Replace("\"Infrastructure\\Infrastructure.csproj\"","\"src\\Infrastructure\\Infrastructure.csproj\""));
        // Middleware keeps host responsibilities, but receives Application ports.
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("API/Tenancy/")))
        {
            string text=File.ReadAllText(file);
            foreach(string name in new[]{"PlatformDbContext","RequestQuotaStore","TenantDatabaseRouteResolver"})
                text=Regex.Replace(text,@"\b"+name+@"\b",Ports[name]);
            var root=Parse(text);
            text=root.RemoveNodes(root.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u=>u.Name!.ToString().StartsWith("EdgIQ.MurimiOS.Infrastructure")),SyntaxRemoveOptions.KeepNoTrivia)!.ToFullString();
            File.WriteAllText(file,text);
        }
        DeduplicateImports();
        File.WriteAllText(Full("output/refactoring/infrastructure/moves.json"),JsonSerializer.Serialize(moves,Json));
        Console.WriteLine($"Moved {moves.Count} Infrastructure source files; migrations remain in place.");
    }

    static void MoveSourceFiles(Dictionary<string,string> moves)
    {
        var types=Sources().SelectMany(f=>TopTypes(Parse(File.ReadAllText(f))).Select(t=>(Ns:Namespace(t),Name:TypeName(t)))).Distinct().ToArray();
        var map=moves.SelectMany(p=>TopTypes(Parse(File.ReadAllText(Full(p.Key)))).Select(t=>new{Before=Namespace(t)+"."+TypeName(t),After=NewNamespace(p.Value)+"."+TypeName(t)}))
            .ToDictionary(t=>t.Before,t=>t.After);
        var oldNamespaces=moves.ToDictionary(p=>p.Value,p=>Parse(File.ReadAllText(Full(p.Key))).Members.OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString());
        var affectedNamespaces=map.Keys.Select(k=>k[..k.LastIndexOf('.')]).Where(n=>n.Length>0).Distinct().ToArray();
        foreach(var move in moves)Move(move.Key,move.Value);
        foreach(string file in Sources().ToArray())
        {
            string rel=Relative(file),before=File.ReadAllText(file);
            string? newNs=moves.ContainsValue(rel)?NewNamespace(rel):null;
            if(newNs==null&&!affectedNamespaces.Any(n=>before.Contains(n,StringComparison.Ordinal)))continue;
            string after=RewriteNamespaces(before,newNs,map,types);
            if(newNs!=null&&oldNamespaces[rel] is string oldNs)
            {
                var tokens=Names(Parse(before));
                var siblings=types.Where(t=>t.Ns==oldNs&&tokens.Contains(t.Name))
                    .Select(t=>map.GetValueOrDefault(t.Ns+"."+t.Name,t.Ns+"."+t.Name)).Select(t=>t[..t.LastIndexOf('.')]).Distinct().Where(n=>n!=newNs);
                after=string.Concat(siblings.Select(n=>"using "+n+";\n"))+after;
            }
            if(before!=after)File.WriteAllText(file,after);
        }
    }

    static void SplitPlatformConfigurations()
    {
        string path=Full("Infrastructure/Platform/PlatformDbContext.cs"),text=File.ReadAllText(path);
        var root=Parse(text);var owner=root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
        var methods=owner.Members.OfType<MethodDeclarationSyntax>().Where(m=>m.Identifier.ValueText.StartsWith("Configure")&&m.ParameterList.Parameters.Count==1&&m.ParameterList.Parameters[0].Type!.ToString()=="ModelBuilder").ToArray();
        var domains=Sources().Where(f=>Relative(f).StartsWith("src/Domain/")).SelectMany(f=>TopTypes(Parse(File.ReadAllText(f))).Select(t=>(Name:TypeName(t),Owner:Relative(f).Split('/')[2]))).ToArray();
        var edits=new List<(int Start,int Length,string Text)>(); int count=0;
        string imports=string.Join("\n",root.Usings.Select(u=>u.ToString()));
        foreach(var method in methods)
        {
            var statements=method.Body!.Statements;
            var groups=new List<(string Type,List<StatementSyntax> Statements)>();
            foreach(var statement in statements)
            {
                var entity=statement is LocalDeclarationStatementSyntax local
                    ?local.Declaration.Variables.Single().Initializer?.Value.DescendantNodesAndSelf().OfType<GenericNameSyntax>().FirstOrDefault(n=>n.Identifier.ValueText=="Entity") : null;
                if(entity!=null)groups.Add((entity.TypeArgumentList.Arguments[0].ToString(),new()));
                if(groups.Count==0)throw new InvalidOperationException("Unclassified mapping statement: "+statement);
                groups[^1].Statements.Add(statement);
            }
            var calls=new List<string>();
            foreach(var group in groups)
            {
                string type=group.Type;
                string entityName=type=="PlatformLivestockTreatment"?"LivestockTreatment":type;
                string aggregate=domains.FirstOrDefault(d=>d.Name==entityName).Owner??"Projections";
                string className=type+"Configuration",folder="Infrastructure/Data/Config/"+aggregate;
                string ns="EdgIQ.MurimiOS.Infrastructure.Data.Config."+aggregate;
                string body=string.Join("\n",group.Statements.Select(s=>s.ToFullString()));
                body=Regex.Replace(body,@"\bConfigureTenantEntity\(","context.ConfigureTenantEntity(")
                    .Replace("ConfigureOperationalCostRelationships(","PlatformDbContext.ConfigureOperationalCostRelationships(");
                // Construct the filter expression in the DbContext itself, never in a cached configuration instance.
                body=Regex.Replace(body,@"(\w+)\.HasQueryFilter\(x => x.TenantId == tenantContext.TenantId\);",m=>"context.Configure"+entityName+"Filter("+m.Groups[1].Value+");");
                Directory.CreateDirectory(Full(folder));
                File.WriteAllText(Full(folder+"/"+className+".cs"),imports+"\nnamespace "+ns+";\ninternal static class "+className+"\n{\n    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)\n    {\n"+body+"\n    }\n}\n");
                calls.Add("        global::"+ns+"."+className+".Configure(modelBuilder, this);");count++;
            }
            edits.Add((method.FullSpan.Start,method.FullSpan.Length,""));
            var call=owner.DescendantNodes().OfType<ExpressionStatementSyntax>().Single(s=>s.Expression.ToString()==method.Identifier+"(modelBuilder)");
            edits.Add((call.FullSpan.Start,call.FullSpan.Length,string.Join("\n",calls)+"\n"));
        }
        foreach(var edit in edits.OrderByDescending(e=>e.Start))text=text.Remove(edit.Start,edit.Length).Insert(edit.Start,edit.Text);
        text=text.Replace("private void ConfigureTenantEntity", "internal void ConfigureTenantEntity")
            .Replace("private static void ConfigureOperationalCostRelationships", "internal static void ConfigureOperationalCostRelationships");
        string filters="";
        foreach(string type in new[]{"AuditEvent","SyncChange","OutboxMessage"})
            filters+="\n    internal void Configure"+type+"Filter(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<"+type+"> builder)\n        => builder.HasQueryFilter(x => x.TenantId == tenantContext.TenantId);\n";
        text=text.Insert(text.LastIndexOf('}'),filters);
        File.WriteAllText(path,text);
        Console.WriteLine($"Extracted {count} explicit entity configurations in their original order.");
    }

    static void SplitApiRegistration()
    {
        string path=Full("API/Program.cs"),text=File.ReadAllText(path);
        string imports=string.Join("\n",Parse(text).Usings.Where(u=>!u.Name!.ToString().Contains(".API.")).Select(u=>u.ToString()))
            +"\nusing Microsoft.Extensions.DependencyInjection;\nusing Microsoft.Extensions.Configuration;\nusing Microsoft.Extensions.Hosting;\n";
        void Extract(string concern,string start,string end,string parameters,string arguments,string returnType="IServiceCollection",string result="services")
        {
            int first=text.IndexOf(start,StringComparison.Ordinal),last=text.IndexOf(end,first,StringComparison.Ordinal);
            if(first<0||last<0)throw new InvalidOperationException("Registration block not found: "+concern);
            string body=text[first..last].Replace("builder.Services","services").Replace("builder.Configuration","configuration").Replace("builder.Environment","environment");
            string name="AddMurimiOS"+concern;
            string file=Full("src/Infrastructure/DependencyInjection/"+concern+"ServiceCollectionExtensions.cs");
            File.WriteAllText(file,imports+"\nnamespace EdgIQ.MurimiOS.Infrastructure.DependencyInjection;\npublic static class "+concern+"ServiceCollectionExtensions\n{\n    public static "+returnType+" "+name+"(this IServiceCollection services"+parameters+")\n    {\n"+body+"\n        return "+result+";\n    }\n}\n");
            string call="builder.Services."+name+"("+arguments+");\n";
            if(returnType!="IServiceCollection")call="var (scannerProvider, clamAvOptions) = "+call;
            text=text[..first]+call+text[last..];
        }
        Extract("QuotaProtection","builder.Services.AddOptions<RequestQuotaOptions>()","builder.Services.AddSingleton<GeospatialImportParser>();",", IConfiguration configuration","builder.Configuration");
        Extract("Compliance","builder.Services.AddSingleton<ComplianceStandardPackCatalog>();","string configuredMediaRoot",", IConfiguration configuration","builder.Configuration");
        Extract("MediaStorage","string configuredMediaRoot","builder.Services.AddHttpClient<ScotEidMovementAdapter>",", IConfiguration configuration, IHostEnvironment environment","builder.Configuration, builder.Environment","(string scannerProvider, ClamAvScannerOptions clamAvOptions)","(scannerProvider, clamAvOptions)");
        Extract("LivestockAdapters","builder.Services.AddHttpClient<ScotEidMovementAdapter>","string platformConnection","","");
        Extract("TenantRouting","builder.Services.AddOptions<TenantDatabaseOptions>()","var rabbitOptions",", IConfiguration configuration, string platformConnection","builder.Configuration, platformConnection");
        File.WriteAllText(path,text);
        string registration=Full("src/Infrastructure/DependencyInjection/ApplicationServiceCollectionExtensions.cs");
        string content=File.ReadAllText(registration).Replace("        services.AddScoped<EdgIQ.MurimiOS.Application.Common.Abstractions.IPlatformDbContext>",
            "        return services.AddApplicationPersistenceContracts();\n    }\n\n    public static IServiceCollection AddApplicationPersistenceContracts(this IServiceCollection services)\n    {\n        services.AddScoped<EdgIQ.MurimiOS.Application.Common.Abstractions.IPlatformDbContext>");
        File.WriteAllText(registration,content);
        string worker=Full("Worker/Program.cs");
        File.WriteAllText(worker,File.ReadAllText(worker).Replace("builder.Services.AddMurimiOSApplication();","builder.Services.AddApplicationPersistenceContracts();"));
        RefreshApplicationAliases();DeduplicateImports();
    }

    static void RepairInfrastructureAliases()
    {
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("src/Infrastructure/")))
        {
            string text=File.ReadAllText(file);var root=Parse(text);
            var aliases=root.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u=>u.Alias?.Name.ToString()=="BaseEntityConfig").ToArray();
            if(aliases.Length!=0)
                File.WriteAllText(file,"using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;\n"+root.RemoveNodes(aliases,SyntaxRemoveOptions.KeepNoTrivia)!.ToFullString());
        }
        RefreshApplicationAliases();DeduplicateImports();
    }
}
