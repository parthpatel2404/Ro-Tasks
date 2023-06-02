using System;
using System.Collections.Generic;

namespace CIPlatform.Entities.Models;

public partial class PasswordReset
{
    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int Id { get; set; }

    public bool Expire { get; set; }
}
