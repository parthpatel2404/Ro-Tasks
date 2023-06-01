using System;
using System.Collections.Generic;

namespace MvcApplication.Models;

public partial class StoryView
{
    public long ViewId { get; set; }

    public long? StoryId { get; set; }

    public long? UserId { get; set; }

    public virtual Story Story { get; set; }

    public virtual User User { get; set; }
}
