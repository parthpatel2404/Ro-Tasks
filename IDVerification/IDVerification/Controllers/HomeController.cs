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
            string filePath = "";
            string jsonString = "";
            IDVResult iDVResult = null;
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
            {
                return BadRequest("Firstname and Lastname are required.");
            }
            FirstName = FirstName.ToLower();
            LastName = LastName.ToLower();
            if (FirstName == "mock" && LastName == "pass")
            {
                var a = Directory.GetCurrentDirectory();
                //filePath = @"D:/Tatvasoft/RO Task/IDVerification/IDVerification/Documents/pass.json";
                 filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents/pass.json");
            }
            else if (FirstName == "mock" && LastName == "fail")
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents/fail.json");
            }
            else if (FirstName == "mock" && LastName == "partial")
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents/partial.json");
            }
            else
            {
                return BadRequest("Enter valid Firstname and Lastname");
            }

            jsonString = System.IO.File.ReadAllText(filePath);
            iDVResult = JsonConvert.DeserializeObject<IDVResult>(jsonString);

            return Created("", iDVResult);
        }
    }
}
