# Before running

- Install Azure Storage Emulator. [Download it here](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-emulator#get-the-storage-emulator).
- Add a local.settings.json with the following content
```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "TableStorageConnection": "UseDevelopmentStorage=true",
    "BlobStorageConnection": "UseDevelopmentStorage=true"
  }
}
```