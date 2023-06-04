namespace CountryWiki.BLL.Services;

public class CountryFileUploadValidatorService : 
    ICountryFileUploadValidatorService
{
    public CountryFileUploadValidatorService() { }

    public bool ValidateFile(CountryUploadedFileModel countryUploadedFile)
    {
        return countryUploadedFile.FileName.ToLower().EndsWith(".json") &&
               countryUploadedFile.ContentType == "application/json";
    }

    public async Task<IEnumerable<CreateCountryModel>?> ParseFile(Stream content)
    {
        try
        {
            var parsedCountries = await JsonSerializer
                .DeserializeAsync<IEnumerable<CreateCountryModel>>(
                    content, 
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return (parsedCountries ?? Array.Empty<CreateCountryModel>())
                .Any(x => string.IsNullOrEmpty(x.Name) ||
                          string.IsNullOrEmpty(x.Anthem) ||
                          string.IsNullOrEmpty(x.Description) ||
                          string.IsNullOrEmpty(x.FlagUri) ||
                          string.IsNullOrEmpty(x.CapitalCity) ||
                          !x.Languages.Any())
                ? null
                : parsedCountries;
        }
        catch
        {
            return null;
        }
    }
}