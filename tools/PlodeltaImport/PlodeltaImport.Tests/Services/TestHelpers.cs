//  Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Win32;
using Moq;
using Moq.Protected;

namespace PlodeltaImport.PlodeltaImport.Tests.Services
{
    public static class TestHelpers
    {
        public static Mock<HttpClient> CreateMockHttpClient(HttpResponseMessage response)
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<System.Threading.CancellationToken>())
                .ReturnsAsync(response);
            
            return new Mock<HttpClient>(handler.Object);
        }

        public static string CreateTempFileWithContent(byte[] content)
        {
            string tempFile = Path.GetTempFileName();
            File.WriteAllBytes(tempFile, content);
            return tempFile;
        }

        public static Mock<RegistryKey> CreateMockRegistryKey(string name, object value)
        {
            var mockKey = new Mock<RegistryKey>();
            mockKey.Setup(k => k.GetValue(It.IsAny<string>())).Returns(value);
            mockKey.Setup(k => k.Name).Returns(name);
            return mockKey;
        }
    }
}