using System;
using System.IO;
using Tiler.Adapters;
using Tiler.Exceptions;
using Tiler.Models;
using Microsoft.Extensions.FileProviders;

namespace Tiler
{
    public class Converter : IConverter
    {
        private readonly IVipsAdapter vipsAdapter;
        private readonly IFileProvider outputDirProvider;

        public Converter(IVipsAdapter vipsAdapter, IFileProvider outputDirProvider)
        {
            this.vipsAdapter = vipsAdapter;
            this.outputDirProvider = outputDirProvider;
        }

        /// <summary>
        /// Converts provided image into tiles using Vips
        /// </summary>
        /// <param name="tilingRequest"></param>
        /// <returns></returns>
        /// <exception cref="TilerException"></exception>
        /// <exception cref="VipsException"></exception>
        public TilingResult Convert(TilingRequest tilingRequest)
        {
            ValidateVips(vipsAdapter);
            ValidateRequest(tilingRequest);

            var image = vipsAdapter.NewFromFile(tilingRequest.InputFile.PhysicalPath);
            vipsAdapter.Dzsave(image, tilingRequest.DzSaveOptions);

            return new TilingResult
            {
                ImageWidth = image.Width,
                ImageHeight = image.Height,
                OutputFolder = $"{tilingRequest.DzSaveOptions.OutputName}_files"
            };
        }

        private void ValidateVips(IVipsAdapter vipsAdapter)
        {
            if (!vipsAdapter.IsInitialized)
            {
                throw new TilerException("Vips is not initialized correctly. See inner exception for more details", vipsAdapter.InitializationException);
            }
        }

        private void ValidateRequest(TilingRequest tilingRequest)
        {
            if (tilingRequest.DzSaveOptions == null)
            {
                throw new ArgumentNullException($"{nameof(tilingRequest.DzSaveOptions)} cannot be null");
            }

            if (tilingRequest.InputFile == null)
            {
                throw new ArgumentNullException($"{nameof(tilingRequest.InputFile)} cannot be null");
            }

            if (!tilingRequest.InputFile.Exists)
            {
                throw new FileNotFoundException("Input file for tiling does not exist.", tilingRequest.InputFile.PhysicalPath);
            }

            if (!outputDirProvider.GetDirectoryContents("").Exists)
            {
                throw new DirectoryNotFoundException($"Output directory ({tilingRequest.DzSaveOptions.OutputDirectory}) does not exist.");
            }
        }
    }
}
