

using BlobApiDemo.Dtos;
using BlobApiDemo.Models;

namespace BlobApiDemo.Services;
public interface IFileService
{
    public Task Upload(FileModel fileModel);
    public Task<Stream> Get(string fileName);

    public Task<BlobResponseDto> DeleteAsync(string blobFilename);

    public Task<List<BlobDto>> ListAsync();

}