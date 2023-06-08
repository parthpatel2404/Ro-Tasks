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
        public IActionResult GetIDVResult(string FirstName, string LastName)
        {
            //if (FirstName == "Mock" && LastName == "Pass")
            //{

            //}
            // Assuming jsonString contains the JSON data you provided
            string filePath = @"D:/Tatvasoft/RO Task/IDVerification/IDVerification/Documents/Pass1.json";
            string jsonString = System.IO.File.ReadAllText(filePath);

            IDVResult iDVResult = JsonConvert.DeserializeObject<IDVResult>(jsonString);

            //string jsonString = "D:/Tatvasoft/RO Task/IDVerification/IDVerification/Documents/Pass1.json"; // JSON data

            //IDVResult iDVResult = JsonConvert.DeserializeObject<IDVResult>(jsonString);

            return Created("", iDVResult);
        }
    }
}
