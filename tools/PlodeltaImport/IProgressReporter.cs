namespace PlodeltaImport
{
    public interface IProgressReporter
    {
        void ReportProgress(int percentage, string status);
        void ReportComplete();
        void ReportError(string message);
    }
}