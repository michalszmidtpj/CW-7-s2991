using CW_7_s2991.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CW_7_s2991.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripController(ITripsService tripsService): ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTripsAsync()
    {
        return Ok(await tripsService.GetTripsAsync());
    }
}