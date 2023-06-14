using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;

namespace IDVerification.Models;

public partial class FatfJurisdictionRating
{
    [Ignore]
    public int JurisdictionId { get; set; }

    public string? Jurisdiction { get; set; }

    public float? EffectivenessScore { get; set; }

    public string? CountryEffectivenessRating { get; set; }

    public float? TechnicalComplianceScore { get; set; }

    public string? TechnicalComplianceTcRating { get; set; }

    public string? Comments { get; set; }
}
