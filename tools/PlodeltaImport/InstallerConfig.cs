namespace PlodeltaImport;

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