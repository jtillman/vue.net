namespace TesterApp
{
    public interface ICompiler<TIN, TOUT>
    {
        TOUT Compile(TIN input);
    }
}
