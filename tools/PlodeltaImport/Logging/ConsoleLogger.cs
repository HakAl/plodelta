namespace PlodeltaImport.Logging
{
    internal sealed class ConsoleLogger : ILogger
    {
        public void Log(string msg) => Console.WriteLine(msg);
    }
}