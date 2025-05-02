using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CW_7_s2991.Controllers;

public class TripController
{
    [HttpGet]
    public async Task<IActionResult> GetTripsAsync()
    {
        return Ok(await );
    }
}