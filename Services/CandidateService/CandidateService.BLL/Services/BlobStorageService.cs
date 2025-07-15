using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using CandidateService.BLL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CandidateService.BLL.Services;
public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobStorageService(IConfiguration configuration)
    {
        _blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage"));
        _containerName = configuration["AzureBlobStorage:ContainerName"]!;
    }

    public async Task<string> UploadOrReplaceResumeAsync(Stream fileStream, string originalFileName, string path)
    {
        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();

        if (extension != ".pdf")
            throw new InvalidOperationException("Only PDF resumes are allowed.");

        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var blobPath = path + extension; // path: e.g. "profiles/{id}" or "applications/{id}"
        var blobClient = containerClient.GetBlobClient(blobPath);

        if (await blobClient.ExistsAsync())
            await blobClient.DeleteAsync();

        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = "application/pdf" });

        return blobPath;
    }


    public async Task DeleteFileAsync(string blobFileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobFileName);

        if (await blobClient.ExistsAsync())
        {
            await blobClient.DeleteAsync();
        }
        else
        {
            throw new FileNotFoundException("Resume file not found in blob storage");
        }
    }

    public async Task<string> GetResumeSasUriAsync(string blobFileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobFileName);

        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException("Resume file not found");
        }

        return GenerateSasUri(blobClient);
    }

    private string GenerateSasUri(BlobClient blobClient)
    {
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTime.UtcNow.AddMinutes(30)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }
}
