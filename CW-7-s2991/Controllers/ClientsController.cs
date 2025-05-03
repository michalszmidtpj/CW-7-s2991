using CW_7_s2991.Models.DTOs;
using CW_7_s2991.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


namespace CW_7_s2991.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ClientsController(IClientsService clientsService) : ControllerBase
{
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientTripsAsync([FromRoute] int id)
    {
        var res = (List<ClientTripGetDTO>) await clientsService.GetTripsAsync(id);
        if (res.Count > 0)
            return Ok(res);
        return NotFound("No trips found or no such client");
    }

    // [HttpPost()]
    // public async Task<IActionResult> CreateNewClientAsync([FromBody] ClientCreateDTO body
    // )
    // {
    //     Ok(clientsService.PostCreateClientAsync());
    // }
    //
    // [HttpPut()]
    // public async Task<IActionResult> RegisterClientsTripAsync()
    // {
    //     Ok(clientsService.PutRegisterClientsTripAsync());
    // }
    //
    // [HttpDelete()]
    // public async Task<IActionResult> RemoveClientsTripAsync()
    // {
    //     Ok(clientsService.DeleteClientsTripAsync());
    // }
}