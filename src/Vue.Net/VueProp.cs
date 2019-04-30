namespace Vue.Net
{
    public class VueProp
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public bool? Required { get; set; }

        public object Default { get; set; }

        public VueMethod Validator { get; set; }
    }
}
