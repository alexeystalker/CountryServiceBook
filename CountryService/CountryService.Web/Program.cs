using System.IO.Compression;
using Calzolari.Grpc.AspNetCore.Validation;
using CountryService.gRPC.Compression;
using CountryService.Web;
using CountryService.Web.Interceptors;
using CountryService.Web.Validator;
using Grpc.Net.Compression;
using v1 = CountryService.Web.Services.v1; // <--
using v2 = CountryService.Web.Services.v2; // <--

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 6291456; // 6 Mb
    options.MaxSendMessageSize = 6291456; // 6 Mb
    options.CompressionProviders = new ICompressionProvider[] { new BrotliCompressionProvider(CompressionLevel.Optimal) };
    options.ResponseCompressionAlgorithm = "br"; //задаём grpc-accept-encoding, соответствует указанному в провайдере
    options.ResponseCompressionLevel = CompressionLevel.Optimal;
    options.Interceptors.Add<ExceptionInterceptor>(); //регистрируем наш перехватчик
    options.EnableMessageValidation(); //Метод расширения из пакета!
});
builder.Services.AddGrpcValidation(); //Добавляем сервисы валидации
builder.Services.AddValidator<CountryCreateRequestValidator>(); //Добавляем сам валидатор

builder.Services.AddGrpcReflection();

builder.Services.AddSingleton<CountryManagementService>();

var app = builder.Build();

app.MapGrpcReflectionService();

app.MapGrpcService<v1.CountryGrpcService>(); // <-- 
app.MapGrpcService<v2.CountryGrpcService>(); // <--

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
