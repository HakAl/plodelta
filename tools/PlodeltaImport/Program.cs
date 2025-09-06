//  Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
using System;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using PlodeltaImport;

namespace PloDeltaImport
{

    internal static class Program
{
    [STAThread]
    static async Task Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        try
        {
            // NEW: adapters for the nested interfaces
            var config   = new InstallerConfig();
            var logger   = new ConsoleLoggerAdapter();      // ← your tiny adapter
            var progress = new ConsoleProgressAdapter();    // ← your tiny adapter
            var installer = new HoldemManagerInstaller(config, logger, progress);

            /*  =====  everything below is the original code  =====  */
            if (args.Length > 0 && args[0] == "/install")
            {
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    var ex = (Exception)e.ExceptionObject;
                    MessageBox.Show($"Install crash:\n{ex}", "Plodelta",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(-1);
                };

                try
                {
                    await installer.InstallAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Install error:\n{ex}", "Plodelta",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(ex.HResult);
                }
                return;
            }

            if (!await installer.InstallIfMissingAsync()) return;
            await installer.DownloadReportAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Plodelta",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(ex.HResult);
        }
    }
}

    
    // at bottom of Program.cs
    internal sealed class ConsoleLoggerAdapter : ILogger
    {
        public void LogInfo(string message) => Console.WriteLine($"[INFO] {DateTime.Now:HH:mm:ss} {message}");
        public void LogWarning(string message) => Console.WriteLine($"[WARN] {DateTime.Now:HH:mm:ss} {message}");
        public void LogError(string message, Exception ex = null) =>
            Console.WriteLine($"[ERROR] {DateTime.Now:HH:mm:ss} {message}{(ex != null ? $": {ex.Message}" : "")}");
    }
    internal sealed class ConsoleProgressAdapter  : IProgressReporter
    {
        public void ReportProgress(int percentage, string status) => Console.WriteLine($"[{percentage}%] {status}");
        public void ReportComplete() => Console.WriteLine("Installation completed successfully!");
        public void ReportError(string message) => Console.WriteLine($"Error: {message}");
    }
}