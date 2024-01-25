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


    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }
        var tempFilePath = Path.GetTempFileName();

        using (var stream = System.IO.File.Create(tempFilePath))
        {
            await file.CopyToAsync(stream);
        }

        await _client.UploadFileAsync("personfullstackblob", tempFilePath);
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
