namespace CountryService.gRPC.Mappers;

public static class CountryReplyMapper
{
    public static CountryReply ToReply(this CountryModel country)
    {
        var countryReply = new CountryReply
        {
            Id = country.Id,
            Name = country.Name,
            Description = country.Description,
            Anthem = country.Anthem,
            CapitalCity = country.CapitalCity,
            FlagUri = country.FlagUri
        };
        countryReply.Languages.AddRange(country.Languages);
        return countryReply;
    }
}