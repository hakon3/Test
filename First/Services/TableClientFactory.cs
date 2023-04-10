using System.Collections.Concurrent;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;

namespace First.Services
{
    internal interface ITableClientFactory
    {
        TableClient Create(string tableName);
    }

    internal class TableClientFactory : ITableClientFactory
    {
        private readonly IConfiguration _configuration;
        private readonly object _lock = new object();
        private readonly ConcurrentDictionary<string, TableClient> _tableClients = new ConcurrentDictionary<string, TableClient>();

        public TableClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public TableClient Create(string tableName)
        {
            lock (_lock)
            {
                var connectionString = _configuration.GetConnectionStringOrSetting("TableStorageConnection");
                if (_tableClients.TryGetValue(tableName, out var tableClient))
                { 
                    return tableClient;
                }

                var serviceClient = new TableServiceClient(connectionString);
                tableClient = serviceClient.GetTableClient(tableName);
                _tableClients.TryAdd(tableName, tableClient);
                return tableClient;
            }
        }
    }
}
