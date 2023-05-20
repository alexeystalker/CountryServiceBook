﻿namespace CountryService.BLL.Services;

public class CountryServices : ICountryServices
{
    private readonly ICountryRepository _countryRepository;

    public CountryServices(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public Task<int> CreateAsync(CreateCountryModel countryToCreate) =>
        _countryRepository.CreateAsync(countryToCreate);

    public async Task<bool> UpdateAsync(UpdateCountryModel countryToUpdate) => 
        await _countryRepository.UpdateAsync(countryToUpdate) > 0;

    public async Task<bool> DeleteAsync(int id) => 
        await _countryRepository.DeleteAsync(id) > 0;

    public  Task<CountryModel> GetAsync(int id) => 
        _countryRepository.GetAsync(id);

    public async Task<IEnumerable<CountryModel>> GetAllAsync() => 
        await _countryRepository.GetAllAsync();
}