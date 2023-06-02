using CIPlatform.Entities.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatform.Entities.ViewModel
{
    public class MissionViewModel
    {

        public long MissionId { get; set; }

        public int SkillId { get; set; }
        public long CrudId { get; set; }
        
        public List<MissionSkill> MissionSkills { get; set; }

        public int ThemeId { get; set; }
        public List<CmsPage>? cmsPages { get; set; }
        public string? Theme { get; set; }

        public long CityId { get; set; }

        public long CountryId { get; set; }

        public string CityName { get; set; } = null!;
        public string CountryName { get; set; }

        public string Title { get; set; } = null!;
        public string StoryTitle { get; set; } = null!;
        public List<Mission> missionTitles { get; set; } = null!;

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public long SeatLeft { get; set; }

        public DateTime CreatedAt { get; set; }
        public string MissionType { get; set; }
        public string Mission { get; set; }

        public string? OrganizationName { get; set; }

        public string? OrganizationDescription { get; set; }
        
        public string? MediaPath { get; set; }
        public List<MissionDocument>? Documents { get; set; }
        public List<StoryMedium>? ListMediaPath { get; set; }
        public List<MissionMedium>? missionListMediaPath { get; set; }
        public List<IFormFile>? file { get; set; }
        public List<IFormFile>? missionDocuments { get; set; }
        public IFormFile? bannerPhoto { get; set; }
        public List<string> URL { get; set; }
        public List<int> skillList { get; set; }


        public int Rating { get; set; }
        public int RatingUser { get; set; }

        public string? GoalObjectiveText { get; set; }
        public float? achievedValue { get; set; }
        public int GoalValue { get; set; }
        public float? progressRatio { get; set; }
        public int? TotalSeat { get; set; }

        public DateTime Deadline { get; set; }
    public string? Availability { get; set; }

        public List<MissionViewModel> RelatedMissions { get; set; }
        public List<User> userList { get; set; }
        public List<StoryInvite> alreadyInvited { get; set; }
        public List<MissionInvite> alreadyMisInvited { get; set; }

        public List<MissionApplication> missionApplication { get; set; }
        public string Comments { get; set; }

        public int? favMission { get; set;}
        public int? appMission { get; set;}
        public List<Comment> commentList { get; set; }

        public DateTime? PublishedDate { get; set; }

        public long UserId { get; set; }
        public long StoryId { get; set; }
        public string Avatar { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string whyIVolunteer { get; set; }
        public long? views { get; set; }

        public string storyStatus { get; set; }
        public string Status { get; set; } = null!;
        public string Slug { get; set; } = null!;


    }
}
