using System;
using System.Collections.Generic;


namespace IDVerification.Models;

public partial class MissionInvite
{
    public long MissionInviteId { get; set; }

    public long MissionId { get; set; }

    public long FromUserId { get; set; }

    public long ToUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
   
    public DateTime? DeletedAt { get; set; }

    public virtual Mission Mission { get; set; } = null!;
}
