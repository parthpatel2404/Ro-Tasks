using System;
using System.Collections.Generic;

namespace IDVerification.Models;

public partial class UserTable
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public long? PhoneNumber { get; set; }
}
