using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RoTaskWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Route("getString")]
        public string getString(string? value)
        {
            if (value != null)
            {
                return (value);
            }
            return ("Enter String");
        }
        
        //[HttpGet]

        //public IActionResult getString1(string? value)
        //{
        //    if (value != null)
        //    {
        //        return Ok(value);
        //    }
        //    return BadRequest("Enter String");
        //}


    }
}
