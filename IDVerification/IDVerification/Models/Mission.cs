using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IDVerification.Models;

public partial class Mission
{
    public long MissionId { get; set; }

    public long ThemeId { get; set; }

    public long CityId { get; set; }

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

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? DeletedAt { get; set; }

    public long SeatLeft { get; set; }

    public long? TotalSeats { get; set; }

    public DateTime? Deadline { get; set; }

    public virtual ICollection<MissionInvite> MissionInvites { get; set; } = new List<MissionInvite>();

    public virtual ICollection<MissionRating> MissionRatings { get; set; } = new List<MissionRating>();

    public virtual MissionTheme Theme { get; set; } = null!;
}
