using CIPlatform.Entities.Data;
using CIPlatform.Entities.Models;
using CIPlatform.Entities.ViewModel;
using CIPlatform.Repository.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Mvc;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CI_Platform.Controllers
{

    public class UserController : Controller
    {

        public readonly IUserRepository _UserRepository;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(IUserRepository userRepository, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _UserRepository = userRepository;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }
        // public IActionResult Index(String returnUrl = "")
        // {
        //     RegistrationViewModel rv = new RegistrationViewModel();
        //     rv.returnUrl = returnUrl;

        //     return View(rv);
        // }
        public IActionResult Index(String ReturnUrl = "")
        {
            User RU = new User();
            RU.ReturnUrl = ReturnUrl;
            ViewBag.Banner = _UserRepository.BannerList();
            return View(RU);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User obj)
        {
            var user = _UserRepository.login(obj);
            //if (user.DeletedAt != null)
            //{
            //    TempData["error"] = "User was deleted by admin";
            //    return View();

            //}
            if (user == null)
            {
                TempData["error"] = "Invalid Username or Password";
                ViewBag.Banner = _UserRepository.BannerList();
                return View();
            }
            //string role = "User";
            var claims = new List<Claim>
                {
                        new Claim("user",user.Role),
                        new Claim("Name", $"{user.FirstName} {user.LastName}"),
                        new Claim("Email", user.Email),
                        new Claim("Uid", user.UserId.ToString()),
                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: credentials
                );
            var returnToken = new JwtSecurityTokenHandler().WriteToken(token);
            //_httpContextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {returnToken}");
            //var identity = new ClaimsIdentity(claims, "AuthCookie");
            //var Principle = new ClaimsPrincipal(identity);
            //HttpContext.User = Principle;
            //var abc = HttpContext.SignInAsync(Principle);
            //return new JwtSecurityTokenHandler().WriteToken(token);

            HttpContext.Session.SetInt32("UserId", (int)user.UserId);
            HttpContext.Session.SetString("UserName", user.FirstName + " " + user.LastName);
            HttpContext.Session.SetString("Email", user.Email);
            if (user.Avatar != null)
            {
                HttpContext.Session.SetString("Avatar", user.Avatar);
            }
            else
            {
                HttpContext.Session.SetString("Avatar", "");
            }
            HttpContext.Response.Cookies.Append("token", returnToken, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddMinutes(20) });

            if (!string.IsNullOrEmpty(obj.ReturnUrl))
            {
                return LocalRedirect(obj.ReturnUrl);
            }

            if (user.Role == "Admin")
            {
                TempData["success"] = "Login Successfully...";
                return RedirectToAction("dashboard", "Admin");

            }
            TempData["success"] = "Login Successfully...";
            return RedirectToAction("gridView", "Platform");
            //return Ok(returnToken);
        }

        public IActionResult registration()
        {
            ViewBag.Banner = _UserRepository.BannerList();
            return View();
        }
        
        [Authorize]
        public IActionResult Invalid()
        {
            return View();
        }
        
        public IActionResult UsersData()
        {
            var user = _UserRepository.GetUserTableList();
            ViewBag.User = user;
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult registration(RegistrationViewModel obj)
        {
            if (ModelState.IsValid)
            {
                var user = _UserRepository.GetUserTableList().Where(a => a.Email == obj.Email).Count();
                if (user != 0)
                {
                    TempData["error"] = "User already exists.";
                    ViewBag.Banner = _UserRepository.BannerList();
                    return View(obj);
                }
                _UserRepository.register(obj);
                TempData["success"] = "Registration Successfull";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult forgotPassword()
        {
            ViewBag.Banner = _UserRepository.BannerList();
            return View();
        }

        [HttpPost]
        public IActionResult forgotPassword(User obj)
        {
            if (obj.Email != null)
            {
                var user = _UserRepository.forgot(obj);
                if (user == null)
                {
                    TempData["error"] = "Email doesn't exists!!";
                    return View();
                }
                TempData["info"] = "Check your email to reset password";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Enter your email id";
            return View();
        }

        public IActionResult reset()
        {
            ViewBag.Banner = _UserRepository.BannerList();
            return View();
        }

        [HttpPost]
        public IActionResult Reset(RegistrationViewModel obj, string token)
        {
            var validToken = _UserRepository.reset(obj, token);
            if (validToken != null)
            {
                if (validToken.Expire == true)
                {
                    TempData["error"] = "Token is expired";
                    return RedirectToAction("Index");
                }

                TempData["success"] = "Your Password is changed";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Token not Found";
            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            //HttpContext.SignOutAsync().Wait();
            HttpContext.Session.Clear();
            Response.Cookies.Delete("token");
            return RedirectToAction("Index");
        }
        
        [Authorize]
        public IActionResult privacyPolicy()
        {
            string name = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = name;
            string Avatar = HttpContext.Session.GetString("Avatar");
            ViewBag.Avatar = (Avatar == "") ? "user1.png" : Avatar;
            long userId = (int)HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;
            MissionViewModel mv = new MissionViewModel();

            List<CmsPage> cmsPage = _UserRepository.cmsPages();
            mv.cmsPages = cmsPage;
            return View(mv);
        }
        
        [Authorize]
        public IActionResult editProfile(int id)
        {
            string name = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = name;
            string Avatar = HttpContext.Session.GetString("Avatar");
            ViewBag.Avatar = (Avatar == "") ? "user1.png" : Avatar;

            long userId = (int)HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;

            List<EditProfileViewModel> editProfiles = _UserRepository.getUserList(userId);
            var modelData = editProfiles.Where(x => x.UserId == id).FirstOrDefault();

            return View(modelData);
        }

        [HttpPost]
        public IActionResult editProfile(EditProfileViewModel editProfileView)
        {
            string name = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = name;
            string Avatar = HttpContext.Session.GetString("Avatar");
            ViewBag.Avatar = (Avatar == "") ? "user1.png" : Avatar;

            long userId = (int)HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;

            if (editProfileView != null)
            {
                var data = _UserRepository.updateUserData(editProfileView, userId);
                if (editProfileView.Profile != null)
                {
                    HttpContext.Session.SetString("Avatar", editProfileView.Profile.FileName);
                }
                HttpContext.Session.SetString("UserName", editProfileView.FirstName + " " + editProfileView.LastName);

                TempData["success"] = "User Profile Update Successfully...";
                return RedirectToAction("gridView", "Platform");

            }
            return View();
        }

        public JsonResult userEditProfile(string OldPassword, string Password, string FirstName, string Email, string subject, string Message)
        {

            long userId = (int)HttpContext.Session.GetInt32("UserId");

            if (OldPassword != null)
            {
                var data = _UserRepository.checkOldpassword(OldPassword, userId);
                return Json(data);
            }
            if (Password != null)
            {
                var data = _UserRepository.changeUserPassword(Password, userId);
                return Json(data);
            }
            if (FirstName != null && Email != null && subject != null && Message != null)
            {
                var data = _UserRepository.contactUs(FirstName, Email, subject, Message, userId);
                return Json(data);
            }
            return Json(null);
        }

        [Authorize]
        public IActionResult timeSheet()
        {
            string name = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = name;
            string Avatar = HttpContext.Session.GetString("Avatar");
            ViewBag.Avatar = (Avatar == "") ? "user1.png" : Avatar;
            long userId = (int)HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;

            List<EditProfileViewModel> editProfiles = _UserRepository.getUserList(userId);
            var modelData = editProfiles.Where(x => x.UserId == userId).FirstOrDefault();

            return View(modelData);
        }

        public JsonResult addTimeSheets(long MissionId, int goalAction, DateTime Date, string Message, int timeHour, int timeMinute, long TimesheetId)
        {
            long userId = (int)HttpContext.Session.GetInt32("UserId");


            //var checkDate = _UserRepository.checkSameDate(Date, userId, TimesheetId, MissionId);
            //if (checkDate == false)
            //{
            //    return Json(checkDate);
            //}

            var data = _UserRepository.addTimeSheets(userId, MissionId, goalAction, Date, Message, timeHour, timeMinute, TimesheetId);
            return Json(data);
        }
        
        public JsonResult getTimeSheets(long TimesheetId)
        {
            var data1 = _UserRepository.getTimeSheets(TimesheetId);
            return Json(data1);
        }

        public JsonResult checkSameDate(long MissionId, DateTime Date, long TimesheetId)
        {
            long userId = (int)HttpContext.Session.GetInt32("UserId");

            var date = _UserRepository.checkSameDate(Date, userId, TimesheetId, MissionId);
            return Json(date);
        }

        public JsonResult UpdateSetting(List<string> settingList)
        {
            long userId = (int)HttpContext.Session.GetInt32("UserId");
            _UserRepository.UpdateSetting(settingList, userId);
            return Json(true);
        }
        
        public void ReadNotification(long NotificationId)
        {
            _UserRepository.ReadNotification(NotificationId);
        }

        public IActionResult ExportAsPdf()
        {
            long userId = (int)HttpContext.Session.GetInt32("UserId");
            var report = StiReport.CreateNewReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/DataViaBusinessObject.mrt");
            report.Load(path);
            //report.Dictionary.Variables["UserId"].Value = userId.ToString();
            //// Find the TextBox element by its name or other means
            //StiText textBox = report.GetComponentByName("Text2") as StiText;
            //// Update the value or expression of the TextBox
            //textBox.Text.Value = "bablu";
            //(report.GetComponentByName("Text2") as StiText).Text.Value = "BAaaaaaaaDMASS";
            (report.GetComponentByName("Text13") as StiText).Text.Value = "TATVASOFT";

            //report.Compile();

            return StiNetCoreReportResponse.ResponseAsPdf(report);
        }

        public IActionResult ExportAsWord()
        {
            long userId = (int)HttpContext.Session.GetInt32("UserId");
            var report = StiReport.CreateNewReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/EmployeeProfile.mrt");
            report.Load(path);
            report.Dictionary.Variables["UserId"].Value = userId.ToString();

            return StiNetCoreReportResponse.ResponseAsWord2007(report);

        }
        
        public IActionResult ExportAsPng()
        {
            long userId = (int)HttpContext.Session.GetInt32("UserId");
            var report = StiReport.CreateNewReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/EmployeeProfile.mrt");
            report.Load(path);
            report.Dictionary.Variables["UserId"].Value = userId.ToString();

            return StiNetCoreReportResponse.ResponseAsPng(report);
        }
    }
}
