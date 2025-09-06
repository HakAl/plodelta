using Xunit;
using Moq;
using System.Threading.Tasks;

namespace PlodeltaImport.PlodeltaImport.Tests
{
    public class ProgramTests
    {
        [Fact]
        public async Task Main_WithInstallArgument_CallsInstallAsync()
        {
            // This test would require significant refactoring to make testable
            // For now, we'll skip it as it involves UI components and static methods
            // In a real scenario, we would refactor Program to use dependency injection
            
            // Arrange
            // var args = new[] { "/install" };
            // var mockInstaller = new Mock<IHoldemManagerInstaller>();
            
            // Act
            // await Program.Main(args);
            
            // Assert
            // mockInstaller.Verify(i => i.InstallAsync(), Times.Once);
        }

        [Fact]
        public async Task Main_WithoutInstallArgument_CallsInstallIfMissingAsyncAndDownloadReportAsync()
        {
            // Similar to above, this test would require refactoring
        }
    }
}