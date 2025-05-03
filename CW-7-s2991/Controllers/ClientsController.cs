using CW_7_s2991.Models.DTOs;
using CW_7_s2991.Services;
using Microsoft.AspNetCore.Mvc;
using CW_7_s2991.Exceptions;

namespace CW_7_s2991.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ClientsController(IClientsService clientsService) : ControllerBase
{
    // pobiera wycieczki dla  klienta o zadanym id 
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientTripsAsync([FromRoute] int id)
    {
        var res = (List<ClientTripGetDTO>) await clientsService.GetTripsAsync(id);
        if (res.Count > 0)
            return Ok(res);
        return NotFound("No trips found or no such client");
    }

    // dodaje kliena do bazy
    [HttpPost("/api/[controller]")]
    public async Task<IActionResult> CreateNewClientAsync([FromBody] ClientCreateDTO bodyclient)
    {
        var id = -1;
        try
        {
            id = await clientsService.PostCreateClientAsync(bodyclient);
        }
        catch (IllegalParamException e)
        {
            return BadRequest(e.Message);
        }
        return Ok(id);
    }
    
    // rejestruje klienta na wycieczke
    [HttpPut("{id}/trips/{tripid}")]
    public async Task<IActionResult> RegisterClientsTripAsync([FromRoute] int id, [FromRoute] int tripid)
    {
        try
        {
            await clientsService.PutRegisterClientsTripAsync(id, tripid);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        return Ok();
    }
    
    
    // usuwa klienta z wycieczki
    [HttpDelete("{id}/trips/{tripid}")]
    public async Task<IActionResult> RemoveClientsTripAsync([FromRoute] int id, [FromRoute] int tripid)
    {
        try
        {
            await clientsService.DeleteClientsTripAsync(id, tripid);
        }
        catch (NoSuchClientTripException e)
        {
            return NotFound(e.Message);
        }
        return Ok();
    }
}