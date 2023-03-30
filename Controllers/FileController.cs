
using BlobApiDemo.Models;
using BlobApiDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlobApiDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase 
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService){
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm]FileModel? fileModel){
        try
        {
            if (fileModel == null)
            {
                return BadRequest(new {message = "Please choose file to upload!"});
            }
            await _fileService.Upload(fileModel);
            return Ok(new {message = "Upload image success"});
        }
        catch (System.Exception)
        {
            return StatusCode(500, new {message = "an error has occurred while uploading!"});
        }
    }
    [HttpGet("getall")]    
    public async Task<IActionResult> GetAll(){
       var listBlob = await _fileService.ListAsync();


       
        // return Ok(ListFile);
        return await Task.FromResult(Ok(listBlob));
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetFile(string FileName ){
        var imageFileStream = await _fileService.Get(FileName);
        string fileType = "jpeg";
        if (FileName.Contains("png"))
        {
            fileType = "png";
        }
        return File(imageFileStream, $"image/{fileType}");
    }
    [HttpGet("download")]
    public async Task<IActionResult> DownloadFile(string FileName ){
        var imageFileStream = await _fileService.Get(FileName);
        string fileType = "jpeg";
        if (FileName.Contains("png"))
        {
            fileType = "png";
        }
        return File(imageFileStream, $"image/{fileType}", $"blobfile.{fileType}");
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteFile(string FileName){
        var BlobResp = await _fileService.DeleteAsync(FileName);
        if (BlobResp.Error)
        {
            return StatusCode(400, BlobResp.Status);
        }
        return Ok(BlobResp.Status);
    }
}