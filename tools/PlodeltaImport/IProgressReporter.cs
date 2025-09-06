//  Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
namespace PlodeltaImport
{
    public interface IProgressReporter
    {
        void ReportProgress(int percentage, string status);
        void ReportComplete();
        void ReportError(string message);
    }
}