using System;
using System.Collections.Generic;

namespace MvcApplication.Models;

public partial class PasswordReset
{
    public string Email { get; set; }

    public string Token { get; set; }

    public DateTime CreatedAt { get; set; }

    public int Id { get; set; }

    public bool Expire { get; set; }
}
