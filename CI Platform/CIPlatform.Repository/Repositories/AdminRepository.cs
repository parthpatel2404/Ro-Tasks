using Aspose.Pdf;
using CIPlatform.Entities.Data;
using CIPlatform.Entities.Models;
using CIPlatform.Entities.ViewModel;
using CIPlatform.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Web.Helpers;
using System.Drawing;

namespace CIPlatform.Repository.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        public readonly CIPlatformDbContext _CIPlatformDbContext;

        public AdminRepository(CIPlatformDbContext cIPlatformDbContext)
        {
            _CIPlatformDbContext = cIPlatformDbContext;
        }

        public AdminViewModel getAdminDetails()
        {
            AdminViewModel adminViewModel = new AdminViewModel();
            adminViewModel.users = _CIPlatformDbContext.Users.Where(u => u.DeletedAt == null).Include(u => u.Country).Include(u => u.City).ToList();
            adminViewModel.cmsPages = _CIPlatformDbContext.CmsPages.Where(u => u.DeletedAt == null).ToList();
            adminViewModel.missions = _CIPlatformDbContext.Missions.Where(u => u.DeletedAt == null).Include(m => m.MissionMedia).Include(m => m.MissionDocuments).Include(m => m.GoalMissions).Include(m => m.MissionSkills).ToList();
            adminViewModel.missionThemes = _CIPlatformDbContext.MissionThemes.Where(u => u.DeletedAt == null).ToList();
            adminViewModel.skills = _CIPlatformDbContext.Skills.Where(u => u.DeletedAt == null).ToList();
            adminViewModel.missionApplications = _CIPlatformDbContext.MissionApplications.Include(m => m.Mission).Include(u => u.User).Where(u => u.DeletedAt == null && u.ApprovalStatus == "PENDING").ToList();
            adminViewModel.stories = _CIPlatformDbContext.Stories.Include(u => u.User).Include(m => m.Mission).Where(u => u.DeletedAt == null && u.Status == "PENDING").ToList();
            adminViewModel.country = _CIPlatformDbContext.Countries.ToList();
            adminViewModel.city = _CIPlatformDbContext.Cities.ToList();
            adminViewModel.themeTitle = _CIPlatformDbContext.MissionThemes.ToList();
            adminViewModel.totalSkill = _CIPlatformDbContext.Skills.Where(s => s.DeletedAt == null).ToList();
            adminViewModel.banners = _CIPlatformDbContext.Banners.Where(b => b.DeletedAt == null).ToList();
            adminViewModel.missionSkills = _CIPlatformDbContext.MissionSkills.Where(m => m.DeletedAt == null || m.UpdatedAt > m.DeletedAt).ToList();
            //List<MissionSkill> missionSkills = new List<MissionSkill>();
            //List<Skill> skills = new List<Skill>();
            //foreach (var m in adminViewModel.missions)
            //{
            //    var missionSkill = _CIPlatformDbContext.MissionSkills.Include(s => s.Skill).Where(a => a.MissionId == m.MissionId && (a.DeletedAt == null || a.UpdatedAt > a.DeletedAt)).ToList();
            //    missionSkills.AddRange(missionSkill);
            //    var missionSkillIds = missionSkill.Select(x => x.SkillId).ToList();
            //    var skill = _CIPlatformDbContext.Skills.Include(u => u.MissionSkills).Where(s => !missionSkillIds.Contains(s.SkillId)).ToList();
            //    skills.AddRange(skill);
            //}

            //adminViewModel.missionSkills = missionSkills;
            return adminViewModel;
        }


        public AdminViewModel getSearchAdmin(string? search, int pg)
        {
            var pageSize = 3;
            AdminViewModel adminView = getAdminDetails();

            if (search != "")
            {
                search = search.ToLower();
                adminView.users = adminView.users.OrderByDescending(u => u.UserId).Where(a => a.FirstName.ToLower().Contains(search) || a.LastName.ToLower().Contains(search) || a.Email.ToLower().Contains(search) || a.Department.ToLower().Contains(search) || a.Status.ToLower().Contains(search)).ToList();
                adminView.cmsPages = adminView.cmsPages.Where(c => c.Title.ToLower().Contains(search) || c.Status.ToLower().Contains(search)).ToList();
                adminView.missions = adminView.missions.Where(m => m.Title.ToLower().Contains(search) || m.MissionType.ToLower().Contains(search)).ToList();
                adminView.missionThemes = adminView.missionThemes.Where(t => t.Title.ToLower().Contains(search)).ToList();
                adminView.skills = adminView.skills.Where(t => t.SkillName.ToLower().Contains(search)).ToList();
                adminView.missionApplications = adminView.missionApplications.Where(m => m.Mission.Title.ToLower().Contains(search) || m.User.FirstName.ToLower().Contains(search) || m.User.LastName.ToLower().Contains(search)).ToList();
                adminView.stories = adminView.stories.Where(s => s.Title.ToLower().Contains(search) || s.User.FirstName.ToLower().Contains(search) || s.User.LastName.ToLower().Contains(search) || s.Mission.Title.ToLower().Contains(search)).ToList();
                adminView.banners = adminView.banners.Where(s => s.Text.ToLower().Contains(search) || s.Image.ToLower().Contains(search)).ToList();
            }

            if (pg != 0)
            {
                adminView.users = adminView.users.OrderByDescending(u => u.UserId).Skip((pg - 1) * pageSize).Take(pageSize).ToList();
                adminView.cmsPages = adminView.cmsPages.Skip((pg - 1) * pageSize).Take(pageSize).ToList();
                adminView.missions = adminView.missions.Skip((pg - 1) * pageSize).Take(pageSize).ToList();
                adminView.missionThemes = adminView.missionThemes.Skip((pg - 1) * (pageSize + 7)).Take(pageSize + 7).ToList();
                adminView.skills = adminView.skills.Skip((pg - 1) * (pageSize + 2)).Take(pageSize + 2).ToList();
                adminView.missionApplications = adminView.missionApplications.Skip((pg - 1) * pageSize).Take(pageSize).ToList();
                adminView.stories = adminView.stories.Skip((pg - 1) * pageSize).Take(pageSize).ToList();
                adminView.banners = adminView.banners.Skip((pg - 1) * (pageSize + 2)).Take(pageSize + 2).ToList();
            }

            return adminView;
        }

        public void addViaAdmin(MissionViewModel userData, string type, long Id)
        {
            if (userData.MissionType != null)
            {
                if (userData.CrudId == 0)
                {
                    if (userData.MissionType == "user")
                    {
                        User user = new User();
                        user.FirstName = userData.FirstName;
                        user.LastName = userData.LastName;
                        user.Email = userData.UserName;
                        user.Password = Crypto.HashPassword(userData.Password);
                        user.PhoneNumber = userData.UserId;
                        user.Role = userData.Title;

                        _CIPlatformDbContext.Users.Add(user);
                        _CIPlatformDbContext.SaveChanges();
                    }
                    else if (userData.MissionType == "cms")
                    {
                        CmsPage cmsPage = new CmsPage();
                        cmsPage.Title = userData.Title;
                        cmsPage.Description = userData.Description;
                        cmsPage.Slug = userData.Slug;
                        cmsPage.Status = userData.Status;

                        _CIPlatformDbContext.CmsPages.Add(cmsPage);
                        _CIPlatformDbContext.SaveChanges();
                    }
                    else if (userData.MissionType == "mission")
                    {
                        Mission mission = new Mission();
                        mission.Title = userData.Title;
                        mission.Description = userData.Description;
                        mission.ShortDescription = userData.ShortDescription;
                        mission.CountryId = userData.CountryId;
                        mission.CityId = userData.CityId;
                        mission.MissionType = userData.Mission;
                        mission.OrganizationName = userData.OrganizationName;
                        mission.OrganizationDetail = userData.OrganizationDescription;
                        mission.StartDate = userData.StartDate;
                        mission.EndDate = userData.EndDate;
                        mission.ThemeId = userData.ThemeId;
                        mission.Availability = userData.Availability;

                        if (userData.Mission == "Time")
                        {
                            mission.TotalSeats = userData.TotalSeat;
                            mission.SeatLeft = (long)userData.TotalSeat;
                            mission.Deadline = userData.Deadline;
                            _CIPlatformDbContext.Missions.Add(mission);
                            _CIPlatformDbContext.SaveChanges();

                        }
                        else
                        {
                            _CIPlatformDbContext.Missions.Add(mission);
                            _CIPlatformDbContext.SaveChanges();

                            GoalMission goalMission = new GoalMission();
                            goalMission.MissionId = mission.MissionId;
                            goalMission.GoalObjectiveText = userData.GoalObjectiveText;
                            goalMission.GoalValue = userData.GoalValue;
                            _CIPlatformDbContext.GoalMissions.Add(goalMission);
                            _CIPlatformDbContext.SaveChanges();

                        }

                        MissionSkill missionSkill = new MissionSkill();
                        if (userData.skillList[0] != null)
                        {
                            foreach (var item in userData.skillList)
                            {

                                missionSkill.MissionId = mission.MissionId;
                                missionSkill.SkillId = item;
                                _CIPlatformDbContext.MissionSkills.Add(missionSkill);

                            }
                            _CIPlatformDbContext.SaveChanges();
                        }
                        var url = new List<string>();
                        if (userData.URL[0] != null)
                        {
                            foreach (var j in userData.URL)
                            {
                                MissionMedium sm = new MissionMedium();
                                sm.MissionId = mission.MissionId;
                                sm.MediaType = "mp4";
                                sm.MediaPath = j;
                                _CIPlatformDbContext.MissionMedia.Add(sm);
                                _CIPlatformDbContext.SaveChanges();

                            }
                        }

                        var filePath = new List<string>();
                        if (userData.missionDocuments != null)
                        {
                            foreach (var i in userData.missionDocuments)
                            {
                                MissionDocument docs = new MissionDocument();
                                docs.MissionId = mission.MissionId;
                                docs.DocumentName = i.FileName;
                                docs.DocumentPath = i.FileName;
                                _CIPlatformDbContext.MissionDocuments.Add(docs);
                                _CIPlatformDbContext.SaveChanges();

                                if (i.Length > 0)
                                {
                                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets/Documents", i.FileName);
                                    filePath.Add(path);
                                    using (var stream = new FileStream(path, FileMode.Create))
                                    {
                                        i.CopyTo(stream);
                                    }
                                    // Open document
                                    Document document = new Document("wwwroot/Assets/Documents/" + i.FileName);
                                    // Encrypt PDF  
                                    document.Encrypt("parth", "admin", 0 /*permissions*/, CryptoAlgorithm.RC4x128);
                                    // Save updated PDF
                                    document.Save("wwwroot/Assets/Documents/" + i.FileName);
                                }
                            }
                        }
                        if (userData.file != null)
                        {
                            foreach (var i in userData.file)
                            {
                                MissionMedium img = new MissionMedium();
                                img.MissionId = mission.MissionId;
                                img.MediaName = i.FileName;
                                img.MediaPath = i.FileName;
                                _CIPlatformDbContext.MissionMedia.Add(img);
                                _CIPlatformDbContext.SaveChanges();
                                if (i.Length > 0)
                                {
                                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets", i.FileName);
                                    filePath.Add(path);
                                    using (var stream = new FileStream(path, FileMode.Create))
                                    {
                                        i.CopyTo(stream);
                                    }
                                }

                            }
                        }
                    }
                    else if (userData.MissionType == "theme")
                    {
                        MissionTheme theme = new MissionTheme();
                        theme.Title = userData.Title;
                        theme.Status = userData.Status;

                        _CIPlatformDbContext.MissionThemes.Add(theme);
                        _CIPlatformDbContext.SaveChanges();
                    }
                    else if (userData.MissionType == "skill")
                    {
                        Skill skill = new Skill();
                        skill.SkillName = userData.Title;
                        skill.Status = userData.Status;

                        _CIPlatformDbContext.Skills.Add(skill);
                        _CIPlatformDbContext.SaveChanges();
                    }
                    else if (userData.MissionType == "banner")
                    {
                        Banner banner = new Banner();
                        banner.Image = userData.bannerPhoto.FileName;
                        banner.Text = userData.Description;
                        banner.SortOrder = (int)userData.UserId;

                        _CIPlatformDbContext.Banners.Add(banner);
                        _CIPlatformDbContext.SaveChanges();

                        var filePath = new List<string>();
                        if (userData.bannerPhoto.Length > 0)
                        {
                            //string path = Server.MapPath("~/wwwroot/Assets/Story");
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets", userData.bannerPhoto.FileName);
                            filePath.Add(path);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                userData.bannerPhoto.CopyTo(stream);
                            }
                        }
                    }

                }
                else
                {
                    if (userData.MissionType == "user")
                    {
                        User user = _CIPlatformDbContext.Users.FirstOrDefault(x => x.UserId == userData.CrudId);
                        user.Role = userData.StoryTitle;
                        user.EmployeeId = userData.Slug;
                        user.Department = userData.Theme;
                        user.Status = userData.Status;
                        if (userData.bannerPhoto != null)
                        {
                            user.Avatar = userData.bannerPhoto.FileName;

                        }

                        _CIPlatformDbContext.Users.Update(user);
                        _CIPlatformDbContext.SaveChanges();
                    }
                    else if (userData.MissionType == "cms")
                    {
                        CmsPage cmsPage = _CIPlatformDbContext.CmsPages.FirstOrDefault(x => x.CmsPageId == userData.CrudId);

                        cmsPage.Title = userData.Title;
                        cmsPage.Description = userData.Description;
                        cmsPage.Slug = userData.Slug;
                        cmsPage.Status = userData.Status;
                        cmsPage.UpdatedAt = DateTime.Now;

                        _CIPlatformDbContext.CmsPages.Update(cmsPage);
                        _CIPlatformDbContext.SaveChanges();
                    }
                    else if (userData.MissionType == "mission")
                    {
                        Mission mission = _CIPlatformDbContext.Missions.FirstOrDefault(m => m.MissionId == userData.CrudId);

                        mission.Title = userData.Title;
                        mission.Description = userData.Description;
                        mission.ShortDescription = userData.ShortDescription;
                        mission.CountryId = userData.CountryId;
                        mission.CityId = userData.CityId;
                        mission.MissionType = userData.Mission;
                        mission.OrganizationName = userData.OrganizationName;
                        mission.OrganizationDetail = userData.OrganizationDescription;
                        mission.StartDate = userData.StartDate;
                        mission.EndDate = userData.EndDate;
                        mission.ThemeId = userData.ThemeId;
                        if (userData.Mission == "Time")
                        {
                            mission.TotalSeats = userData.TotalSeat;
                            mission.SeatLeft = (long)userData.TotalSeat;
                            mission.Deadline = userData.Deadline;
                        }
                        else
                        {
                            GoalMission goalMission = _CIPlatformDbContext.GoalMissions.FirstOrDefault(g => g.MissionId == userData.CrudId);
                            goalMission.GoalObjectiveText = userData.GoalObjectiveText;
                            goalMission.GoalValue = userData.GoalValue;
                            _CIPlatformDbContext.GoalMissions.Update(goalMission);
                        }
                        mission.Availability = userData.Availability;
                        mission.UpdatedAt = DateTime.Now;

                        _CIPlatformDbContext.Missions.Update(mission);
                        _CIPlatformDbContext.SaveChanges();

                        List<MissionSkill> usk = _CIPlatformDbContext.MissionSkills.Where(a => a.MissionId == userData.CrudId && (a.DeletedAt == null || a.UpdatedAt > a.DeletedAt)).ToList();
                        if (userData.skillList != null)
                        {
                            List<int> remainingId = userData.skillList.Except(usk.Select(u => u.SkillId)).ToList();
                            foreach (var i in remainingId)
                            {
                                var update = _CIPlatformDbContext.MissionSkills.FirstOrDefault(a => a.SkillId == i && a.MissionId == userData.CrudId);
                                if (update != null)
                                {
                                    update.UpdatedAt = DateTime.Now;
                                    _CIPlatformDbContext.SaveChanges();
                                }
                                else
                                {
                                    MissionSkill add = new MissionSkill();
                                    add.MissionId = userData.CrudId;
                                    add.SkillId = i;
                                    _CIPlatformDbContext.MissionSkills.Add(add);
                                    _CIPlatformDbContext.SaveChanges();
                                }

                            }
                            List<int> rmSkill = usk.Select(u => u.SkillId).ToList().Except(userData.skillList).ToList();
                            foreach (var i in rmSkill)
                            {
                                var rm = _CIPlatformDbContext.MissionSkills.FirstOrDefault(x => x.MissionId == userData.CrudId && x.SkillId == i);
                                rm.DeletedAt = DateTime.Now;
                                _CIPlatformDbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            foreach (var i in usk)
                            {
                                var rm = _CIPlatformDbContext.MissionSkills.FirstOrDefault(x => x.MissionId == userData.CrudId && x.SkillId == i.SkillId);
                                rm.DeletedAt = DateTime.Now;
                                _CIPlatformDbContext.SaveChanges();
                            }

                        }

                        var url = new List<string>();
                        if (userData.URL[0] != null)
                        {
                            foreach (var j in userData.URL)
                            {
                                MissionMedium sm = new MissionMedium();
                                sm.MissionId = mission.MissionId;
                                sm.MediaType = "mp4";
                                sm.MediaPath = j;
                                _CIPlatformDbContext.MissionMedia.Add(sm);
                            }
                        }
                        _CIPlatformDbContext.SaveChanges();

                        var filePath = new List<string>();  

                        if (userData.missionDocuments != null)
                        {
                            List<MissionDocument> missionDocuments = _CIPlatformDbContext.MissionDocuments.Where(m => m.MissionId == userData.CrudId).ToList();
                            foreach (var item in missionDocuments)
                            {
                                _CIPlatformDbContext.MissionDocuments.Remove(item);
                            }
                            foreach (var i in userData.missionDocuments)
                            {
                                MissionDocument docs = new MissionDocument();
                                docs.MissionId = mission.MissionId;
                                docs.DocumentName = i.FileName;
                                docs.DocumentPath = i.FileName;

                                //// Encrypt the file data
                                byte[] encryptedData;
                                string Skey = "ZXCVBNMasdfghjkl!@#$%^&*12345678";
                                string Siv = "passwordpassword";
                                // Convert a C# string to a byte array
                                byte[] key = Encoding.UTF8.GetBytes(Skey);
                                byte[] iv = Encoding.UTF8.GetBytes(Siv);
                                //byte[] key = GetEncryptionKey();
                                //byte[] iv = GetEncryptionIV();
                                using (var memoryStream = new MemoryStream())
                                {
                                    i.CopyTo(memoryStream);
                                    byte[] fileData = memoryStream.ToArray();

                                    encryptedData = EncryptData(fileData, key, iv); // Replace with your encryption method
                                }

                                _CIPlatformDbContext.MissionDocuments.Add(docs);
                                _CIPlatformDbContext.SaveChanges();

                                if (encryptedData.Length > 0)
                                {
                                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets/Documents/Encrypt", i.FileName);
                                    filePath.Add(path);

                                    // Save the encrypted data to the file
                                    File.WriteAllBytes(path, encryptedData);

                                    //using (var stream = new FileStream(path, FileMode.Create))
                                    //{
                                    //    stream.Write(encryptedData, 0, encryptedData.Length);
                                    //    i.CopyTo(stream);
                                    //}
                                }
                                //byte[] encryptedData;
                                //string Skey = "ZXCVBNMasdfghjkl!@#$%^&*12345678";
                                //string Siv = "passwordpassword";
                                //// Convert a C# string to a byte array
                                //byte[] key = Encoding.UTF8.GetBytes(Skey);
                                //byte[] iv = Encoding.UTF8.GetBytes(Siv);
                                ////byte[] key = GetEncryptionKey();
                                ////byte[] iv = GetEncryptionIV();
                                //using (var memoryStream = new MemoryStream())
                                //{
                                //    i.CopyTo(memoryStream);
                                //    byte[] fileData = memoryStream.ToArray();

                                //    encryptedData = EncryptData(fileData, key, iv); // Replace with your encryption method
                                //}

                                //_CIPlatformDbContext.MissionDocuments.Add(docs);
                                //_CIPlatformDbContext.SaveChanges();

                                //if (encryptedData.Length > 0)
                                //{
                                //    var fileName = Guid.NewGuid().ToString(); // Generate a unique file name
                                //    var fileExtension = Path.GetExtension(i.FileName).ToLowerInvariant();
                                //    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets/Documents", i.FileName);
                                //    filePath.Add(path);

                                //    // Save the encrypted data to the file
                                //    using (var stream = new FileStream(path, FileMode.Create))
                                //    {
                                //        stream.Write(encryptedData, 0, encryptedData.Length);
                                //    }

                                //    if (fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".jpeg")
                                //    {
                                //        using (var image = System.Drawing.Image.FromFile(filePath))
                                //        {
                                //            // Perform operations on the image here
                                //            // Example: Resize the image to a specific width and height
                                //            var resizedImage = new Bitmap(image, 800, 600);
                                //            resizedImage.Save(filePath); // Save the resized image
                                //        }
                                //    }
                                //}
                            }
                        }


                        // Encryption method using AES algorithm



                        //// Open document
                        //Document document = new Document("wwwroot/Assets/Documents/" + i.FileName);
                        //// Encrypt PDF  
                        //document.Encrypt("parth", "admin", 0 /*permissions*/, CryptoAlgorithm.RC4x128);
                        //// Save updated PDF
                        //document.Save("wwwroot/Assets/Documents/" + i.FileName);
                        //// Decrypt PDF  
                        //Document document1 = new Document("wwwroot/Assets/Documents/" + i.FileName, "parth");
                        //document1.Decrypt();
                        //// Save updated PDF
                        //document1.Save("wwwroot/Assets/Documents/" + i.FileName);
                        if (userData.file != null)
                        {
                            List<MissionMedium> MissionMedium = _CIPlatformDbContext.MissionMedia.Where(m => m.MissionId == userData.CrudId).ToList();
                            foreach (var item in MissionMedium)
                            {
                                _CIPlatformDbContext.MissionMedia.Remove(item);
                            }
                            foreach (var i in userData.file)
                            {
                                MissionMedium img = new MissionMedium();
                                img.MissionId = mission.MissionId;
                                img.MediaName = i.FileName;
                                img.MediaPath = i.FileName;
                                _CIPlatformDbContext.MissionMedia.Add(img);
                                _CIPlatformDbContext.SaveChanges();
                                if (i.Length > 0)
                                {
                                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets", i.FileName);
                                    filePath.Add(path);
                                    using (var stream = new FileStream(path, FileMode.Create))
                                    {
                                        i.CopyTo(stream);
                                    }
                                }

                            }
                        }

                    }
                    else if (userData.MissionType == "theme")
                    {
                        MissionTheme theme = _CIPlatformDbContext.MissionThemes.FirstOrDefault(t => t.MissionThemeId == userData.CrudId);

                        theme.Title = userData.Title;
                        theme.Status = userData.Status;
                        theme.UpdatedAt = DateTime.Now;

                        _CIPlatformDbContext.MissionThemes.Update(theme);
                        _CIPlatformDbContext.SaveChanges();
                    }
                    else if (userData.MissionType == "skill")
                    {
                        Skill skill = _CIPlatformDbContext.Skills.FirstOrDefault(t => t.SkillId == userData.CrudId);

                        skill.SkillName = userData.Title;
                        skill.Status = userData.Status;
                        skill.UpdatedAt = DateTime.Now;

                        _CIPlatformDbContext.Skills.Update(skill);
                        _CIPlatformDbContext.SaveChanges();
                    }
                    else if (userData.MissionType == "banner")
                    {
                        Banner banner = _CIPlatformDbContext.Banners.FirstOrDefault(b => b.BannerId == userData.CrudId);
                        if (userData.bannerPhoto != null)
                        {
                            banner.Image = userData.bannerPhoto.FileName;

                        }
                        banner.Text = userData.Description;
                        banner.SortOrder = (int)userData.UserId;

                        _CIPlatformDbContext.Banners.Update(banner);
                        _CIPlatformDbContext.SaveChanges();
                        var filePath = new List<string>();
                        if (userData.bannerPhoto != null)
                        {

                            //string path = Server.MapPath("~/wwwroot/Assets/Story");
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets", userData.bannerPhoto.FileName);
                            filePath.Add(path);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                userData.bannerPhoto.CopyTo(stream);
                            }

                        }
                    }
                }
            }
            else
            {
                if (type == "user")
                {
                    var data = _CIPlatformDbContext.Users.Where(u => u.UserId == Id).FirstOrDefault();
                    data.DeletedAt = DateTime.Now;
                    _CIPlatformDbContext.SaveChanges();
                }
                else if (type == "cms")
                {
                    var data = _CIPlatformDbContext.CmsPages.Where(u => u.CmsPageId == Id).FirstOrDefault();
                    data.DeletedAt = DateTime.Now;
                    _CIPlatformDbContext.SaveChanges();
                }
                else if (type == "mission")
                {
                    var data = _CIPlatformDbContext.Missions.Where(u => u.MissionId == Id).FirstOrDefault();
                    data.DeletedAt = DateTime.Now;
                    var data1 = _CIPlatformDbContext.MissionApplications.Where(u => u.MissionId == Id).FirstOrDefault();
                    data1.DeletedAt = DateTime.Now;
                    var data2 = _CIPlatformDbContext.MissionDocuments.Where(u => u.MissionId == Id).FirstOrDefault();
                    data2.DeletedAt = DateTime.Now;
                    var data3 = _CIPlatformDbContext.MissionInvites.Where(u => u.MissionId == Id).FirstOrDefault();
                    data3.DeletedAt = DateTime.Now;
                    var data4 = _CIPlatformDbContext.MissionMedia.Where(u => u.MissionId == Id).FirstOrDefault();
                    data4.DeletedAt = DateTime.Now;
                    var data5 = _CIPlatformDbContext.MissionRatings.Where(u => u.MissionId == Id).FirstOrDefault();
                    data5.DeletedAt = DateTime.Now;
                    var data6 = _CIPlatformDbContext.MissionSkills.Where(u => u.MissionId == Id).FirstOrDefault();
                    data6.DeletedAt = DateTime.Now;
                    var data7 = _CIPlatformDbContext.GoalMissions.Where(u => u.MissionId == Id).FirstOrDefault();
                    data7.DeletedAt = DateTime.Now;
                    var data8 = _CIPlatformDbContext.Comments.Where(u => u.MissionId == Id).FirstOrDefault();
                    data8.DeletedAt = DateTime.Now;
                    var data9 = _CIPlatformDbContext.FavoriteMissions.Where(u => u.MissionId == Id).FirstOrDefault();
                    data9.DeletedAt = DateTime.Now;
                    var data10 = _CIPlatformDbContext.Stories.Where(u => u.MissionId == Id).FirstOrDefault();
                    data10.DeletedAt = DateTime.Now;
                    var data11 = _CIPlatformDbContext.Timesheets.Where(u => u.MissionId == Id).FirstOrDefault();
                    data11.DeletedAt = DateTime.Now;

                    _CIPlatformDbContext.SaveChanges();
                }
                else if (type == "theme")
                {
                    var data = _CIPlatformDbContext.MissionThemes.Where(u => u.MissionThemeId == Id).FirstOrDefault();
                    data.DeletedAt = DateTime.Now;
                    _CIPlatformDbContext.SaveChanges();
                }
                else if (type == "skill")
                {
                    var data = _CIPlatformDbContext.Skills.Where(u => u.SkillId == Id).FirstOrDefault();
                    data.DeletedAt = DateTime.Now;
                    _CIPlatformDbContext.SaveChanges();
                }
                else if (type == "approveApplication")
                {
                    var data = _CIPlatformDbContext.MissionApplications.Where(u => u.MissionApplicationId == Id).FirstOrDefault();
                    data.ApprovalStatus = "Approve";
                    _CIPlatformDbContext.SaveChanges();
                }
                else if (type == "declineApplication")
                {
                    var data = _CIPlatformDbContext.MissionApplications.Where(u => u.MissionApplicationId == Id).FirstOrDefault();
                    data.ApprovalStatus = "Decline";
                    _CIPlatformDbContext.SaveChanges();
                }
                else if (type == "approveStory")
                {
                    var data = _CIPlatformDbContext.Stories.Where(u => u.StoryId == Id).FirstOrDefault();
                    data.Status = "PUBLISHED";
                    data.PublishedAt = DateTime.Now;
                    _CIPlatformDbContext.SaveChanges();
                }
                else if (type == "declineStory")
                {
                    var data = _CIPlatformDbContext.Stories.Where(u => u.StoryId == Id).FirstOrDefault();
                    data.Status = "DECLINED";
                    data.PublishedAt = DateTime.Now;
                    _CIPlatformDbContext.SaveChanges();
                }
                else if (type == "deleteStory")
                {
                    var data = _CIPlatformDbContext.Stories.Where(u => u.StoryId == Id).FirstOrDefault();
                    data.DeletedAt = DateTime.Now;
                    data.PublishedAt = DateTime.Now;
                    _CIPlatformDbContext.SaveChanges();
                }
                else if (type == "banner")
                {
                    var data = _CIPlatformDbContext.Banners.Where(u => u.BannerId == Id).FirstOrDefault();
                    data.DeletedAt = DateTime.Now;
                    _CIPlatformDbContext.SaveChanges();
                }
            }

        }
        private byte[] EncryptData(byte[] data, byte[] key, byte[] iv)
        {
            using (AesManaged aes = new AesManaged())
            {
                //// Set the encryption key and initialization vector (IV)
                //aes.Key = key; // Replace with your method to retrieve or generate the encryption key
                //aes.IV = iv;
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var memoryStream = new MemoryStream())
                {
                    // Write the IV to the output stream
                    memoryStream.Write(iv, 0, iv.Length);

                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);
                        cryptoStream.FlushFinalBlock();
                    }

                    return memoryStream.ToArray();
                }
            }
        }
        private byte[] GetEncryptionKey()
        {
            // Create a new AES instance to generate a random key
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                return aes.Key;
            }
        }

        private byte[] GetEncryptionIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                return aes.IV;
            }
        }

        

        public AdminViewModel getViaAdmin(long Id, string type)
        {
            AdminViewModel adminView = getAdminDetails();
            if (type == "user")
            {
                adminView.users = adminView.users.Where(x => x.UserId == Id).ToList();
            }
            else if (type == "cms")
            {
                adminView.cmsPages = adminView.cmsPages.Where(x => x.CmsPageId == Id).ToList();
            }
            else if (type == "mission")
            {
                adminView.missions = adminView.missions.Where(x => x.MissionId == Id).ToList();
            }
            else if (type == "theme")
            {
                adminView.missionThemes = adminView.missionThemes.Where(x => x.MissionThemeId == Id).ToList();
            }
            else if (type == "skill")
            {
                adminView.skills = adminView.skills.Where(x => x.SkillId == Id).ToList();
            }
            else if (type == "application")
            {
                adminView.missionApplications = adminView.missionApplications.Where(x => x.MissionApplicationId == Id).ToList();
            }
            else if (type == "story")
            {
                adminView.stories = adminView.stories.Where(x => x.StoryId == Id).ToList();
            }
            else if (type == "banner")
            {
                adminView.banners = adminView.banners.Where(x => x.BannerId == Id).ToList();
            }
            return adminView;
        }

        public bool checkSameData(string Email, long sortOrder, long bannerId)
        {
            if (Email != null)
            {
                var check = _CIPlatformDbContext.Users.Where(e => e.Email == Email).ToList();
                if (!check.Any())
                {
                    return false;
                }
                return true;
            }
            else if (sortOrder != 0)
            {
                var check = _CIPlatformDbContext.Banners.Where(e => e.SortOrder == sortOrder).ToList();
                if (check.Count != 0)
                {
                    if (check.Where(b => b.BannerId == bannerId).Count() != 0)
                    {
                        return false;
                    }
                    return true;
                }
                return true;
            }
            else
            {
                return true;
            }
        }
    }
}
