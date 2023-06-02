using CIPlatform.Entities.Models;
using CIPlatform.Entities.ViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatform.Repository.Interface
{
    public interface IStoryRepository
    {
        List<MissionViewModel> getStoryCardList(long userId);
        List<MissionViewModel> getStoryList(string? search, string[] countries, string[] cities, string[] themes, string[] skills, int pg, long userid);
        List<Mission> getMissionList(long userId);
        bool sendMail(string[] emailList, long storyId, long userId);
        void views(int? storyId, long userId);
        bool addStory(MissionViewModel mvm, long userId, string name);
        //List<StoryMedium> getStoryPhoto(int? storyId);
        //IFormFile getFile(long storyId);
    }
}
