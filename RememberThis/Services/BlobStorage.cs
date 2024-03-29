using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace RememberThis.Services;

public class BlobStorage
{
     private readonly IConfiguration _config;
     
      public BlobStorage(IConfiguration configuration)
        {
            // _logger = logger
            _config = configuration;

        }
    public async Task<string> DeleteFromAzureStorageAsync(string fileName)
    {       
        string methodReturnValue = string.Empty;
        string StorageConnectionString = _config["AZURE_STORAGE_CONNECTION_STRING"]!;
        string ImageContainer = _config["ImageContainer"]!;     

        BlobContainerClient containerClient = new BlobContainerClient(StorageConnectionString, ImageContainer);
        BlobClient blobClient = containerClient.GetBlobClient(fileName);        

        try
        {
            bool storageReturnCode = await blobClient.DeleteIfExistsAsync();
            methodReturnValue = "DeleteBlobSuccess";
        }
        catch (Exception Ex)
        {
            methodReturnValue = Ex.Message;
            methodReturnValue = "ERROR-Blob Delete";
            // throw;
        }

        return methodReturnValue;

    }  // end of write to azure storage

    public async Task<string> WritetoAzureStorageAsync(MemoryStream _ms, string filename)
    {       
        string methodReturnValue = string.Empty;
        string StorageConnectionString = _config["AZURE_STORAGE_CONNECTION_STRING"]!;
        string ImageContainer = _config["ImageContainer"]!;
        Boolean OverWrite = true;

        string trustedExtension = Path.GetExtension(filename).ToLowerInvariant();
        // string trustedNewFileName = Guid.NewGuid().ToString();
        string trustedNewFileName = string.Format("{0:MM-dd-yyyy-H:mm:ss.fff}", DateTime.UtcNow);
        string trustedFileNameAndExt = trustedNewFileName + trustedExtension;

        methodReturnValue = trustedFileNameAndExt;

        BlobContainerClient containerClient = new BlobContainerClient(StorageConnectionString, ImageContainer);
        BlobClient blobClient = containerClient.GetBlobClient(trustedFileNameAndExt);

        _ms.Position = 0;

        try
        {
            await blobClient.UploadAsync(_ms, OverWrite);
    
        }
        catch (Exception Ex)
        {
            methodReturnValue = Ex.Message;
            methodReturnValue = "ERROR-Storage";
            // throw;
        }

        return methodReturnValue;

    }  // end of write to azure storage
    
}