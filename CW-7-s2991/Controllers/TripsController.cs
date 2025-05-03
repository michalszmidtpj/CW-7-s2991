using CW_7_s2991.Services;
using Microsoft.AspNetCore.Mvc;

namespace CW_7_s2991.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController(ITripsService tripsService): ControllerBase
{
    [HttpGet]
    // zwraca wszystkie wycieczki
    public async Task<IActionResult> GetTripsAsync()
    {
        return Ok(await tripsService.GetTripsAsync());
    }
}