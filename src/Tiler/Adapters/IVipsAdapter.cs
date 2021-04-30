using NetVips;
using System;
using Tiler.Models;

namespace Tiler.Adapters
{
    public interface IVipsAdapter
    {
        Exception InitializationException { get; }
        bool IsInitialized { get; }

        void Dzsave(Image image, DzSaveOptions dzSaveOptions);
        Image NewFromFile(string filePath);
    }
}