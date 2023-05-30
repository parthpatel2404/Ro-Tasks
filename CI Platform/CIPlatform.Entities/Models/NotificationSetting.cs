using System;
using System.Collections.Generic;

namespace CIPlatform.Entities.Models;

public partial class NotificationSetting
{
    public long NotificattionSettingsId { get; set; }

    public long UserId { get; set; }

    public string SettingName { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
