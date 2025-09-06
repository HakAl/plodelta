//  Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
namespace PlodeltaImport;

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