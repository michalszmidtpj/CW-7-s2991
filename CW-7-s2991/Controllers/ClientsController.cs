using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CW_7_s2991.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientsController
{
    
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientTripsAsync(
        [FromRoute]int id
    )
    {

    }
    
    [HttpPost()]
    public async Task<IActionResult> CreateNewClientAsync([FromBody] VisitCreateDTO body
    )
    {

    }

    [HttpPut()]
    public async Task<IActionResult> RegisterClientsTripAsync()
    {
        
    }
    
    [HttpDelete()]
    public async Task<IActionResult> RemoveClientsTripAsync()
    {
        
    }
}