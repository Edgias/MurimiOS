using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static partial class RefactorTool
{
    static void RegroupApi(bool resume=false)
    {
        if(!resume)Backup("api");
        var manifests=Directory.EnumerateFiles(Full("output/refactoring"),"slices.json",SearchOption.AllDirectories)
            .SelectMany(f=>JsonSerializer.Deserialize<List<SliceManifest>>(File.ReadAllText(f))!)
            .Select(m=>m.Feature=="Bin"?m with { Feature="Bins" }:m).ToArray();
        if(Directory.Exists(Full("API/Endpoints/Bin")))
            foreach(string file in Directory.EnumerateFiles(Full("API/Endpoints/Bin"),"*.cs",SearchOption.AllDirectories).ToArray())
                Move(Relative(file),Relative(file).Replace("/Bin/","/Bins/"));
        var endpointFiles=new List<string>();
        int count=0;
        if(!resume)
        {
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("API/")&&f.EndsWith("Endpoints.cs")).ToArray())
        {
            string text=File.ReadAllText(file);var root=Parse(text);
            var owner=root.DescendantNodes().OfType<ClassDeclarationSyntax>().First(t=>t.Identifier.ValueText.EndsWith("Endpoints"));
            string imports=string.Join("\n",root.Usings.Where(u=>!u.Name!.ToString().Contains(".Infrastructure")||u.Name.ToString().Contains(".API.Infrastructure")).Select(u=>u.ToString()));
            var routes=root.DescendantNodes().OfType<InvocationExpressionSyntax>().Where(i=>i.Expression is MemberAccessExpressionSyntax a&&Regex.IsMatch(a.Name.Identifier.ValueText,"^Map(Get|Post|Put|Delete|Patch|Methods)$")).ToArray();
            var edits=new List<(int Start,int Length,string Text)>();
            foreach(var route in routes)
            {
                var request=route.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().FirstOrDefault(n=>n.Type.ToString().Contains("EdgIQ.MurimiOS.Application."))
                    ??throw new InvalidOperationException("Route not extracted: "+route.Expression);
                string[] parts=request.Type.ToString().Split('.');
                string feature=parts[^3],useCase=parts[^2],ns="EdgIQ.MurimiOS.API.Endpoints."+feature+"."+useCase;
                string destination="API/Endpoints/"+feature+"/"+useCase+"/"+useCase+"Endpoint.cs";
                var statement=route.Ancestors().OfType<ExpressionStatementSyntax>().First();
                string receiver=((MemberAccessExpressionSyntax)route.Expression).Expression.ToString();
                Directory.CreateDirectory(Path.GetDirectoryName(Full(destination))!);
                File.WriteAllText(Full(destination),imports+"\nusing "+Namespace(owner)+";\nusing static "+Namespace(owner)+"."+owner.Identifier+";\nnamespace "+ns+";\ninternal static class "+useCase+"Endpoint\n{\n    internal static void Map(IEndpointRouteBuilder "+receiver+")\n    {\n"+statement.ToFullString()+"\n    }\n}\n");
                endpointFiles.Add(destination);
                edits.Add((statement.FullSpan.Start,statement.FullSpan.Length,"        global::"+ns+"."+useCase+"Endpoint.Map("+receiver+");\n"));count++;
            }
            foreach(var edit in edits.OrderByDescending(e=>e.Start))text=text.Remove(edit.Start,edit.Length).Insert(edit.Start,edit.Text);
            File.WriteAllText(file,text);
        }
        // Adopt a generated mapper copy as the moved original; other slices retain
        // independent copies. This preserves tracked history without retaining API logic.
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("API/")&&f.EndsWith("Mapper.cs")).ToArray())
        {
            string name=Path.GetFileNameWithoutExtension(file);
            var slice=manifests.FirstOrDefault(m=>m.DtoNames.Contains(name)&&m.Source.Split('/')[1]==Relative(file).Split('/')[1]);
            if(slice==null)continue;
            string destination="src/Application/"+slice.Feature+"/"+slice.UseCase+"/"+name+".cs";
            string content=File.ReadAllText(Full(destination));
            Move(destination,"output/refactoring/api/generated/"+slice.Feature+"/"+slice.UseCase+"/"+name+".cs");
            Move(Relative(file),destination);File.WriteAllText(Full(destination),content);
        }
        }
        RelocateApiContracts(manifests);
        var moves=new Dictionary<string,string>();
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("API/")).ToArray())
        {
            string rel=Relative(file),tail=rel[4..];
            if(!tail.StartsWith("Endpoints/")&&tail.EndsWith("Endpoints.cs"))
            {
                string feature=Path.GetFileNameWithoutExtension(tail).Replace("Endpoints","");
                if(feature=="Bin")feature="Bins";
                tail="Endpoints/"+feature+"/"+Path.GetFileName(tail);
            }
            else if(tail is not("Program.cs" or "GlobalUsings.cs")&&!tail.StartsWith("Endpoints/")&&!tail.StartsWith("Infrastructure/"))
                tail="Infrastructure/"+tail;
            moves[rel]="src/API/"+tail;
        }
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("Worker/")))moves[Relative(file)]="src/"+Relative(file);
        MoveSourceFiles(moves);
        foreach(string host in new[]{"API","Worker"})
        {
            foreach(string file in Directory.EnumerateFiles(Full(host),"*",SearchOption.AllDirectories).Where(f=>!Regex.IsMatch(Relative(f),@"/(bin|obj|App_Data)/",RegexOptions.IgnoreCase)).ToArray())
                Move(Relative(file),"src/"+Relative(file));
            string project=Full("src/"+host+"/"+host+".csproj");
            File.WriteAllText(project,File.ReadAllText(project).Replace("..\\src\\", "..\\"));
        }
        // Move runtime keys/media together with their content root, without opening,
        // copying over, or regenerating any existing data.
        if(Directory.Exists(Full("API/App_Data")))
        {
            if(Directory.Exists(Full("src/API/App_Data")))throw new InvalidOperationException("Runtime data destination already exists.");
            Directory.Move(Full("API/App_Data"),Full("src/API/App_Data"));
        }
        foreach(string file in Directory.EnumerateFiles(Root,"*.csproj",SearchOption.AllDirectories).Where(f=>!Regex.IsMatch(Relative(f),@"/(bin|obj)/",RegexOptions.IgnoreCase)))
        {
            string before=File.ReadAllText(file),after=before.Replace("..\\..\\API\\API.csproj","..\\..\\src\\API\\API.csproj").Replace("..\\..\\Worker\\Worker.csproj","..\\..\\src\\Worker\\Worker.csproj");
            if(before!=after)File.WriteAllText(file,after);
        }
        string solution=Full("MurimiOS.sln");
        File.WriteAllText(solution,File.ReadAllText(solution).Replace("\"API\\API.csproj\"","\"src\\API\\API.csproj\"").Replace("\"Worker\\Worker.csproj\"","\"src\\Worker\\Worker.csproj\""));
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("src/API/")&&!f.EndsWith("Program.cs")))
        {
            string text=File.ReadAllText(file);var root=Parse(text);
            text=root.RemoveNodes(root.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u=>u.Name!.ToString().StartsWith("EdgIQ.MurimiOS.Infrastructure.")),SyntaxRemoveOptions.KeepNoTrivia)!.ToFullString();
            File.WriteAllText(file,text);
        }
        File.WriteAllText(Full("output/refactoring/api/moves.json"),JsonSerializer.Serialize(moves,Json));
        DeduplicateImports();Console.WriteLine($"Created {count} individual endpoint files and moved API/Worker hosts.");
    }
    static void RelocateApiContracts(SliceManifest[] manifests)
    {
        // Match contracts by their original fully-qualified identity, not by a
        // simple name shared by the legacy and current farm APIs.
        using var archive=System.IO.Compression.ZipFile.OpenRead(Full("output/refactoring/api/sources.zip"));
        var desired=new Dictionary<string,string>();
        var originals=new Dictionary<string,string>();
        foreach(var entry in archive.Entries.Where(e=>e.FullName.StartsWith("API/")&&!e.FullName.StartsWith("API/Endpoints/")&&e.FullName.EndsWith(".cs")))
        {
            using var reader=new StreamReader(entry.Open());var root=Parse(reader.ReadToEnd());
            foreach(var type in TopTypes(root).Where(t=>t is RecordDeclarationSyntax))
            {
                string name=TypeName(type);
                var consumers=manifests.Where(m=>m.DtoNames.Contains(name)&&m.Source.Replace('\\','/').Split('/')[1]==entry.FullName.Split('/')[1]).ToArray();
                if(consumers.Length==0)continue;
                string feature=consumers.GroupBy(m=>m.Feature).OrderByDescending(g=>g.Count()).First().Key;
                desired[Namespace(type)+"."+name]="API/Endpoints/"+feature+"/"+(consumers.Length==1?consumers[0].UseCase:"Shared")+"/"+name+".cs";
                var ns=root.Members.OfType<BaseNamespaceDeclarationSyntax>().Single();
                originals[Namespace(type)+"."+name]=root.ReplaceNode(ns,ns.WithMembers(SyntaxFactory.SingletonList(type))).ToFullString();
            }
        }
        var types=Sources().SelectMany(f=>TopTypes(Parse(File.ReadAllText(f))).Select(t=>(Ns:Namespace(t),Name:TypeName(t)))).Distinct().ToArray();
        var apiRoots=Sources().Where(f=>Relative(f).StartsWith("API/")).ToDictionary(Relative,f=>Parse(File.ReadAllText(f)));
        foreach(var pair in desired)
        {
            if(!apiRoots.Values.SelectMany(TopTypes).Any(t=>Namespace(t)+"."+TypeName(t)==pair.Key))
            {
                if(File.Exists(Full(pair.Value)))throw new InvalidOperationException("Recovery contract collision: "+pair.Value);
                Directory.CreateDirectory(Path.GetDirectoryName(Full(pair.Value))!);
                File.WriteAllText(Full(pair.Value),originals[pair.Key]);apiRoots[pair.Value]=Parse(originals[pair.Key]);
            }
            var candidates=apiRoots.Select(f=>new{File=f.Key,Root=f.Value})
                .SelectMany(f=>TopTypes(f.Root).Where(t=>Namespace(t)+"."+TypeName(t)==pair.Key).Select(t=>new{f.File,f.Root,Type=t})).ToArray();
            if(candidates.Length==0)throw new InvalidOperationException("Contract missing: "+pair.Key);
            var selected=candidates.FirstOrDefault(c=>c.File==pair.Value)??candidates.FirstOrDefault(c=>c.File.StartsWith("API/Endpoints/"))??candidates[0];
            var ns=selected.Root.Members.OfType<BaseNamespaceDeclarationSyntax>().Single();
            string content=selected.Root.ReplaceNode(ns,ns.WithMembers(SyntaxFactory.SingletonList(selected.Type))).ToFullString();
            if(selected.File!=pair.Value&&TopTypes(selected.Root).Count()==1){Move(selected.File,pair.Value);apiRoots.Remove(selected.File);}
            else if(selected.File!=pair.Value&&File.Exists(Full(pair.Value)))throw new InvalidOperationException("Conflicting contract target: "+pair.Value);
            Directory.CreateDirectory(Path.GetDirectoryName(Full(pair.Value))!);File.WriteAllText(Full(pair.Value),content);
            apiRoots[pair.Value]=Parse(content);
            foreach(var candidate in candidates.Where(c=>c.File!=pair.Value&&File.Exists(Full(c.File))))
            {
                var remaining=candidate.Root.RemoveNode(candidate.Type,SyntaxRemoveOptions.KeepNoTrivia)!;
                if(!TopTypes(remaining).Any()){Move(candidate.File,"output/refactoring/api/contract-recovery/"+candidate.File);apiRoots.Remove(candidate.File);}
                else {File.WriteAllText(Full(candidate.File),remaining.ToFullString());apiRoots[candidate.File]=remaining;}
            }
        }
        var map=desired.ToDictionary(p=>p.Key,p=>NewNamespace(p.Value)+"."+Path.GetFileNameWithoutExtension(p.Value));
        foreach(string file in Sources().ToArray())
        {
            string rel=Relative(file),before=File.ReadAllText(file);
            string after=RewriteNamespaces(before,desired.ContainsValue(rel)?NewNamespace(rel):null,map,types);
            if(before!=after)File.WriteAllText(file,after);
        }
        DeduplicateImports();
    }
    sealed record SliceManifest(string Source,string Feature,string UseCase,string Request,string Handler,string[] DtoNames);

    static void RepairApiAliases()
    {
        var apiTypes=Sources().Where(f=>Relative(f).StartsWith("src/API/")).SelectMany(f=>TopTypes(Parse(File.ReadAllText(f)))
            .Where(t=>t is not TypeDeclarationSyntax d||d.TypeParameterList==null).Select(t=>(Ns:Namespace(t),Name:TypeName(t)))).ToArray();
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("src/API/")))
        {
            string text=File.ReadAllText(file);var root=Parse(text);
            var ns=root.Members.OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();if(ns==null)continue;
            var existing=root.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u=>u.Alias!=null).Select(u=>u.Alias!.Name.ToString()).ToHashSet();
            var declared=TopTypes(root).Select(TypeName).ToHashSet();var tokens=Names(ns);
            string aliases=string.Join("\n",apiTypes.Where(t=>tokens.Contains(t.Name)&&!existing.Contains(t.Name)&&!declared.Contains(t.Name))
                .GroupBy(t=>t.Name).Where(g=>g.Count()==1&&g.First().Ns!=ns.Name.ToString()).Select(g=>"using "+g.Key+" = global::"+g.First().Ns+"."+g.Key+";"));
            int offset=ns is FileScopedNamespaceDeclarationSyntax f?f.SemicolonToken.Span.End:((NamespaceDeclarationSyntax)ns).OpenBraceToken.Span.End;
            if(aliases.Length!=0)File.WriteAllText(file,text.Insert(offset,"\n"+aliases+"\n"));
        }
        DeduplicateImports();
    }
    static void FinishBinPlacement()
    {
        string target="src/Application/Bins/GetGroup/BinMapper.cs";
        string content=File.ReadAllText(Full(target));
        Move(target,"output/refactoring/api/generated/Bins/GetGroup/BinMapper.cs");
        Move("src/API/Infrastructure/Bins/BinMapper.cs",target);File.WriteAllText(Full(target),content);
        MoveSourceFiles(new() { ["src/API/Infrastructure/Bins/BinResponse.cs"]="src/API/Endpoints/Bins/Shared/BinResponse.cs" });
        DeduplicateImports();
    }
}
