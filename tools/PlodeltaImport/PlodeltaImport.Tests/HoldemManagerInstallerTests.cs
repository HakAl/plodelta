//  Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
using Xunit;
using Moq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Win32;
using PlodeltaImport.PlodeltaImport.Tests.Services;
using PlodeltaImport.PlodeltaImport.Tests.Models;

using PloDeltaImport;

namespace PlodeltaImport.PlodeltaImport.Tests
{
    public class HoldemManagerInstallerTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IProgressReporter> _mockProgressReporter;
        private readonly InstallerConfig _config;

        public HoldemManagerInstallerTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockProgressReporter = new Mock<IProgressReporter>();
            _config = new InstallerConfig();
        }

        [Fact]
        public void Constructor_WithNullConfig_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new HoldemManagerInstaller(null, _mockLogger.Object, _mockProgressReporter.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new HoldemManagerInstaller(_config, null, _mockProgressReporter.Object));
        }

        [Fact]
        public void Constructor_WithNullProgressReporter_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new HoldemManagerInstaller(_config, _mockLogger.Object, null));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task InstallIfMissingAsync_WhenHm3Installed_ReturnsCorrectValue(bool isInstalled)
        {
            // Arrange
            var installer = new Mock<HoldemManagerInstaller>(
                _config, _mockLogger.Object, _mockProgressReporter.Object) { CallBase = true };
            
            installer.Setup(i => i.IsHm3Installed()).Returns(isInstalled);

            // Act
            var result = await installer.Object.InstallIfMissingAsync();

            // Assert
            Assert.Equal(isInstalled, result);
            installer.Verify(i => i.IsHm3Installed(), Times.Once);
            
            if (!isInstalled)
            {
                installer.Verify(i => i.InstallInternalAsync(It.IsAny<CancellationToken>()), Times.Never);
            }
        }

        [Fact]
        public async Task InstallIfMissingAsync_WhenNotInstalledAndUserCancels_ReturnsFalse()
        {
            // Arrange
            var installer = new Mock<HoldemManagerInstaller>(
                _config, _mockLogger.Object, _mockProgressReporter.Object) { CallBase = true };
            
            installer.Setup(i => i.IsHm3Installed()).Returns(false);
            installer.Setup(i => i.InstallInternalAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new InstallerException(InstallerErrorCode.CancelledByUser, "Cancelled"));

            // Act
            var result = await installer.Object.InstallIfMissingAsync();

            // Assert
            Assert.False(result);
            _mockLogger.Verify(l => l.LogWarning("Installation cancelled by user."), Times.Once);
            _mockProgressReporter.Verify(p => p.ReportError("Cancelled"), Times.Once);
        }

        [Theory]
        [MemberData(nameof(InstallerTestData.ValidExeHeaders), MemberType = typeof(InstallerTestData))]
        public void ValidateInstaller_WithValidExeHeader_SetsCorrectFlags(byte[] header)
        {
            // Arrange
            var installer = new HoldemManagerInstaller(
                _config, _mockLogger.Object, _mockProgressReporter.Object);
            
            string tempFile = TestHelpers.CreateTempFileWithContent(header);

            try
            {
                // Act
                installer.ValidateInstaller(tempFile, out bool isValidExe, out bool isValidMsi, out bool is16Bit);

                // Assert
                Assert.True(isValidExe);
                Assert.False(isValidMsi);
                Assert.False(is16Bit);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Theory]
        [MemberData(nameof(InstallerTestData.ValidMsiHeaders), MemberType = typeof(InstallerTestData))]
        public void ValidateInstaller_WithValidMsiHeader_SetsCorrectFlags(byte[] header)
        {
            // Arrange
            var installer = new HoldemManagerInstaller(
                _config, _mockLogger.Object, _mockProgressReporter.Object);
            
            string tempFile = TestHelpers.CreateTempFileWithContent(header);

            try
            {
                // Act
                installer.ValidateInstaller(tempFile, out bool isValidExe, out bool isValidMsi, out bool is16Bit);

                // Assert
                Assert.False(isValidExe);
                Assert.True(isValidMsi);
                Assert.False(is16Bit);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Theory]
        [MemberData(nameof(InstallerTestData.SixteenBitHeaders), MemberType = typeof(InstallerTestData))]
        public void ValidateInstaller_With16BitHeader_SetsCorrectFlags(byte[] header)
        {
            // Arrange
            var installer = new HoldemManagerInstaller(
                _config, _mockLogger.Object, _mockProgressReporter.Object);
            
            string tempFile = TestHelpers.CreateTempFileWithContent(header);

            try
            {
                // Act
                installer.ValidateInstaller(tempFile, out bool isValidExe, out bool isValidMsi, out bool is16Bit);

                // Assert
                Assert.True(isValidExe);
                Assert.False(isValidMsi);
                Assert.True(is16Bit);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Theory]
        [MemberData(nameof(InstallerTestData.InvalidFileHeaders), MemberType = typeof(InstallerTestData))]
        public void ValidateInstaller_WithInvalidHeader_ThrowsInstallerException(byte[] header)
        {
            // Arrange
            var installer = new HoldemManagerInstaller(
                _config, _mockLogger.Object, _mockProgressReporter.Object);
            
            string tempFile = TestHelpers.CreateTempFileWithContent(header);

            try
            {
                // Act & Assert
                Assert.Throws<InstallerException>(() => 
                    installer.ValidateInstaller(tempFile, out _, out _, out _));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void DetectFileType_WithPdfHeader_ReturnsPdf()
        {
            // Arrange
            var installer = new HoldemManagerInstaller(
                _config, _mockLogger.Object, _mockProgressReporter.Object);
            
            byte[] pdfHeader = { 0x25, 0x50 };

            // Act
            string fileType = installer.GetType()
                .GetMethod("DetectFileType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(installer, new object[] { pdfHeader }) as string;

            // Assert
            Assert.Equal("PDF", fileType);
        }

        [Fact]
        public void DetectFileType_WithZipHeader_ReturnsZip()
        {
            // Arrange
            var installer = new HoldemManagerInstaller(
                _config, _mockLogger.Object, _mockProgressReporter.Object);
            
            byte[] zipHeader = { 0x50, 0x4B };

            // Act
            string fileType = installer.GetType()
                .GetMethod("DetectFileType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(installer, new object[] { zipHeader }) as string;

            // Assert
            Assert.Equal("ZIP", fileType);
        }

        [Fact]
        public void DetectFileType_WithUnknownHeader_ReturnsUnknown()
        {
            // Arrange
            var installer = new HoldemManagerInstaller(
                _config, _mockLogger.Object, _mockProgressReporter.Object);
            
            byte[] unknownHeader = { 0xFF, 0xFF };

            // Act
            string fileType = installer.GetType()
                .GetMethod("DetectFileType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(installer, new object[] { unknownHeader }) as string;

            // Assert
            Assert.Equal("Unknown", fileType);
        }

        [Fact]
        public void GetFileExtensionFromUrl_WithExeUrl_ReturnsExe()
        {
            // Arrange
            var installer = new HoldemManagerInstaller(
                _config, _mockLogger.Object, _mockProgressReporter.Object);
            
            string url = "https://example.com/setup.exe";

            // Act
            string extension = installer.GetType()
                .GetMethod("GetFileExtensionFromUrl", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(installer, new object[] { url }) as string;

            // Assert
            Assert.Equal(".exe", extension);
        }

        [Fact]
        public void GetFileExtensionFromUrl_WithMsiUrl_ReturnsMsi()
        {
            // Arrange
            var installer = new HoldemManagerInstaller(
                _config, _mockLogger.Object, _mockProgressReporter.Object);
            
            string url = "https://example.com/installer.msi";

            // Act
            string extension = installer.GetType()
                .GetMethod("GetFileExtensionFromUrl", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(installer, new object[] { url }) as string;

            // Assert
            Assert.Equal(".msi", extension);
        }

        [Fact]
        public void GetFileExtensionFromUrl_WithUnknownUrl_ReturnsExe()
        {
            // Arrange
            var installer = new HoldemManagerInstaller(
                _config, _mockLogger.Object, _mockProgressReporter.Object);
            
            string url = "https://example.com/download";

            // Act
            string extension = installer.GetType()
                .GetMethod("GetFileExtensionFromUrl", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(installer, new object[] { url }) as string;

            // Assert
            Assert.Equal(".exe", extension);
        }

        [Theory]
        [InlineData("http://example.com/setup.exe", "setup.exe")]
        [InlineData("https://test.org/installer.msi", "installer.msi")]
        [InlineData("http://site.com/path/to/file.msi", "file.msi")]
        public void FindExeLink_WithValidLink_ReturnsCorrectUrl(string html, string expectedUrl)
        {
            // Arrange
            string htmlContent = $"<a href=\"{expectedUrl}\">Download</a>";

            // Act
            string result = HoldemManagerInstaller.FindExeLink(htmlContent);

            // Assert
            Assert.Equal(expectedUrl, result);
        }

        [Fact]
        public void FindExeLink_WithNoLink_ReturnsNull()
        {
            // Arrange
            string htmlContent = "<p>No download link here</p>";

            // Act
            string result = HoldemManagerInstaller.FindExeLink(htmlContent);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData("<title>Test Page</title>", "Test Page")]
        [InlineData("<head><title>Another Title</title></head>", "Another Title")]
        [InlineData("<html><head><title>Page Title</title></head></html>", "Page Title")]
        public void ExtractHtmlTitle_WithTitleTag_ReturnsTitle(string html, string expectedTitle)
        {
            // Act
            string result = HoldemManagerInstaller.ExtractHtmlTitle(html);

            // Assert
            Assert.Equal(expectedTitle, result);
        }

        [Fact]
        public void ExtractHtmlTitle_WithoutTitleTag_ReturnsNull()
        {
            // Arrange
            string htmlContent = "<html><body>No title here</body></html>";

            // Act
            string result = HoldemManagerInstaller.ExtractHtmlTitle(htmlContent);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData("<body><p>Content here</p></body>", "<p>Content here</p>")]
        [InlineData("<html><body>Simple content</body></html>", "Simple content")]
        [InlineData("<body><div>Multi-line<br>content</div></body>", "<div>Multi-line<br>content</div>")]
        public void ExtractHtmlBody_WithBodyTag_ReturnsBodyContent(string html, string expectedBody)
        {
            // Act
            string result = HoldemManagerInstaller.ExtractHtmlBody(html);

            // Assert
            Assert.Equal(expectedBody, result);
        }

        [Fact]
        public void ExtractHtmlBody_WithoutBodyTag_ReturnsNull()
        {
            // Arrange
            string htmlContent = "<html><head><title>Test</title></head></html>";

            // Act
            string result = HoldemManagerInstaller.ExtractHtmlBody(htmlContent);

            // Assert
            Assert.Null(result);
        }
    }
}