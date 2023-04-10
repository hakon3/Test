using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using First.Contracts;
using Microsoft.Extensions.Logging;

namespace First.Services
{
    internal interface ITableService
    {
        Task SaveLogEntry(string blobName, bool success);
        Task<List<LogEntryDescription>> GetLogEntriesAsync(DateTime from, DateTime to);
    }

    internal class PublicApiLogService : ITableService
    {
        private readonly ILogger<PublicApiLogService> _logger;
        private readonly ITableClientFactory _tableClientFactory;
        private const string TableName = "logentries";

        public PublicApiLogService(ILogger<PublicApiLogService> logger, ITableClientFactory tableClientFactory)
        {
            _logger = logger;
            _tableClientFactory = tableClientFactory;
        }

        public async Task SaveLogEntry(string blobName, bool success)
        {
            var id = DateTime.UtcNow.Ticks.ToString("d19");
            var publicApiEntity = new TableEntity(id, id)
            {
                {"BlobName", blobName},
                {"Succeeded", success}
            };

            var tableClient = _tableClientFactory.Create(TableName);
            await tableClient.CreateIfNotExistsAsync();
            await tableClient.AddEntityAsync(publicApiEntity);
            _logger.LogInformation("Table entry {id} added", id);
        }

        public async Task<List<LogEntryDescription>> GetLogEntriesAsync(DateTime from, DateTime to)
        {
            var tableClient = _tableClientFactory.Create(TableName);
            await tableClient.CreateIfNotExistsAsync();
            var entities = new List<LogEntryDescription>();
            await foreach (var results in tableClient.QueryAsync<TableEntity>($"PartitionKey ge {from.Ticks:d19} and PartitionKey lt {to.Ticks:d19}"))
            {
                var entry = new LogEntryDescription
                {
                    TimeStamp = results.Timestamp,
                    LogEntryName = results["BlobName"].ToString(),
                    Succeeded = (bool)results["Succeeded"]
                };
                entities.Add(entry);
            }

            return entities;
        }
    }
}
