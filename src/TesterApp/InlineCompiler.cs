using System;

namespace TesterApp
{
    public class InlineCompiler<TIN, TOUT> : ICompiler<TIN, TOUT>
    {
        public Func<TIN,TOUT> CompileFunc{ get; }

        public InlineCompiler(Func<TIN,TOUT> compile)
        {
            CompileFunc = compile;
        }

        public TOUT Compile(TIN input) => CompileFunc(input);
    }
}
