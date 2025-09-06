//  Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
using Xunit;

namespace PlodeltaImport.PlodeltaImport.Tests
{
    public class InstallerConstantsTests
    {
        [Fact]
        public void ProductName_IsCorrect()
        {
            Assert.Equal("Holdem Manager 3", InstallerConstants.ProductName);
        }

        [Fact]
        public void CompanyName_IsCorrect()
        {
            Assert.Equal("Max Value Software", InstallerConstants.CompanyName);
        }

        [Fact]
        public void ReportFileName_IsCorrect()
        {
            Assert.Equal("Home_Saved Reports_GTOStatAnalyzer.report", InstallerConstants.ReportFileName);
        }

        [Fact]
        public void PossibleUninstallPaths_HasExpectedValues()
        {
            var expectedPaths = new[]
            {
                @"C:\Program Files (x86)\Holdem Manager 3\uninstall.exe",
                @"C:\Program Files (x86)\Holdem Manager 3\unins000.exe",
                @"C:\Program Files (x86)\Max Value Software\Holdem Manager 3\uninstall.exe",
                @"C:\Program Files (x86)\Max Value Software\Holdem Manager 3\unins000.exe"
            };

            Assert.Equal(expectedPaths, InstallerConstants.PossibleUninstallPaths);
        }
    }
}