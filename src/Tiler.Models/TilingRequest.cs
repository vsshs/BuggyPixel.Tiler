using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Tiler.Models
{
    public class TilingRequest
    {
        public IFileInfo InputFile { get; set; }
        
        public DzSaveOptions DzSaveOptions { get; set; }
    }
}
