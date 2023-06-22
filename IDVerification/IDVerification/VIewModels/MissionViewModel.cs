using IDVerification.Models;

namespace IDVerification.VIewModels
{
    public class MissionViewModel
    {
        public long Id { get; set; }

        public string Address { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string MissionType { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string? OrganizationName { get; set; }

        public string? OrganizationDetail { get; set; }

        public string? Availability { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public long SeatLeft { get; set; }

        public long? TotalSeats { get; set; }

        public DateTime? Deadline { get; set; }

        public MissionTheme Theme { get; set; }
        
        public List<MissionInvite> Invites { get; set; }
        
        public List<MissionRating> Ratings { get; set; }
    }
    public class MissionTheme
    {
        public long Id { get; set; }
        public string? Title { get; set; }
    }

    public class MissionInvite
    {
        public long Id { get; set; }
        public long FromUserId { get; set; }
        public long ToUserId { get; set; }
    }

    public class MissionRating
    {
        public long Id { get; set; }
        public long MissionId { get; set; }
        public int Rating { get; set; }
    }

}
