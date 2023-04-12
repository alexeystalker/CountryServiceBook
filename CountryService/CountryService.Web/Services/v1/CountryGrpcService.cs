using CountryService.Web.gRPC.v1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static CountryService.Web.gRPC.v1.CountryService;

namespace CountryService.Web.Services.v1;

public class CountryGrpcService : CountryServiceBase
{
    private readonly CountryManagementService _countryManagementService;
    private readonly ILogger<CountryGrpcService> _logger;

    public CountryGrpcService(
        CountryManagementService countryManagementService,
        ILogger<CountryGrpcService> logger)
    {
        _countryManagementService = countryManagementService;
        _logger = logger;
    }


    public override async Task GetAll(Empty request, IServerStreamWriter<CountryReply> responseStream, ServerCallContext context)
    {
        //Стримим все найденные страны клиенту
        var replies = await _countryManagementService.GetAllAsync();
        foreach (var countryReply in replies)
        {
            await responseStream.WriteAsync(countryReply);
        }
    }


    public override async Task<CountryReply> Get(CountryIdRequest request, ServerCallContext context)
    {
        //Нам может вернуться null, если передан несуществующий Id, вернем NotFound в этом случае
        var result = await _countryManagementService.GetAsync(request);
        return result ?? throw new RpcException(new Status(StatusCode.NotFound, $"No country with id {request.Id}"));
    }

    public override async Task<Empty> Delete(IAsyncStreamReader<CountryIdRequest> requestStream, ServerCallContext context)
    {
        //Сперва загрузим все запросы на удаление
        var requestsList = new List<CountryIdRequest>();
        await foreach (var idRequest in requestStream.ReadAllAsync())
        {
            requestsList.Add(idRequest);
        }
        //Теперь удалим всё разом
        await _countryManagementService.DeleteAsync(requestsList);

        return new Empty();
    }

    public override async Task<Empty> Update(CountryUpdateRequest request, ServerCallContext context)
    {
        await _countryManagementService.UpdateAsync(request);
        return new Empty();
    }

    public override async Task Create(IAsyncStreamReader<CountryCreationRequest> requestStream, IServerStreamWriter<CountryCreationReply> responseStream, ServerCallContext context)
    {
        //Сперва загрузим все запросы на создание
        var requestsList = new List<CountryCreationRequest>();
        await foreach (var createRequest in requestStream.ReadAllAsync())
        {
            requestsList.Add(createRequest);
        }
        var createdCountries = await _countryManagementService.CreateAsync(requestsList);
        foreach (var createdCountry in createdCountries)
        {
            await responseStream.WriteAsync(createdCountry);
        }

    }
}