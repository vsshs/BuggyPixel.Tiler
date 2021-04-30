using NetVips;
using System;
using System.IO;
using Tiler.Models;

namespace Tiler.Adapters
{
    public class VipsAdapter : IVipsAdapter
    {
        public bool IsInitialized => ModuleInitializer.VipsInitialized;
        public Exception InitializationException => ModuleInitializer.Exception;

        public Image NewFromFile(string filePath)
        {
            return Image.NewFromFile(filePath);
        }

        public void Dzsave(Image image, DzSaveOptions dzSaveOptions)
        {
            image.Dzsave(
                Path.Combine(dzSaveOptions.OutputDirectory.ToString(), dzSaveOptions.OutputName),
                overlap: dzSaveOptions.OverlapPx,
                tileSize: dzSaveOptions.TileSizePx,
                suffix: $".jpg[Q={dzSaveOptions.Quality}]");
        }
    }
}
