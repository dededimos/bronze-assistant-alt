using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BronzeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobsController : ControllerBase
    {
        private readonly ILogger<BlobsController> logger;
        private readonly IConfiguration config;
        private const string blobConnStringName = "BlobStorageConnString";
        private const string blobContainerConfigName = "BlobStorageContainerName";
        private const string azureBlobsSectionName = "AzureBlobs";

        public BlobsController(ILogger<BlobsController> logger,IConfiguration config)
        {
            this.logger = logger;
            this.config = config;
        }

        /// <summary>
        /// Returns a Sas Uri to Download a certain File from The Accessories-Images Blob Container
        /// </summary>
        /// <param name="blobName">The Url of the File without including the BlobContainer</param>
        /// <returns></returns>
        [HttpGet("GetBlobSasDownloadUri", Name = "GetBlobSasDownloadUri")]
        public async Task<IActionResult> GetBlobSasDownloadUri([FromQuery] string blobName)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                return new BadRequestObjectResult("Requested Sas Url was Empty");
            }
            
            string conString = config.GetConnectionString(blobConnStringName) ?? string.Empty;
            if(string.IsNullOrEmpty(conString)) { return new BadRequestObjectResult("Connection String was not present in the Configuration Settings"); }

            string containerName = config.GetSection(azureBlobsSectionName)[blobContainerConfigName] ?? string.Empty;
            if (string.IsNullOrEmpty(containerName)) { return new BadRequestObjectResult("Container Name was not present in the Configuration Settings"); }

            //Create the Client
            var blobClient = new BlobContainerClient(config.GetConnectionString(blobConnStringName), containerName);
            //Get the Specific Blob Client for the provided blob Url
            var blob = blobClient.GetBlobClient(blobName);
            //if the blob exists continue
            if (await blob.ExistsAsync())
            {
                //Create the SAS with altered Content-Disposition so User can Download directly instead of opening url on another window
                BlobSasBuilder sasBuilder = new()
                {
                    BlobContainerName = blob.BlobContainerName,//Set the Container Name
                    BlobName = blob.Name,//Set the Blob Name
                    ContentDisposition = "attachment",//Override Content Disposition to attachment (D/ls the blob directly)
                    ContentType = "application/octet-stream",//Set the Content Type to octet Stream (D/ls the blob directly)
                    Resource = "b",//Sas Key is for a blob 'b' (otherwise for whole container 'c')
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);//Sas Permission Key is only READ
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);   //Expires in 1hour

                //Generate the SAS URI , with which user can download blob
                Uri sasUri = blob.GenerateSasUri(sasBuilder);
                //Return the Whole Uri (if only string is returns Deserilization is difficult because of invalid charachters)
                return new OkObjectResult(sasUri);
            }
            else
            {
                return new BadRequestObjectResult("Requested blob url is Invalid or Removed");
            }
        }
    }
}
