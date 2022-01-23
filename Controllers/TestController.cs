using Microsoft.AspNetCore.Mvc;

namespace BasicAuthApiWithDotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult test()
        {
            return Ok("OK!");
        }
    }
}
