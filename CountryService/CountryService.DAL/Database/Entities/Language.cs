﻿namespace CountryService.DAL.Database.Entities;

public class Language
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Country> Countries { get; set; }
}