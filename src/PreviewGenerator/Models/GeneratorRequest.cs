using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PreviewGenerator.Models
{
    public class GeneratorRequest
    {
        public int OverlapPx { get; set; }
        public int TileSizePx { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public string ImagesFolderName { get; set; }
    }
}
