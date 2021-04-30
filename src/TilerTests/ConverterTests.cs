using AutoFixture;
using AutoFixture.Xunit2;
using System;
using Tiler;
using Tiler.Models;
using Xunit;
using Moq;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Tiler.Adapters;
using Tiler.Exceptions;
using NetVips;
using System.Linq;

namespace TilerTests
{
    public class ConverterTests
    {
        [Theory, AutoDomainData]
        public void Convert_InputFileNull_Error(TilingRequest tilingRequest, Converter sut)
        {
            // Arrange
            tilingRequest.InputFile = null;

            // Act // Assert
            Assert.Throws<ArgumentNullException>(() => sut.Convert(tilingRequest));
        }

        [Theory, AutoDomainData]
        public void Convert_DzSaveOptionsNull_Error(TilingRequest tilingRequest, Converter sut)
        {
            // Arrange
            tilingRequest.DzSaveOptions = null;

            // Act // Assert
            Assert.Throws<ArgumentNullException>(() => sut.Convert(tilingRequest));
        }

        [Theory, AutoDomainData]
        public void Convert_InputFileDoesNotExist_Error([Frozen] Mock<IFileInfo> fileInfoMock, TilingRequest tilingRequest, Converter sut)
        {
            // Arrange
            tilingRequest.InputFile = fileInfoMock.Object;
            fileInfoMock.Setup(fi => fi.Exists)
                .Returns(false);

            // Act // Assert
            Assert.Throws<FileNotFoundException>(() => sut.Convert(tilingRequest));
        }

        [Theory, AutoDomainData]
        public void Convert_OutputDirectoryDoesNotExist_Error(
            [Frozen] Mock<IFileProvider> fileproviderMock,
            [Frozen] Mock<IDirectoryContents> directoryContentsMock,
            TilingRequest tilingRequest,
            Converter sut)
        {
            // Arrange
            fileproviderMock
                .Setup(fp => fp.GetDirectoryContents(It.IsAny<string>()))
                .Returns(directoryContentsMock.Object);
            directoryContentsMock
                .Setup(dc => dc.Exists)
                .Returns(false);

            // Act // Assert
            Assert.Throws<DirectoryNotFoundException>(() => sut.Convert(tilingRequest));
        }

        [Theory, AutoDomainData]
        public void Convert_VipsNotIntialized_Error(
            [Frozen] Mock<IVipsAdapter> vipsAdapterMock,
            TilingRequest tilingRequest,
            Converter sut)
        {
            // Arrange
            vipsAdapterMock.Setup(va => va.IsInitialized)
                .Returns(false);

            // Act // Assert
            Assert.Throws<TilerException>(() => sut.Convert(tilingRequest));
            vipsAdapterMock.Verify(va => va.InitializationException, Times.AtLeastOnce);
        }

        [Theory, AutoDomainData]
        public void Convert_Valid_ExpedtedResult(
            [Frozen] Mock<IVipsAdapter> vipsAdapterMock,
            [Frozen] Mock<IFileInfo> fileInfoMock,
            [Frozen] Mock<IDirectoryContents> directoryContentsMock,
            string physicalPath,
            TilingRequest tilingRequest,
            Converter sut)
        {
            // Arrange
            directoryContentsMock.Setup(dc => dc.Exists)
                .Returns(true);

            tilingRequest.InputFile = fileInfoMock.Object;
            vipsAdapterMock.Setup(va => va.IsInitialized)
                .Returns(true);

            var image = Image.NewTempFile("%s.v");
            vipsAdapterMock.Setup(va => va.NewFromFile(It.IsAny<string>()))
                .Returns(image);
            fileInfoMock.Setup(fi => fi.PhysicalPath)
                .Returns(physicalPath);


            // Act 
            var result = sut.Convert(tilingRequest);

            // Assert
            vipsAdapterMock.Verify(va => va.NewFromFile(physicalPath), Times.Once);
            vipsAdapterMock.Verify(va => va.Dzsave(image, tilingRequest.DzSaveOptions), Times.Once);

            Assert.Equal(image.Height, result.ImageHeight);
            Assert.Equal(image.Width, result.ImageWidth);
            Assert.Equal($"{tilingRequest.DzSaveOptions.OutputName}_files", result.OutputFolder);
        }
    }

    public class AutoDomainDataAttribute : AutoDataAttribute
    {
        public static IFixture FixtureFactory()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
            var fim = new Mock<IFileInfo>();
            fim
                .Setup(f => f.Exists)
                .Returns(true);

            fixture.Customize<TilingRequest>(f => f.With(fi => fi.InputFile, fim.Object));

            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }

        public AutoDomainDataAttribute() : base(FixtureFactory) { }
    }
}
