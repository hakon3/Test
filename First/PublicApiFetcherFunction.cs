using System;
using System.Net.Http;
using System.Threading.Tasks;
using First.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace First
{
    internal class PublicApiFetcherFunction
    {
        private readonly ITableService _tableService;
        private readonly IBlobService _blobService;
        private readonly IPayLoadBuilder _payLoadBuilder;
        private readonly HttpClient _client;

        public PublicApiFetcherFunction(IHttpClientFactory clientFactory, ITableService tableService, IBlobService blobService, IPayLoadBuilder payLoadBuilder)
        {
            _tableService = tableService;
            _blobService = blobService;
            _payLoadBuilder = payLoadBuilder;
            _client = clientFactory.CreateClient();
        }

        [FunctionName("PublicApiFetcherFunction")]
        public async Task Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, ILogger logger)
        {
            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            HttpResponseMessage responseMessage = null;
            var status = new ProcessStatus();

            try
            {
                responseMessage = await _client.GetAsync("https://api.publicapis.org/random?auth=null");
                var body = await responseMessage.Content.ReadAsStringAsync();
                logger.LogInformation("Result fetched, {Response}", body);
                status.ApiSuccess = true;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error calling publicapis");
            }

            string blobName = null;
            if (status.ApiSuccess)
            {
                try
                {
                    blobName = $"{Guid.NewGuid()}";
                    var payload = await _payLoadBuilder.GetPayloadFromHttpResponse(responseMessage);
                    await _blobService.UploadStringBlob(blobName, payload);
                    logger.LogInformation("File {FileName} uploaded", blobName);
                    status.BlobSuccess = true;
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error uploading blob");
                    blobName = null;
                }
            }

            try
            {
                await _tableService.SaveLogEntry(blobName, status.ApiSuccess && status.BlobSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error saving logentry to table storage");
            }
        }

        private class ProcessStatus
        {
            public bool ApiSuccess { get; set; }
            public bool BlobSuccess { get; set; }
        }
    }
}
