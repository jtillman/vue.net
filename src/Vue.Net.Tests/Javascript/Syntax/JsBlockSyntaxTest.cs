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

    public class JsBlockSyntaxTest : BaseJsSyntaxTest
    {
        [Fact]
        public void TestConstructorSetsStatements()
        {
            var statements = new JsStatementSyntax[0];
            var syntax = new JsBlockSyntax(statements);

            Assert.Equal(statements, syntax.Statements);
        }

        [Fact]
        public void TestWritesCorrectlyWhenNoStatements()
        {
            var statements = new JsStatementSyntax[0];
            var syntax = new JsBlockSyntax(statements);

            AssertWrites("{}", syntax);
        }

        [Fact]
        public void TestWritesCorrectlyWhenSingleStatement()
        {
            var statements = new JsStatementSyntax[1] {
                new JsExpressionStatementSyntax(
                    new JsLiteralExpressionSyntax(JsSyntaxToken.Null))
            };
            var syntax = new JsBlockSyntax(statements);

            AssertWrites("{null;}", syntax);
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

            AssertWrites("{null;null;null;}", syntax);
        }

         
        [Fact]
        public void TestExtra()
        {
            var package = new JavascriptPackage("myapplication");
            package.AddType<HttpClient>();
            package.AddType<TestExtraTestClass>();
        }
    }
}
