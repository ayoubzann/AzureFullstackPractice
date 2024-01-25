using AzureFullstackPractice.Data;
using AzureFullstackPractice.Models;
using Microsoft.AspNetCore.Mvc;

namespace AzureFullstackPractice.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonsController : ControllerBase
{
    private readonly PersonDbContext _context;
    public PersonsController(PersonDbContext context)
    {
        _context = context;
    }

    [HttpGet("getAll")]
    public ActionResult<List<Person>> GetAll()
    {
        var result = _context.Persons.ToList();
        return result;
    }

    [HttpPost("add")]
    public ActionResult<Person> AddPerson(Person person)
    {
        if (person == null) return BadRequest("The person object supplied is invalid. Fill out all fields and try again");

        _context.Persons.Add(person);
        return Ok(person);
    }

}
