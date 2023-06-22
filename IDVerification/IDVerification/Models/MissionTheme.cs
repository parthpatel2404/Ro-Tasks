﻿using System;
using System.Collections.Generic;

namespace IDVerification.Models;

public partial class MissionTheme
{
    public long MissionThemeId { get; set; }

    public string? Title { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
  
    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Mission> Missions { get; set; } = new List<Mission>();
}
