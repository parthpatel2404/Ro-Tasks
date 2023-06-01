using System;
using System.Collections.Generic;

namespace MvcApplication.Models;

public partial class Notification
{
    public long NotificationId { get; set; }

    public long UserId { get; set; }

    public long? MissionId { get; set; }

    public long? StoryId { get; set; }

    public string Type { get; set; }

    public bool Status { get; set; }

    public long? FromId { get; set; }

    public virtual Mission Mission { get; set; }

    public virtual Story Story { get; set; }

    public virtual User User { get; set; }
}
