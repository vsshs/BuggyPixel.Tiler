using PreviewGenerator.Models;

namespace PreviewGenerator
{
    public interface IPreviewGenerator
    {
        string GeneratePreviewHtmlFromTemplate(GeneratorRequest generatorRequest);
    }
}