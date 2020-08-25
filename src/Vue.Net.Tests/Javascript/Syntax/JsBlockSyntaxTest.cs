using Microsoft.AspNetCore.Mvc.Razor.Extensions;
using System.Net.Http;
using System.Threading.Tasks;
using Vue.Net.Javascript;
using Vue.Net.Javascript.Syntax;
using Xunit;

namespace Vue.Net.Tests.Javascript.Syntax
{
    public class DoStuff
    {
        public static string Get { get; }
    }

    public class TestExtraTestClass
    {
        public int test = 3;

        public string _name;
        public string Name { get; set; }
        
        public TestExtraTestClass Root { get; set; }

        public void Main(string name)
        {
            Name = name;
            _ = Task.Delay(30);
            this._name = Name.Length.ToString();
            this.test = Name.Length;
            this.test = this.Root.test;
            this._name = DoStuff.Get;
            this._name = string.Empty;
        }
    }

    public class TodoManager
    {
        public TodoManager() { }
    }

    public class JsBlockSyntaxTest
    {
        [Fact]
        public void TestConstructorSetsStatements()
        {
            var statements = new JsStatementSyntax[0];
            var syntax = new JsBlockSyntax(statements);

            Assert.Equal(statements, syntax.Statements.Items);
        }

        [Fact]
        public void TestWritesCorrectlyWhenNoStatements()
        {
            var statements = new JsStatementSyntax[0];
            var syntax = new JsBlockSyntax(statements);

            syntax.AssertWrites("{}");
        }

        [Fact]
        public void TestWritesCorrectlyWhenSingleStatement()
        {
            var statements = new JsStatementSyntax[1] {
                new JsExpressionStatementSyntax(
                    new JsLiteralExpressionSyntax(JsSyntaxToken.Null))
            };
            var syntax = new JsBlockSyntax(statements);

            syntax.AssertWrites("{null;}");
        }

        [Fact]
        public void TestWritesCorrectlyWhenMultipleStatements()
        {
            var statements = new JsStatementSyntax[3] {
                new JsExpressionStatementSyntax(
                    new JsLiteralExpressionSyntax(
                        JsSyntaxToken.Null)),
                new JsExpressionStatementSyntax(
                    new JsLiteralExpressionSyntax(
                        JsSyntaxToken.Null)),
                new JsExpressionStatementSyntax(
                    new JsLiteralExpressionSyntax(
                        JsSyntaxToken.Null))
            };
            var syntax = new JsBlockSyntax(statements);

            syntax.AssertWrites("{null;null;null;}");
        }

         
        [Fact]
        public void TestExtra()
        {
            var package = new JavascriptPackage();
            var env = new CSharpToJsTranspilerEnvironment("myapp");
            var jsType = env.TranspileType(typeof(TestExtraTestClass));
            //package.AddType<HttpClient>();
            //package.AddType<TestExtraTestClass>();

        }
    }

    public class JsCSharpInterpreter
    {
    }
}
