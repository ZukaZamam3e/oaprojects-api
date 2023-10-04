using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace OAProjects.API.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
[EnableCors("_myAllowSpecificOrigins")]
public class BlahController : ControllerBase
{
    private readonly ILogger<BlahController> _logger;

    public BlahController(ILogger<BlahController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Unauth()
    {
        return base.Unauthorized();
    }
}
