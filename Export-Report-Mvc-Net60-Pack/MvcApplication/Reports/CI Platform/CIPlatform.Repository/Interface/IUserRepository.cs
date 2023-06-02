using CIPlatform.Entities.Models;
using CIPlatform.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatform.Repository.Interface
{
    public interface IUserRepository
    {
        List<Banner> BannerList();
        User? login(User obj);
        void register(RegistrationViewModel obj);
        User? forgot(User obj);
        PasswordReset? reset(RegistrationViewModel obj, string token);
        List<User> GetUserList();
        List<PasswordReset> GetPasswordResetList();
        public User getUser(string Email);
        List<EditProfileViewModel> getUserList(long userId);
        bool updateUserData(EditProfileViewModel editProfileView, long userId);
        bool changeUserPassword(string Password, long userId);
        bool checkOldpassword(string OldPassword, long userId);
        bool contactUs(string FirstName, string Email, string subject, string Message, long userId);
        long? AvailabilityIndex (long userId);
        bool addTimeSheets(long userId, long MissionId, int goalAction, DateTime goalDate, string goalMessage, int timeHour, int timeMinute, long TimesheetId);
        Timesheet getTimeSheets(long TimesheetId);
        bool checkSameDate(DateTime oldDate,long userId, long TimesheetId, long MissionId);
        List<CmsPage> cmsPages();
        bool UpdateSetting(List<string> settingList, long userId);
        NotificationViewModel Notifications(long userId);
        void addNotificationList(long userId);
        void ReadNotification(long NotificationId);
    }
}
