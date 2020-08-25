namespace WebApplicationExample.Pages
{
    public class WebApplication
    {
        public int IntField = 4;
        public string Name { get; set; }

        public string Message { get { return $"Hello, {Name}!"; } set { Name = value; } }

        public int MessageLength { get { return Message.Length; } }

        public int InitializedNumber { get; } = 5;

        public int ExpressionNumber => 4;

        public void MethodCall()
        {
            var list = new WebApplication(3, Name);
            Name = nameof(MethodCall).ToString();
        }

        public WebApplication() { Name = "Empty"; }

        public WebApplication(int number) { Name = "Integer"; }

        public WebApplication(int number, string str) { Name = str; }
    }
}
