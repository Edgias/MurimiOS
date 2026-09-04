using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static partial class RefactorTool
{
    static void RegroupTests()
    {
        Backup("tests");
        var moves=new Dictionary<string,string>();
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("tests/Platform.UnitTests/")).ToArray())
        {
            string text=File.ReadAllText(file);
            string layer=Regex.IsMatch(text,@"Edg(IQ|ias)\.MurimiOS\.API\.")?"API.FunctionalTests"
                :text.Contains("EdgIQ.MurimiOS.Infrastructure.")?"Infrastructure.IntegrationTests"
                :text.Contains("EdgIQ.MurimiOS.Application.")?"Application.UnitTests":"Domain.UnitTests";
            moves[Relative(file)]="tests/"+layer+"/"+Path.GetFileName(file);
        }
        foreach(string file in Sources().Where(f=>Relative(f).StartsWith("tests/Platform.IntegrationTests/")))
            moves[Relative(file)]="tests/API.FunctionalTests/"+Path.GetFileName(file);
        MoveSourceFiles(moves);
        Move("tests/Platform.UnitTests/Platform.UnitTests.csproj","tests/Domain.UnitTests/Domain.UnitTests.csproj");
        Move("tests/Platform.IntegrationTests/Platform.IntegrationTests.csproj","tests/API.FunctionalTests/API.FunctionalTests.csproj");
        foreach(string file in Directory.EnumerateFiles(Full("tests/Platform.IntegrationTests/Contracts"),"*"))
            Move(Relative(file),"tests/API.FunctionalTests/Contracts/"+Path.GetFileName(file));
        string testPackages="    <PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"17.14.1\" />\n    <PackageReference Include=\"xunit\" Version=\"2.9.3\" />\n    <PackageReference Include=\"xunit.runner.visualstudio\" Version=\"3.1.4\" />\n";
        void Project(string name,string[] references,string packages="")
        {
            string path=Full("tests/"+name+"/"+name+".csproj");Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            string content="<Project Sdk=\"Microsoft.NET.Sdk\">\n  <PropertyGroup>\n    <TargetFramework>net10.0</TargetFramework>\n    <RootNamespace>EdgIQ.MurimiOS."+name+"</RootNamespace>\n    <ImplicitUsings>enable</ImplicitUsings>\n    <Nullable>enable</Nullable>\n    <IsPackable>false</IsPackable>\n  </PropertyGroup>\n  <ItemGroup>\n"+testPackages+packages+"    <Using Include=\"Xunit\" />\n    <None Update=\"Contracts\\*.json\" CopyToOutputDirectory=\"PreserveNewest\" />\n"+string.Concat(references.Select(r=>"    <ProjectReference Include=\"..\\..\\src\\"+r+"\\"+r+".csproj\" />\n"))+"  </ItemGroup>\n</Project>\n";
            File.WriteAllText(path,content);
        }
        const string memory="    <PackageReference Include=\"Microsoft.EntityFrameworkCore.InMemory\" Version=\"10.0.11\" />\n";
        const string coverage="    <PackageReference Include=\"coverlet.collector\" Version=\"6.0.4\" />\n";
        Project("Domain.UnitTests",["Domain"],coverage);
        Project("Application.UnitTests",["Application"]);
        Project("Infrastructure.IntegrationTests",["Infrastructure"],memory+coverage);
        Project("API.FunctionalTests",["API"],memory+"    <PackageReference Include=\"Microsoft.AspNetCore.Mvc.Testing\" Version=\"10.0.9\" />\n");
        Project("ArchitectureTests",["Domain","Application","Infrastructure","API"],"    <PackageReference Include=\"NetArchTest.Rules\" Version=\"1.3.2\" />\n");
        string solution=Full("MurimiOS.sln");
        File.WriteAllText(solution,File.ReadAllText(solution).Replace("Platform.UnitTests","Domain.UnitTests").Replace("Platform.IntegrationTests","API.FunctionalTests"));
        string contract=Full("tests/API.FunctionalTests/ApiContractTests.cs");
        File.WriteAllText(contract,File.ReadAllText(contract).Replace("\"Platform.IntegrationTests\"","\"API.FunctionalTests\""));
        DeduplicateImports();
    }
}
