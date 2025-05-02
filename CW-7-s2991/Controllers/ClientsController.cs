using CW_7_s2991.Models.DTOs;
using CW_7_s2991.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CW_7_s2991.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientsController(IClientsService clientsService)
{
    
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientTripsAsync(
        [FromRoute]int id
    )
    {
        return Ok(clientsService.GetTripsAsync(id));
    }
    
    [HttpPost()]
    public async Task<IActionResult> CreateNewClientAsync([FromBody] ClientCreateDTO body
    )
    {
        Ok(clientsService.PostCreateClientAsync());
    }

    [HttpPut()]
    public async Task<IActionResult> RegisterClientsTripAsync()
    {
        Ok(clientsService.PutRegisterClientsTripAsync());
    }
    
    [HttpDelete()]
    public async Task<IActionResult> RemoveClientsTripAsync()
    {
        Ok(clientsService.DeleteClientsTripAsync());
    }
}