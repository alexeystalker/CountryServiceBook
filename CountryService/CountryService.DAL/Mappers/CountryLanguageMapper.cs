namespace CountryService.DAL.Mappers;

public static class CountryLanguageMapper
{
    public static IQueryable<CountryModel> ToDomain(this IQueryable<Country> countries)
        => countries.Select(c => new CountryModel
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            CapitalCity = c.CapitalCity,
            Anthem = c.Anthem,
            FlagUri = c.FlagUri,
            Languages = c.Languages.Select(l => l.Name)
        });
}