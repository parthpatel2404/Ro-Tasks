using CIPlatform.Entities.Models;
using CIPlatform.Entities.ViewModel;
using CIPlatform.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CI_Platform.Controllers
{
    [Authorize]
    public class PlatformController : Controller
    {
        public readonly IPlatformRepository _PlatformRepository;
        public readonly IUserRepository _UserRepository;
        public PlatformController(IPlatformRepository platformRepository, IUserRepository userRepository)
        {
            _PlatformRepository = platformRepository;
            _UserRepository = userRepository;
        }
        public IActionResult gridView(int pgSize, string type)
        {
            string name = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = name;
            string Avatar = HttpContext.Session.GetString("Avatar");
            ViewBag.Avatar = (Avatar == "") ? "user1.png" : Avatar;
            long userId = (int)HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;
            _UserRepository.addNotificationList(userId);
            var notification = _UserRepository.Notifications(userId);
            ViewBag.Notifications = notification;
            var email = HttpContext.Session.GetString("Email");
            List<Mission> missions = _PlatformRepository.getMissions();

            if (missions.Count == 0)
            {
                return PartialView("_noMissionFound");
            }
            User user = _UserRepository.getUser(email);
            //user.Avatar= string.IsNullOrEmpty(user.Avatar) ? "volunteer3.png" : user.Avatar;
            List<MissionViewModel> missionlistingList = _PlatformRepository.getMissionCardsList(user.UserId);
            ViewBag.missionListing = missionlistingList;

            List<Country> country = _PlatformRepository.getCountryList();
            ViewBag.Country = country;

            List<MissionTheme> theme = _PlatformRepository.getThemeList();
            ViewBag.Theme = theme;

            List<Skill> skill = _PlatformRepository.getSkillList();
            ViewBag.Skill = skill;

            var totalMission = _PlatformRepository.getMissionCount();
            ViewBag.totalMission = totalMission;

            ViewBag.user = user;
            if (pgSize == 0)
            {
                pgSize = 2;
            }
            string[] theme1s = {};
            string[] skill1s = {};
            var cityid = user.CityId.ToString().ToCharArray().Select(c=>c.ToString()).ToArray();
            var countryid = user.CountryId.ToString().ToCharArray().Select(c => c.ToString()).ToArray();
            if (cityid != null && countryid!= null)
            {
                List<MissionViewModel> m1 = _PlatformRepository.getMissionList("", countryid, cityid, theme1s, skill1s, 0, userId, 1, pgSize);
                ViewBag.Totalpages = Math.Ceiling(m1.Count() / (float)pgSize);
                ViewBag.totalMission = m1.Count()+"(by default user's location wise)";
                ViewBag.missionListing = m1.Skip((1 - 1) * pgSize).Take(pgSize).ToList();
                ViewBag.pg_no = 1;
                return View();
            }


            ViewBag.Totalpages = Math.Ceiling(missionlistingList.Count() / (float)pgSize);
            ViewBag.missionListing = missionlistingList.Skip((1 - 1) * pgSize).Take(pgSize).ToList();
            ViewBag.pg_no = 1;
            //int fav = _PlatformRepository.getFavMission(user.UserId, (long)missionlistingList.);
            //ViewBag.fav = fav;
            //int appMiss = _PlatformRepository.getAppMission(user.UserId, (long)id);
            //ViewBag.appMiss = appMiss;

            if (pgSize >= 1 && type=="grid")
            {
                return PartialView("_gridCardPartial");
            }
            if (pgSize >= 1 && type=="list")
            {
                return PartialView("_listCardPartial");
            }
            //return View();
            return View(missions.FirstOrDefault());
        }

        public ActionResult Search(string search, string[] countries, string[] cities, string[] themes, string[] skills, int? sort, int pg, int pgSize)
        {
            long userId = (int)HttpContext.Session.GetInt32("UserId");

            search = string.IsNullOrEmpty(search) ? "" : search;

            List<MissionViewModel> pagination = _PlatformRepository.getMissionList(search, countries, cities, themes, skills, sort, userId, 0, pgSize);
            List<MissionViewModel> missions = _PlatformRepository.getMissionList(search, countries, cities, themes, skills, sort, userId, pg, pgSize);


            if (missions.Count == 0)
            {
                return PartialView("_noMissionFound");
            }
            else if (missions.Count >= 1)
            {
                ViewBag.totalMission = pagination.Count;
            }

            if (pgSize == 0)
            {
                pgSize = 2;
            }
            ViewBag.pg_no = pg;
            ViewBag.Totalpages = Math.Ceiling(pagination.Count() / (float)pgSize);
            ViewBag.missionListing = missions.Skip((1 - 1) * pgSize).Take(pgSize).ToList();

            return PartialView("_gridCardPartial");

        }

        public ActionResult SearchList(string search, string[] countries, string[] cities, string[] themes, string[] skills, int? sort, int pg, int pgSize)
        {
            long userId = (int)HttpContext.Session.GetInt32("UserId");

            search = string.IsNullOrEmpty(search) ? "" : search;

            List<MissionViewModel> pagination = _PlatformRepository.getMissionList(search, countries, cities, themes, skills, sort, userId, 0, pgSize);
            List<MissionViewModel> missions = _PlatformRepository.getMissionList(search, countries, cities, themes, skills, sort, userId, pg, pgSize);


            if (missions.Count == 0)
            {
                return PartialView("_noMissionFound");
            }
            else if (missions.Count >= 1)
            {
                ViewBag.totalMission = pagination.Count;
            }
            if (pgSize == 0)
            {
                pgSize = 2;
            }

            ViewBag.pg_no = pg;
            ViewBag.Totalpages = Math.Ceiling(pagination.Count() / (float)pgSize);
            ViewBag.missionListing = missions.Skip((1 - 1) * pgSize).Take(pgSize).ToList();

            return PartialView("_listCardPartial");

        }

        public JsonResult getCity(List<string> countryId)
        {
            List<City> city = _PlatformRepository.getCityList(countryId);
            var json = JsonConvert.SerializeObject(city);
            return Json(json);
        }


        public IActionResult missionDetails(int? id)
        {
            long userId = (int)HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;
            string Avatar = HttpContext.Session.GetString("Avatar");
            ViewBag.Avatar = (Avatar == "") ? "user1.png" : Avatar;



            string name = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = name;
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var missionlistingList = _PlatformRepository.getMissionCardsList(userId).FirstOrDefault(u => u.MissionId == id);
            var relatedMissions = _PlatformRepository.getMissionCardsList(userId).Where
            (r => (r.CityName == missionlistingList.CityName 
            || r.CountryId == missionlistingList.CountryId 
            || r.Theme == missionlistingList.Theme) 
            && r.MissionId != id)
            .OrderByDescending(r => r.CityId == missionlistingList.CityId)
            .ThenByDescending(r => r.CountryId == missionlistingList.CountryId)
            .ThenByDescending(r => r.ThemeId == missionlistingList.ThemeId)
            .ThenByDescending(r => r.OrganizationName == missionlistingList.OrganizationName)
            .Take(3)
            .ToList();

            //relatedMissions.Remove(missionlistingList);
            ViewBag.relatedMissions = relatedMissions;
            //ViewBag.relCount = relatedMissions.Count();
            if (missionlistingList == null)
            {
                return NotFound();
            }
            //int fav = _PlatformRepository.getFavMission(userId, (long)id);
            //ViewBag.fav = fav;
            //int appMiss = _PlatformRepository.getAppMission(userId, (long)id);
            //ViewBag.appMiss = appMiss;
            List<MissionRating> rating = _PlatformRepository.getMissionRatings((long)id).ToList();
            var oUser = rating.Where(x => x.UserId == userId).FirstOrDefault()?.Rating;
            if (oUser == null)
            {
                oUser = 0;
            }
            ViewBag.oUser = oUser;
            var pag = _PlatformRepository.getMisAppList(0, (long)id);
            ViewBag.Totalpag = Math.Ceiling(pag.Count() / 1.0);
            ViewBag.pag = pag.Skip((1 - 1) * 1).Take(1).ToList();
            ViewBag.pag_no = 1;
            //var rVolunteer = _PlatformRepository.getMisAppList(1,(long)id);
            //ViewBag.avatar = pag.FirstOrDefault().User.Avatar;
            //ViewBag.fname = pag.FirstOrDefault().User.FirstName;
            //ViewBag.lname = pag.FirstOrDefault().User.LastName;

            return View(missionlistingList);
        }

        [HttpPost]
        public bool addToFavourite(int missionId)
        {
            var userId = (int)HttpContext.Session.GetInt32("UserId");
            var favourite = _PlatformRepository.addFavourite(missionId, userId);
            return favourite;
        }

        [HttpPost]
        public int applyMission(int missionId)
        {
            var userId = (int)HttpContext.Session.GetInt32("UserId");
            var apply = _PlatformRepository.applyMission(missionId, userId);
            if (apply == 1)
            {
                return apply;
            }
            else if (apply == 2)
            {
                return apply;
            }
            return 0;
        }


        public void addComments(string comment, long MissionId)
        {
            long userId = (int)HttpContext.Session.GetInt32("UserId");

            _PlatformRepository.addComments(comment, userId, MissionId);
        }

        //[HttpGet]
        //public IActionResult SuggestCoWorker()
        //{
        //    var mission = _PlatformRepository.userList();
        //    return PartialView("_CoWorkerPartial", mission);
        //}
        [HttpPost]
        public JsonResult RecommendCoWorker(string[] emailList, long missionId)
        {
            long userId = (int)HttpContext.Session.GetInt32("UserId");

            if (emailList != null)
            {
                var mail = _PlatformRepository.sendMail(emailList, missionId, userId);
                return Json(mail);
            }
            return Json(null);
        }

        [HttpPost]
        public JsonResult RateMission(int missionId, int rate)
        {
            long userId = (int)HttpContext.Session.GetInt32("UserId");
            var rating = _PlatformRepository.addRatings(rate, missionId, userId);
            return Json(rating);
        }

        [HttpPost]
        public IActionResult rVolunteer(int pg, long missionId)
        {
            List<MissionViewModel> rVolunteer = _PlatformRepository.getMisAppList(pg, missionId);

            ViewBag.Totalpag = Math.Ceiling(_PlatformRepository.getMisAppList(0, missionId).Count() / 1.0);
            ViewBag.pag = rVolunteer.Skip((1 - 1) * 1).Take(1).ToList();
            ViewBag.pag_no = 1;
            return PartialView("_recentVolunteers");
        }
    }
}
