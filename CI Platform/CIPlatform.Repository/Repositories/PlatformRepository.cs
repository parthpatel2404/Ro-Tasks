using CIPlatform.Entities.Data;
using CIPlatform.Entities.Models;
using CIPlatform.Entities.ViewModel;
using CIPlatform.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Security.Cryptography;

namespace CIPlatform.Repository.Repositories
{
    public class PlatformRepository : IPlatformRepository
    {
        public readonly CIPlatformDbContext _CIPlatformDbContext;
        public PlatformRepository(CIPlatformDbContext cIPlatformDbContext)
        {
            _CIPlatformDbContext = cIPlatformDbContext;
        }

        public List<Country> getCountryList()
        {
            List<Country> countrylist = _CIPlatformDbContext.Countries.ToList();
            return countrylist;
        }

        public List<City> getCityList(List<string> countryId)
        {
            List<City> cityList = _CIPlatformDbContext.Cities.ToList();
            cityList = cityList.Where(i => countryId.Contains(i.CountryId.ToString())).ToList();
            return cityList;
        }

        public List<MissionTheme> getThemeList()
        {
            List<MissionTheme> themeList = _CIPlatformDbContext.MissionThemes.ToList();
            return themeList;
        }

        public List<Skill> getSkillList()
        {
            List<Skill> skillList = _CIPlatformDbContext.Skills.ToList();
            return skillList;
        }

        //public List<Mission> getMissionDetails()
        //{
        //    List<Mission> missionDetails = _CIPlatformDbContext.Missions.ToList();
        //    return missionDetails;
        //}

        public string getCityName(long cityId)
        {

            City city = _CIPlatformDbContext.Cities.FirstOrDefault(i => i.CityId == cityId);
            return city.Name;
        }

        public List<MissionRating> getMissionRatings(long missionId)
        {
            List<MissionRating> rating = _CIPlatformDbContext.MissionRatings.Where(a => a.MissionId == missionId).ToList();
            return rating;
        }

        public string getMissionThemes(long themeID)
        {
            MissionTheme theme = _CIPlatformDbContext.MissionThemes.FirstOrDefault(a => a.MissionThemeId == themeID);
            return theme.Title;
        }

        public int getMissionCount()
        {

            int missionNumber = _CIPlatformDbContext.Missions.Count();
            return missionNumber;

        }

        public string getMediaPathFromId(long missionId)
        {
            MissionMedium media = _CIPlatformDbContext.MissionMedia.FirstOrDefault(x => x.MissionId == missionId);
            return media != null ? media.MediaPath : string.Empty;
        }

        /// <summary>
        ///  DATABSE MA TIMEMISSION NU TBLE BANAVVU. PACHI AA UNCOMMENT KARVU.
        /// </summary>
        /// <returns></returns>
        //public int? getTotalSeatsOfMission(long missionId)
        //{
        //    TimeMission seats = _CIPlatformDbContext.TimeMissions.FirstOrDefault(x => x.MissionId == missionId);
        //    return seats.TotalSeat;
        //}



        public List<Mission> getMissions()
        {
            return _CIPlatformDbContext.Missions.ToList();
        }

        public List<MissionViewModel> getMissionList(string? search, string[] countries, string[] cities, string[] themes, string[] skills, int? sort, long UserId, int pg, int pageSize)
        {
            if (pageSize == 0)
            {
                pageSize = 2;
            }
            List<MissionViewModel> missions = getMissionCardsList(UserId);

            if (search != "")
            {
                search = search.ToLower();
                missions = missions.Where(a => a.Title.ToLower().Contains(search) || a.OrganizationName.ToLower().Contains(search)).ToList();
            }
            if (countries.Length > 0)
            {
                missions = missions.Where(c => countries.Contains(c.CountryId.ToString())).ToList();


            }
            if (cities.Length > 0)
            {
                missions = missions.Where(c => cities.Contains(c.CityId.ToString())).ToList();


            }
            if (themes.Length > 0)
            {
                missions = missions.Where(c => themes.Contains(c.ThemeId.ToString())).ToList();


            }
            if (skills.Length > 0)
            {
                missions = missions.Where(c => skills.Contains(c.SkillId.ToString())).ToList();

            }
            switch (sort)
            {
                case 1:
                    missions = missions.OrderByDescending(i => i.CreatedAt).ToList();
                    break;

                case 2:
                    missions = missions.OrderBy(i => i.CreatedAt).ToList();
                    break;

                case 3:
                    missions = missions.OrderBy(i => i.SeatLeft).ToList();
                    break;

                case 4:
                    missions = missions.OrderByDescending(i => i.SeatLeft).ToList();
                    break;

                case 5:
                    missions = missions.OrderByDescending(x => x.favMission).ToList();
                    break;

                case 6:
                    missions = missions.OrderByDescending(i => i.Deadline).ToList();
                    break;
            }

            if (pg != 0)
            {
                missions = missions.Skip((pg - 1) * pageSize).Take(pageSize).ToList();
            }

            return missions;
        }

