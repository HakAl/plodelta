using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PloDeltaImport
{
    // Custom Exceptions
    public class InstallerException : Exception
    {
        public InstallerErrorCode ErrorCode { get; }

        public InstallerException(InstallerErrorCode errorCode, string message, Exception innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }

    public enum InstallerErrorCode
    {
        DownloadFailed,
        InvalidFile,
        ExistingInstallation,
        InstallationFailed,
        PermissionDenied,
        FileInUse,
        Unsupported16Bit,
        CancelledByUser
    }

    // Configuration
    public class InstallerConfig
    {
        public string RedirectPageUrl { get; set; } = "https://www.holdemmanager.com/hm3/download.php?download=true";
        public string ReportDownloadUrl { get; set; } = "https://hakal.github.io/plodelta/static/media/Home_Saved%20Reports_GTOStatAnalyzer.1d4f528bd07b1b407a2b.report";
        public string InstallPath { get; set; } = @"C:\Program Files\Max Value Software\Holdem Manager 3";
        public string OldInstallPath { get; set; } = @"C:\Program Files (x86)\Holdem Manager 3";
        public string ProductCode { get; set; } = "{F1A0512A-1DDC-4C61-887E-20A9F275A03A}";
        public int DownloadTimeoutMinutes { get; set; } = 10;
        public long MinFileSizeBytes { get; set; } = 1_000_000;
        public int RedirectDelaySeconds { get; set; } = 3;
    }

    // Constants
    public static class InstallerConstants
    {
        public const string ProductName = "Holdem Manager 3";
        public const string CompanyName = "Max Value Software";
        public const string ReportFileName = "Home_Saved Reports_GTOStatAnalyzer.report";
        public static readonly string[] PossibleUninstallPaths = {
            @"C:\Program Files (x86)\Holdem Manager 3\uninstall.exe",
            @"C:\Program Files (x86)\Holdem Manager 3\unins000.exe",
            @"C:\Program Files (x86)\Max Value Software\Holdem Manager 3\uninstall.exe",
            @"C:\Program Files (x86)\Max Value Software\Holdem Manager 3\unins000.exe"
        };
    }

    // Interfaces
    public interface ILogger
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message, Exception ex = null);
    }

    public interface IProgressReporter
    {
        void ReportProgress(int percentage, string status);
        void ReportComplete();
        void ReportError(string message);
    }

    // Implementations
    public class ConsoleLogger : ILogger
    {
        public void LogInfo(string message) => Console.WriteLine($"[INFO] {DateTime.Now:HH:mm:ss} {message}");
        public void LogWarning(string message) => Console.WriteLine($"[WARN] {DateTime.Now:HH:mm:ss} {message}");
        public void LogError(string message, Exception ex = null) =>
            Console.WriteLine($"[ERROR] {DateTime.Now:HH:mm:ss} {message}{(ex != null ? $": {ex.Message}" : "")}");
    }

    public class ConsoleProgressReporter : IProgressReporter
    {
        public void ReportProgress(int percentage, string status) => Console.WriteLine($"[{percentage}%] {status}");
        public void ReportComplete() => Console.WriteLine("Installation completed successfully!");
        public void ReportError(string message) => Console.WriteLine($"Error: {message}");
    }

    // Main Installer Class
    public class HoldemManagerInstaller
    {
        private readonly HttpClient _httpClient;
        private readonly InstallerConfig _config;
        private readonly ILogger _logger;
        private readonly IProgressReporter _progressReporter;

        public HoldemManagerInstaller(InstallerConfig? config, ILogger? logger, IProgressReporter? progressReporter)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _progressReporter = progressReporter ?? throw new ArgumentNullException(nameof(progressReporter));

            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            };

            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            _httpClient.Timeout = TimeSpan.FromMinutes(_config.DownloadTimeoutMinutes);
        }

        public async Task<bool> InstallIfMissingAsync(CancellationToken cancellationToken = default)
        {
            if (IsHm3Installed()) return true;

            _logger.LogInfo("Holdem Manager 3 is not installed. Starting installation process.");

            var result = MessageBox.Show(
                $"{InstallerConstants.ProductName} is not installed.\n\n" +
                "Click OK to download and install it (~170 MB).",
                "Plodelta Setup", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (result != DialogResult.OK)
            {
                _logger.LogWarning("Installation cancelled by user.");
                return false;
            }

            try
            {
                await InstallInternalAsync(cancellationToken);
                _progressReporter.ReportComplete();
                return true;
            }
            catch (InstallerException ex)
            {
                _logger.LogError($"Installation failed: {ex.Message}", ex);
                _progressReporter.ReportError(ex.Message);
                return false;
            }
        }

        public async Task InstallAsync(CancellationToken cancellationToken = default)
        {
            await InstallInternalAsync(cancellationToken);
        }

        private async Task InstallInternalAsync(CancellationToken cancellationToken = default)
        {
            _progressReporter.ReportProgress(0, "Starting installation...");

            var exeUrl = await FetchHm3InstallerUrlAsync(cancellationToken);
            var tempSetup = await DownloadInstallerAsync(exeUrl, cancellationToken);

            ValidateInstaller(tempSetup, out bool isValidExe, out bool isValidMsi, out bool is16Bit);

            if (is16Bit)
            {
                Handle16BitInstaller(tempSetup);
                return;
            }

            await HandleExistingInstallationAsync(cancellationToken);
            await RunInstallerAsync(tempSetup, isValidExe, isValidMsi, cancellationToken);

            _progressReporter.ReportProgress(100, "Installation completed!");
            _logger.LogInfo("Installation completed successfully!");
        }

        // Add this non-generic version to the HoldemManagerInstaller class
        private async Task WithRetryAsync(Func<Task> operation, int maxRetries = 3, int delayMs = 1000, CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    await operation();
                    return;
                }
                catch (Exception ex) when (i < maxRetries - 1 && !cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning($"Operation failed (attempt {i + 1}/{maxRetries}): {ex.Message}");
                    await Task.Delay(delayMs, cancellationToken);
                }
            }

            // Last attempt - let the exception propagate
            await operation();
        }

        public async Task DownloadReportAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInfo("Downloading GTO report...");
            _progressReporter.ReportProgress(0, "Initializing download...");

            string targetDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                InstallerConstants.CompanyName, InstallerConstants.ProductName, "3.0", "Reports", "Saved");

            Directory.CreateDirectory(targetDir);
            string destPath = Path.Combine(targetDir, InstallerConstants.ReportFileName);

            try
            {
                await WithRetryAsync(async () =>  // Now using the non-generic version
                {
                    using var response = await _httpClient.GetAsync(_config.ReportDownloadUrl,
                        HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                    response.EnsureSuccessStatusCode();

                    var contentLength = response.Content.Headers.ContentLength ?? 0;
                    _progressReporter.ReportProgress(10, "Downloading report...");

                    await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    await using var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write);
                    await stream.CopyToAsync(fileStream, cancellationToken);

                    _progressReporter.ReportProgress(100, "Download completed!");
                }, maxRetries: 3, delayMs: 2000, cancellationToken);

                MessageBox.Show(
                    $"✔ Downloaded & saved to\n{destPath}\n\nRefresh Plodelta in your browser.",
                    "Plodelta Import", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _logger.LogInfo($"Report downloaded successfully to: {destPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Report download failed: {ex.Message}", ex);
                MessageBox.Show($"Download failed:\n{ex.Message}", "Plodelta",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private async Task<T> WithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3, int delayMs = 1000, CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex) when (i < maxRetries - 1 && !cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning($"Operation failed (attempt {i + 1}/{maxRetries}): {ex.Message}");
                    await Task.Delay(delayMs, cancellationToken);
                }
            }

            return await operation();
        }

        private bool IsHm3Installed()
        {
            _logger.LogInfo("Checking if Holdem Manager 3 is installed...");

            string expectedExe = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                InstallerConstants.CompanyName, InstallerConstants.ProductName, "HoldemManager.exe");

            if (File.Exists(expectedExe))
            {
                _logger.LogInfo($"Found installation at: {expectedExe}");
                return true;
            }

            // Check registry
            var registryPaths = new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (var registryPath in registryPaths)
            {
                using var key = Registry.LocalMachine.OpenSubKey(registryPath);
                if (key != null)
                {
                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        using var subKey = key.OpenSubKey(subKeyName);
                        if (subKey?.GetValue("DisplayName")?.ToString()?.Contains(InstallerConstants.ProductName) == true)
                        {
                            _logger.LogInfo($"Found registry entry for {InstallerConstants.ProductName}");
                            return true;
                        }
                    }
                }
            }

            _logger.LogInfo("Holdem Manager 3 is not installed");
            return false;
        }

        private async Task<Uri> FetchHm3InstallerUrlAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInfo("Fetching installer URL...");
            _progressReporter.ReportProgress(5, "Getting download URL...");

            var response = await _httpClient.GetAsync(_config.RedirectPageUrl,
                HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            Uri? url = response.RequestMessage?.RequestUri;
            if (url == null)
            {
                throw new InstallerException(InstallerErrorCode.DownloadFailed, "No installer URL returned.");
            }

            _logger.LogInfo($"Found installer URL: {url}");
            return url;
        }

        private async Task<string> DownloadInstallerAsync(Uri exeUrl, CancellationToken cancellationToken = default)
        {
            _progressReporter.ReportProgress(10, "Downloading installer...");

            // First, get the download page
            using var response = await _httpClient.GetAsync(exeUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (string.IsNullOrEmpty(contentType) || !contentType.Contains("text/html"))
            {
                throw new InstallerException(InstallerErrorCode.DownloadFailed,
                    $"Expected HTML page but got {contentType ?? "unknown content type"}");
            }

            string htmlContent = await response.Content.ReadAsStringAsync(cancellationToken) ?? string.Empty;
            _logger.LogInfo($"Download page size: {htmlContent.Length} bytes");

            // Parse the HTML to find the actual download URL
            string? downloadUrl = ExtractDownloadUrl(htmlContent) ?? FindExeLink(htmlContent);
            if (downloadUrl == null)
            {
                throw new InstallerException(InstallerErrorCode.DownloadFailed,
                    "Could not find download URL in the page. The page structure might have changed.");
            }

            _logger.LogInfo($"Found download URL: {downloadUrl}");

            // Make the URL absolute if it's relative
            if (!Uri.IsWellFormedUriString(downloadUrl, UriKind.Absolute))
            {
                downloadUrl = new Uri(exeUrl, downloadUrl).AbsoluteUri;
            }

            // Wait for the redirect to be ready
            _logger.LogInfo("Waiting for download to be ready...");
            await Task.Delay(TimeSpan.FromSeconds(_config.RedirectDelaySeconds), cancellationToken);

            // Determine file extension
            string fileExt = GetFileExtensionFromUrl(downloadUrl);
            string tempSetup = Path.Combine(Path.GetTempPath(), $"HM3-Setup{fileExt}");

            // Now download the actual installer
            _progressReporter.ReportProgress(20, "Downloading installer file...");

            using var downloadResponse = await _httpClient.GetAsync(downloadUrl,
                HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            downloadResponse.EnsureSuccessStatusCode();

            var downloadContentType = downloadResponse.Content.Headers.ContentType?.MediaType;
            var contentLength = downloadResponse.Content.Headers.ContentLength;
            _logger.LogInfo($"Download response - Content-Type: {downloadContentType ?? "null"}, " +
                $"Content-Length: {contentLength?.ToString() ?? "null"}");

            // Check if we're getting HTML again (error)
            if (!string.IsNullOrEmpty(downloadContentType) && downloadContentType.Contains("text/html"))
            {
                string errorHtml = await downloadResponse.Content.ReadAsStringAsync(cancellationToken) ?? string.Empty;
                string errorTitle = ExtractHtmlTitle(errorHtml) ?? "Download Error";
                string errorBody = ExtractHtmlBody(errorHtml) ?? (errorHtml.Length > 200 ?
                    errorHtml.Substring(0, 200) + "..." : errorHtml);

                throw new InstallerException(InstallerErrorCode.DownloadFailed,
                    $"Server returned an error page: {errorTitle}");
            }

            // Stream the installer to file
            _progressReporter.ReportProgress(40, "Saving installer file...");
            await using (var stream = await downloadResponse.Content.ReadAsStreamAsync(cancellationToken))
            await using (var fs = new FileStream(tempSetup, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fs, cancellationToken);
            }

            // Validation checks
            long bytesWritten = new FileInfo(tempSetup).Length;
            _logger.LogInfo($"Downloaded {bytesWritten} bytes");

            if (bytesWritten < _config.MinFileSizeBytes)
            {
                string content = await GetFileContentForDebugging(tempSetup);
                throw new InstallerException(InstallerErrorCode.DownloadFailed,
                    $"Download too small ({bytesWritten} bytes). Content: {content}");
            }

            _progressReporter.ReportProgress(50, "Download completed!");
            return tempSetup;
        }

        private void ValidateInstaller(string filePath, out bool isValidExe, out bool isValidMsi, out bool is16Bit)
        {
            _logger.LogInfo("Validating installer file...");
            _progressReporter.ReportProgress(55, "Validating installer...");

            byte[] header = File.ReadAllBytes(filePath).Take(64).ToArray();
            string hexDump = BitConverter.ToString(header).Replace("-", " ");

            isValidExe = header.Length >= 2 && header[0] == 0x4D && header[1] == 0x5A;
            isValidMsi = header.Length >= 8 && header[0] == 0xD0 && header[1] == 0xCF &&
                         header[2] == 0x11 && header[3] == 0xE0 &&
                         header[4] == 0xA1 && header[5] == 0xB1 &&
                         header[6] == 0x1A && header[7] == 0xE1;

            is16Bit = false;
            if (isValidExe && header.Length >= 64)
            {
                int peHeaderOffset = BitConverter.ToInt32(header, 0x3C);
                if (peHeaderOffset > 0 && peHeaderOffset < header.Length - 4)
                {
                    if (header[peHeaderOffset] == 0x4E && header[peHeaderOffset + 1] == 0x45)
                    {
                        is16Bit = true;
                    }
                }
            }

            if (!isValidExe && !isValidMsi)
            {
                string fileType = DetectFileType(header);
                throw new InstallerException(InstallerErrorCode.InvalidFile,
                    $"Downloaded file is not a valid installer. Detected type: {fileType}");
            }
        }

        private string DetectFileType(byte[] header)
        {
            if (header.Length >= 2)
            {
                if (header[0] == 0x25 && header[1] == 0x50) return "PDF";
                if (header[0] == 0x50 && header[1] == 0x4B) return "ZIP";
                if (header[0] == 0x7F && header[1] == 0x45) return "ELF";
                if (header[0] == 0xCA && header[1] == 0xFE) return "Java Class";
                if (header[0] == 0x3C && header[1] == 0x21) return "HTML/XML";
                if (header[0] == 0xFF && header[1] == 0xD8) return "JPEG";
                if (header[0] == 0x89 && header[1] == 0x50) return "PNG";
                if (header[0] == 0xD0 && header[1] == 0xCF) return "Structured Storage (possibly MSI)";
            }
            return "Unknown";
        }

        private void Handle16BitInstaller(string filePath)
        {
            bool is64Bit = Environment.Is64BitOperatingSystem;
            string message = $"The downloaded installer is a 16-bit application which is not supported on modern Windows.\n\n" +
                           $"File: {Path.GetFileName(filePath)}\n" +
                           $"System: {(is64Bit ? "64-bit" : "32-bit")} Windows\n\n";

            if (is64Bit)
            {
                message += "64-bit Windows cannot run 16-bit applications. Options:\n\n" +
                          "1. Contact the vendor for a 64-bit compatible installer\n" +
                          "2. Install and run in a 32-bit virtual machine\n" +
                          "3. Use Windows 10/11's 16-bit emulation (limited support)\n\n" +
                          "Would you like to save the installer for manual installation later?";
            }
            else
            {
                message += "Even on 32-bit Windows, 16-bit applications have limited compatibility.\n\n" +
                          "Would you like to save the installer for manual installation later?";
            }

            var result = MessageBox.Show(message, "Unsupported 16-bit Application",
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Error);

            if (result == DialogResult.Yes)
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string savedPath = Path.Combine(desktopPath, Path.GetFileName(filePath));
                File.Copy(filePath, savedPath, true);
                MessageBox.Show($"Installer saved to:\n{savedPath}\n\nYou can try to run it manually or in compatibility mode.",
                              "Installer Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            throw new InstallerException(InstallerErrorCode.Unsupported16Bit, "16-bit installer is not supported.");
        }

        private async Task HandleExistingInstallationAsync(CancellationToken cancellationToken = default)
        {
            bool hasExistingInstallation = Directory.Exists(_config.OldInstallPath) ||
                                         Directory.Exists(_config.InstallPath);

            if (!hasExistingInstallation) return;

            _logger.LogInfo("Found existing installation. Asking user for action...");
            _progressReporter.ReportProgress(60, "Checking existing installation...");

            string message = $"An existing installation of {InstallerConstants.ProductName} was found.\n\n" +
                             "This may cause the new installation to fail.\n\n" +
                             "Options:\n" +
                             "1. Try to uninstall existing version first (recommended)\n" +
                             "2. Proceed with installation anyway (may fail)\n" +
                             "3. Cancel installation\n\n" +
                             "What would you like to do?";

            var result = MessageBox.Show(message, "Existing Installation Found",
                                       MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                _progressReporter.ReportProgress(65, "Uninstalling existing version...");
                bool uninstallSuccess = await TryUninstallExistingVersionAsync(cancellationToken);

                if (!uninstallSuccess)
                {
                    message = "Failed to uninstall the existing version automatically.\n\n" +
                              "You can try to uninstall it manually through 'Programs and Features' in Control Panel,\n" +
                              "then run this installer again.\n\n" +
                              "Would you like to proceed with the installation anyway?";

                    var proceedResult = MessageBox.Show(message, "Uninstall Failed",
                                                      MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (proceedResult == DialogResult.No)
                    {
                        throw new InstallerException(InstallerErrorCode.CancelledByUser,
                            "Installation cancelled by user due to existing version.");
                    }
                }
            }
            else if (result == DialogResult.Cancel)
            {
                throw new InstallerException(InstallerErrorCode.CancelledByUser, "Installation cancelled by user.");
            }
        }

        private async Task<bool> TryUninstallExistingVersionAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInfo("Attempting to uninstall existing version...");

            // Close any running HM3 processes
            await CloseRunningProcessesAsync(cancellationToken);

            // Try to uninstall using Windows Installer
            if (await UninstallUsingMsiexecAsync(cancellationToken))
            {
                return true;
            }

            // Try to uninstall using uninstaller executable
            if (await UninstallUsingExeAsync(cancellationToken))
            {
                return true;
            }

            // Try to rename the old directories
            return await RenameOldDirectoriesAsync(cancellationToken);
        }

        private async Task CloseRunningProcessesAsync(CancellationToken cancellationToken = default)
        {
            Process[] processes = Process.GetProcessesByName("HM3");
            if (processes.Length > 0)
            {
                _logger.LogInfo($"Found {processes.Length} running HM3 processes, attempting to close them");
                foreach (var process in processes)
                {
                    try
                    {
                        process.Kill();
                        await Task.Delay(1000, cancellationToken);
                    }
                    catch { }
                }
            }
        }

        private async Task<bool> UninstallUsingMsiexecAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInfo("Attempting to uninstall using Windows Installer...");

                var psi = new ProcessStartInfo
                {
                    FileName = "msiexec.exe",
                    Arguments = $"/x {_config.ProductCode} /quiet /norestart",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    Verb = "runas"
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    await proc.WaitForExitAsync(cancellationToken);

                    if (proc.ExitCode == 0)
                    {
                        _logger.LogInfo("Successfully uninstalled using Windows Installer");
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning($"Windows Installer uninstall failed with exit code: {proc.ExitCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uninstalling with msiexec: {ex.Message}", ex);
            }

            return false;
        }

        private async Task<bool> UninstallUsingExeAsync(CancellationToken cancellationToken = default)
        {
            foreach (string uninstallPath in InstallerConstants.PossibleUninstallPaths)
            {
                if (File.Exists(uninstallPath))
                {
                    try
                    {
                        _logger.LogInfo($"Found uninstaller at {uninstallPath}, attempting to run it...");

                        var psi = new ProcessStartInfo
                        {
                            FileName = uninstallPath,
                            Arguments = "/S",
                            UseShellExecute = true,
                            CreateNoWindow = false,
                            Verb = "runas"
                        };

                        using var proc = Process.Start(psi);
                        if (proc != null)
                        {
                            await proc.WaitForExitAsync(cancellationToken);

                            if (proc.ExitCode == 0)
                            {
                                _logger.LogInfo("Successfully uninstalled using uninstaller executable");
                                return true;
                            }
                            else
                            {
                                _logger.LogWarning($"Uninstaller executable failed with exit code: {proc.ExitCode}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error uninstalling with {uninstallPath}: {ex.Message}", ex);
                    }
                }
            }

            return false;
        }

        private async Task<bool> RenameOldDirectoriesAsync(CancellationToken cancellationToken = default)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            foreach (string installPath in new[] { _config.OldInstallPath, _config.InstallPath })
            {
                if (Directory.Exists(installPath))
                {
                    try
                    {
                        string backupPath = installPath + "_backup_" + timestamp;
                        Directory.Move(installPath, backupPath);
                        _logger.LogInfo($"Renamed installation to {backupPath}");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to rename {installPath}: {ex.Message}", ex);
                    }
                }
            }

            return false;
        }

        private async Task RunInstallerAsync(string filePath, bool isValidExe, bool isValidMsi, CancellationToken cancellationToken = default)
        {
            _progressReporter.ReportProgress(70, "Starting installer...");

            string logFile = Path.Combine(Path.GetTempPath(), "HM3-Install.log");
            ProcessStartInfo psi;

            if (isValidExe)
            {
                psi = new ProcessStartInfo
                {
                    FileName = filePath,
                    Arguments = $@"/S /D=""{_config.InstallPath}""",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WorkingDirectory = Path.GetTempPath(),
                    Verb = "runas"
                };
            }
            else if (isValidMsi)
            {
                psi = new ProcessStartInfo
                {
                    FileName = "msiexec.exe",
                    Arguments = $@"/i ""{filePath}"" /quiet /norestart TARGETDIR=""{_config.InstallPath}"" /l*vx ""{logFile}""",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WorkingDirectory = Path.GetTempPath(),
                    Verb = "runas"
                };
            }
            else
            {
                throw new InstallerException(InstallerErrorCode.InvalidFile, "Unknown installer type");
            }

            _logger.LogInfo($"Starting installer with command: {psi.FileName} {psi.Arguments}");
            _progressReporter.ReportProgress(75, "Running installer...");

            try
            {
                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    await proc.WaitForExitAsync(cancellationToken);

                    if (proc.ExitCode != 0)
                    {
                        string errorMessage = await GetInstallerErrorMessage(proc.ExitCode, logFile, isValidMsi);
                        throw new InstallerException(InstallerErrorCode.InstallationFailed, errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("16-bit") || ex.Message.Contains("unsupported"))
                {
                    throw new InstallerException(InstallerErrorCode.Unsupported16Bit,
                        "16-bit installer is not supported.", ex);
                }
                throw new InstallerException(InstallerErrorCode.InstallationFailed,
                    "Failed to run installer.", ex);
            }
        }

        private async Task<string> GetInstallerErrorMessage(int exitCode, string logFile, bool isValidMsi)
        {
            string errorMessage = $"Installer failed with exit code: {exitCode}";

            if (isValidMsi)
            {
                switch (exitCode)
                {
                    case 1603:
                        errorMessage += "\n\nError 1603: Fatal error during installation.\n\n" +
                                      "This is commonly caused by:\n" +
                                      "- Existing installation files that couldn't be removed\n" +
                                      "- Insufficient permissions\n" +
                                      "- Files from the previous installation are in use\n\n" +
                                      "Solutions:\n" +
                                      "1. Manually uninstall the old version first\n" +
                                      "2. Close all HM3 processes before installing\n" +
                                      "3. Run this application as administrator";
                        break;
                    case 1618:
                        errorMessage += "\n\nError 1618: Another installation is already in progress.\n" +
                                      "Please wait for the other installation to complete and try again.";
                        break;
                    case 1602:
                        errorMessage += "\n\nError 1602: Installation was cancelled by the user.";
                        break;
                    case 1601:
                        errorMessage += "\n\nError 1601: Windows Installer service could not be accessed.\n" +
                                      "Please ensure the Windows Installer service is running.";
                        break;
                }

                // Check log file for additional errors
                if (File.Exists(logFile) && new FileInfo(logFile).Length > 0)
                {
                    try
                    {
                        var logLines = await File.ReadAllLinesAsync(logFile);
                        var errorLines = logLines.Where(line =>
                            line.Contains("Error ") ||
                            line.Contains("failed") ||
                            line.Contains("aborting") ||
                            line.Contains("return value 3") ||
                            line.Contains("rollback skipped")).Take(10).ToArray();

                        if (errorLines.Length > 0)
                        {
                            errorMessage += "\n\nRecent errors from installation log:\n" +
                                            string.Join("\n", errorLines);
                        }
                    }
                    catch { }
                }

                errorMessage += $"\n\nDetailed log saved to: {logFile}";
            }

            return errorMessage;
        }

        private async Task<string> GetFileContentForDebugging(string filePath)
        {
            try
            {
                string content = await File.ReadAllTextAsync(filePath);
                return content.Length > 500 ? content.Substring(0, 500) + "..." : content;
            }
            catch
            {
                byte[] bytes = await File.ReadAllBytesAsync(filePath);
                return BitConverter.ToString(bytes.Take(100).ToArray()).Replace("-", " ");
            }
        }

        private string GetFileExtensionFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return ".exe";

            url = url.ToLowerInvariant();
            if (url.Contains(".msi")) return ".msi";
            if (url.Contains(".exe")) return ".exe";

            return ".exe";
        }

        // HTML parsing methods
        private static string? ExtractDownloadUrl(string html)
        {
            if (string.IsNullOrEmpty(html)) return null;

            var patterns = new[]
            {
                (Regex: new Regex(@"<meta\s+http-equiv\s*=\s*[""']refresh[""']\s+content\s*=\s*[""']\d+;\s*url\s*=\s*([^""']+)", RegexOptions.IgnoreCase), GroupIndex: 1),
                (Regex: new Regex(@"window\.location\s*=\s*[""']([^""']+)[""']", RegexOptions.IgnoreCase), GroupIndex: 1),
                (Regex: new Regex(@"<a\s+[^>]*href\s*=\s*[""']([^""']*\.(?:exe|msi))[""'][^>]*>", RegexOptions.IgnoreCase), GroupIndex: 1)
            };

            foreach (var (regex, groupIndex) in patterns)
            {
                var match = regex.Match(html);
                if (match.Success && match.Groups.Count > groupIndex)
                {
                    var group = match.Groups[groupIndex];
                    if (group != null && group.Value != null)
                    {
                        return group.Value.Trim();
                    }
                }
            }

            return null;
        }

        private static string? FindExeLink(string html)
        {
            if (string.IsNullOrEmpty(html)) return null;

            var match = Regex.Match(html, @"[""']([^""']*\.(?:exe|msi))[""']", RegexOptions.IgnoreCase);
            if (match.Success && match.Groups.Count > 1)
            {
                var group = match.Groups[1];
                if (group != null && group.Value != null)
                {
                    return group.Value.Trim();
                }
            }

            return null;
        }

        private static string? ExtractHtmlTitle(string html)
        {
            if (string.IsNullOrEmpty(html)) return null;

            try
            {
                var match = Regex.Match(html, @"<title>\s*(.*?)\s*</title>", RegexOptions.IgnoreCase);
                if (match.Success && match.Groups.Count > 1)
                {
                    var group = match.Groups[1];
                    if (group != null && group.Value != null)
                    {
                        return group.Value.Trim();
                    }
                }
            }
            catch { }

            return null;
        }

        private static string? ExtractHtmlBody(string html)
        {
            if (string.IsNullOrEmpty(html)) return null;

            try
            {
                var match = Regex.Match(html, @"<body[^>]*>\s*(.*?)\s*</body>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (match.Success && match.Groups.Count > 1)
                {
                    var group = match.Groups[1];
                    if (group != null && group.Value != null)
                    {
                        return group.Value.Trim();
                    }
                }
            }
            catch { }

            return null;
        }
    }

    // Main Program
    internal static class Program
    {
        [STAThread]
        static async Task Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                var config = new InstallerConfig();
                var logger = new ConsoleLogger();
                var progressReporter = new ConsoleProgressReporter();
                var installer = new HoldemManagerInstaller(config!, logger!, progressReporter!);

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
                MessageBox.Show($"Error: {ex.Message}", "Plodelta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(ex.HResult);
            }
        }
    }
}