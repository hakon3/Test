using System.IO;
using System.Text;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace First.Services
{
    internal interface IBlobService
    {
        Task<string> GetPayloadFromBlobStorageAsync(string fileName);
        Task UploadStringBlob(string fileName, string content);
    }

    internal class BlobService : IBlobService
    {
        private readonly ILogger<BlobService> _logger;
        private readonly string _connectionString;
        private BlobContainerClient _containerClient;
        private readonly string _blobContainerName = "logentry-payloads";

        public BlobService(IConfiguration configuration, ILogger<BlobService> logger)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionStringOrSetting("BlobStorageConnection");
            _containerClient = new BlobContainerClient(_connectionString, _blobContainerName);
        }

        public async Task UploadStringBlob(string fileName, string content)
        {
            var contentToUpload = Encoding.UTF8.GetBytes(content);
            using var memoryStream = new MemoryStream(contentToUpload);
            await _containerClient.CreateIfNotExistsAsync();
            await _containerClient.UploadBlobAsync(fileName, memoryStream);
        }

        public async Task<string> GetPayloadFromBlobStorageAsync(string fileName)
        {
            await _containerClient.CreateIfNotExistsAsync();
            var blobClient = new BlobClient(_connectionString, _blobContainerName, fileName);
            try
            {
                var result = await blobClient.DownloadContentAsync();
                return result.Value?.Content.ToString();
            }
            catch (Azure.RequestFailedException  e) when (e.Status == 404)
            {
                _logger.LogWarning(e, "Blob {fileName} not found", fileName);
                return null;
            }
        }
    }
}
