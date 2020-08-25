using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vue.Net.Javascript.Syntax;

namespace TesterApp
{
    public class JsFieldMember : ITypeMember
    {
        public string Name { get; }

        public JsExpressionSyntax Initializer { get; }

        public JsFieldMember(string name, JsExpressionSyntax initializer)
        {
            this.Name = name;
            this.Initializer = initializer;
        }
    }

    public class JsPropertyMember : ITypeMember
    {
        public string Name { get; }

        public JsFunctionExpressionSyntax Getter { get; }

        public JsFunctionExpressionSyntax Setter { get; }

    }

    public interface ITypeMember
    {
        string Name { get; }
    }

    public class JsType
    {
        public string FullName { get; }

        public MemberCollection Members { get; }
            = new MemberCollection();

        public JsType(string fullName)
        {
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        }
    }

    public class CSharpClass
    {
        public string _name = "name", title = "fskjs";

        public string Name => this._name;
    }

    public class TodoViewModel
    {
        public string Title { get; set; }

        public bool IsCompleted { get; set; }

        public void SetTitle(string title)
        {
            Title = title;
        }
    }

    public class TodoPageViewModel
    {
        public List<TodoViewModel> TodoItems { get; }
    }

    public class TestClass
    {
        public string classField = "Initalized Value";

        public TestClass(string ctrParameter)
        {
            classField = ctrParameter; // AssignmentStatement, FieldAccessor, ParameterAccessor
        }

        public TestClass(int number)
        {
            _number = number;
        }

        public void TestLocalVariableEmptyStringArray()
        {
            var variable = new string[0];
        }

        public void TestLocationVariableInitializedStringArray()
        {
            var variable = new string[2] { "first", "second" };
        }

        public int DoMath(int x, int y)
        {
            var count = 4;
            x = count * y;
            var c = new TestClass(23);
            var xe = Task.Delay(count);
            xe.Wait();
            return x * y;
        }

        private int _number = 1;

        public int Number { get { return _number; } set { _number = value * 2; } }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var jsRuntime = new JavascriptRuntime()
                .AddType<TestClass>();

            var script = jsRuntime.ToString();
            Console.ReadLine();
        }
    }

}
