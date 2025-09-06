namespace PlodeltaImport.Reporting
{
    internal sealed class ConsoleProgressReporter : IProgressReporter
    {
        public void ReportProgress(int percentage, string status) => Console.WriteLine(status);

        public void ReportComplete() => Console.WriteLine("Complete");

        public void ReportError(string message) => Console.WriteLine(message);
    }
}