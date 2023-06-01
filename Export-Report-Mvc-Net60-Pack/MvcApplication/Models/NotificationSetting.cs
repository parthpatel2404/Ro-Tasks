using System;
using System.Collections.Generic;

namespace MvcApplication.Models;

public partial class NotificationSetting
{
    public long NotificattionSettingsId { get; set; }

    public long UserId { get; set; }

    public string SettingName { get; set; }

    public virtual User User { get; set; }
}
