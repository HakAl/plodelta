//  Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
namespace PlodeltaImport
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message, Exception ex = null);
    }
}