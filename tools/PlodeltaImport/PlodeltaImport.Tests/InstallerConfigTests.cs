using Xunit;

namespace PlodeltaImport.PlodeltaImport.Tests
{
    public class InstallerConfigTests
    {
        [Fact]
        public void DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var config = new InstallerConfig();

            // Assert
            Assert.Equal("https://www.holdemmanager.com/hm3/download.php?download=true", config.RedirectPageUrl);
            Assert.Equal("https://hakal.github.io/plodelta/static/media/Home_Saved%20Reports_GTOStatAnalyzer.1d4f528bd07b1b407a2b.report", config.ReportDownloadUrl);
            Assert.Equal(@"C:\Program Files\Max Value Software\Holdem Manager 3", config.InstallPath);
            Assert.Equal(@"C:\Program Files (x86)\Holdem Manager 3", config.OldInstallPath);
            Assert.Equal("{F1A0512A-1DDC-4C61-887E-20A9F275A03A}", config.ProductCode);
            Assert.Equal(10, config.DownloadTimeoutMinutes);
            Assert.Equal(1_000_000, config.MinFileSizeBytes);
            Assert.Equal(3, config.RedirectDelaySeconds);
        }

        [Fact]
        public void CanModifyValues()
        {
            // Arrange
            var config = new InstallerConfig();

            // Act
            config.RedirectPageUrl = "https://example.com";
            config.InstallPath = @"D:\Apps\HM3";
            config.DownloadTimeoutMinutes = 5;

            // Assert
            Assert.Equal("https://example.com", config.RedirectPageUrl);
            Assert.Equal(@"D:\Apps\HM3", config.InstallPath);
            Assert.Equal(5, config.DownloadTimeoutMinutes);
        }
    }
}