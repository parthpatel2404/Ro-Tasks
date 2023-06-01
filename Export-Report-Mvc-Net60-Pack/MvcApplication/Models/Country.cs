using System;
using System.Collections.Generic;

namespace MvcApplication.Models;

public partial class Country
{
    public long CountryId { get; set; }

    public string Name { get; set; }

    public string Iso { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual ICollection<Mission> Missions { get; set; } = new List<Mission>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
