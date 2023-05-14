﻿namespace CountryService.Domain.Models;

public record CreateCountryModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string FlagUri { get; set; }
    public string CapitalCity { get; set; }
    public string Anthem { get; set; }
    public DateTime CreateDate { get; set; }
    public IEnumerable<int> Languages { get; set; }
}