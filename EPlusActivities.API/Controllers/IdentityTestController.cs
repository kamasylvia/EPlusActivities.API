using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPlusActivities.API.Controllers
{
    [Route("api/[controller]")]
    public class IdentityTestController : Controller
    {
        // GET: api/values
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> Get()
        {
            var result = Json(from c in HttpContext.User.Claims select new { c.Type, c.Value });
            return Ok(result);
            // return new string[] { "value1", "value2", "Hello IS4" };
        }
    }
}
