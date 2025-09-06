namespace PlodeltaImport.Reporting
{
    internal sealed class ConsoleProgressReporter : IProgressReporter
    {
        public void Report(string msg) => Console.WriteLine(msg);
    }
}