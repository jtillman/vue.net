namespace TesterApp
{
    public class CompilerPassthrough<TIN, TBETWEEN, TOUT> : ICompiler<TIN, TOUT>
    {
        public ICompiler<TIN, TBETWEEN> IngressCompiler { get; }

        public ICompiler<TBETWEEN, TOUT> EgressCompiler { get; }

        public CompilerPassthrough(
            ICompiler<TIN, TBETWEEN> ingressCompiler,
            ICompiler<TBETWEEN, TOUT> egressCompiler)
        {
            IngressCompiler = ingressCompiler;
            EgressCompiler = egressCompiler;
        }

        public TOUT Compile(TIN input) => EgressCompiler.Compile(IngressCompiler.Compile(input));
    }
}
