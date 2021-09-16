using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok("Hello World!");
        }
    }
}
