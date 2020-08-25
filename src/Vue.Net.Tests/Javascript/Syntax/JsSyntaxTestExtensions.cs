using System.IO;
using Vue.Net.Javascript.Syntax;
using Xunit;

namespace Vue.Net.Tests.Javascript.Syntax
{
    public static class JsSyntaxTestExtensions
    {
        public static void AssertWrites(this JsSyntax syntax, string expected)
        {
            using (var stringWriter = new StringWriter())
            {
                syntax.WriteTo(stringWriter);
                Assert.Equal(expected, stringWriter.ToString());
            }
        }
    }
}
