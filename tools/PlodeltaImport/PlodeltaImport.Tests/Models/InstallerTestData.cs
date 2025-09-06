using System.Collections.Generic;

namespace PlodeltaImport.PlodeltaImport.Tests.Models
{
    public static class InstallerTestData
    {
        public static IEnumerable<object[]> ValidExeHeaders => new List<object[]>
        {
            new object[] { new byte[] { 0x4D, 0x5A } }, // Valid EXE header
            new object[] { new byte[] { 0x4D, 0x5A, 0x90, 0x00 } } // Valid EXE with more bytes
        };

        public static IEnumerable<object[]> ValidMsiHeaders => new List<object[]>
        {
            new object[] { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } // Valid MSI header
        };

        public static IEnumerable<object[]> InvalidFileHeaders => new List<object[]>
        {
            new object[] { new byte[] { 0x25, 0x50 } }, // PDF header
            new object[] { new byte[] { 0x50, 0x4B } }, // ZIP header
            new object[] { new byte[] { 0x7F, 0x45 } }, // ELF header
            new object[] { new byte[] { 0x3C, 0x21 } }  // HTML header
        };

        public static IEnumerable<object[]> SixteenBitHeaders => new List<object[]>
        {
            new object[] { new byte[] { 
                0x4D, 0x5A, // MZ
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x40, 0x00, 0x00, 0x00, // PE header offset at 0x3C
                0x4E, 0x45, 0x00, 0x00  // NE header at offset 0x40
            } }
        };
    }
}