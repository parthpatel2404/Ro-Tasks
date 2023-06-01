using System;
using System.Collections.Generic;

namespace MvcApplication.Models;

public partial class User
{
    public long UserId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public long PhoneNumber { get; set; }

    public string Avatar { get; set; }

    public string WhyIVolunteer { get; set; }

    public string EmployeeId { get; set; }

    public string Department { get; set; }

    public long? CityId { get; set; }

    public long? CountryId { get; set; }

    public string ProfileText { get; set; }

    public string LinkedInUrl { get; set; }

    public string Title { get; set; }

    public string Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string Availability { get; set; }

    public string Manager { get; set; }

    public string ReturnUrl { get; set; }

    public string Role { get; set; }

    public virtual City City { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<ContactU> ContactUs { get; set; } = new List<ContactU>();

    public virtual Country Country { get; set; }

    public virtual ICollection<FavoriteMission> FavoriteMissions { get; set; } = new List<FavoriteMission>();

    public virtual ICollection<MissionApplication> MissionApplications { get; set; } = new List<MissionApplication>();

    public virtual ICollection<MissionInvite> MissionInvites { get; set; } = new List<MissionInvite>();

    public virtual ICollection<MissionRating> MissionRatings { get; set; } = new List<MissionRating>();

    public virtual ICollection<NotificationSetting> NotificationSettings { get; set; } = new List<NotificationSetting>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Story> Stories { get; set; } = new List<Story>();

    public virtual ICollection<StoryInvite> StoryInvites { get; set; } = new List<StoryInvite>();

    public virtual ICollection<StoryView> StoryViews { get; set; } = new List<StoryView>();

    public virtual ICollection<Timesheet> Timesheets { get; set; } = new List<Timesheet>();

    public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
}
