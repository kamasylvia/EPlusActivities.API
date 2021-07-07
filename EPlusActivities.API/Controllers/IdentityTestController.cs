using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPlusActivities.API.Controllers
{

    [Route("api/[controller]")]
    public class IdentityTestController : Controller
    {
        // GET: api/values
        [HttpGet]
        // [Authorize(Roles = "test")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> Get()
        {
            var result = Json(from c in HttpContext.User.Claims select new { c.Type, c.Value });
            return Ok(result);
            // return new string[] { "value1", "value2", "Hello IS4" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

}