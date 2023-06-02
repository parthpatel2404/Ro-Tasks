using CIPlatform.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatform.Entities.ViewModel
{
    public class AdminViewModel
    {
        public List<User> users { get; set; }
        public List<CmsPage> cmsPages { get; set; }
        public List<Mission> missions { get; set; }
        public List<MissionTheme> missionThemes { get; set; }
        public List<Skill> skills { get; set; }
        public List<MissionApplication> missionApplications { get; set; }
        public List<Story> stories { get; set; }
        public List<Country> country { get; set; }
        public List<City> city { get; set; }
        public List<MissionTheme> themeTitle { get; set; }
        public List<Skill> totalSkill { get; set; }
        public List<Banner> banners { get; set; }
        public List<MissionSkill> missionSkills { get; set; }
        //public List<int> skillsToAdd { get; set; }

    }
}
