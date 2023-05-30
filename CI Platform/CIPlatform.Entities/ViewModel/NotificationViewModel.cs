using CIPlatform.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatform.Entities.ViewModel
{
    public class NotificationViewModel
    {
        public List<string> Settings { get; set; }
        //public string RecommendAvatar { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<User> users { get; set; }
        public List<Mission> missions { get; set; }
        public List<Story> stories { get; set; }
        public int NotificationCount { get; set; }
        public List<Notification> NewRecommendations { get; set; }
        public List<Notification> OldRecommendations { get; set; }
        public List<Notification> NewStories { get; set; }
        public List<Notification> OldStories { get; set; }
        public List<Notification> NewMissionApplications { get; set; }
        public List<Notification> OldMissionApplications { get; set; }
        public List<Notification> NewStoryRecommendations { get; set; }
        public List<Notification> OldStoryRecommendations { get; set; }
        public List<Notification> NewMissions { get; set; }
        public List<Notification> OldMissions { get; set; }
    }
}
