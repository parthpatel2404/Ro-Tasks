using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcApplication.Data;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace MvcApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly CIPlatformDbContext _context;
        public HomeController(CIPlatformDbContext dbContext)
        {
            _context = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ExportReport()
        {
            var report = StiReport.CreateNewReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/parthTempData.mrt");
            report.Load(path);
            report.Dictionary.Variables["UserId"].Value = "10010";
            //// Find the TextBox element by its name or other means
            //StiText textBox = report.GetComponentByName("Text2") as StiText;
            //// Update the value or expression of the TextBox
            //textBox.Text.Value = "bablu";
            (report.GetComponentByName("Text2") as StiText).Text.Value = "BADMASS";
            (report.GetComponentByName("Text4") as StiText).Text.Value = "COMPANY";

            report.Compile();

            return StiNetCoreReportResponse.ResponseAsPdf(report);
        }
        public IActionResult ExportReport1()
        {
            var report = StiReport.CreateNewReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/EmploymentContract.mrt");
            report.Load(path);
            List<string> vs = new List<string>() { "Cricket", "Hockey", "Volley Ball", "Chess", "Basket Ball", "Cricket", "Hockey", "Volley Ball", "Chess", "Basket Ball", "Cricket", "Hockey", "Volley Ball", "Chess", "Basket Ball", "Cricket", "Hockey", "Volley Ball", "Chess", "Basket Ball" };

            StiDataBand dataBand = report.GetComponentByName("cBand") as StiDataBand;

            for (int i = 0; i < vs.Count; i++)
            {
                StiText textBox = dataBand.Components[$"cList{i}"] as StiText;

                textBox.Text.Value = vs[i];
            }

            report.Compile();


            return StiNetCoreReportResponse.ResponseAsPdf(report);

        }
        public IActionResult ExportReport3()
        {
            var report = StiReport.CreateNewReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/parthTempData.mrt");
            report.Load(path);

            return StiNetCoreReportResponse.ResponseAsWord2007(report);

        }
        public IActionResult ExportReport2()
        {
            var report = StiReport.CreateNewReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/Invoice.mrt");
            report.Load(path);

            return StiNetCoreReportResponse.ResponseAsPng(report);
        }
        public IActionResult GetReport()
        {
            StiNetCoreViewer.ViewerEventResult(this);
            var report = StiReport.CreateNewReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/BusinessData.mrt");
            report.Load(path);
            //var dataa = new MyDataObject
            //{
            //    Name = "John Doe",
            //    Age = 30,
            //};
            //var userData = _context.Users.Where(x=>x.UserId == 9).Include(x=>x.Country).Include(x => x.City).FirstOrDefault();
            var userData = _context.Users.Where(x => x.UserId == 9).Include(x => x.Country).Include(x => x.City).Select(x => new
            {
                x.FirstName, x.LastName, x.Email, x.LinkedInUrl, x.PhoneNumber, x.ProfileText, x.Availability,
                CountryName = x.Country.Name, CityName = x.City.Name, x.Department, x.EmployeeId, x.Role, x.Title, x.WhyIVolunteer,
                Country=x.Country
            });
            //var cc = userData.Country.Name;
            report.RegBusinessObject("data", userData);

            return StiNetCoreViewer.GetReportResult(this, report);
        }

        public IActionResult ViewerEvent()
        {
            return StiNetCoreViewer.ViewerEventResult(this);
        }
    }
}