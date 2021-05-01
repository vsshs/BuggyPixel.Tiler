using PreviewGenerator;
using System;
using System.IO;
using Tiler;
using Tiler.Adapters;
using Tiler.Exceptions;
using Tiler.Models;
using Microsoft.Extensions.FileProviders;

namespace TilerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFileFullPath = "C:\\Users\\SadiaV\\Desktop\\OperaHouseTiles\\operahouse.tif";
            var outputDirectoryFullPath = $"C:\\TEMP\\dz-tests\\";
            var outputDir = "C:\\TEMP\\dz-tests\\";

            var outputName = $"try-{DateTime.Now.ToString("yyyy-MM-dd-HH.m")}";
            
            IVipsAdapter vipsAdapter = new VipsAdapter();
            IFileProvider outputDirProvider = new PhysicalFileProvider(outputDir);
            IConverter converter = new Converter(vipsAdapter, outputDirProvider);
            IPreviewGenerator previewGenerator = new PreviewGenerator.PreviewGenerator();

            var dzSaveOptions = new DzSaveOptions
            {
                TileSizePx = 1024,
                OverlapPx = 1,
                Quality = 60,
                OutputDirectory = outputDir,
                OutputName = outputName
            };

            var templateOutFile = Path.Combine(dzSaveOptions.OutputDirectory, $"{outputName}.html");



            var tilingRequest = new TilingRequest
            {
                InputFile = new PhysicalFileProvider("C:\\Users\\SadiaV\\Desktop\\OperaHouseTiles\\").GetFileInfo("operahouse.tif"),
                DzSaveOptions = dzSaveOptions
            };

            TilingResult tilingResult = null;
            Console.Write($"Processing {inputFileFullPath}... ");
            try
            {
                tilingResult = converter.Convert(tilingRequest);
                Console.WriteLine("Done");
            }
            catch (TilerException e)
            {
                Console.WriteLine($"Failed: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed: {e.Message}");
            }

            if (tilingResult == null)
                return;

            PreviewGenerator.Models.GeneratorRequest generatorRequest = new PreviewGenerator.Models.GeneratorRequest
            {
                ImageHeight = tilingResult.ImageHeight,
                ImageWidth = tilingResult.ImageWidth,
                ImagesFolderName = tilingResult.OutputFolder,
                OverlapPx = dzSaveOptions.OverlapPx,
                TileSizePx = dzSaveOptions.TileSizePx
            };

            Console.Write("Generating template... ");
            var templateHtml = previewGenerator.GeneratePreviewHtmlFromTemplate(generatorRequest);
            Console.WriteLine("Done");

            Console.Write($"Writing template to {templateOutFile}... ");
            File.WriteAllText(templateOutFile, templateHtml);
            Console.WriteLine("Done");

            Console.Write("Starting web browser...");
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = $"file:///{templateOutFile.Replace('\\', '/')}",
                UseShellExecute = true
            });
            Console.WriteLine("Done");

            Console.WriteLine("Upload to Azure Blob Storage? (y/n)");
            var k = Console.ReadKey().KeyChar;

            switch (k)
            {
                case 'y':
                case 'Y':
                    Console.WriteLine("UPLOADING!!!!");
                    break;

                default:
                    Console.WriteLine("BYE");
                    break;
            }
            
        }
    }
}
