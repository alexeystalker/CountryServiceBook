namespace CountryWiki.DAL.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly CountryServiceClient _countryServiceClient;

    public CountryRepository(CountryServiceClient countryServiceClient)
    {
        _countryServiceClient = countryServiceClient;
    }
    public async IAsyncEnumerable<CreatedCountryModel> CreateAsync(IEnumerable<CreateCountryModel> countriesToCreate)
    {
        using var bidirectionalStreamingCall = _countryServiceClient.Create();
        foreach (var countryToCreate in countriesToCreate)
        {
            var countryToCreateRequest = new CountryCreationRequest
            {
                Name = countryToCreate.Name,
                Description = countryToCreate.Description,
                Anthem = countryToCreate.Anthem,
                CapitalCity = countryToCreate.CapitalCity,
                FlagUri = countryToCreate.FlagUri
            };
            countryToCreateRequest.Languages.AddRange(countryToCreate.Languages);
            await bidirectionalStreamingCall.RequestStream.WriteAsync(countryToCreateRequest);
        }
        // отправляем уведомление о том, что мы закончили
        await bidirectionalStreamingCall.RequestStream.CompleteAsync();

        // Читаем
        while (await bidirectionalStreamingCall.ResponseStream.MoveNext(CancellationToken.None))
        {
            var country = bidirectionalStreamingCall.ResponseStream.Current;
            yield return new CreatedCountryModel
            {
                Id = country.Id,
                Name = country.Name
            };
        }
    }
    
    public async Task DeleteAsync(int id) => 
        await _countryServiceClient.DeleteAsync(new CountryIdRequest {Id = id});

    public async IAsyncEnumerable<CountryModel> GetAllAsync()
    {
        using var serverStreamingCall = _countryServiceClient.GetAll(new Empty());
        while (await serverStreamingCall.ResponseStream.MoveNext(CancellationToken.None))
        {
            yield return serverStreamingCall.ResponseStream.Current.ToDomain();
        }
    }
    
    public async Task<CountryModel> GetAsync(int id) => 
        (await _countryServiceClient.GetAsync(new CountryIdRequest {Id = id})).ToDomain();

    public async Task UpdateAsync(UpdateCountryModel countryToUpdate) =>
        await _countryServiceClient.UpdateAsync(new CountryUpdateRequest
        {
            Id = countryToUpdate.Id,
            Description = countryToUpdate.Description,
        });
}