using AutoFixture.Xunit2;
using PreviewGenerator.Models;
using System;
using Xunit;

namespace PreviewGenerator.Tests
{
    public class PreviewGeneratorTests
    {
        [Theory, AutoData]
        public void GeneratePreviewHtmlFromTemplate_Valid_ReplacesTexts(GeneratorRequest request, PreviewGenerator sut)
        {
            // Arrange

            // Act
            var output = sut.GeneratePreviewHtmlFromTemplate(request);

            // Assert
            Assert.Contains($"{request.ImagesFolderName}/", output);
            Assert.Contains($"{request.OverlapPx}", output);
            Assert.Contains($"{request.TileSizePx}", output);
            Assert.Contains($"{request.ImageWidth}", output);
            Assert.Contains($"{request.ImageHeight}", output);
        }
    }
}
