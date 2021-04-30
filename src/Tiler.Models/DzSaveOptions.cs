using Microsoft.Extensions.FileProviders;

namespace Tiler.Models
{
    public class DzSaveOptions
    {
        public string OutputDirectory { get; set; }
        public int OverlapPx { get; set; }
        public int TileSizePx { get; set; }
        public int Quality { get; set; }
        public string OutputName { get; set; }
    }
}
