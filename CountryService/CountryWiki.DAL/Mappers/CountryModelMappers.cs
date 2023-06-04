namespace CountryWiki.DAL.Mappers;

public static class CountryModelMappers
{
    public static CountryModel ToDomain(this CountryReply countryReply) =>
            new CountryModel
            {
                Id = countryReply.Id,
                Name = countryReply.Name,
                Description = countryReply.Description,
                Anthem = countryReply.Anthem,
                FlagUri = countryReply.FlagUri,
                CapitalCity = countryReply.CapitalCity,
                Languages = countryReply.Languages
            };
}