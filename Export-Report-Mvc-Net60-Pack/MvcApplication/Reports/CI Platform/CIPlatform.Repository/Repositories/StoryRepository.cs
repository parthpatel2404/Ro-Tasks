using CIPlatform.Entities.Data;
using CIPlatform.Entities.Models;
using CIPlatform.Entities.ViewModel;
using CIPlatform.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Microsoft.AspNetCore.Http;

namespace CIPlatform.Repository.Repositories
{
    public class StoryRepository : IStoryRepository
    {
        public readonly CIPlatformDbContext _CIPlatformDbContext;
        public readonly IPlatformRepository _PlatformRepository;
        public StoryRepository(CIPlatformDbContext cIPlatformDbContext, IPlatformRepository platformRepository)
        {
            _CIPlatformDbContext = cIPlatformDbContext;
            _PlatformRepository = platformRepository;
        }

        public List<MissionViewModel> getStoryCardList(long userId)
        {
            var stories = _CIPlatformDbContext.Stories.Include(s => s.StoryMedia).Where(x => x.UserId == userId ? true : x.Status != "DRAFT").ToList();

            List<MissionViewModel> storyListingList = new List<MissionViewModel>();
            foreach (var obj in stories)
            {
                var missionskill = _CIPlatformDbContext.MissionSkills.FirstOrDefault(u => u.MissionId == obj.MissionId);
                var missions = _CIPlatformDbContext.Missions.FirstOrDefault(u => u.MissionId == obj.MissionId);
                var users = _CIPlatformDbContext.Users.FirstOrDefault(u => u.UserId == obj.UserId);
                //List<MissionSkill> missionskill1 = _CIPlatformDbContext.MissionSkills.Include(c => c.Skill).Where(c => obj.MissionId == c.MissionId).ToList();
                //List<Comment> comments = _CIPlatformDbContext.Comments.Where(a => a.MissionId == obj.MissionId && a.ApprovalStatus == "Approve").Include(c => c.User).Include(c => c.Mission).ToList();

                MissionViewModel storyListing = new MissionViewModel();

                storyListing.MissionId = obj.MissionId;
                storyListing.UserId = obj.UserId;
                storyListing.StoryId = obj.StoryId;
                storyListing.Avatar = users.Avatar;
                storyListing.UserName = users.FirstName + " " + users.LastName;
                storyListing.StoryTitle = obj.Title;
                storyListing.Description = obj.Description;
                storyListing.whyIVolunteer = users.WhyIVolunteer;
                //storyListing.ShortDescription = obj.ShortDescription; 
                storyListing.PublishedDate = obj.PublishedAt;
                storyListing.SkillId = missionskill.SkillId;
                storyListing.MediaPath = obj.StoryMedia.FirstOrDefault(x => x.StoryId == storyListing.StoryId).Path;
                storyListing.ListMediaPath = obj.StoryMedia.Where(x => x.StoryId == storyListing.StoryId && x.Type == "png").ToList();
                storyListing.Theme = _PlatformRepository.getMissionThemes(missions.ThemeId);
                storyListing.ThemeId = (int)missions.ThemeId;
                storyListing.CityId = missions.CityId;
                storyListing.CountryId = missions.CountryId;
                storyListing.missionTitles = getMissionList(storyListing.UserId);
                var allUser = _CIPlatformDbContext.Users.Where(x => x.DeletedAt == null).ToList();
                var alreaduInvite = _CIPlatformDbContext.StoryInvites.Where(x => x.FromUserId == userId && x.StoryId == storyListing.StoryId).Include(x => x.ToUser).ToList();
                foreach (var i in alreaduInvite)
                {
                    allUser = allUser.Where(x => x.UserId != i.ToUserId).ToList();
                }

                storyListing.userList = allUser;
                storyListing.alreadyInvited = alreaduInvite;
                storyListing.views = _CIPlatformDbContext.StoryViews.Where(s => s.StoryId == storyListing.StoryId).Count();
                storyListing.storyStatus = obj.Status;
                var URL1 = _CIPlatformDbContext.StoryMedia.Where(x => x.StoryId == storyListing.StoryId && x.Type == "mp4").ToList();
                var stringList = new List<string>();
                foreach (var item in URL1)
                {
                    string stl = item.Path;
                    string newLink = stl.Replace("/watch?v=", "/embed/");
                    //stl.Replace("/watch?v=", "/embed/");
                    stringList.Add(newLink);
                }
                storyListing.URL = stringList;
                //storyListing.file = getFile(storyListing.StoryId);

                //storyListing.userList = userList();
                //storyListing.OrganizationName = obj.OrganizationName;

                storyListingList.Add(storyListing);
            }
            return storyListingList;
        }

