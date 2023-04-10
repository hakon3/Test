using System;
using System.Threading.Tasks;
using First.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Web;
using First.Contracts;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;

namespace First
{
    internal class ApiFunctions
    {
        private readonly ITableService _tableService;
        private readonly IBlobService _blobService;

        public ApiFunctions(ITableService tableService, IBlobService blobService)
        {
            _tableService = tableService;
            _blobService = blobService;
        }

        [FunctionName("ApiGetInRangeFunction")]
        [OpenApiOperation(operationId: "ApiGetInRangeFunction", tags: new[] { "Log Entries" })]
        [OpenApiParameter(name: "utcFrom", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "URL encoded UTC Timestamp in ISO 8601 format e.g 2023-04-06T22%3A38%3A00Z")]
        [OpenApiParameter(name: "utcTo", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "URL encoded UTC Timestamp in ISO 8601 format e.g 2023-04-06T23%3A38%3A00Z")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(GetLogentriesResponse), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "The Bad Request response")]
        public async Task<IActionResult> ApiGetInRangeFunction(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "logentries")]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function ApiGetInRangeFunction processing a request.");

            string from = HttpUtility.UrlDecode(req.Query["utcFrom"]);
            string to = HttpUtility.UrlDecode(req.Query["utcTo"]);

            if (!DateTime.TryParse(from, out var dateFrom))
            {
                return new BadRequestObjectResult("Invalid date format for 'utcFrom'");
            }

            if (!DateTime.TryParse(to, out var dateTo))
            {
                return new BadRequestObjectResult("Invalid date format for 'utcTo'");
            }

            var logEntries = await _tableService.GetLogEntriesAsync(dateFrom.ToUniversalTime(), dateTo.ToUniversalTime());

            var response = new GetLogentriesResponse
            {
                LogEntries = logEntries
            };

            return new OkObjectResult(response);
        }

        [FunctionName("ApiGetPayloadFunction")]
        [OpenApiOperation(operationId: "ApiGetPayloadFunction", tags: new[] { "Log Entries" })]
        [OpenApiParameter(name: "logentryname", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Name of the log entry to get")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(string), Description = "The OK response is a json string", Example = typeof(Example))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/plain", bodyType: typeof(string), Description = "The Not Found response")]
        public async Task<IActionResult> ApiGetPayloadFunction(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "logentries/{logentryname}")]
            HttpRequest req,
            string logEntryName,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function ApiGetPayloadFunction processing a request.");

            var result = await _blobService.GetPayloadFromBlobStorageAsync(logEntryName);

            if (result == null)
            {
                return new NotFoundObjectResult($"Log entry with name '{logEntryName}' not found");
            }

            return new ContentResult
            {
                Content = result,
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        private class Example : OpenApiExample<string>
        {
            public override IOpenApiExample<string> Build(NamingStrategy namingStrategy = null)
            {
                var example = OpenApiExampleResolver.Resolve("Example response", "{\"Content\": {  ...  }, \"ContentHeaders\": {  ...  }, \"Headers\": {  ...  }}");
                Examples.Add(example);
                return this;
            }
        }
    }
}
