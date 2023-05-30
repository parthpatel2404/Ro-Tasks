using CIPlatform.Entities.Models;
using CIPlatform.Entities.ViewModel;
using CIPlatform.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CI_Platform.Controllers
{
    [Authorize]
    public class StoryController : Controller
    {
        public readonly IPlatformRepository _PlatformRepository;
        public readonly IUserRepository _UserRepository;
        public readonly IStoryRepository _StoryRepository;
        public StoryController(IPlatformRepository platformRepository, IUserRepository userRepository, IStoryRepository storyRepository)
        {
            _PlatformRepository = platformRepository;
            _UserRepository = userRepository;
            _StoryRepository = storyRepository;
        }
        public IActionResult storyListing()
        {
            string name = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = name;
            string Avatar = HttpContext.Session.GetString("Avatar");
            ViewBag.Avatar = (Avatar == "") ? "user1.png" : Avatar;

            long userId = (int)HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;

            var email = HttpContext.Session.GetString("Email");
            User user = _UserRepository.getUser(email);
            ViewBag.user = user;

            List<Country> country = _PlatformRepository.getCountryList();
            ViewBag.Country = country;

            List<MissionTheme> theme = _PlatformRepository.getThemeList();
            ViewBag.Theme = theme;

            List<Skill> skill = _PlatformRepository.getSkillList();
            ViewBag.Skill = skill;

            List<MissionViewModel> storylistingList = _StoryRepository.getStoryCardList(user.UserId);

            ViewBag.Totalpages = Math.Ceiling(storylistingList.Count() / 3.0);
            ViewBag.storyListing = storylistingList.Skip((1 - 1) * 3).Take(3).OrderBy(x => x.storyStatus).ToList();
            ViewBag.pg_no = 1;

            return View();
        }

        public ActionResult Search(string? search, string[] countries, string[] cities, string[] themes, string[] skills, int pg)
        {
            long userId = (int)HttpContext.Session.GetInt32("UserId");

            search = string.IsNullOrEmpty(search) ? "" : search;
            List<MissionViewModel> pagination = _StoryRepository.getStoryList(search, countries, cities, themes, skills, 0, userId);
            List<MissionViewModel> stories = _StoryRepository.getStoryList(search, countries, cities, themes, skills, pg, userId);

            if (stories.Count == 0)
            {
                return PartialView("_noMissionFound");
            }

            ViewBag.pg_no = pg;
            ViewBag.Totalpages = Math.Ceiling(pagination.Count() / 3.0);
            ViewBag.storyListing = stories.Skip((1 - 1) * 3).Take(3).OrderBy(x => x.storyStatus).ToList();

            return PartialView("_storyCardPartial");

        }

        public IActionResult shareStory(int? id)
        {
            string name = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = name;
            string Avatar = HttpContext.Session.GetString("Avatar");
            ViewBag.Avatar = (Avatar == "") ? "user1.png" : Avatar;



            long userId = (int)HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;
            var misApp = _StoryRepository.getMissionList(userId);
            ViewBag.misApp = misApp;
            // ahiya aakhul list aave che mission titles na but if story id @!= null hoy matlab k edit mate toh tyre direct ena according title select thavu joiye.
            // ena mate ahiya thi ek title moklo or view ma j direct for loop na else ma ek condition rakho k id!=0 hoy toh aa title selected aave.
            //replace valu b jovu(embeded watch)
            if (misApp.Count != 0)
            {
                var missionId = misApp.FirstOrDefault().MissionId;
                var abc  = _StoryRepository.getStoryCardList(userId).FirstOrDefault(x => x.UserId == userId && x.MissionId == missionId);
                if (abc != null)
                {
                ViewBag.StoryId  = abc.StoryId;

                }
            }

            List<MissionViewModel> mvm = _StoryRepository.getStoryCardList(userId);
            var modelData = mvm.Where(x => x.StoryId == id).FirstOrDefault();
            ViewBag.storyListing = modelData;
            if (modelData != null)
            {
            return View(modelData);

            }
            return View();
            //getStoryPhoto(id);
        }

        //public IActionResult getStoryPhoto(int? storyId)
        //{
        //    var photo = _StoryRepository.getStoryPhoto(storyId);
        //    return Json(photo);
        //}


        [HttpPost]
        public IActionResult shareStory(MissionViewModel mvm, string name)
        {
            string username = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = username;
            string Avatar = HttpContext.Session.GetString("Avatar");
            ViewBag.Avatar = (Avatar == "") ? "user1.png" : Avatar;
            long userId = (int)HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;
            var misApp = _StoryRepository.getMissionList(userId);
            ViewBag.misApp = misApp;

            if (mvm != null)
            {
                _StoryRepository.addStory(mvm, userId, name);

                List<MissionViewModel> mData = _StoryRepository.getStoryCardList(userId);
                var modelData = mData.Where(x => x.StoryId == mvm.StoryId).FirstOrDefault();
                ViewBag.storyListing = modelData;
                if (name == "Save")
                {
                    ViewBag.disabled = true;
                    ViewBag.StoryId = _StoryRepository.getStoryCardList(userId).FirstOrDefault(x => x.UserId == userId && x.MissionId == mvm.MissionId).StoryId;
                    //mvm.ListMediaPath = modelData.ListMediaPath;
                    //mvm.URL = modelData.URL;
                    //TempData["success"] = "story added as Draft...";
                    return View(mvm);
                }
                if (name == "Submit")
                {
                    TempData["success"] = "story added Successfully...";
                    Task.Delay(3000).Wait();
                    TempData["info"] = "wait for approval of story...";
                    return RedirectToAction("storyListing");
                }
            }
            return View();
        }


        public IActionResult storyDetail(int? id)
        {
            string name = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = name;
            string Avatar = HttpContext.Session.GetString("Avatar");
            ViewBag.Avatar = (Avatar == "") ? "user1.png" : Avatar;


            long userId = (int)HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;
            _StoryRepository.views(id, userId);
            var storylistingList = _StoryRepository.getStoryCardList(userId).FirstOrDefault(x => x.StoryId == id);
            return View(storylistingList);
        }

        [HttpPost]
        public JsonResult RecommendCoWorker(string[] emailList, long storyId)
        {
            long userId = (int)HttpContext.Session.GetInt32("UserId");

            if (emailList != null)
            {
                var mail = _StoryRepository.sendMail(emailList, storyId, userId);
                return Json(mail);
            }
            return Json(null);
        }

    }
}