        public List<MissionViewModel> getStoryList(string? search, string[] countries, string[] cities, string[] themes, string[] skills, int pg, long userId)
        {
            var pageSize = 3;
            List<MissionViewModel> stories = getStoryCardList(userId);

            if (search != "")
            {
                search = search.ToLower();
                stories = stories.Where(a => a.StoryTitle.ToLower().Contains(search) || a.Description.ToLower().Contains(search) || a.UserName.ToLower().Contains(search)).ToList();
            }
            if (countries.Length > 0)
            {
                stories = stories.Where(c => countries.Contains(c.CountryId.ToString())).ToList();
            }
            if (cities.Length > 0)
            {
                stories = stories.Where(c => cities.Contains(c.CityId.ToString())).ToList();
            }
            if (themes.Length > 0)
            {
                stories = stories.Where(c => themes.Contains(c.ThemeId.ToString())).ToList();
            }
            if (skills.Length > 0)
            {
                stories = stories.Where(c => skills.Contains(c.SkillId.ToString())).ToList();
            }
            if (pg != 0)
            {
                stories = stories.Skip((pg - 1) * pageSize).Take(pageSize).ToList();
            }

            return stories;
        }

        public List<Mission> getMissionList(long userId)
        {
            var validUser = _CIPlatformDbContext.MissionApplications.Where(a => a.UserId == userId && a.ApprovalStatus == "Approve").ToList();

            var list = new List<long>();

            foreach (var app in validUser)
            {
                list.Add(app.MissionId);
            }

            var missions = _CIPlatformDbContext.Missions.Where(a => list.Contains(a.MissionId)).ToList();

            return missions;
        }

        //public List<StoryMedium> getStoryPhoto(int? storyId)
        //{
        //    if (storyId != 0)
        //    {
        //        List<StoryMedium> storyMedia = _CIPlatformDbContext.StoryMedia.Where(s => s.StoryId == storyId).ToList();
        //        return storyMedia;
        //    }
        //    return null;
        //}

        public bool sendMail(string[] emailList, long storyId, long userId)
        {
            User fromUser = _CIPlatformDbContext.Users.FirstOrDefault(u => u.UserId == userId);
            foreach (var item in emailList)
            {
                User toUser = _CIPlatformDbContext.Users.FirstOrDefault(u => u.Email == item);
                StoryInvite svi = _CIPlatformDbContext.StoryInvites.FirstOrDefault(x => x.ToUserId == toUser.UserId);
                if (svi == null)
                {

                    StoryInvite storyInvite = new StoryInvite();
                    storyInvite.FromUserId = userId;
                    storyInvite.ToUserId = toUser.UserId;
                    storyInvite.StoryId = storyId;
                    _CIPlatformDbContext.StoryInvites.Add(storyInvite);
                }

            }
            _CIPlatformDbContext.SaveChanges();

            var mailBody = "<h1>" + fromUser.FirstName + "Recommended Story</h1><br><h2><a href='" + "https://localhost:7001/Story/storyDetail?id=" + storyId + "'>Go to Story</a></h2>";
            foreach (var item in emailList)
            {
                User toUser = _CIPlatformDbContext.Users.FirstOrDefault(u => u.Email == item);
                StoryInvite svie = _CIPlatformDbContext.StoryInvites.FirstOrDefault(x => x.ToUserId == toUser.UserId);
                if (svie == null)
                {


                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse("parthv480@gmail.com"));
                    email.To.Add(MailboxAddress.Parse(item));
                    email.Subject = "Co-Worker Suggestion";
                    email.Body = new TextPart(TextFormat.Html) { Text = mailBody };

                    // send email
                    using var smtp = new SmtpClient();
                    smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate("parthv480@gmail.com", "xnbmnadzgazdgeim");
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }
            }
            return true;
        }

