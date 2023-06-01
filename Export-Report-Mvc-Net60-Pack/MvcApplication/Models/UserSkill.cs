using System;
using System.Collections.Generic;

namespace MvcApplication.Models;

public partial class UserSkill
{
    public long UserSkillId { get; set; }

    public long? UserId { get; set; }

    public int SkillId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Skill Skill { get; set; }

    public virtual User User { get; set; }
}
