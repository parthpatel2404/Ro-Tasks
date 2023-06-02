using CIPlatform.Entities.Models;
using CIPlatform.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatform.Repository.Interface
{
    public interface IPlatformRepository
    {
        List<Country> getCountryList();
        List<City> getCityList(List<string> countryId);
        List<MissionTheme> getThemeList();
        List<Skill> getSkillList();


        string getCityName(long cityId);
        //DateTime? deadlineOfMission(long missionId);
        //int? getTotalSeatsOfMission(long missionId);
        string getMediaPathFromId(long mediaId);
        List<MissionRating> getMissionRatings(long missionID);
        string getMissionThemes(long themeID);
        int getMissionCount();
        GoalMission getGoalMissionDetails(long missionId);

        List<MissionViewModel> getMissionCardsList(long UserId);
        List<MissionViewModel> getMissionList(string? search, string[] countries, string[] cities, string[] themes, string[] skills, int? sort, long UserId,int pg, int pgSize);
        List<Mission> getMissions();
        bool addFavourite(int missionId, int userId);
        int applyMission(int missionId, int userId);
        //List<User> userList();

        void addComments(string comment, long UserId, long MissionId);
        int getFavMission(long userId,long missionId);
        int getAppMission(long userId, long missionId); 
        bool sendMail(string[] email, long missionId, long userId);
        bool addRatings(int rate, long missionId, long userId);
        List<MissionViewModel> getMisAppList(int pg,long missionId);

    }
}