        public List<MissionViewModel> getMissionCardsList(long UserId)
        {

            var missions = _CIPlatformDbContext.Missions.ToList();

            List<MissionViewModel> missionListingList = new List<MissionViewModel>();
            foreach (var obj in missions)
            {
                var missionskill = _CIPlatformDbContext.MissionSkills.FirstOrDefault(u => u.MissionId == obj.MissionId);
                List<MissionSkill> missionskill1 = _CIPlatformDbContext.MissionSkills.Include(c => c.Skill).Where(c => obj.MissionId == c.MissionId).ToList();
                List<Comment> comments = _CIPlatformDbContext.Comments.Where(a => a.MissionId == obj.MissionId && a.ApprovalStatus == "Approve").Include(c => c.User).Include(c => c.Mission).ToList();

                MissionViewModel missionListing = new MissionViewModel();


                missionListing.MissionId = obj.MissionId;
                missionListing.UserId = UserId;
                missionListing.CityName = getCityName(obj.CityId);
                missionListing.CountryName = _CIPlatformDbContext.Countries.FirstOrDefault(c => c.CountryId == obj.CountryId).Name;
                missionListing.MissionType = obj.MissionType;
                missionListing.MissionSkills = missionskill1;
                missionListing.SkillId = missionskill.SkillId;
                missionListing.OrganizationName = obj.OrganizationName;
                missionListing.OrganizationDescription = obj.OrganizationDetail;
                missionListing.ShortDescription = obj.ShortDescription;
                missionListing.Description = obj.Description;
                missionListing.CityId = obj.CityId;
                missionListing.CountryId = obj.CountryId;
                missionListing.StartDate = (DateTime)obj.StartDate;
                missionListing.CreatedAt = obj.CreatedAt;
                missionListing.EndDate = (DateTime)obj.EndDate;
                missionListing.SeatLeft = obj.SeatLeft;
                missionListing.MediaPath = getMediaPathFromId(missionListing.MissionId);
                missionListing.missionListMediaPath = _CIPlatformDbContext.MissionMedia.Where(m => m.MissionId == missionListing.MissionId).ToList();
                missionListing.Title = obj.Title;
                missionListing.ThemeId = (int)obj.ThemeId;
                var Ratings = getMissionRatings(missionListing.MissionId);
                var rating = 0;
                if (Ratings.Count != 0)
                {
                    rating = (int)Ratings.Average(r => r.Rating);
                }
                missionListing.Rating = rating;
                missionListing.RatingUser = _CIPlatformDbContext.MissionRatings.Where(x => x.MissionId == missionListing.MissionId).Count();
                missionListing.favMission = getFavMission(UserId, missionListing.MissionId);
                missionListing.appMission = getAppMission(UserId, missionListing.MissionId);

                var Documents = _CIPlatformDbContext.MissionDocuments.Where(d => d.MissionId == missionListing.MissionId).ToList();
                missionListing.Documents = Documents.Count() != 0 ? Documents : null;
                {
                    string Skey = "ZXCVBNMasdfghjkl!@#$%^&*12345678";
                    string Siv = "passwordpassword";
                    byte[] key = Encoding.UTF8.GetBytes(Skey);
                    byte[] iv = Encoding.UTF8.GetBytes(Siv);
                    if (missionListing.Documents != null)
                    {
                        foreach (var i in missionListing.Documents)
                        {
                            string filePath11 = "wwwroot/Assets/Documents/" + i.DocumentName;
                            string fileExtension = Path.GetExtension(filePath11);
                            if (fileExtension == ".pdf")
                            {
                                byte[] fileData = File.ReadAllBytes(filePath11);
                                // Decrypt the file data
                                //byte[] decryptedData = DecryptData(encryptedData, key, iv);
                                byte[] decryptedData = DecryptData(fileData, key, iv);

                                // Use the decrypted data as needed
                                // For example, you can save it to another file or process it further
                                var decryptedFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/", i.DocumentName);
                                using (var decryptedStream = new FileStream(decryptedFilePath, FileMode.Create))
                                {
                                    decryptedStream.Write(decryptedData, 0, decryptedData.Length);
                                }
                            }

                        }
                    }

                }

                missionListing.Theme = getMissionThemes(obj.ThemeId);
                //missionListing.Deadline = (missionListing.StartDate).AddDays(-7);
                if (missionListing.MissionType == "Time")
                {
                    missionListing.Deadline = (DateTime)obj.Deadline;
                }
                missionListing.missionApplication = _CIPlatformDbContext.MissionApplications.ToList();
                if (_CIPlatformDbContext.GoalMissions.Where(x => missionListing.MissionId.Equals(x.MissionId)).Count() != 0)
                {
                    missionListing.GoalObjectiveText = getGoalMissionDetails(obj.MissionId).GoalObjectiveText;
                    missionListing.achievedValue = getGoalMissionDetails(obj.MissionId).AchievedValue;
                    var totalGoal = getGoalMissionDetails(obj.MissionId).GoalValue;
                    missionListing.progressRatio = (missionListing.achievedValue / totalGoal) * 100;

                }
                missionListing.Availability = obj.Availability;

                var allUser = _CIPlatformDbContext.Users.Where(x => x.DeletedAt == null).ToList();
                var alreaduInvite = _CIPlatformDbContext.MissionInvites.Where(x => x.FromUserId == UserId && x.MissionId == missionListing.MissionId).Include(x => x.ToUser).ToList();
                foreach (var i in alreaduInvite)
                {
                    allUser = allUser.Where(x => x.UserId != i.ToUserId).ToList();
                }
                missionListing.userList = allUser;
                missionListing.alreadyMisInvited = alreaduInvite;
                missionListing.commentList = comments;
                //missionListing.favMission = _CIPlatformDbContext.FavoriteMissions.Where(i=>i.MissionId == missionListing.MissionId).ToList();
                //missionlisting.deadline = deadlineofmission(obj.missionid);
                //missionListing.TotalSeat = getTotalSeatsOfMission(obj.MissionId);

                missionListingList.Add(missionListing);
            }
            return missionListingList;
        }
        private byte[] DecryptData(byte[] encryptedData, byte[] key, byte[] iv)
        {
            using (AesManaged aes = new AesManaged())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(encryptedData, 0, encryptedData.Length);
                        cryptoStream.FlushFinalBlock();
                    }

