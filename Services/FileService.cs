using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobApiDemo.Dtos;
using BlobApiDemo.Models;

namespace BlobApiDemo.Services;

public class FileService : IFileService
{
    private readonly BlobServiceClient _blobService;
    private readonly string _nameContainer = "ex-container-quannguyenstorage1234";
    public FileService (BlobServiceClient blobService){
        _blobService = blobService;
    }

    public async Task Upload(FileModel fileModel){
        var containerIntance = _blobService.GetBlobContainerClient(_nameContainer);
        var blobIntance = containerIntance.GetBlobClient(fileModel.ImageFile!.FileName);

        await blobIntance.UploadAsync(fileModel.ImageFile.OpenReadStream());
    }

    public async Task<Stream> Get(string fileName){
        var containerIntance = _blobService.GetBlobContainerClient(_nameContainer);
        var blobIntance = containerIntance.GetBlobClient(fileName);

        var downloadContent = await blobIntance.DownloadAsync() ;
        return downloadContent.Value.Content;
    }

    public async Task<BlobResponseDto> DeleteAsync(string blobFilename)
    {
        var containerIntance = _blobService.GetBlobContainerClient(_nameContainer);

        BlobClient file = containerIntance.GetBlobClient(blobFilename);

        try
        {
            // Delete the file
            await file.DeleteAsync();
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            // File did not exist, log to console and return new response to requesting method
                
            return new BlobResponseDto { Error = true, Status = $"File with name {blobFilename} not found." };
        }

        // Return a new BlobResponseDto to the requesting method
        return new BlobResponseDto { Error = false, Status = $"File: {blobFilename} has been successfully deleted." };
    }

    public async Task<List<BlobDto>> ListAsync()
    {
        // Get a reference to a container
        var containerIntance = _blobService.GetBlobContainerClient(_nameContainer);
        // Create a new list object for 
        List<BlobDto> files = new List<BlobDto>();

        await foreach (BlobItem file in containerIntance.GetBlobsAsync())
        {
            // Add each file retrieved from the storage container to the files list by creating a BlobDto object
            string uri = containerIntance.Uri.ToString();
            var name = file.Name;
            var fullUri = $"{uri}/{name}";

            files.Add(new BlobDto
            {
                Uri = fullUri,
                Name = name,
                ContentType = file.Properties.ContentType
                
            });
        }
        return files;
    }
}