using System;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Importer
{
    internal static class Program
    {
        [STAThread]
        static async Task Main(string[] args)
        {
        //             if (!IsHm3Installed())
        //             {
        //                 MessageBox.Show("Holdem Manager 3 does not appear to be installed.\n" +
        //                                 "Please install HM3 first.", "PloDelta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                 return;
        //             }

        await InstallHm3IfMissingAsync();
        // … proceed with downloading the report …

        const string URL = "https://hakal.github.io/plodelta/static/media/Home_Saved%20Reports_GTOStatAnalyzer.1d4f528bd07b1b407a2b.report";

        string targetDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Max Value Software", "Holdem Manager", "3.0", "Reports", "Saved");

        Directory.CreateDirectory(targetDir);

        // keep the original file name the web app expects
        string fileName = "Home_Saved Reports_GTOStatAnalyzer.report";
        string destPath   = Path.Combine(targetDir, fileName);

        using var http = new HttpClient();
        try
        {
            var bytes = await http.GetByteArrayAsync(URL);
            await File.WriteAllBytesAsync(destPath, bytes);

            MessageBox.Show($"✔ Downloaded & saved to\n{destPath}\n\nRefresh Plodelta in your browser.",
                            "Plodelta Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Download failed:\n{ex.Message}", "Plodelta",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    static bool IsHm3Installed()
    {
        // 1. quick file-system check
        string expectedExe = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "Max Value Software", "Holdem Manager 3", "HoldemManager.exe");

        if (File.Exists(expectedExe)) return true;

        // 2. 64-bit registry hive
        using var reg = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
        if (reg != null)
        {
            foreach (string sub in reg.GetSubKeyNames())
            {
                using var key = reg.OpenSubKey(sub);
                if (key?.GetValue("DisplayName")?.ToString()?.Contains("Holdem Manager 3") == true)
                    return true;
            }
        }

        // 3. 32-bit hive on 64-bit OS
        using var reg32 = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
        if (reg32 != null)
        {
            foreach (string sub in reg32.GetSubKeyNames())
            {
                using var key32 = reg32.OpenSubKey(sub);
                if (key32?.GetValue("DisplayName")?.ToString()?.Contains("Holdem Manager 3") == true)
                    return true;
            }
        }

        return false;
    }

    // 1.  Scraper that returns the current MSI URL
    static async Task<Uri> FetchHm3InstallerUrlAsync(HttpClient http, CancellationToken ct = default)
    {
        const string REDIRECT_PAGE = "https://www.holdemmanager.com/hm3/download.php?download=true";
        var response = await http.GetAsync(REDIRECT_PAGE, HttpCompletionOption.ResponseHeadersRead, ct);
        Uri? url = response.RequestMessage?.RequestUri;
        if (url is null)
            throw new InvalidOperationException("No installer URL returned.");

        return url;
    }

    // 2.  Full installer flow (calls #1, uses msiexec)
    static async Task InstallHm3IfMissingAsync()
    {
        if (IsHm3Installed()) return;

        string tempSetup = Path.Combine(Path.GetTempPath(), "HM3-Setup.exe");

        var answer = MessageBox.Show(
                "Holdem Manager 3 is not installed.\n\n" +
                "Click OK to download and install it (~170 MB).",
                "Plodelta Setup",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);

        if (answer != DialogResult.OK)        // user declined
            Environment.Exit(0);

        try
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

            Uri exeUrl = await FetchHm3InstallerUrlAsync(http);
            using var stream = await http.GetStreamAsync(exeUrl);
            using var fs = new FileStream(tempSetup, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fs);

            var psi = new ProcessStartInfo
            {
                FileName = tempSetup,
                Arguments = @"/S /D=""C:\Program Files\Max Value Software\Holdem Manager 3""",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(psi);
            proc?.WaitForExit();

            File.Delete(tempSetup);
            MessageBox.Show("Holdem Manager 3 has been installed.\nYou can now start your free 14-day trial.",
                            "Plodelta", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Install failed:\n{ex.Message}", "Plodelta",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1);
        }
    }
}
}