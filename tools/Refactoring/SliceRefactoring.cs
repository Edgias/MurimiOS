using System.Text.RegularExpressions;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static partial class RefactorTool
{
    static readonly Dictionary<string,string> Ports = new()
    {
        ["PlatformDbContext"]="IPlatformDbContext", ["MurimiOSDbContext"]="ILegacyDbContext",
        ["OrganisationScopeAccess"]="IOrganisationScopeAccess", ["IntegrationCredentialProtector"]="IIntegrationCredentialProtector",
        ["HttpTenantContext"]="IMutableTenantContext", ["IcarAdeExchangeAdapter"]="IIcarAdeExchangeAdapter",
        ["ScotEidMovementAdapter"]="IScotEidMovementAdapter", ["ComplianceAuditPackGenerator"]="IComplianceAuditPackGenerator",
        ["ComplianceAuditPackSignatureService"]="IComplianceAuditPackSignatureService", ["RequestQuotaStore"]="IRequestQuotaStore",
        ["TenantDatabaseRegistry"]="ITenantDatabaseRegistry", ["TenantDatabaseRouteResolver"]="ITenantDatabaseRouteResolver"
        , ["HttpContext"]="UseCaseContext", ["HttpResponse"]="IResponseMetadata", ["HttpRequest"]="IncomingContent"
    };
    static string PortTypes(string text)
    {
        foreach(var pair in Ports)text=Regex.Replace(text,@"\b"+pair.Key+@"\b",pair.Value);
        return text;
    }
    static string ApplicationBody(string text)
    {
        var parsed=Parse("class Temporary { "+text+" }");
        var contextNames=parsed.DescendantNodes().OfType<ParameterSyntax>().Where(p=>p.Type?.ToString()=="HttpContext").Select(p=>p.Identifier.ValueText).Distinct().ToArray();
        var responseNames=parsed.DescendantNodes().OfType<ParameterSyntax>().Where(p=>p.Type?.ToString()=="HttpResponse").Select(p=>p.Identifier.ValueText).Distinct().ToArray();
        var requestNames=parsed.DescendantNodes().OfType<ParameterSyntax>().Where(p=>p.Type?.ToString()=="HttpRequest").Select(p=>p.Identifier.ValueText).Distinct().ToArray();
        static string HeaderName(string name)=>name switch {"UserAgent"=>"User-Agent","CacheControl"=>"Cache-Control","RetryAfter"=>"Retry-After",_=>name};
        foreach(string name in contextNames)
        {
            text=Regex.Replace(text,@"\b"+name+@"\.User\b",name+".User.Principal");
            text=text.Replace(name+".TraceIdentifier",name+".User.CorrelationId").Replace(name+".Connection.RemoteIpAddress",name+".User.RemoteAddress")
                .Replace(name+".Request.Method",name+".User.Method").Replace(name+".Request.Path",name+".User.Path").Replace(name+".Items",name+".State");
            text=Regex.Replace(text,@"\b"+name+"\\.Request\\.Headers\\[([^\\]]+)\\]",m=>name+".User.HeaderValues("+m.Groups[1].Value+")");
            text=Regex.Replace(text,@"\b"+name+@"\.Request\.Headers\.(\w+)\.ToString\(\)",m=>name+".User.Header(\""+HeaderName(m.Groups[1].Value)+"\")");
            text=Regex.Replace(text,@"\b"+name+@"\.Response\.Headers\.(\w+)",m=>name+".Response[\""+HeaderName(m.Groups[1].Value)+"\"]");
            text=text.Replace(name+".Response.Headers[",name+".Response[");
        }
        foreach(string name in responseNames)
        {
            text=Regex.Replace(text,@"\b"+name+@"\.Headers\.(\w+)",m=>name+"[\""+HeaderName(m.Groups[1].Value)+"\"]");
            text=text.Replace(name+".Headers[",name+"[");
        }
        foreach(string name in requestNames)
        {
            text=Regex.Replace(text,@"\b"+name+"\\.Headers\\[([^\\]]+)\\]\\.FirstOrDefault\\(\\)",m=>name+".FirstHeader("+m.Groups[1].Value+")");
            text=Regex.Replace(text,@"\b"+name+"\\.Query\\[([^\\]]+)\\]\\.FirstOrDefault\\(\\)",m=>name+".FirstQuery("+m.Groups[1].Value+")");
        }
        text=PortTypes(text);
        text=Regex.Replace(text,@"\b(TypedResults|Results)\.(Ok|Created|Accepted|BadRequest|Conflict|NotFound|NoContent|Forbid|StatusCode|UnprocessableEntity|Json|Text|File|Stream|Problem)\b","UseCaseResults.$2");
        text=Regex.Replace(text,@"\bIResult\b","UseCaseResult");
        text=Regex.Replace(text,@"\bStatusCodes\.Status(\d{3})\w*","$1");
        return text;
    }
    static HashSet<string> Names(SyntaxNode node)=>node.DescendantTokens().Where(t=>t.IsKind(SyntaxKind.IdentifierToken)).Select(t=>t.ValueText).ToHashSet();
    static string AppImports(IEnumerable<CompilationUnitSyntax> roots, bool legacy=false)
    {
        var imports=roots.SelectMany(r=>r.DescendantNodes().OfType<UsingDirectiveSyntax>()).Where(u=>
            !u.Name!.ToString().Contains(".API.")&&!u.Name.ToString().Contains(".Infrastructure.")&&!u.Name.ToString().StartsWith("Microsoft.AspNetCore")&&!u.Name.ToString().StartsWith("Minio"))
            .Select(u=>u.WithGlobalKeyword(default).ToString()).ToList();
        if(legacy)imports.AddRange(Parse(File.ReadAllText(Full("API/GlobalUsings.cs"))).Usings.Where(u=>u.Name!.ToString().Contains(".Domain.")).Select(u=>u.WithGlobalKeyword(default).ToString()));
        imports.AddRange(new[]{"using EdgIQ.Dispatcher;","using EdgIQ.MurimiOS.Application.Common.Abstractions;","using EdgIQ.MurimiOS.Application.Common.Results;","using Microsoft.EntityFrameworkCore;","using Microsoft.Extensions.Logging;","using Microsoft.Extensions.Options;"});
        return string.Join("\n",imports.Distinct())+"\n";
    }
    sealed record ApiType(string File, CompilationUnitSyntax Root, MemberDeclarationSyntax Declaration)
    {
        public string Name=>TypeName(Declaration);
        public string Ns=>Namespace(Declaration);
    }
    static void ExtractSlices(string[] selected)
    {
        if(selected.Length==0)throw new InvalidOperationException("Supply explicit endpoint source paths after the workspace argument.");
        string phase="slices-"+Path.GetFileNameWithoutExtension(selected[0]);
        Backup(phase);
        var apiTypes=Sources().Where(f=>Relative(f).StartsWith("API/")).SelectMany(f=>
        {
            var root=Parse(File.ReadAllText(f));
            return TopTypes(root).Where(t=>t is RecordDeclarationSyntax || TypeName(t).EndsWith("Mapper") || TypeName(t).EndsWith("Model") || TypeName(t)=="SyncCommandProcessor")
                .Select(t=>new ApiType(Relative(f),root,t));
        }).ToArray();
        var log=new List<object>();
        var domainTypes=Sources().Where(f=>Relative(f).StartsWith("src/Domain/")).SelectMany(f=>TopTypes(Parse(File.ReadAllText(f))).Where(t=>t is not TypeDeclarationSyntax d||d.TypeParameterList==null).Select(t=>(Ns:Namespace(t),Name:TypeName(t)))).ToArray();
        foreach(string file in selected)
        {
            string source=File.ReadAllText(Full(file)); var root=Parse(source);
            var owner=root.DescendantNodes().OfType<ClassDeclarationSyntax>().First(t=>t.Identifier.ValueText.EndsWith("Endpoints"));
            string feature=owner.Identifier.ValueText.Replace("Endpoints","");
            if(feature.Equals("Bin",StringComparison.OrdinalIgnoreCase))feature="Bins";
            var imports=root.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u=>u.Alias==null).Select(u=>u.Name!.ToString()).ToHashSet();
            if(Relative(file).StartsWith("src/Infrastructure/"))
                imports.UnionWith(Parse(File.ReadAllText(Full("src/Infrastructure/GlobalUsings.cs"))).Usings.Where(u=>u.Alias==null).Select(u=>u.Name!.ToString()));
            var visibleTypes=apiTypes.Where(t=>t.Ns==Namespace(owner)||imports.Contains(t.Ns)).GroupBy(t=>t.Name)
                .ToDictionary(g=>g.Key,g=>g.FirstOrDefault(t=>t.Ns==Namespace(owner))??g.First());
            var methods=owner.Members.OfType<MethodDeclarationSyntax>().ToArray();
            var routes=owner.DescendantNodes().OfType<InvocationExpressionSyntax>().Where(i=>i.Expression is MemberAccessExpressionSyntax m&&Regex.IsMatch(m.Name.Identifier.ValueText,"^Map(Get|Post|Put|Delete|Patch|Methods)$")).ToArray();
            var edits=new List<(int Start,int Length,string Text)>(); var removed=new HashSet<MemberDeclarationSyntax>(); var allocated=new HashSet<string>();
            foreach(var route in routes)
            {
                var routeMember=(MemberAccessExpressionSyntax)route.Expression;
                var argument=route.ArgumentList.Arguments.Last();
                MethodDeclarationSyntax? method=null; ParameterListSyntax parameters; CSharpSyntaxNode body; string? originalReturn; string useCase;
                if(argument.Expression is IdentifierNameSyntax name)
                {
                    method=methods.SingleOrDefault(m=>m.Identifier.ValueText==name.Identifier.ValueText);
                    if(method==null)throw new InvalidOperationException("Unknown route method: "+argument);
                    parameters=method.ParameterList; body=(CSharpSyntaxNode?)method.Body??method.ExpressionBody!.Expression;
                    originalReturn=method.ReturnType.ToString();useCase=Regex.Replace(method.Identifier.ValueText,"Async$","");
                }
                else if(argument.Expression is ParenthesizedLambdaExpressionSyntax lambda)
                {
                    parameters=lambda.ParameterList;body=lambda.Body;originalReturn=lambda.ReturnType?.ToString();
                    string path=route.ArgumentList.Arguments[0].Expression.ToString().Trim('"');
                    useCase=routeMember.Name.Identifier.ValueText[3..]+char.ToUpperInvariant(routeMember.Expression.ToString()[0])+routeMember.Expression.ToString()[1..]
                        +string.Concat(Regex.Matches(path,@"[A-Za-z][A-Za-z0-9]*").Select(m=>char.ToUpperInvariant(m.Value[0])+m.Value[1..]));
                }
                else throw new InvalidOperationException("Unsupported route handler: "+argument);
                if(owner.Identifier.ValueText=="PlatformEndpoints")
                {
                    string registration=route.Ancestors().OfType<MethodDeclarationSyntax>().First().Identifier.ValueText;
                    feature=registration switch { "MapSyncEndpoints"=>"Sync", "MapInventoryEndpoints"=>"Inventory", "MapFarmEndpoints"=>"Farms", "MapProductionEndpoints"=>"Production", "MapLivestockEndpoints"=>"Livestock", _=>"Platform" };
                }
                string originalUseCase=useCase;int suffix=2;while(!allocated.Add(useCase))useCase=originalUseCase+suffix++;
                bool query=routeMember.Name.Identifier.ValueText is "MapGet" or "MapMethods";
                string requestName=useCase+(query?"Query":"Command"),handlerName=useCase+"Handler";
                string ns="EdgIQ.MurimiOS.Application."+feature+"."+useCase,folder="src/Application/"+feature+"/"+useCase;
                var selectedMembers=new HashSet<MemberDeclarationSyntax>();
                if(method!=null)selectedMembers.Add(method);
                var tokens=Names(body);tokens.UnionWith(Names(parameters));
                bool progress;
                do
                {
                    progress=false;
                    foreach(var member in owner.Members)
                    {
                        if(selectedMembers.Contains(member))continue;
                        string[] names=member switch
                        {
                            MethodDeclarationSyntax m=>[m.Identifier.ValueText],
                            FieldDeclarationSyntax f=>f.Declaration.Variables.Select(v=>v.Identifier.ValueText).ToArray(),
                            BaseTypeDeclarationSyntax t=>[t.Identifier.ValueText], _=>[]
                        };
                        if(names.Any(tokens.Contains)){selectedMembers.Add(member);tokens.UnionWith(Names(member));progress=true;}
                    }
                }while(progress);
                var dtos=new HashSet<ApiType>();
                do
                {
                    progress=false;
                    foreach(var type in visibleTypes.Values.Where(t=>(tokens.Contains(t.Name)
                        ||t.Name.EndsWith("Mapper")&&t.Declaration.DescendantNodes().OfType<MethodDeclarationSyntax>().Any(m=>tokens.Contains(m.Identifier.ValueText)))&&!dtos.Contains(t)))
                    {dtos.Add(type);tokens.UnionWith(Names(type.Declaration));progress=true;}
                }while(progress);
                var services=parameters.Parameters.Where(p=>IsService(p.Type!.ToString(),visibleTypes)).ToArray();
                string metadata=PreservedMetadata(body,originalReturn,dtos);
                var inputs=parameters.Parameters.Where(p=>p.Type!.ToString()!="CancellationToken"&&!services.Contains(p)).ToArray();
                string appImports=AppImports(dtos.Select(t=>t.Root).Append(root),file.Split('/')[1] is not("Platform" or "Access" or "Reporting"));
                var importedDomainNamespaces=Parse(appImports).Usings.Where(u=>u.Alias==null).Select(u=>u.Name!.ToString()).ToHashSet();
                string aliases=string.Join("\n",domainTypes.Where(t=>tokens.Contains(t.Name)&&importedDomainNamespaces.Contains(t.Ns)&&!dtos.Any(d=>d.Name==t.Name))
                    .GroupBy(t=>t.Name).Where(g=>g.Count()==1).Select(g=>"using "+g.Key+" = global::"+g.First().Ns+"."+g.Key+";"));
                string preamble=appImports+"namespace "+ns+";\n"+aliases+"\n";
                Directory.CreateDirectory(Full(folder));
                string request=preamble+"\npublic sealed record "+requestName+"("+string.Join(", ",inputs.Select(p=>PortTypes(p.Type!.ToString())+" "+p.Identifier))+ ") : IRequest<UseCaseResult>;\n";
                File.WriteAllText(Full(folder+"/"+requestName+".cs"),request);
                string constructor=string.Join(", ",services.Select(p=>PortTypes(p.Type!.ToString())+" "+p.Identifier));
                var entry=method??SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("Task<IResult>"),"ExecuteAsync")
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword),SyntaxFactory.Token(SyntaxKind.StaticKeyword),SyntaxFactory.Token(SyntaxKind.AsyncKeyword)))
                    .WithParameterList(parameters);
                string entryName=entry.Identifier.ValueText;
                bool isAsync=method?.Modifiers.Any(SyntaxKind.AsyncKeyword)??((ParenthesizedLambdaExpressionSyntax)argument.Expression).AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword);
                bool returnsTask=isAsync||originalReturn?.StartsWith("Task<")==true;
                bool resultReturn=originalReturn?.Contains("IResult")==true||originalReturn?.Contains("Results<")==true||originalReturn?.Contains("Ok<")==true||originalReturn?.Contains("Created<")==true
                    ||body.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().Any(i=>Regex.IsMatch(i.Expression.ToString(),@"^(TypedResults|Results)\."));
                if(!resultReturn)
                {
                    if(body is BlockSyntax block)body=block.ReplaceNodes(block.DescendantNodes().OfType<ReturnStatementSyntax>(),(n,_)=>n.WithExpression(SyntaxFactory.ParseExpression("UseCaseResults.Ok("+n.Expression+")")));
                    else body=SyntaxFactory.ParseExpression("UseCaseResults.Ok("+body+")");
                }
                if(method==null)entry=entry.WithModifiers(SyntaxFactory.TokenList((isAsync?new[]{SyntaxKind.PrivateKeyword,SyntaxKind.StaticKeyword,SyntaxKind.AsyncKeyword}:new[]{SyntaxKind.PrivateKeyword,SyntaxKind.StaticKeyword}).Select(k=>SyntaxFactory.Token(k).WithTrailingTrivia(SyntaxFactory.Space))));
                entry=entry.WithReturnType(SyntaxFactory.ParseTypeName(returnsTask?"Task<UseCaseResult>":"UseCaseResult").WithTrailingTrivia(SyntaxFactory.Space))
                    .WithBody(body as BlockSyntax).WithExpressionBody(body is BlockSyntax?null:SyntaxFactory.ArrowExpressionClause((ExpressionSyntax)body))
                    .WithSemicolonToken(body is BlockSyntax?default:SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                string call=entryName+"("+string.Join(", ",parameters.Parameters.Select(p=>p.Type!.ToString()=="CancellationToken"?"cancellationToken":services.Contains(p)?p.Identifier.ToString():"message."+p.Identifier))+ ")";
                if(!returnsTask)call="Task.FromResult("+call+")";
                string handler=preamble+"\npublic sealed partial class "+handlerName+"("+constructor+") : IRequestHandler<"+requestName+", UseCaseResult>\n{\n"
                    +"    public Task<UseCaseResult> Handle("+requestName+" message, CancellationToken cancellationToken) => "+call+";\n"
                    +ApplicationBody(entry.ToFullString())+"\n"+string.Join("\n",selectedMembers.Where(m=>m!=method).Select(m=>ApplicationBody(m.ToFullString())))+"\n}\n";
                File.WriteAllText(Full(folder+"/"+handlerName+".cs"),handler);
                foreach(var dto in dtos)
                    File.WriteAllText(Full(folder+"/"+dto.Name+".cs"),preamble+ApplicationBody(dto.Declaration.ToFullString()));
                string lambdaParameters=string.Join(", ",new[]{"EdgIQ.Dispatcher.ISender sender","CancellationToken cancellationToken"}.Concat(inputs.Select(p=>p.ToString())));
                string lambdaBody="async Task<IResult> ("+lambdaParameters+") => (await sender.Send(new global::"+ns+"."+requestName+"("+
                    string.Join(", ",inputs.Select(p=>MapInput(p.Type!,p.Identifier.ToString(),visibleTypes,ns)))+"), cancellationToken)).ToHttpResult()";
                edits.Add((argument.Expression.SpanStart,argument.Expression.Span.Length,lambdaBody));
                if(metadata.Length>0)edits.Add((route.Span.End,0,metadata));
                foreach(var member in selectedMembers)removed.Add(member);
                log.Add(new{Source=file,Feature=feature,UseCase=useCase,Request=requestName,Handler=handlerName,DtoNames=dtos.Select(d=>d.Name).ToArray()});
            }
            // All original implementation helpers are now owned by their consuming slices.
            // Retain the registration methods and transport DTO declarations for binding.
            foreach(var member in removed.Where(m=>!m.DescendantNodes().OfType<InvocationExpressionSyntax>().Any(routes.Contains)))
                edits.Add((member.FullSpan.Start,member.FullSpan.Length,""));
            foreach(var edit in edits.OrderByDescending(e=>e.Start))source=source.Remove(edit.Start,edit.Length).Insert(edit.Start,edit.Text);
            File.WriteAllText(Full(file),"using EdgIQ.MurimiOS.API.Infrastructure;\n"+source);
        }
        File.WriteAllText(Full("output/refactoring/"+phase+"/slices.json"),JsonSerializer.Serialize(log,Json));
        DeduplicateImports();Console.WriteLine($"Extracted {log.Count} Dispatcher slices.");
    }
    static string PreservedMetadata(CSharpSyntaxNode body,string? originalReturn,HashSet<ApiType> dtos)
    {
        if(originalReturn?.Contains("IResult")==true)return "";
        var codes=new Dictionary<string,int>{{"Ok",200},{"Created",201},{"Accepted",202},{"NoContent",204},{"BadRequest",400},{"NotFound",404},{"Conflict",409}};
        if(originalReturn!=null)
        {
            var syntax=SyntaxFactory.ParseTypeName(originalReturn);
            return string.Concat(syntax.DescendantNodesAndSelf().OfType<TypeSyntax>().Where(t=>t is GenericNameSyntax or IdentifierNameSyntax)
                .Where(t=>codes.ContainsKey(t is GenericNameSyntax g?g.Identifier.ValueText:t.ToString()))
                .Select(t=>t is GenericNameSyntax g?".Produces<"+g.TypeArgumentList.Arguments[0]+">("+codes[g.Identifier.ValueText]+")":".Produces("+codes[t.ToString()]+")"));
        }
        var returns=body is BlockSyntax?body.DescendantNodes().OfType<ReturnStatementSyntax>().Select(r=>r.Expression).ToArray():new[]{body as ExpressionSyntax};
        if(returns.Length!=1)return ""; // mixed Results/TypedResults infer IResult
        var value=returns[0];
        int status=200;
        if(value is InvocationExpressionSyntax call&&call.Expression is MemberAccessExpressionSyntax member&&member.Expression.ToString()=="TypedResults")
        {
            if(!codes.TryGetValue(member.Name.Identifier.ValueText,out status))return "";
            value=call.ArgumentList.Arguments.LastOrDefault()?.Expression;
            if(value==null)return ".Produces("+status+")";
        }
        string? type=value is ObjectCreationExpressionSyntax created?created.Type.ToString():null;
        if(value is IdentifierNameSyntax id)
            type=body.DescendantNodes().OfType<VariableDeclarationSyntax>().FirstOrDefault(d=>d.Variables.Any(v=>v.Identifier.ValueText==id.Identifier.ValueText))?.Type.ToString();
        if(type==null||type=="var")
        {
            var mapper=dtos.SelectMany(d=>d.Declaration.DescendantNodes().OfType<MethodDeclarationSyntax>()).FirstOrDefault(m=>m.Identifier.ValueText=="AsApiResponse");
            if(value?.ToString().Contains(".Select(")==true&&mapper!=null)type="IEnumerable<"+mapper.ReturnType+">";
        }
        return type==null||type=="var"?"":".Produces<"+type+">("+status+")";
    }
    static bool IsService(string type,Dictionary<string,ApiType> apiTypes)
    {
        if(type=="HttpRequest")return false;
        if(Regex.IsMatch(type,@"^IEnumerable<I[A-Z]"))return true;
        if(Ports.ContainsKey(type))return true;
        if(type=="SyncCommandProcessor")return true;
        if(type=="CancellationToken"||apiTypes.ContainsKey(type.TrimEnd('?')))return false;
        return type is "TimeProvider" or "SyncCommandProcessor" or "ProductionReportPdfBuilder" or "EnterpriseConsolidationEngine" or "GeospatialImportParser" or "ComplianceStandardPackCatalog" or "ComplianceStandardPackInstaller" or "ComplianceEvaluationEngine" or "TenantDatabaseRoutingScope"
            ||Regex.IsMatch(type,@"^I[A-Z]")&&!type.StartsWith("IReadOnly")&&!type.StartsWith("IEnumerable");
    }
    static string MapInput(TypeSyntax type,string expression,Dictionary<string,ApiType> types,string ns)
    {
        if(type.ToString()=="HttpRequest")return "EdgIQ.MurimiOS.API.Infrastructure.IncomingContentAdapter.Bind("+expression+")";
        if(type is NullableTypeSyntax nullable)
        {
            string mapped=MapInput(nullable.ElementType,expression,types,ns);
            return mapped==expression?expression:expression+" is null ? null : "+mapped;
        }
        if(type is ArrayTypeSyntax array)
        {
            string mapped=MapInput(array.ElementType,"item",types,ns);
            if(mapped!="item")return expression+".Select(item => "+mapped+").ToArray()";
        }
        if(type is GenericNameSyntax generic&&generic.TypeArgumentList.Arguments.Count==1)
        {
            string mapped=MapInput(generic.TypeArgumentList.Arguments[0],"item",types,ns);
            if(mapped!="item")return expression+".Select(item => "+mapped+")."+(generic.Identifier.ValueText=="List"?"ToList()":"ToArray()");
        }
        if(!types.TryGetValue(type.ToString(),out var dto))return expression;
        if(dto.Declaration is RecordDeclarationSyntax record&&record.ParameterList!=null)
            return "new global::"+ns+"."+dto.Name+"("+string.Join(", ",record.ParameterList.Parameters.Select(p=>MapInput(p.Type!,expression+"."+p.Identifier,types,ns)))+")";
        var props=dto.Declaration.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();
        var current=dto.Declaration as TypeDeclarationSyntax;
        while(current?.BaseList?.Types.FirstOrDefault()?.Type is TypeSyntax baseType&&types.TryGetValue(baseType.ToString(),out var parent))
        {props.AddRange(parent.Declaration.DescendantNodes().OfType<PropertyDeclarationSyntax>());current=parent.Declaration as TypeDeclarationSyntax;}
        return "new global::"+ns+"."+dto.Name+" { "+string.Join(", ",props.Select(p=>p.Identifier+" = "+MapInput(p.Type,expression+"."+p.Identifier,types,ns)))+" }";
    }

    static void RepairSlices()
    {
        // On Windows the SDK output path 'bin' is case-insensitive. Never move its
        // compiled artifacts: only the generated source declarations are relocated.
        string bin=Full("src/Application/Bin");
        if(Directory.Exists(bin))
            foreach(string file in Directory.EnumerateFiles(bin,"*.cs",SearchOption.AllDirectories).Where(f=>!Relative(f).Contains("/Debug/")&&!Relative(f).Contains("/Release/")).ToArray())
                Move(Relative(file),Relative(file).Replace("src/Application/Bin/","src/Application/Bins/"));
        foreach(string file in Sources())
        {
            string text=File.ReadAllText(file);string after=text.Replace("EdgIQ.MurimiOS.Application.Bin.","EdgIQ.MurimiOS.Application.Bins.")
                .Replace("privatestaticasyncTask<UseCaseResult>","private static async Task<UseCaseResult> ")
                .Replace("privatestaticasyncUseCaseResultExecuteAsync","private static UseCaseResult ExecuteAsync")
                .Replace("using Quantity = global::EdgIQ.MurimiOS.Domain.Common.Quantity;", "");
            if(text!=after)File.WriteAllText(file,after);
        }
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("src/Application/")&&f.EndsWith("Handler.cs")))
        {
            string text=File.ReadAllText(file);var root=Parse(text);
            var methods=root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToArray();
            var handle=methods.SingleOrDefault(m=>m.Identifier.ValueText=="Handle");
            if(handle?.ExpressionBody?.Expression is not InvocationExpressionSyntax wrapper||wrapper.Expression.ToString()!="Task.FromResult")continue;
            if(wrapper.ArgumentList.Arguments[0].Expression is not InvocationExpressionSyntax call)continue;
            var entry=methods.SingleOrDefault(m=>m.Identifier.ValueText==call.Expression.ToString());
            if(entry?.ExpressionBody?.Expression is not InvocationExpressionSyntax forwarded)continue;
            if(!methods.Any(m=>m.Identifier.ValueText==forwarded.Expression.ToString()&&m.ReturnType.ToString()=="Task<UseCaseResult>"))continue;
            var newRoot=root.ReplaceNodes(new[]{entry,handle},(original,_)=>original==entry
                ?entry.WithReturnType(SyntaxFactory.ParseTypeName("Task<UseCaseResult>").WithTrailingTrivia(SyntaxFactory.Space))
                :handle.WithExpressionBody(handle.ExpressionBody!.WithExpression(call)));
            File.WriteAllText(file,newRoot.ToFullString());
        }
        var api=Sources().Where(f=>Relative(f).StartsWith("API/")).SelectMany(f=>
        {
            var root=Parse(File.ReadAllText(f));return TopTypes(root).Select(t=>new ApiType(Relative(f),root,t));
        }).ToArray();
        foreach(string folder in Directory.EnumerateDirectories(Full("src/Application/Bins")))
        {
            var existing=Directory.EnumerateFiles(folder,"*.cs").ToArray();
            var tokens=existing.SelectMany(f=>Names(Parse(File.ReadAllText(f)))).ToHashSet();
            var selected=new HashSet<ApiType>(); bool changed;
            do
            {
                changed=false;
                foreach(var type in api.Where(t=>t.File.StartsWith("API/Bins/")&&(t.Declaration is RecordDeclarationSyntax||t.Name.EndsWith("Mapper"))&&!selected.Contains(t)
                    &&(tokens.Contains(t.Name)||t.Name.EndsWith("Mapper")&&t.Declaration.DescendantNodes().OfType<MethodDeclarationSyntax>().Any(m=>tokens.Contains(m.Identifier.ValueText)))))
                {selected.Add(type);tokens.UnionWith(Names(type.Declaration));changed=true;}
            }while(changed);
            foreach(var type in selected)
            {
                string path=Path.Combine(folder,type.Name+".cs");if(File.Exists(path))continue;
                File.WriteAllText(path,AppImports(selected.Select(t=>t.Root),true)+"namespace EdgIQ.MurimiOS.Application.Bins."+Path.GetFileName(folder)+";\n"+type.Declaration.ToFullString());
            }
        }
        DeduplicateImports();
    }

    static void ExtractSharedQueries()
    {
        Backup("shared-queries");
        MoveHelpers("API/Platform/AnalyticsMaturityEndpoints.cs","AnalyticsFacts","src/Application/Common/Analytics",["CurrentFacts"]);
        MoveHelpers("API/Platform/LivestockExchangeEndpoints.cs","LivestockRegulatoryDefaults","src/Application/Common/Livestock",
            ["EnsureDefaultsAsync","DefaultProfiles","ScotlandCattleMovementProfileKey","IcarCattleRegistrationProfileKey","IcarCattleMilkProfileKey","IcarAdeProvider"]);
    }
    static void MoveHelpers(string file,string className,string folder,string[] names)
    {
        string original=File.ReadAllText(Full(file));var root=Parse(original);
        var owner=root.DescendantNodes().OfType<ClassDeclarationSyntax>().First(t=>t.Identifier.ValueText.EndsWith("Endpoints"));
        var members=owner.Members.Where(m=>m is MethodDeclarationSyntax method&&names.Contains(method.Identifier.ValueText)
            ||m is FieldDeclarationSyntax field&&field.Declaration.Variables.Any(v=>names.Contains(v.Identifier.ValueText))).ToArray();
        string ns=NewNamespace(folder+"/"+className+".cs");
        string content=AppImports([root])+"namespace "+ns+";\n\npublic static class "+className+"\n{\n"+
            string.Join("\n",members.Select(m=>ApplicationBody(Regex.Replace(m.ToFullString(),@"\b(private|internal)\b","public"))))+"\n}\n";
        Directory.CreateDirectory(Full(folder));File.WriteAllText(Full(folder+"/"+className+".cs"),content);
        string after=root.RemoveNodes(members,SyntaxRemoveOptions.KeepNoTrivia)!.ToFullString();
        foreach(string name in names)after=Regex.Replace(after,@"(?<!\.)\b"+name+@"\b",className+"."+name);
        File.WriteAllText(Full(file),"using "+ns+";\n"+after);
        foreach(string source in Sources().Where(f=>Relative(f)!=file))
        {
            string before=File.ReadAllText(source),text=before;
            foreach(string name in names)text=text.Replace(owner.Identifier.ValueText+"."+name,"global::"+ns+"."+className+"."+name);
            if(text!=before)File.WriteAllText(source,text);
        }
    }

    static void RefreshApplicationAliases()
    {
        var domain=Sources().Where(f=>Relative(f).StartsWith("src/Domain/")).SelectMany(f=>TopTypes(Parse(File.ReadAllText(f)))
            .Where(t=>t is not TypeDeclarationSyntax d||d.TypeParameterList==null).Select(t=>(Ns:Namespace(t),Name:TypeName(t)))).ToArray();
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("src/Application/")||Relative(f).StartsWith("src/Infrastructure/")))
        {
            string text=File.ReadAllText(file);var root=Parse(text);
            var ns=root.Members.OfType<FileScopedNamespaceDeclarationSyntax>().SingleOrDefault();if(ns==null)continue;
            var imports=root.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u=>u.Alias==null).Select(u=>u.Name!.ToString()).ToHashSet();
            if(Relative(file).StartsWith("src/Infrastructure/"))
                imports.UnionWith(Parse(File.ReadAllText(Full("src/Infrastructure/GlobalUsings.cs"))).Usings.Where(u=>u.Alias==null).Select(u=>u.Name!.ToString()));
            var existing=root.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u=>u.Alias!=null).Select(u=>u.Alias!.Name.ToString()).ToHashSet();
            var declared=TopTypes(root).Select(TypeName).ToHashSet();var tokens=Names(ns);
            string aliases=string.Join("\n",domain.Where(t=>imports.Contains(t.Ns)&&tokens.Contains(t.Name)&&!existing.Contains(t.Name)&&!declared.Contains(t.Name))
                .GroupBy(t=>t.Name).Where(g=>g.Count()==1).Select(g=>"using "+g.Key+" = global::"+g.First().Ns+"."+g.Key+";"));
            if(aliases.Length!=0)File.WriteAllText(file,text.Insert(ns.SemicolonToken.Span.End,"\n"+aliases+"\n"));
        }
    }

    static void RepairTransport()
    {
        if(File.Exists(Full("src/Application/Common/Encoding/Base64UrlEncoding.cs")))
            Move("src/Application/Common/Encoding/Base64UrlEncoding.cs", "src/Application/Common/TextEncoding/Base64UrlEncoding.cs");
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("src/Application/")))
        {
            string before=File.ReadAllText(file),after=before.Replace("Common.Encoding", "Common.TextEncoding").Replace("global::EdgIQ.MurimiOS.Application.Common.TextEncoding", "EdgIQ.MurimiOS.Application.Common.TextEncoding");
            if(before!=after)File.WriteAllText(file,after);
        }
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("src/Application/")&&f.EndsWith("Handler.cs")))
        {
            string original=File.ReadAllText(file),text=original;
            text=Regex.Replace(text,@"(?:Microsoft\.AspNetCore\.WebUtilities\.)?WebEncoders\.Base64UrlEncode", "global::EdgIQ.MurimiOS.Application.Common.TextEncoding.Base64UrlEncoding.Encode");
            if(text.Contains("context.RequestServices.GetRequiredService<IRequestQuotaStore>()"))
            {
                text=text.Replace("context.RequestServices.GetRequiredService<IRequestQuotaStore>()","quotaStore");
                var root=Parse(text);var owner=root.DescendantNodes().OfType<ClassDeclarationSyntax>().First(t=>t.Identifier.ValueText.EndsWith("Handler"));
                var changed=owner.WithParameterList(owner.ParameterList!.AddParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("quotaStore")).WithType(SyntaxFactory.ParseTypeName("IRequestQuotaStore").WithTrailingTrivia(SyntaxFactory.Space))));
                changed=changed.ReplaceNodes(changed.Members.OfType<MethodDeclarationSyntax>().Where(m=>m.Modifiers.Any(SyntaxKind.StaticKeyword)),(m,_)=>m.WithModifiers(SyntaxFactory.TokenList(m.Modifiers.Where(t=>!t.IsKind(SyntaxKind.StaticKeyword)))));
                text=root.ReplaceNode(owner,changed).ToFullString();
            }
            if(text!=original)File.WriteAllText(file,text);
        }
    }
    static void FinishSync()
    {
        const string folder="src/Application/Sync/PostSyncPush/";
        string processor=File.ReadAllText(Full(folder+"SyncCommandProcessor.cs"));
        Move(folder+"SyncCommandProcessor.cs","output/refactoring/sync-generated-processor.cs");
        Move("API/Platform/SyncCommandProcessor.cs",folder+"SyncCommandProcessor.cs");
        File.WriteAllText(Full(folder+"SyncCommandProcessor.cs"),processor);
        string request=Full(folder+"PostSyncPushCommand.cs");
        File.WriteAllText(request,File.ReadAllText(request).Replace(", SyncCommandProcessor processor", ""));
        string handler=Full(folder+"PostSyncPushHandler.cs");
        File.WriteAllText(handler,File.ReadAllText(handler).Replace("PostSyncPushHandler()", "PostSyncPushHandler(SyncCommandProcessor processor)").Replace("message.processor", "processor"));
        string api=Full("API/Platform/PlatformEndpoints.cs");
        File.WriteAllText(api,File.ReadAllText(api).Replace(", SyncCommandProcessor processor", "")
            .Replace(", new global::EdgIQ.MurimiOS.Application.Sync.PostSyncPush.SyncCommandProcessor { Code = processor.Code }", ""));
        string program=Full("API/Program.cs");
        File.WriteAllText(program,File.ReadAllText(program).Replace("AddScoped<SyncCommandProcessor>()", "AddScoped<EdgIQ.MurimiOS.Application.Sync.PostSyncPush.SyncCommandProcessor>()"));
        foreach(string path in new[]{"API/Platform/PlatformInfoResponse.cs", "src/Application/Platform/GetApiPlatform/PlatformInfoResponse.cs"})
        {
            string text=File.ReadAllText(Full(path)).Replace("required string ", "string? ").Replace("required int[] ", "int[]? ").Replace("required string[] ", "string[]? ");
            File.WriteAllText(Full(path),text);
        }
        RefreshApplicationAliases();
    }
    static void RestoreBinMetadata()
    {
        using var archive=System.IO.Compression.ZipFile.OpenRead(Full("output/refactoring/slices-AccessEndpoints/sources.zip"));
        using var reader=new StreamReader(archive.GetEntry("API/Bins/BinEndpoints.cs")!.Open());
        var original=Parse(reader.ReadToEnd());
        string path=Full("API/Bins/BinEndpoints.cs"),text=File.ReadAllText(path);
        static InvocationExpressionSyntax[] Routes(CompilationUnitSyntax root)=>root.DescendantNodes().OfType<InvocationExpressionSyntax>()
            .Where(i=>i.Expression is MemberAccessExpressionSyntax a&&Regex.IsMatch(a.Name.Identifier.ValueText,"^Map(Get|Post|Put|Delete|Patch)$")).ToArray();
        var oldRoutes=Routes(original);var newRoutes=Routes(Parse(text));
        var dtos=Sources().Where(f=>Relative(f).StartsWith("API/Bins/")).SelectMany(f=>{var root=Parse(File.ReadAllText(f));return TopTypes(root).Select(t=>new ApiType(Relative(f),root,t));}).ToHashSet();
        for(int i=oldRoutes.Length-1;i>=0;i--)
        {
            var lambda=(ParenthesizedLambdaExpressionSyntax)oldRoutes[i].ArgumentList.Arguments.Last().Expression;
            string suffix=PreservedMetadata(lambda.Body,lambda.ReturnType?.ToString(),dtos);
            if(!text[newRoutes[i].Span.End..].StartsWith(".Produces"))text=text.Insert(newRoutes[i].Span.End,suffix);
        }
        File.WriteAllText(path,text);
    }
}
