using CIPlatform.Entities.Data;
using CIPlatform.Entities.Models;
using CIPlatform.Repository.Interface;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using CIPlatform.Entities.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Web.Helpers;
using MySql.Data.MySqlClient;
using System.Data;

namespace CIPlatform.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly CIPlatformDbContext _CIPlatformDbContext;
        public readonly RoTaskDbContext _RoTaskDbContext;
        public readonly IPlatformRepository _PlatformRepository;
        public readonly IStoryRepository _StoryRepository;


        public UserRepository(CIPlatformDbContext cIPlatformDbContext, IPlatformRepository platformRepository, IStoryRepository storyRepository, RoTaskDbContext roTaskDbContext)
        {
            _CIPlatformDbContext = cIPlatformDbContext;
            _PlatformRepository = platformRepository;
            _StoryRepository = storyRepository;
            _RoTaskDbContext = roTaskDbContext;
        }

        public List<Banner> BannerList()
        {
            var banners = _CIPlatformDbContext.Banners.Where(b => b.DeletedAt == null).OrderBy(b => b.SortOrder).ToList();
            return banners;
        }

        public User? login(User obj)
        {
            var user = _CIPlatformDbContext.Users.FirstOrDefault(u => u.Email == obj.Email && u.DeletedAt == null);
            if (user == null)
            {
                return null;
            }
            if (obj.DeletedAt == null)
            {
                if (Crypto.VerifyHashedPassword(user.Password, obj.Password))
                {
                    return user;

                }
            }
            return null;
        }

        public void register(RegistrationViewModel register)
        {
            string connectionString = "server=localhost;port=3306;user=root;password=Tatva@123;database=rodatabasetask;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand("spInsertUserData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add input parameters
                    command.Parameters.AddWithValue("FirstName", register.FirstName);
                    command.Parameters.AddWithValue("LastName", register.LastName);
                    command.Parameters.AddWithValue("Email", register.Email);
                    command.Parameters.AddWithValue("PhoneNumber", register.PhoneNumber);
                    command.Parameters.AddWithValue("Password", register.Password);

                    // Execute the stored procedure
                    command.ExecuteNonQuery();
                }
            }
            //_RoTaskDbContext.Database.ExecuteSqlRaw("CALL spInsertUserData" +
            //    "(@FirstName, @LastName, @Email, @PhoneNumber, @Password)",
            //        new MySqlParameter("@FirstName", register.FirstName),
            //        new MySqlParameter("@LastName", register.LastName),
            //        new MySqlParameter("@Email", register.Email),
            //        new MySqlParameter("@PhoneNumber", register.PhoneNumber),
            //        new MySqlParameter("@Password", register.Password));

            //_RoTaskDbContext.UserTables.FromSqlRaw("CALL spInsertUserData({0}, {1}, {2}, {3}, {4})", register.FirstName, register.LastName, register.Email, register.PhoneNumber, register.Password);

            //UserTable userTable = new UserTable();
            //userTable.Email = register.Email;
            //userTable.FirstName = register.FirstName;
            //userTable.LastName = register.LastName;
            //userTable.PhoneNumber = register.PhoneNumber;
            //userTable.Password = register.Password;
            //_RoTaskDbContext.UserTables.Add(userTable);
            //_RoTaskDbContext.SaveChanges();


            //User user = new User();
            //user.CityId = 2;
            //user.CountryId = 1;
            //user.Email = register.Email;
            //user.FirstName = register.FirstName;
            //user.LastName = register.LastName;
            //user.PhoneNumber = register.PhoneNumber;
            //user.Password = Crypto.HashPassword(register.Password);
            //_CIPlatformDbContext.Users.Add(user);
            //_CIPlatformDbContext.SaveChanges();

        }
        public List<User> GetUserList()
        {
            List<User> users = new List<User>();
            users = _CIPlatformDbContext.Users.ToList();
            return users;
        }
        public List<UserTable> GetUserTableList()
        {
            //List<UserTable> users = new List<UserTable>();
            //users = _RoTaskDbContext.UserTables.ToList();
            //var users = _RoTaskDbContext.UserTables.FromSqlRaw("CALL spgetUserData").AsEnumerable().Where(x => x.UserId > 9 && x.UserId < 15).ToList();
            var users = _RoTaskDbContext.UserTables.FromSqlRaw("CALL spgetUserData").ToList();

            return users;
        }
        public List<PasswordReset> GetPasswordResetList()
        {
            List<PasswordReset> passwordResets = new List<PasswordReset>();
            passwordResets = _CIPlatformDbContext.PasswordResets.ToList();
            return passwordResets;
        }

        public User? forgot(User obj)
        {
            var user = _CIPlatformDbContext.Users.FirstOrDefault(u => u.Email.Equals(obj.Email.ToLower()) && u.DeletedAt == null);
            if (user == null)
            {
                return user;
            }
            #region Genrate Token
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[16];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var finalString = new String(stringChars);
            #endregion Genrate Token


            #region Update Password Reset Table
            PasswordReset entry = new PasswordReset();
            var count = _CIPlatformDbContext.PasswordResets.Where(u => u.Email == obj.Email).Count();
            if (count != 0)
            {
                entry = _CIPlatformDbContext.PasswordResets.FirstOrDefault(u => u.Email == obj.Email);
                entry.Token = finalString;
                entry.CreatedAt = DateTime.Now;
                _CIPlatformDbContext.PasswordResets.Update(entry);
                _CIPlatformDbContext.SaveChanges();
            }
            else
            {
                entry.Email = obj.Email;
                entry.Token = finalString;
                _CIPlatformDbContext.PasswordResets.Add(entry);
                _CIPlatformDbContext.SaveChanges();
            }
            #endregion Update Password Reset Table

            #region Send Mail
            var mailBody = "<h1>Click link to reset password</h1><br><h2><a href='" + "https://localhost:7001/User/Reset?token=" + finalString + "'>Reset Password</a></h2>";
            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("parthv480@gmail.com"));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = "Reset Your Password";
            email.Body = new TextPart(TextFormat.Html) { Text = mailBody };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("parthv480@gmail.com", "xnbmnadzgazdgeim");
            smtp.Send(email);
            smtp.Disconnect(true);
            #endregion Send Mail

            return user;
        }

        public PasswordReset? reset(RegistrationViewModel obj, string token)
        {
            var validToken = _CIPlatformDbContext.PasswordResets.FirstOrDefault(x => x.Token == token);
            if (validToken != null)
            {
                DateTime exLink = validToken.CreatedAt;
                if (DateTime.Now.Subtract(exLink).TotalMinutes > 2)
                {
                    validToken.Expire = true;
                    return validToken;
                    //return ahiya je return karu te baar toaster add karse k token expired
                }
                //if (validToken)
                //{

                //}
                var user = _CIPlatformDbContext.Users.FirstOrDefault(x => x.Email == validToken.Email);
                user.Password = Crypto.HashPassword(obj.Password);

                _CIPlatformDbContext.Users.Update(user);
                _CIPlatformDbContext.SaveChanges();

            }
            return validToken;
        }
        public User getUser(string Email)
        {
            User users = _CIPlatformDbContext.Users.FirstOrDefault(y => y.Email == Email);

            return users;
        }

        public List<EditProfileViewModel> getUserList(long userId)
        {
            //var user = _CIPlatformDbContext.Stories.Include(s => s.StoryMedia).ToList();
            var user = _CIPlatformDbContext.Users.ToList();

            List<EditProfileViewModel> editProfileViews = new List<EditProfileViewModel>();
            foreach (var obj in user)
            {
                var userSkill = _CIPlatformDbContext.UserSkills.Include(s => s.Skill).Where(a => a.UserId == userId && (a.DeletedAt == null || a.UpdatedAt > a.DeletedAt)).ToList();
                var skills = _CIPlatformDbContext.Skills.Include(u => u.UserSkills).ToList();
                foreach (var skill in userSkill)
                {
                    skills = skills.Where(s => s.SkillId != skill.SkillId).ToList();
                }
                //var missions = _CIPlatformDbContext.Missions.FirstOrDefault(u => u.MissionId == obj.MissionId);
                //var users = _CIPlatformDbContext.Users.FirstOrDefault(u => u.UserId == obj.UserId);
                //List<MissionSkill> missionskill1 = _CIPlatformDbContext.MissionSkills.Include(c => c.Skill).Where(c => obj.MissionId == c.MissionId).ToList();
                //List<Comment> comments = _CIPlatformDbContext.Comments.Where(a => a.MissionId == obj.MissionId && a.ApprovalStatus == "Approve").Include(c => c.User).Include(c => c.Mission).ToList();

                EditProfileViewModel editProfile = new EditProfileViewModel();

                editProfile.UserId = obj.UserId;
                editProfile.Avatar = obj.Avatar;
                //editProfile.OldPassword = obj.Password;
                editProfile.FirstName = obj.FirstName;
                editProfile.LastName = obj.LastName;
                editProfile.EmployeeId = obj.EmployeeId;
                //editProfile.ManagerName= manager;
                editProfile.Title = obj.Title;
                editProfile.Department = obj.Department;
                editProfile.ProfileText = obj.ProfileText;
                editProfile.WhyIVolunteer = obj.WhyIVolunteer;
                editProfile.CityId = obj.CityId;
                editProfile.CountryId = obj.CountryId;
                editProfile.Country = _PlatformRepository.getCountryList();
                editProfile.CityList = _CIPlatformDbContext.Cities.ToList();
                editProfile.Availability = obj.Availability;
                editProfile.AvailabilityIndex = AvailabilityIndex(userId);
                editProfile.LinkedInUrl = obj.LinkedInUrl;
                editProfile.userSkills = userSkill;
                editProfile.Skills = skills;
                editProfile.Email = obj.Email;
                editProfile.timeTitles = _StoryRepository.getMissionList(userId).Where(m => m.MissionType == "Time").ToList();
                editProfile.goalTitles = _StoryRepository.getMissionList(userId).Where(m => m.MissionType == "Goal").ToList();
                editProfile.timeSheets = timesheets(userId);
                editProfile.goalSheets = goalsheets(userId);
                editProfileViews.Add(editProfile);
            }
            return editProfileViews;
        }

        public List<Timesheet> timesheets(long userID)
        {
            var tS = _CIPlatformDbContext.Timesheets.Where(t => t.UserId == userID && t.DeletedAt == null).Include(m => m.Mission).Where(m => m.Mission.MissionType == "Time").ToList();
            return tS;
        }
        public List<Timesheet> goalsheets(long userID)
        {
            var tS = _CIPlatformDbContext.Timesheets.Where(t => t.UserId == userID && t.DeletedAt == null).Include(m => m.Mission).Where(m => m.Mission.MissionType == "Goal").ToList();
            return tS;
        }

        public long? AvailabilityIndex(long userId)
        {
            var av = _CIPlatformDbContext.Users.FirstOrDefault(u => u.UserId == userId).Availability;
            if (av != null)
            {

                if (av == "daily")
                {
                    av = "1";
                }
                else if (av == "weekly")
                {
                    av = "2";
                }
                else if (av == "week-end")
                {
                    av = "3";
                }
                else if (av == "monthly")
                {
                    av = "4";
                }
                long avv = long.Parse(av);
                return avv;
            }
            return 1;
        }

        public bool updateUserData(EditProfileViewModel editProfileView, long userId)
        {
            //UserTable userTable = new UserTable();
            ////userTable.UserId = (int)userId;
            //userTable.FirstName = editProfileView.FirstName;
            //userTable.LastName = editProfileView.LastName;
            //userTable.Email = editProfileView.FirstName;
            //userTable.PhoneNumber = editProfileView.CountryId;
            //userTable.Password = editProfileView.FirstName;
            //_RoTaskDbContext.UserTables.Add(userTable);
            //_RoTaskDbContext.SaveChanges();

            User user = _CIPlatformDbContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (editProfileView.Profile != null)
            {
                user.Avatar = editProfileView.Profile.FileName;
            }
            user.FirstName = editProfileView.FirstName;
            user.LastName = editProfileView.LastName;
            user.EmployeeId = editProfileView.EmployeeId;
            user.Title = editProfileView.Title;
            user.Department = editProfileView.Department;
            user.ProfileText = editProfileView.ProfileText;
            user.WhyIVolunteer = editProfileView.WhyIVolunteer;
            user.CityId = editProfileView.CityId;
            user.CountryId = editProfileView.CountryId;
            List<UserSkill> usk = _CIPlatformDbContext.UserSkills.Where(a => a.UserId == userId && (a.DeletedAt == null || a.UpdatedAt > a.DeletedAt)).ToList();
            if (editProfileView.skillsToAdd != null)
            {
                //_CIPlatformDbContext.UserSkills.RemoveRange(usk);
                List<int> remainingId = editProfileView.skillsToAdd.Except(usk.Select(u => u.SkillId)).ToList();
                foreach (var i in remainingId)
                {
                    var update = _CIPlatformDbContext.UserSkills.FirstOrDefault(a => a.SkillId == i && a.UserId == userId);
                    if (update != null)
                    {
                        update.UpdatedAt = DateTime.Now;
                        _CIPlatformDbContext.SaveChanges();
                    }
                    else
                    {
                        UserSkill add = new UserSkill();
                        add.UserId = userId;
                        add.SkillId = i;
                        _CIPlatformDbContext.UserSkills.Add(add);
                        _CIPlatformDbContext.SaveChanges();
                    }

                }
                List<int> rmSkill = usk.Select(u => u.SkillId).ToList().Except(editProfileView.skillsToAdd).ToList();
                foreach (var i in rmSkill)
                {
                    var rm = _CIPlatformDbContext.UserSkills.FirstOrDefault(x => x.UserId == userId && x.SkillId == i);
                    rm.DeletedAt = DateTime.Now;
                    _CIPlatformDbContext.SaveChanges();
                }
            }
            else
            {
                foreach (var i in usk)
                {
                    var rm = _CIPlatformDbContext.UserSkills.FirstOrDefault(x => x.UserId == userId && x.SkillId == i.SkillId);
                    rm.DeletedAt = DateTime.Now;
                    _CIPlatformDbContext.SaveChanges();
                }

            }
            if (editProfileView.AvailabilityIndex == 1)
            {
                user.Availability = "daily";
            }
            else if (editProfileView.AvailabilityIndex == 2)
            {
                user.Availability = "weekly";
            }
            else if (editProfileView.AvailabilityIndex == 3)
            {
                user.Availability = "week-end";
            }
            else if (editProfileView.AvailabilityIndex == 4)
            {
                user.Availability = "monthly";
            }
            //avvability
            user.LinkedInUrl = editProfileView.LinkedInUrl;
            //skill
            _CIPlatformDbContext.Users.Update(user);
            _CIPlatformDbContext.SaveChanges();
            return true;
        }

        public bool changeUserPassword(string Password, long userId)
        {
            var oldPass = _CIPlatformDbContext.Users.FirstOrDefault(s => s.UserId == userId);
            if (oldPass != null)
            {
                oldPass.Password = Crypto.HashPassword(Password);
                _CIPlatformDbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkOldpassword(string OldPassword, long userId)
        {
            string oldPass = _CIPlatformDbContext.Users.FirstOrDefault(s => s.UserId == userId).Password;
            if (Crypto.VerifyHashedPassword(oldPass, OldPassword))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool contactUs(string FirstName, string Email, string subject, string Message, long userId)
        {
            ContactU contactU = new ContactU();
            contactU.UserId = userId;
            contactU.Name = FirstName;
            contactU.Email = Email;
            contactU.Subject = subject;
            contactU.Message = Message;
            contactU.CreatedAt = DateTime.Now;

            _CIPlatformDbContext.ContactUs.Add(contactU);
            _CIPlatformDbContext.SaveChanges();

            #region Send Mail
            var mailBody = "<h1>(Message from user)</h1><br><br><br><h2> Message : " + Message + "</h2>";
            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(Email));
            email.To.Add(MailboxAddress.Parse("parthv480@gmail.com"));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = mailBody };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("parthv480@gmail.com", "xnbmnadzgazdgeim");
            smtp.Send(email);
            smtp.Disconnect(true);
            #endregion Send Mail

            return true;
        }

        public bool addTimeSheets(long userId, long MissionId, int goalAction, DateTime Date, string Message, int timeHour, int timeMinute, long TimesheetId)
        {
            if (TimesheetId == 0)
            {
                var type = _CIPlatformDbContext.Missions.FirstOrDefault(m => m.MissionId == MissionId).MissionType;
                Timesheet timesheet = new Timesheet();
                timesheet.UserId = userId;
                timesheet.MissionId = MissionId;
                if (type == "Goal")
                {
                    timesheet.Action = goalAction;
                    GoalMission goalMission = _CIPlatformDbContext.GoalMissions.FirstOrDefault(g => g.MissionId == MissionId);
                    goalMission.AchievedValue = goalMission.AchievedValue + goalAction;
                    _CIPlatformDbContext.SaveChanges();
                }
                if (type == "Time")
                {
                    timesheet.Time = new TimeSpan(timeHour, timeMinute, 0);
                }
                timesheet.DateVolunteered = Date;
                timesheet.Notes = Message;
                timesheet.Status = "PENDING";

                _CIPlatformDbContext.Timesheets.Add(timesheet);
                _CIPlatformDbContext.SaveChanges();
                return true;
            }
            else
            {
                if (MissionId != 0)
                {
                    var sheet = _CIPlatformDbContext.Timesheets.Where(m => m.TimesheetId == TimesheetId).FirstOrDefault();
                    var type = _CIPlatformDbContext.Missions.FirstOrDefault(m => m.MissionId == MissionId).MissionType;
                    sheet.UserId = userId;
                    sheet.MissionId = MissionId;
                    if (type == "Goal")
                    {
                        GoalMission goalMission = _CIPlatformDbContext.GoalMissions.FirstOrDefault(g => g.MissionId == MissionId);
                        Timesheet timesheet = _CIPlatformDbContext.Timesheets.FirstOrDefault(s => s.TimesheetId == TimesheetId);
                        goalMission.AchievedValue = (goalMission.AchievedValue + goalAction) - (int)timesheet.Action;
                        sheet.Action = goalAction;
                        _CIPlatformDbContext.SaveChanges();
                    }
                    if (type == "Time")
                    {
                        sheet.Time = new TimeSpan(timeHour, timeMinute, 0);
                    }
                    sheet.DateVolunteered = Date;
                    sheet.Notes = Message;
                    sheet.Status = "PENDING";
                    sheet.UpdatedAt = DateTime.Now;
                    _CIPlatformDbContext.Timesheets.Update(sheet);
                    _CIPlatformDbContext.SaveChanges();
                    return true;
                }
                else
                {
                    var sheet = _CIPlatformDbContext.Timesheets.Where(m => m.TimesheetId == TimesheetId).FirstOrDefault();
                    sheet.DeletedAt = DateTime.Now;
                    _CIPlatformDbContext.SaveChanges();
                    return true;
                }
            }
        }
        public Timesheet getTimeSheets(long TimesheetId)
        {
            var data = _CIPlatformDbContext.Timesheets.FirstOrDefault(t => t.TimesheetId == TimesheetId);

            return data;
        }

        public bool checkSameDate(DateTime oldDate, long userId, long TimesheetId, long MissionId)
        {
            var date = _CIPlatformDbContext.Timesheets.Where(x => x.UserId == userId && x.MissionId == MissionId && x.DateVolunteered == oldDate && x.DeletedAt == null).ToList();
            var mission = _CIPlatformDbContext.Missions.Where(x => x.MissionId == MissionId).FirstOrDefault();
            if (date.Count != 0)
            {
                if (date.Where(x => x.TimesheetId == TimesheetId).Count() != 0 && (mission.StartDate < oldDate && mission.EndDate > oldDate && oldDate <= DateTime.Now))
                {
                    return true;
                }
                return false;
            }
            if ((mission.StartDate > oldDate || oldDate >= DateTime.Now || mission.EndDate < oldDate))
            {
                return false;
            }
            return true;
        }

        public List<CmsPage> cmsPages()
        {
            var cmsPages = _CIPlatformDbContext.CmsPages.ToList();
            return cmsPages;
        }

        public bool UpdateSetting(List<string> settingList, long userId)
        {
            var settings = _CIPlatformDbContext.NotificationSettings.Where(n => n.UserId == userId).ToList();
            if (settings != null)
            {
                foreach (var i in settings)
                {
                    _CIPlatformDbContext.NotificationSettings.Remove(i);
                }
            }
            foreach (var i in settingList)
            {
                NotificationSetting notificationSetting = new NotificationSetting();
                notificationSetting.UserId = userId;
                notificationSetting.SettingName = i;
                _CIPlatformDbContext.NotificationSettings.Add(notificationSetting);
            }
            _CIPlatformDbContext.SaveChanges();
            //notificationSetting.SettingName = string.Join(",", settingList);
            return true;
        }
        public void ReadNotification(long NotificationId)
        {
            var isRead = _CIPlatformDbContext.Notifications.Find(NotificationId);
            if (isRead != null)
            {
                isRead.Status = true;
                _CIPlatformDbContext.Notifications.Update(isRead);
                _CIPlatformDbContext.SaveChanges();
            }
        }

        public void addNotificationList(long userId)
        {
            var mission_invite = _CIPlatformDbContext.MissionInvites.Where(u => u.ToUserId == userId).ToList();
            var story_invite = _CIPlatformDbContext.StoryInvites.Where(u => u.ToUserId == userId).Include(s => s.Story).ToList();
            var story_status = _CIPlatformDbContext.Stories.Where(s => (s.Status == "PUBLISHED" || s.Status == "DECLINED") && s.UserId == userId).ToList();
            var mission_status = _CIPlatformDbContext.MissionApplications.Where(m => (m.ApprovalStatus == "Approve" || m.ApprovalStatus == "Decline") && m.UserId == userId).ToList();
            var user_skills = _CIPlatformDbContext.UserSkills.Where(k => k.UserId == userId).Select(m => m.SkillId).ToList();

            var To_user_skills = _CIPlatformDbContext.UserSkills.Where(k => k.UserId == userId).ToList();
            var user_availablity = _CIPlatformDbContext.Users.FirstOrDefault(u => u.UserId == userId).Availability;
            var new_mission = _CIPlatformDbContext.MissionSkills.Where(m => user_skills.Contains(m.SkillId) && m.Mission.Availability == user_availablity).Select(m => m.Mission).GroupBy(x => x.MissionId).Select(y => y.First()).ToList();

            var notification_mission = _CIPlatformDbContext.Notifications.Where(n => n.Type == "recommend" && n.UserId == userId).ToList();
            var notification_story = _CIPlatformDbContext.Notifications.Where(n => n.Type == "recommendStory" && n.UserId == userId).ToList();
            var noti_story_status = _CIPlatformDbContext.Notifications.Where(n => n.Type == "story" && n.UserId == userId).ToList();
            var noti_mission_status = _CIPlatformDbContext.Notifications.Where(n => n.Type == "missionapplication" && n.UserId == userId).ToList();
            var noti_newMission = _CIPlatformDbContext.Notifications.Where(n => n.Type == "mission" && n.UserId == userId).ToList();


            if (mission_invite.Count > notification_mission.Count)
            {
                foreach (var item in mission_invite.Skip(notification_mission.Count))
                {
                    Notification notification = new Notification();
                    notification.MissionId = item.MissionId;
                    notification.UserId = item.ToUserId;
                    notification.FromId = item.FromUserId;
                    notification.Type = "recommend";

                    _CIPlatformDbContext.Notifications.Add(notification);
                    _CIPlatformDbContext.SaveChanges();
                }
            }

            if (story_invite.Count > notification_story.Count)
            {
                foreach (var item in story_invite.Skip(notification_story.Count))
                {
                    Notification notification = new Notification();
                    notification.StoryId = item.StoryId;
                    notification.UserId = item.ToUserId;
                    notification.FromId = item.FromUserId;
                    notification.MissionId = item.Story.MissionId;
                    notification.Type = "recommendStory";

                    _CIPlatformDbContext.Notifications.Add(notification);
                    _CIPlatformDbContext.SaveChanges();
                }
            }

            if (story_status.Count > noti_story_status.Count)
            {
                foreach (var item in story_status.Skip(noti_story_status.Count))
                {
                    Notification notification = new Notification();
                    notification.StoryId = item.StoryId;
                    notification.UserId = item.UserId;
                    notification.MissionId = item.MissionId;
                    notification.Type = "story";

                    _CIPlatformDbContext.Notifications.Add(notification);
                    _CIPlatformDbContext.SaveChanges();
                }
            }

            if (mission_status.Count > noti_mission_status.Count)
            {
                foreach (var item in mission_status.Skip(noti_mission_status.Count))
                {
                    Notification notification = new Notification();
                    notification.MissionId = item.MissionId;
                    notification.UserId = item.UserId;
                    notification.Type = "missionapplication";

                    _CIPlatformDbContext.Notifications.Add(notification);
                    _CIPlatformDbContext.SaveChanges();
                }
            }

            if (new_mission.Count > noti_newMission.Count)
            {
                foreach (var item in new_mission.Skip(noti_newMission.Count))
                {
                    Notification notification = new Notification();
                    notification.MissionId = item.MissionId;
                    notification.UserId = userId;
                    notification.Type = "mission";

                    _CIPlatformDbContext.Notifications.Add(notification);
                    _CIPlatformDbContext.SaveChanges();
                }
            }
        }


        public NotificationViewModel Notifications(long userId)
        {
            var check = _CIPlatformDbContext.NotificationSettings.Where(u => u.UserId == userId).ToList();
            if (check.Count == 0)
            {
                List<string> preferences = new List<string> { "recommend", "recommendStory", "story", "missionapplication", "mission" };
                foreach (var i in preferences)
                {
                    NotificationSetting notificationSetting = new NotificationSetting();
                    notificationSetting.UserId = userId;
                    notificationSetting.SettingName = i;
                    _CIPlatformDbContext.NotificationSettings.Add(notificationSetting);
                    _CIPlatformDbContext.SaveChanges();
                }

            }
            NotificationViewModel notification = new NotificationViewModel();

            notification.users = _CIPlatformDbContext.Users.ToList();
            notification.missions = _CIPlatformDbContext.Missions.Include(m => m.MissionInvites).ToList();
            notification.stories = _CIPlatformDbContext.Stories.Include(s => s.StoryInvites).ToList();
            notification.Settings = _CIPlatformDbContext.NotificationSettings.Where(s => s.UserId == userId).Select(x => x.SettingName).ToList();
            notification.Notifications = _CIPlatformDbContext.Notifications.Where(n => n.UserId == userId && n.Status == false).ToList();
            DateTime yesterday = DateTime.Now.Date.AddDays(-1);
            notification.NewRecommendations = _CIPlatformDbContext.Notifications.Where(n => n.Type == "recommend" && n.UserId == userId && n.Mission.MissionInvites.Any(u => u.ToUserId == userId && u.FromUserId == n.FromId && u.MissionId == n.MissionId && u.CreatedAt > yesterday) && n.User.NotificationSettings.Any(s => s.UserId == userId && s.SettingName == n.Type)).ToList();
            notification.OldRecommendations = _CIPlatformDbContext.Notifications.Where(n => n.Type == "recommend" && n.UserId == userId && n.Mission.MissionInvites.Any(u => u.ToUserId == userId && u.FromUserId == n.FromId && u.MissionId == n.MissionId && u.CreatedAt < yesterday) && n.User.NotificationSettings.Any(s => s.UserId == userId && s.SettingName == n.Type)).ToList();
            notification.NewStories = _CIPlatformDbContext.Notifications.Where(n => n.Type == "story" && n.UserId == userId && n.Story.PublishedAt > yesterday && n.User.NotificationSettings.Any(s => s.UserId == userId && s.SettingName == n.Type)).ToList();
            notification.OldStories = _CIPlatformDbContext.Notifications.Where(n => n.Type == "story" && n.UserId == userId && n.Story.PublishedAt < yesterday && n.User.NotificationSettings.Any(s => s.UserId == userId && s.SettingName == n.Type)).ToList();
            notification.NewMissionApplications = _CIPlatformDbContext.Notifications.Where(n => n.Type == "missionapplication" && n.UserId == userId && n.Mission.MissionApplications.Any(u => u.UserId == userId && u.MissionId == n.MissionId && u.CreatedAt > yesterday) && n.User.NotificationSettings.Any(s => s.UserId == userId && s.SettingName == n.Type)).ToList();
            notification.OldMissionApplications = _CIPlatformDbContext.Notifications.Where(n => n.Type == "missionapplication" && n.UserId == userId && n.Mission.MissionApplications.Any(u => u.UserId == userId && u.MissionId == n.MissionId && u.CreatedAt < yesterday) && n.User.NotificationSettings.Any(s => s.UserId == userId && s.SettingName == n.Type)).ToList();
            notification.NewStoryRecommendations = _CIPlatformDbContext.Notifications.Where(n => n.Type == "recommendStory" && n.UserId == userId && n.Story.StoryInvites.Any(u => u.ToUserId == userId && u.FromUserId == n.FromId && u.StoryId == n.StoryId && u.CreatedAt > yesterday) && n.User.NotificationSettings.Any(s => s.UserId == userId && s.SettingName == n.Type)).ToList();
            notification.OldStoryRecommendations = _CIPlatformDbContext.Notifications.Where(n => n.Type == "recommendStory" && n.UserId == userId && n.Story.StoryInvites.Any(u => u.ToUserId == userId && u.FromUserId == n.FromId && u.StoryId == n.StoryId && u.CreatedAt < yesterday) && n.User.NotificationSettings.Any(s => s.UserId == userId && s.SettingName == n.Type)).ToList();
            notification.NewMissions = _CIPlatformDbContext.Notifications.Where(n => n.Type == "mission" && n.UserId == userId && n.Mission.CreatedAt > yesterday && n.User.NotificationSettings.Any(s => s.UserId == userId && s.SettingName == n.Type)).ToList();
            notification.OldMissions = _CIPlatformDbContext.Notifications.Where(n => n.Type == "mission" && n.UserId == userId && n.Mission.CreatedAt < yesterday && n.User.NotificationSettings.Any(s => s.UserId == userId && s.SettingName == n.Type)).ToList();
            int totalCount =
                notification.NewRecommendations.Count(x => x.Status == false) +
                notification.OldRecommendations.Count(x => x.Status == false) +
                notification.NewStories.Count(x => x.Status == false) +
                notification.OldStories.Count(x => x.Status == false) +
                notification.NewMissionApplications.Count(x => x.Status == false) +
                notification.OldMissionApplications.Count(x => x.Status == false) +
                notification.NewStoryRecommendations.Count(x => x.Status == false) +
                notification.OldStoryRecommendations.Count(x => x.Status == false) +
                notification.NewMissions.Count(x => x.Status == false) +
                notification.OldMissions.Count(x => x.Status == false);
            notification.NotificationCount = totalCount;
            foreach (var item in notification.NewMissions)
            {
                var a = item.Mission;
                //var b = a.FirstOrDefault(x => x.UserId == userId && x.MissionId == item.MissionId);

            }
            return notification;
        }
    }
}
