using CIPlatform.Entities.ViewModel;
using CIPlatform.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CI_Platform.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(BaseController))]
    public class AdminController : Controller
    {
        public readonly IAdminRepository _AdminRepository;
        public AdminController(IAdminRepository adminRepository)
        {
            _AdminRepository = adminRepository;
        }

        public IActionResult dashboard()
        {
            //long userId = (int)HttpContext.Session.GetInt32("UserId");
            string Avatar = HttpContext.Session.GetString("Avatar");
            ViewBag.Avatar = (Avatar == "") ? "user1.png" : Avatar;



            string name = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = name;
            var adminView = _AdminRepository.getAdminDetails();

            ViewBag.Totalpages1 = Math.Ceiling(adminView.users.Count() / 3.0);
            ViewBag.Totalpages2 = Math.Ceiling(adminView.cmsPages.Count() / 3.0);
            ViewBag.Totalpages3 = Math.Ceiling(adminView.missions.Count() / 3.0);
            ViewBag.Totalpages4 = Math.Ceiling(adminView.missionThemes.Count() / 10.0);
            ViewBag.Totalpages5 = Math.Ceiling(adminView.skills.Count() / 5.0);
            ViewBag.Totalpages6 = Math.Ceiling(adminView.missionApplications.Count() / 3.0);
            ViewBag.Totalpages7 = Math.Ceiling(adminView.stories.Count() / 3.0);
            ViewBag.Totalpages8 = Math.Ceiling(adminView.banners.Count() / 5.0);
            adminView.users = adminView.users.OrderByDescending(u => u.UserId).Skip((1 - 1) * 3).Take(3).ToList();
            adminView.cmsPages = adminView.cmsPages.Skip((1 - 1) * 3).Take(3).ToList();
            adminView.missions = adminView.missions.Skip((1 - 1) * 3).Take(3).ToList();
            adminView.missionThemes = adminView.missionThemes.Skip((1 - 1) * 10).Take(10).ToList();
            adminView.skills = adminView.skills.Skip((1 - 1) * 5).Take(5).ToList();
            adminView.missionApplications = adminView.missionApplications.Skip((1 - 1) * 3).Take(3).ToList();
            adminView.stories = adminView.stories.Skip((1 - 1) * 3).Take(3).ToList();
            adminView.banners = adminView.banners.Skip((1 - 1) * 5).Take(5).ToList();
            ViewBag.pg_no = 1;
            return View(adminView);
        }

        public IActionResult Search(string? search, int pg, string type)
        {
            search = string.IsNullOrEmpty(search) ? "" : search;
            AdminViewModel pagination = _AdminRepository.getSearchAdmin(search, 0);
            AdminViewModel adminView = _AdminRepository.getSearchAdmin(search, pg);

            ViewBag.pg_no = pg;
            ViewBag.Totalpages1 = Math.Ceiling(pagination.users.Count() / 3.0);
            ViewBag.Totalpages2 = Math.Ceiling(pagination.cmsPages.Count() / 3.0);
            ViewBag.Totalpages3 = Math.Ceiling(pagination.missions.Count() / 3.0);
            ViewBag.Totalpages4 = Math.Ceiling(pagination.missionThemes.Count() / 10.0);
            ViewBag.Totalpages5 = Math.Ceiling(pagination.skills.Count() / 5.0);
            ViewBag.Totalpages6 = Math.Ceiling(pagination.missionApplications.Count() / 3.0);
            ViewBag.Totalpages7 = Math.Ceiling(pagination.stories.Count() / 3.0);
            ViewBag.Totalpages8 = Math.Ceiling(pagination.banners.Count() / 5.0);
            if (type == "user")
            {
                return PartialView("_userAdminPartial", adminView);
            }
            else if (type == "cms")
            {
                return PartialView("_cmsAdminPartial", adminView);
            }
            else if (type == "mission")
            {
                return PartialView("_missionAdminPartial", adminView);
            }
            else if (type == "theme")
            {
                return PartialView("_themeAdminPartial", adminView);
            }
            else if (type == "skill")
            {
                return PartialView("_skillAdminPartial", adminView);
            }
            else if (type == "application")
            {
                return PartialView("_applicationAdminPartial", adminView);
            }
            else if (type == "story")
            {
                return PartialView("_storyAdminPartial", adminView);
            }
            else if (type == "banner")
            {
                return PartialView("_bannerAdminPartial", adminView);
            }
            return View(adminView);
        }


        public IActionResult addViaAdmin(MissionViewModel userData, string type, long Id)
        {
            _AdminRepository.addViaAdmin(userData, type, Id);
            var adminView = _AdminRepository.getAdminDetails();
            ViewBag.Totalpages1 = Math.Ceiling(adminView.users.Count() / 3.0);
            ViewBag.Totalpages2 = Math.Ceiling(adminView.cmsPages.Count() / 3.0);
            ViewBag.Totalpages3 = Math.Ceiling(adminView.missions.Count() / 3.0);
            ViewBag.Totalpages4 = Math.Ceiling(adminView.missionThemes.Count() / 10.0);
            ViewBag.Totalpages5 = Math.Ceiling(adminView.skills.Count() / 5.0);
            ViewBag.Totalpages6 = Math.Ceiling(adminView.missionApplications.Count() / 3.0);
            ViewBag.Totalpages7 = Math.Ceiling(adminView.stories.Count() / 3.0);
            ViewBag.Totalpages8 = Math.Ceiling(adminView.banners.Count() / 5.0);

            adminView.users = adminView.users.OrderByDescending(u => u.UserId).Skip((1 - 1) * 3).Take(3).ToList();
            adminView.cmsPages = adminView.cmsPages.Skip((1 - 1) * 3).Take(3).ToList();
            adminView.missions = adminView.missions.Skip((1 - 1) * 3).Take(3).ToList();
            adminView.missionThemes = adminView.missionThemes.Skip((1 - 1) * 10).Take(10).ToList();
            adminView.skills = adminView.skills.Skip((1 - 1) * 5).Take(5).ToList();
            adminView.missionApplications = adminView.missionApplications.Skip((1 - 1) * 3).Take(3).ToList();
            adminView.stories = adminView.stories.Skip((1 - 1) * 3).Take(3).ToList();
            adminView.banners = adminView.banners.Skip((1 - 1) * 5).Take(5).ToList();

            ViewBag.pg_no = 1;
            if (userData.MissionType == "user" || type == "user")
            {
                return PartialView("_userAdminPartial", adminView);
            }
            else if (userData.MissionType == "cms" || type == "cms")
            {
                return PartialView("_cmsAdminPartial", adminView);
            }
            else if (userData.MissionType == "mission" || type == "mission")
            {
                return PartialView("_missionAdminPartial", adminView);
            }
            else if (userData.MissionType == "theme" || type == "theme")
            {
                return PartialView("_themeAdminPartial", adminView);
            }
            else if (userData.MissionType == "skill" || type == "skill")
            {
                return PartialView("_skillAdminPartial", adminView);
            }
            else if (type == "approveApplication" || type == "declineApplication")
            {
                return PartialView("_applicationAdminPartial", adminView);
            }
            else if (type == "approveStory" || type == "declineStory" || type == "deleteStory")
            {
                return PartialView("_storyAdminPartial", adminView);
            }
            else if (userData.MissionType == "banner" || type == "banner")
            {
                return PartialView("_bannerAdminPartial", adminView);
            }
            return View();
        }

        public JsonResult getViaAdmin(long Id, string type)
        {
            var data = _AdminRepository.getViaAdmin(Id, type);

            if (type == "user")
            {
                var a = data.users.Select(x => new { x.Avatar, x.Role, x.FirstName, x.LastName, x.Email, x.Password, x.PhoneNumber, x.EmployeeId, x.Department, CityName = x.City?.Name, CountryName = x.Country?.Name, x.Status, x.ProfileText, x.UserId }).FirstOrDefault();
                return Json(a);
            }
            else if (type == "cms")
            {
                return Json(data.cmsPages.FirstOrDefault());
            }
            else if (type == "mission")
            {
                var mId=data.missions.FirstOrDefault().MissionId;
                var totalSkill = data.totalSkill;
                var skills = data.missionSkills.Where(s=>s.MissionId == mId);
                foreach (var skill in skills)
                {
                    totalSkill = totalSkill.Where(s => s.SkillId != skill.SkillId).ToList();
                }
                var mission = data.missions.Select(x => new
                {
                    x.MissionId,
                    x.Title,
                    x.Description,
                    x.ShortDescription,
                    x.CountryId,
                    x.CityId,
                    x.MissionType,
                    x.OrganizationName,
                    x.OrganizationDetail,
                    x.StartDate,
                    x.EndDate,
                    x.ThemeId,
                    SkillId=x.MissionSkills.Where(m=>m.DeletedAt == null).Select(y => new { y.SkillId,y.Skill.SkillName}),
                    Skill = totalSkill.Select(y => new { y.SkillId, y.SkillName }),
                    x.TotalSeats,
                    x.Deadline,
                    x.Availability,
                    MissionMedia = x.MissionMedia.Select(y => new { y.MediaPath }),
                    MissionDocuments = x.MissionDocuments.Select(y => new { y.DocumentPath }),
                    GoalValue = x.GoalMissions.Select(y => new { y.GoalValue, y.GoalObjectiveText }),
                });
                return Json(mission.FirstOrDefault());
            }
            else if (type == "theme")
            {
                return Json(data.missionThemes.Select(x => new
                {
                    x.MissionThemeId,
                    x.Title,
                    x.Status
                }).FirstOrDefault());
            }
            else if (type == "skill")
            {
                return Json(data.skills.Select(x => new { x.SkillId, x.SkillName, x.Status }).FirstOrDefault());
            }
            else if (type == "application")
            {
                return Json(data.missionApplications.Select(x => new { x.MissionApplicationId }).FirstOrDefault());
            }
            else if (type == "story")
            {
                return Json(data.stories.Select(x => new { x.StoryId }).FirstOrDefault());
            }
            else if (type == "banner")
            {
                return Json(data.banners.Select(x => new { x.BannerId, x.SortOrder, x.Text, x.Image }).FirstOrDefault());
            }
            return Json(null);
        }


        public JsonResult checkSameData(string Email,long sortOrder, long bannerId)
        {
            var date = _AdminRepository.checkSameData(Email, sortOrder, bannerId);
            return Json(date);
        }
    }
}
