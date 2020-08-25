namespace Vue.Net.SampleWebApp.Pages.Shared.Components
{
    using Vue.Net;

    public class ApplicationComponent : VueModel
    {
        public string LabelText { get; set; }

        public string UserInput { get; set; }

        public int Count { get; set; }

        public void IncrementCount() => Count++;

        public void SetLabel() => LabelText = $"Hello, {UserInput}";
    }
}
