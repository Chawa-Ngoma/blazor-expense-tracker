using ExpenseTracker.Shared;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PingController : ControllerBase
{
    [HttpGet]
    public ActionResult<PingResponse> Get()
    {
        return new PingResponse("pong", DateTimeOffset.UtcNow);
    }
}