        public void views(int? storyId, long userId)
        {
            var view1 = _CIPlatformDbContext.StoryViews.FirstOrDefault(s => s.StoryId == storyId && s.UserId == userId);
            if (view1 == null)
            {
                StoryView sv = new StoryView();
                sv.UserId = userId;
                sv.StoryId = storyId;
                _CIPlatformDbContext.StoryViews.Add(sv);
                _CIPlatformDbContext.SaveChanges();
            }
        }

        public bool addStory(MissionViewModel mvm, long userId, string name)
        {
            Story stories = _CIPlatformDbContext.Stories.FirstOrDefault(s => s.UserId == userId && s.MissionId == mvm.MissionId);
            if (stories == null)
            {
                Story story = new Story();
                story.Title = mvm.StoryTitle;
                story.Description = mvm.Description;
                story.UserId = userId;
                story.MissionId = mvm.MissionId;
                story.PublishedAt = mvm.PublishedDate;
                if (name == "Save")
                {
                    story.Status = "DRAFT";
                }
                if (name == "Submit")
                {
                    story.Status = "PENDING";
                }
                _CIPlatformDbContext.Stories.Add(story);
                _CIPlatformDbContext.SaveChanges();
            }
            if (stories != null)
            {
                stories.Title = mvm.StoryTitle;
                stories.Description = mvm.Description;
                stories.UserId = userId;
                stories.MissionId = mvm.MissionId;
                stories.PublishedAt = mvm.PublishedDate;
                if (name == "Save")
                {
                    stories.Status = "DRAFT";
                }
                if (name == "Submit")
                {
                    stories.Status = "PENDING";
                }
                _CIPlatformDbContext.Stories.Update(stories);
                _CIPlatformDbContext.SaveChanges();
            }

            //List<StoryMedium> storyMedia = _CIPlatformDbContext.StoryMedia.Where(m => m.StoryId == (_CIPlatformDbContext.Stories.FirstOrDefault(u => u.UserId == userId && u.MissionId == mvm.MissionId).StoryId)).ToList();
            //foreach (var item in storyMedia)
            //{
            //    _CIPlatformDbContext.StoryMedia.Remove(item);
            //}
            var filePath = new List<string>();
            if (mvm.file != null)
            {
                foreach (var i in mvm.file)
                {
                    StoryMedium storyMedium = new StoryMedium();
                    storyMedium.StoryId = _CIPlatformDbContext.Stories.FirstOrDefault(s => s.UserId == userId && s.MissionId == mvm.MissionId).StoryId;
                    storyMedium.Type = "png";
                    //storyMedium.Type = i.ContentType.Substring(6);
                    storyMedium.Path = i.FileName;
                    _CIPlatformDbContext.StoryMedia.Add(storyMedium);
                    if (i.Length > 0)
                    {
                        //string path = Server.MapPath("~/wwwroot/Assets/Story");
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets/Story", i.FileName);
                        filePath.Add(path);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            i.CopyTo(stream);
                        }
                    }

                }
            }

            var url = new List<string>();
            if (mvm.URL[0] != null)
            {
                foreach (var j in mvm.URL)
                {
                    StoryMedium sm = new StoryMedium();
                    sm.StoryId = _CIPlatformDbContext.Stories.FirstOrDefault(s => s.UserId == userId && s.MissionId == mvm.MissionId).StoryId;
                    sm.Type = "mp4";
                    sm.Path = j;
                    _CIPlatformDbContext.StoryMedia.Add(sm);
                }
            }
            _CIPlatformDbContext.SaveChanges();
            return true;
        }
        //public IFormFile getFile(long storyId)
        //{
        //    var path = _CIPlatformDbContext.StoryMedia.Where(x => x.StoryId == storyId).FirstOrDefault();
        //    //foreach (var i in path)
        //    //{
        //    string storyPath = "/wwwroot/Assets/Story/" + path.Path;
        //    using (var stream = System.IO.File.OpenRead(storyPath))
        //    {
        //        MissionViewModel model = new MissionViewModel();

        //        model.file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
        //        return model.file;
        //    }
        //    //}

        //}
    }
}