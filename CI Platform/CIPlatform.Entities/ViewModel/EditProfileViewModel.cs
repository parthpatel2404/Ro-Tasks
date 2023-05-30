using CIPlatform.Entities.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatform.Entities.ViewModel
{
    public class EditProfileViewModel
    {
        public long UserId { get; set; }
        public string Email { get; set; } = null!;
        public string? Avatar { get; set; }
        public IFormFile? Profile { get; set; }

        [Required]
        public string OldPassword { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password")]
        public string CnfPassword { get; set; } = null!;

        [Required]
        [MaxLength(16)]
        public string? FirstName { get; set; }
        [Required]
        [MaxLength(16)]
        public string? LastName { get; set; }
        public string? EmployeeId { get; set; }
        public string? ManagerName { get; set; }
        [MaxLength(255)]
        public string? Title { get; set; }
        [MaxLength(16)]
        public string? Department { get; set; }
        public string? ProfileText { get; set; }
        public string? WhyIVolunteer { get; set; }
        [Required]
        public long? CityId { get; set; }
        [Required]
        public long? CountryId { get; set; }
        public List<Country> Country { get; set; }
        public List<City> CityList { get; set; }
        public string? Availability { get; set; }
        public long? AvailabilityIndex { get; set; }
        public string? LinkedInUrl { get; set; }
        public List<UserSkill> userSkills { get; set; }
        public List<int> skillsToAdd { get; set; }
        public List<Skill>? Skills { get; set; }
        public List<Mission> timeTitles { get; set; }
        public List<Mission> goalTitles { get; set; }
        public List<Timesheet> timeSheets { get; set; }
        public List<Timesheet> goalSheets { get; set; }



    }
}
