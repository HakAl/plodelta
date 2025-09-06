namespace PlodeltaImport.Logging
{
    internal sealed class ConsoleLogger : ILogger
    {
        public void LogInfo(string message) => Console.WriteLine(message);
        public void LogWarning(string message) => Console.WriteLine(message);
        public void LogError(string message, Exception ex = null) => Console.WriteLine(message);
    }
}