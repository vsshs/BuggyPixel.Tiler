using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureUploaderApp
{
    public class Program
    {
        static async Task<int> Main(string[] args)
        {
            // vsshspanoramastorage
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("Connection string not set");
            
            var outDir = "C:\\TEMP\\dz-tests\\";
            var outFolderName = $"vsshs{DateTime.UtcNow.Ticks}";

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Create the container and return a container client object
            //var containerClient = await blobServiceClient.CreateBlobContainerAsync(outFolderName);

            var containerClient = blobServiceClient.GetBlobContainerClient("public-pano-storage");

            //for (var i = 0; i < 19; i++)
            //{
            //     await UploadFile(containerClient, $"{outFolderName}/{i}/0_0.jpg");
            //}

            var folderName = "out_q60_azure_files";

            var files = Directory.GetFiles($"C:\\TEMP\\dz-tests\\{folderName}\\", "*", SearchOption.AllDirectories);
            foreach (var filePath in files)
            {
                //Console.WriteLine(filePath.Replace("C:\\TEMP\\dz-tests\\", string.Empty));
                await UploadFile(containerClient, filePath.Replace("C:\\TEMP\\dz-tests\\", string.Empty), filePath);
            }

            Console.WriteLine($"FILES COUNT: {files.Length}");

            return await Task.FromResult(0);
        }

        private static async Task UploadFile(BlobContainerClient blobContainerClient, string fileName, string sourceFilePath)
        {          

            // Get a reference to a blob
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

            Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

            // Open the file and upload its data
            using FileStream uploadFileStream = File.OpenRead(sourceFilePath);
            await blobClient.UploadAsync(uploadFileStream, httpHeaders: new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = "image/jpeg" });
            uploadFileStream.Close();
        }
    }
}
