namespace CountryService.DAL.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly CountryContext _countryContext;

    public CountryRepository(CountryContext countryContext)
    {
        _countryContext = countryContext;
    }
    public async Task<int> CreateAsync(CreateCountryModel countryToCreate)
    {
        var country = new Country
        {
            Name = countryToCreate.Name,
            Description = countryToCreate.Description,
            CapitalCity = countryToCreate.CapitalCity,
            Anthem = countryToCreate.Anthem,
            FlagUri = countryToCreate.FlagUri,
            CreateDate = countryToCreate.CreateDate,
            CountryLanguages = countryToCreate.Languages
                .Select(l => new CountryLanguage {LanguageId = l}).ToList()
        };
        await _countryContext.Countries.AddAsync(country);
        await _countryContext.SaveChangesAsync();

        return country.Id;
    }

    public Task<int> UpdateAsync(UpdateCountryModel countryToUpdate)
    {
        var country = new Country
        {
            Id = countryToUpdate.Id,
            Description = countryToUpdate.Description,
            UpdateDate = countryToUpdate.UpdateDate
        };
        _countryContext.Entry(country).Property(p => p.Description).IsModified = true;
        _countryContext.Entry(country).Property(p => p.UpdateDate).IsModified = true;
        return _countryContext.SaveChangesAsync();
    }

    public Task<int> DeleteAsync(int id)
    {
        var country = new Country
        {
            Id = id
        };
        _countryContext.Entry(country).State = EntityState.Deleted;
        return _countryContext.SaveChangesAsync();
    }

    public Task<CountryModel?> GetAsync(int id) =>
        _countryContext.Countries.AsNoTracking().ToDomain().SingleOrDefaultAsync(c => c.Id == id);

    public Task<List<CountryModel>> GetAllAsync() =>
        _countryContext.Countries.AsNoTracking().ToDomain().ToListAsync();
}