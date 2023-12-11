namespace SCG.ServiceEco.CodeGen.Tests
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis;
    using TPFive.SCG.ServiceEco.Abstractions;
    using TPFive.SCG.ServiceEco.CodeGen.InjectService;
    using System.Reflection;
    using System.Diagnostics;

    public class ServiceEcoCodeGenTest
    {
        [SetUp]
        public void Setup()
        {
        }
        private static Compilation CreateCompilation(string source)
        {
            List<PortableExecutableReference> References =
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
                    .Select(_ => MetadataReference.CreateFromFile(_.Location))
                    .Concat(new[]
                    {
                        // add your app/lib specifics, e.g.:
                        MetadataReference.CreateFromFile(typeof(InjectServiceAttribute).GetTypeInfo().Assembly.Location)
                    })
                    .ToList();
            return CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                References,
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
        }

        [Test]
        public void TestBasicInjectService()
        {
            Compilation inputCompilation = CreateCompilation(@"
namespace SCG.ServiceEco.CodeGen.Tests
{
    using TPFive.SCG.ServiceEco.Abstractions;
    [InjectService(
        Setup = ""true"",
        ServiceList = @""
TPFive.Game.Config.Service,
TPFive.Game.Resource.Service"")]
    public sealed partial class InjectServiceCodeGen
    {
    }
}
");

            SourceGenerator generator = new SourceGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
            GeneratorDriverRunResult runResult = driver.GetRunResult();
            var generatedSource = runResult.Results[0].GeneratedSources[0].SourceText.ToString();

            Assert.IsTrue(generatedSource.Contains("_serviceConfig is IAsyncStartable asyncStartable"));
            Assert.IsTrue(generatedSource.Contains("_serviceResource is IAsyncStartable asyncStartable"));
        }

        [Test]
        public void TestEventFieldDeclaration()
        {
            Compilation inputCompilation = CreateCompilation(@"
namespace SCG.ServiceEco.CodeGen.Tests
{
    using TPFive.SCG.ServiceEco.Abstractions;

    public delegate void TestEventHandler(bool result);

    public interface IServiceProvider
    {
        event TestEventHandler OnTestEventTriggerd;
    }

    [ServiceProvidedBy(typeof(IServiceProvider))]
    public sealed partial class ServiceCodeGen
    {
    }
}
");
            TPFive.SCG.ServiceEco.CodeGen.ServiceProvidedBy.SourceGenerator generator = new TPFive.SCG.ServiceEco.CodeGen.ServiceProvidedBy.SourceGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
            GeneratorDriverRunResult runResult = driver.GetRunResult();
            var generatedSource = runResult.Results[0].GeneratedSources[0].SourceText.ToString();
            Assert.IsTrue(generatedSource.Contains("public event TestEventHandler OnTestEventTriggerd;"));
        }
    }
}
