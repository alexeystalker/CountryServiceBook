﻿using System.Text.Json;
using CountryService.Web.gRPC.v1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using static CountryService.Web.gRPC.v1.CountryService;

var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddSimpleConsole();
    // В книге был уровень Trace, поставим Information.
    logging.SetMinimumLevel(LogLevel.Information);
});
var channel = GrpcChannel.ForAddress("https://localhost:7282", new GrpcChannelOptions
{
    LoggerFactory = loggerFactory
});
var countryClient = new CountryServiceClient(channel);

await Get(countryClient, loggerFactory.CreateLogger(nameof(Get)));
await Create(countryClient, loggerFactory.CreateLogger(nameof(Create)));
await Delete(countryClient, loggerFactory.CreateLogger(nameof(Delete)));
await GetAll(countryClient, loggerFactory.CreateLogger(nameof(GetAll)));

async Task GetAll(CountryServiceClient client, ILogger logger)
{
    using var serverStreamingCall = client.GetAll(new Empty());
    await foreach (var response in serverStreamingCall.ResponseStream.ReadAllAsync())
    {
        logger.LogInformation($"{response.Name}: {response.Description}");
    }
    // Читаем заголовки и трейлеры, сериализуем в JSON и пишем в лог
    var serverStreamingCallHeaders = await serverStreamingCall.ResponseHeadersAsync;
    logger.LogInformation($"Headers:{Environment.NewLine}{JsonSerializer.Serialize(serverStreamingCallHeaders, new JsonSerializerOptions { WriteIndented = true })}");

    var serverStreamingCallTrailers = serverStreamingCall.GetTrailers();
    logger.LogInformation($"Trailers:{Environment.NewLine}{JsonSerializer.Serialize(serverStreamingCallTrailers, new JsonSerializerOptions { WriteIndented = true })}");
}

async Task Delete(CountryServiceClient client, ILogger logger)
{
    using var clientStreamingCall = client.Delete();
    var countriesToDelete = new List<CountryIdRequest>
    {
        new CountryIdRequest {Id = 1},
        new CountryIdRequest {Id = 2}
    };
    // Записываем
    foreach (var request in countriesToDelete)
    {
        await clientStreamingCall.RequestStream.WriteAsync(request);
        logger.LogInformation($"Country with id {request.Id} set for deletion");
    }
    // Сообщаем серверу о завершении передачи
    await clientStreamingCall.RequestStream.CompleteAsync();
    // Завершаем запрос получением ответа
    var emptyResponse = await clientStreamingCall.ResponseAsync;
    // Читаем заголовки и трейлеры, сериализуем в JSON и пишем в лог
    var clientStreamingCallHeaders = await clientStreamingCall.ResponseHeadersAsync;
    logger.LogInformation($"Headers:{Environment.NewLine}{JsonSerializer.Serialize(clientStreamingCallHeaders, new JsonSerializerOptions { WriteIndented = true })}");

    var clientStreamingCallTrailers = clientStreamingCall.GetTrailers();
    logger.LogInformation($"Trailers:{Environment.NewLine}{JsonSerializer.Serialize(clientStreamingCallTrailers, new JsonSerializerOptions { WriteIndented = true })}");

    //await clientStreamingCall; //Так тоже можно завершить, но тогда заголовки и трейлеры не прочитать.
}

async Task Create(CountryServiceClient client, ILogger logger)
{
    using var bidirectionalStreamingCall = countryClient.Create();
    var countriesToCreate = new List<CountryCreationRequest>
    {
        new CountryCreationRequest
        {
            Name = "France",
            Description = "Western european country",
            CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc))
        },
        new CountryCreationRequest
        {
            Name = "Poland",
            Description = "Eastern european country",
            CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc))
        }
    };
    // Записываем
    foreach (var request in countriesToCreate)
    {
        await bidirectionalStreamingCall.RequestStream.WriteAsync(request);
        logger.LogInformation($"Country {request.Name} set for creation");
    }
    // Сообщаем серверу о завершении передачи
    await bidirectionalStreamingCall.RequestStream.CompleteAsync();
    // Читаем поток с сервера
    await foreach (var createdCountry in bidirectionalStreamingCall.ResponseStream.ReadAllAsync())
    {
        logger.LogInformation($"{createdCountry.Name} has been created with Id {createdCountry.Id}");
    }
    // Читаем заголовки и трейлеры, сериализуем в JSON и пишем в лог
    var bidirectionalStreamingCallHeaders = await bidirectionalStreamingCall.ResponseHeadersAsync;
    logger.LogInformation($"Headers:{Environment.NewLine}{JsonSerializer.Serialize(bidirectionalStreamingCallHeaders, new JsonSerializerOptions { WriteIndented = true })}");

    var bidirectionalStreamingCallTrailers = bidirectionalStreamingCall.GetTrailers();
    logger.LogInformation($"Trailers:{Environment.NewLine}{JsonSerializer.Serialize(bidirectionalStreamingCallTrailers, new JsonSerializerOptions { WriteIndented = true })}");
}

async Task Get(CountryServiceClient client, ILogger logger)
{
    // создаём объект вызова
    var countryCall = client.GetAsync(new CountryIdRequest {Id = 1});
    // читаем ответ
    var country = await countryCall.ResponseAsync;
    logger.LogInformation($"{country.Id}: {country.Name}");
    // Читаем заголовки и трейлеры, сериализуем в JSON и пишем в лог
    var countryCallHeaders = await countryCall.ResponseHeadersAsync;
    logger.LogInformation($"Headers:{Environment.NewLine}{JsonSerializer.Serialize(countryCallHeaders, new JsonSerializerOptions { WriteIndented = true })}");

    var countryCallTrailers = countryCall.GetTrailers();
    logger.LogInformation($"Trailers:{Environment.NewLine}{JsonSerializer.Serialize(countryCallTrailers, new JsonSerializerOptions { WriteIndented = true })}");

    // альтернативный вариант:
    // var country = await client.GetAsync(new CountryRequest { Id = 1 });
    // Но при этом заголовки и трейлеры будут недоступны
}