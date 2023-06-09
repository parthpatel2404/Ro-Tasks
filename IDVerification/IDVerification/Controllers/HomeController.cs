using IDVerification.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace IDVerification.Controllers
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

        [HttpGet]
        [Route("/idv/single/idv-result")]
        public async Task<IActionResult> GetIDVResult(string FirstName, string LastName)
        {
            string filePath="";
            string jsonString = "";
            IDVResult iDVResult = null;
            if (FirstName == "Mock" && LastName == "Pass")
            {
                filePath = @"D:/Tatvasoft/RO Task/IDVerification/IDVerification/Documents/pass.json";
            }
            else if (FirstName == "Mock" && LastName == "Fail")
            {
                filePath = @"D:/Tatvasoft/RO Task/IDVerification/IDVerification/Documents/fail.json";
            }
            else if (FirstName == "Mock" && LastName == "Partial")
            {
                filePath = @"D:/Tatvasoft/RO Task/IDVerification/IDVerification/Documents/partial.json";
            }
            else
            {
                return BadRequest("Enter Valid Name");
            }

            jsonString = System.IO.File.ReadAllText(filePath);
            iDVResult = JsonConvert.DeserializeObject<IDVResult>(jsonString);
            
            return Created("", iDVResult);
        }
    }
}
