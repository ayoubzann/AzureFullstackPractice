using Azure.Storage.Blobs;
using AzureFullstackPractice.Data;
using AzureFullstackPractice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AzureFullstackPractice.Controllers;
[ApiController]
[Route("[controller]")]
public class PersonsController : ControllerBase
{
    private readonly PersonDbContext _context;
    private readonly BlobStorageService _client;

    public PersonsController(PersonDbContext context, BlobStorageService client)
    {
        _context = context;
        _client = client;
    }

   [HttpGet("download")]
public async Task<IActionResult> DownloadFile([FromQuery] string containerName, [FromQuery] string blobName)
{
    string downloadFilePath = "../Data";
    try
    {
        await _client.DownloadFileAsync(containerName, blobName, downloadFilePath);

        var fileStream = new FileStream(downloadFilePath, FileMode.Open);
        return File(fileStream, "application/octet-stream", blobName);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}


    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, string fileName)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using (var stream = System.IO.File.Create(fileName))
        {
            await file.CopyToAsync(stream);
        }

        await _client.UploadFileAsync("personfullstackblob", fileName);
        return Ok("File uploaded successfully.");
    }

    [HttpGet("getAll")]
    public async Task<ActionResult<List<Person>>> GetAll()
    {
        var result = await _context.Persons.ToListAsync();
        return result;
    }

    [HttpPost("add")]
    public async Task<ActionResult<Person>> AddPerson(Person person)
    {
        if (person == null) return BadRequest("The person object supplied is invalid. Fill out all fields and try again");

        _context.Persons.AddAsync(person);
        await _context.SaveChangesAsync();

        return Ok(person);
    }
}
