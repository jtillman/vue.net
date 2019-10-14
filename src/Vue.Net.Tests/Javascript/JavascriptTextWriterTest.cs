using System;
using System.IO;
using Vue.Net.Javascript;
using Xunit;

namespace Vue.Net.Tests.Javascript
{
    public class JavascriptTextWriterTest
    {
        private string GetWriterOutput(Action<JavascriptTextWriter> action, JavascriptFormattingOptions options = null)
        {
            using (var writer = new StringWriter())
            {
                var js = new JavascriptTextWriter(writer, options ?? new JavascriptFormattingOptions(newLineString: " "));
                action(js);
                return writer.ToString();
            }
        }
        private void AssertWrites(string expected, Action<JavascriptTextWriter> action)
        {
            Assert.Equal(
                expected,
                GetWriterOutput(action));
        }

        [Fact]
        public void TestConstructorThrowsOnNullTextWriter()
        {
            Assert.Throws<ArgumentNullException>(() => new JavascriptTextWriter(null));
        }

        [Fact]
        public void TestConstructorAcceptsTextWriter()
        {
            new JavascriptTextWriter(TextWriter.Null);
        }

        [Fact]
        public void TestWriteStartObjectDeclarationOpensObject()
        {
            AssertWrites(
                "{",
                js => js.WriteStartJsObject());
        }

        [Fact]
        public void TestEmptyObjectWrittenCorrectly()
        {
            AssertWrites(
                "{}",
                js =>
                {
                    js.WriteStartJsObject();
                    js.WriteEndJsObject();
                });
        }

        [Fact]
        public void TestWriteStartPropertyDeclarationWritesPropertyName()
        {
            AssertWrites(
                "{name: ",
                js =>
                {
                    js.WriteStartJsObject();
                    js.WriteStartJsObjectProperty("name");
                });
        }

        [Fact]
        public void TestWriteValueAfterPropertyDeclarationSetsValue()
        {
            AssertWrites(
                "{name: 1}",
                js =>
                {
                    js.WriteStartJsObject(); // -> Object
                    js.WriteStartJsObjectProperty("name"); // -> ObjectProperty
                    js.WriteJsValue(1); // -> Object
                    js.WriteEndJsObject(); // Document
                });
        }

        [Fact]
        public void TestWriteMultipleObjectProperites()
        {
            AssertWrites(
                "{first: 1, second: 2, third: 3}",
                js =>
                {
                    js.WriteStartJsObject();
                    js.WriteStartJsObjectProperty("first");
                    js.WriteJsValue(1);
                    js.WriteStartJsObjectProperty("second");
                    js.WriteJsValue(2);
                    js.WriteStartJsObjectProperty("third");
                    js.WriteJsValue(3);
                    js.WriteEndJsObject();
                });
        }
        
        [Fact]
        public void TestWriteObjectMethod()
        {
            AssertWrites(
                "{method(",
                js =>
                {
                    js.WriteStartJsObject();
                    js.WriteStartJsMethodDefinition("method");
                });
        }

        [Fact]
        public void TestWriteObjectMethodArgument()
        {

            AssertWrites(
                "{method(first",
                js =>
                {
                    js.WriteStartJsObject();
                    js.WriteStartJsMethodDefinition("method");
                    js.WriteJsMethodArgument("first");
                });
        }

        [Fact]
        public void TestWriteMultipleObjectMethodArguments()
        {
            AssertWrites(
                "{method(first, second, third",
                js =>
                {
                    js.WriteStartJsObject();
                    js.WriteStartJsMethodDefinition("method");
                    js.WriteJsMethodArgument("first");
                    js.WriteJsMethodArgument("second");
                    js.WriteJsMethodArgument("third");
                });
        }

        [Fact]
        public void TestWriteEndMethodDefinitionClosesMethod()
        {
            AssertWrites(
                "{method() {}",
                js =>
                {
                    js.WriteStartJsObject();
                    js.WriteStartJsMethodDefinition("method");
                    js.WriteEndJsMethodDefinition();
                });
        }

        [Fact]
        public void TestWriteEndMethodDefinitionClosesAfterArguments()
        {
            AssertWrites(
                "{method(first) {}}",
                js =>
                {
                    js.WriteStartJsObject();
                    js.WriteStartJsMethodDefinition("method");
                    js.WriteJsMethodArgument("first");
                    js.WriteEndJsMethodDefinition();
                    js.WriteEndJsObject();
                });

            AssertWrites(
                "{method(first, second, third) {}}",
                js =>
                {
                    js.WriteStartJsObject();
                    js.WriteStartJsMethodDefinition("method");
                    js.WriteJsMethodArgument("first");
                    js.WriteJsMethodArgument("second");
                    js.WriteJsMethodArgument("third");
                    js.WriteEndJsMethodDefinition();
                    js.WriteEndJsObject();
                });
        }

        [Fact]
        public void TestWriteObjectWithPropertyAndMethod()
        {
            AssertWrites(
                "{property: 1, method(first) {}}",
                js =>
                {
                    js.WriteStartJsObject();
                    js.WriteStartJsObjectProperty("property");
                    js.WriteJsValue(1);
                    js.WriteStartJsMethodDefinition("method");
                    js.WriteJsMethodArgument("first");
                    js.WriteEndJsMethodDefinition();
                    js.WriteEndJsObject();
                });
        }

        [Fact]
        public void TestWriteVariableDeclarationStatementUsesCorrectSyntax()
        {
            AssertWrites(
                "var name = ",
                js =>
                {
                    js.WriteStartVariableDeclarationStatement("name");
                });
        }
    }
}
