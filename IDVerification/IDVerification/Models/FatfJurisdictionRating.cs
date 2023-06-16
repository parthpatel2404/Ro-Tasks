using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;

namespace IDVerification.Models;

public partial class FatfJurisdictionRating
{
    [Ignore]
    public int Id { get; set; }

    public string? Jurisdiction { get; set; }

    public decimal? EffectiveScore { get; set; }

    public string? CountryEffectiveRating { get; set; }

    public decimal? TechnicalComplianceScore { get; set; }

    public string? TechnicalComplianceRating { get; set; }

    public string? Comments { get; set; }
}
