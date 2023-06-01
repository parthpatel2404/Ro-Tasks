using System;
using System.Collections.Generic;

namespace MvcApplication.Models;

public partial class ContactU
{
    public long ContactId { get; set; }

    public long? UserId { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string Subject { get; set; }

    public string Message { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; }
}
