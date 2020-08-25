using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsClassFieldDeclarationsSyntax : JsStatementSyntax
    {
        public JsAssignmentExpressionSyntax[] FieldAssignments { get; }

        public JsClassFieldDeclarationsSyntax(JsAssignmentExpressionSyntax[] fieldAssignments)
        {
            FieldAssignments = fieldAssignments;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(FieldAssignments);
        }
    }
}
