using PreviewGenerator.Models;
using System;
using System.IO;

namespace PreviewGenerator
{
    public class PreviewGenerator : IPreviewGenerator
    {
        public PreviewGenerator()
        { }

        public string GeneratePreviewHtmlFromTemplate(GeneratorRequest generatorRequest)
        {
            var template = File.ReadAllText("preview-template.html");
            template = template.Replace("[FOLDER]", $"{generatorRequest.ImagesFolder.Name}/");
            template = template.Replace("[OVERLAP]", $"{generatorRequest.OverlapPx}");
            template = template.Replace("[TILESIZE]", $"{generatorRequest.TileSizePx}");
            template = template.Replace("[WIDTH]", $"{generatorRequest.ImageWidth}");
            template = template.Replace("[HEIGHT]", $"{generatorRequest.ImageHeight}");
            return template;
        }
    }
}