                    return memoryStream.ToArray();
                }
            }
        }
        public List<MissionViewModel> getMisAppList(int pg, long missionId)
        {
            var pageSize = 1;
            List<MissionApplication> missionApplications = _CIPlatformDbContext.MissionApplications.Where(m => m.ApprovalStatus == "Approve" && m.MissionId == missionId).Include(x => x.User).ToList();

            List<MissionViewModel> misView = new List<MissionViewModel>();
            foreach (MissionApplication app in missionApplications)
            {
                MissionViewModel mView = new MissionViewModel();
                User user = _CIPlatformDbContext.Users.FirstOrDefault(u => u.UserId == app.UserId);
                mView.MissionId = missionId;
                mView.Avatar = user.Avatar;
                mView.UserName = user.FirstName + " " + user.LastName;

                misView.Add(mView);
            }


            if (pg != 0)
            {
                misView = misView.Skip((pg - 1) * pageSize).Take(pageSize).ToList();
            }

            return misView;
        }
        public int getFavMission(long userId, long missionId)
        {
            int favorites = _CIPlatformDbContext.FavoriteMissions.FirstOrDefault(x => x.MissionId == missionId && x.UserId == userId && (x.UpdatedAt == x.DeletedAt || x.UpdatedAt > x.DeletedAt)) != null ? 1 : 0;

            return favorites;
        }
        public int getAppMission(long userId, long missionId)
        {
            int apply = _CIPlatformDbContext.MissionApplications.FirstOrDefault(x => x.MissionId == missionId && x.UserId == userId && x.ApprovalStatus == "Approve") != null ? 1 : 0;

            return apply;
        }
        //public List<User> userList()
        //{
        //    List<User> users = _CIPlatformDbContext.Users.Where(x=>x.DeletedAt == null).ToList();
        //    return users;
        //}
        public GoalMission getGoalMissionDetails(long missionId)
        {
            GoalMission obj = _CIPlatformDbContext.GoalMissions.FirstOrDefault(x => x.MissionId == missionId);
            return obj;
        }


        public bool addFavourite(int missionId, int userId)
        {
            FavoriteMission favourite = new();
            favourite.MissionId = missionId;
            favourite.UserId = userId;

            var favMission = _CIPlatformDbContext.FavoriteMissions.FirstOrDefault(f => f.MissionId == missionId && f.UserId == userId);

            if (favMission == null)
            {

                _CIPlatformDbContext.FavoriteMissions.Add(favourite);
                _CIPlatformDbContext.SaveChanges();
                return true;
            }
            else
            {
                if (favMission.DeletedAt == null)
                {
                    favMission.DeletedAt = DateTime.Now;
                    // if error occurs then remove below line and check whether it works or not. also check the getdate() as default in createdat
                    //also check for createdat and appliedat in mission application table why getdate() not working...
                    //_CIPlatformDbContext.FavoriteMissions.Update(favourite);
                    _CIPlatformDbContext.SaveChanges();
                    return false;
                }
                else
                {
                    if (favMission.UpdatedAt == null)
                    {
                        favMission.UpdatedAt = DateTime.Now;
                        //_CIPlatformDbContext.FavoriteMissions.Update(favourite);
                        _CIPlatformDbContext.SaveChanges();
                        return true;
                    }
                    else
                    {
                        if (favMission.DeletedAt < favMission.UpdatedAt)
                        {
                            favMission.DeletedAt = DateTime.Now;
                            //_CIPlatformDbContext.FavoriteMissions.Update(favourite);
                            _CIPlatformDbContext.SaveChanges();
                            return false;
                        }
                        favMission.UpdatedAt = DateTime.Now;
                        //_CIPlatformDbContext.FavoriteMissions.Update(favourite);
                        _CIPlatformDbContext.SaveChanges();
                        return true;
                    }
                }
            }
        }

        public int applyMission(int missionId, int userId)
        {
            MissionApplication application = new();
            application.MissionId = missionId;
            application.UserId = userId;

            var applicable = _CIPlatformDbContext.MissionApplications.FirstOrDefault(a => a.MissionId == missionId && a.UserId == userId);

            if (applicable == null)
            {
                application.CreatedAt = DateTime.Now;
                application.AppliedAt = DateTime.Now;
                _CIPlatformDbContext.MissionApplications.Add(application);
                Mission seat = _CIPlatformDbContext.Missions.FirstOrDefault(a => a.MissionId == missionId);
                seat.SeatLeft = seat.SeatLeft - 1;
                _CIPlatformDbContext.SaveChanges();
                return 1;
            }
            else
            {
                if (applicable.ApprovalStatus == "PENDING")
                {
                    return 2;
                }
                return 0;
            }
        }


        public void addComments(string comment, long UserId, long MissionId)
        {
            Comment commentNew = new Comment();
            commentNew.Comments = comment;
            commentNew.MissionId = MissionId;
            commentNew.UserId = UserId;

            _CIPlatformDbContext.Comments.Add(commentNew);
            _CIPlatformDbContext.SaveChanges();
        }

        public bool sendMail(string[] emailList, long missionId, long userId)
        {
            User fromUser = _CIPlatformDbContext.Users.FirstOrDefault(u => u.UserId == userId);
            foreach (var item in emailList)
            {
                User toUser = _CIPlatformDbContext.Users.FirstOrDefault(u => u.Email == item);
                MissionInvite missionInvite = new MissionInvite();
                missionInvite.FromUserId = userId;
                missionInvite.ToUserId = toUser.UserId;
                missionInvite.MissionId = missionId;

                _CIPlatformDbContext.MissionInvites.Add(missionInvite);

            }
            _CIPlatformDbContext.SaveChanges();

            var mailBody = "<h1>" + fromUser.FirstName + "Recommended Mission</h1><br><h2><a href='" + "https://localhost:7001/Platform/missionDetails?id=" + missionId + "'>Go to Mission</a></h2>";
            //var mailBody = "<h1>" + HttpContext.Session.GetString("UserName") + " Suggest Mission : " + mission.Title + " to You</h1><br><h2><a href='https://localhost:7227/Mission/MisionDetail?id= " + model.MissionId + "'>Go Website</a></h2>";
            //create email message
            foreach (var item in emailList)
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
            return true;
        }


        public bool addRatings(int rate, long missionId, long userId)
        {
            var missionRating = new MissionRating();
            var alradyRate = _CIPlatformDbContext.MissionRatings.Where(x => x.MissionId == missionId && x.UserId == userId).Count();

            if (alradyRate == 0)
            {
                missionRating.MissionId = missionId;
                missionRating.Rating = rate;
                missionRating.UserId = userId;
                _CIPlatformDbContext.MissionRatings.Add(missionRating);
            }
            else
            {
                missionRating = (MissionRating)_CIPlatformDbContext.MissionRatings.FirstOrDefault(x => x.MissionId == missionId && x.UserId == userId);
                missionRating.Rating = rate;
                missionRating.UpdatedAt = DateTime.Now;
                _CIPlatformDbContext.MissionRatings.Update(missionRating);
            }
            _CIPlatformDbContext.SaveChanges();

            return true;
        }

    }
}