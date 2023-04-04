using System.IO.Compression;
using Azure.Core;
using CountryService.gRPC.Compression;
using CountryService.Web;
using CountryService.Web.Interceptors;
using CountryService.Web.Services;
using Grpc.Core;
using Grpc.Net.Compression;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.IgnoreUnknownServices = true;
    options.MaxReceiveMessageSize = 6291456; // 6 Mb
    options.MaxSendMessageSize = 6291456; // 6 Mb
    options.CompressionProviders = new ICompressionProvider[] { new BrotliCompressionProvider(CompressionLevel.Optimal) };
    options.ResponseCompressionAlgorithm = "br"; //задаём grpc-accept-encoding, соответствует указанному в провайдере
    options.ResponseCompressionLevel = CompressionLevel.Optimal;
    options.Interceptors.Add<ExceptionInterceptor>(); //регистрируем наш перехватчик
});
builder.Services.AddGrpcReflection();

builder.Services.AddSingleton<CountryManagementService>();

var app = builder.Build();

app.MapGrpcReflectionService();
app.MapGrpcService<CountryGrpcService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Use(async(context, next) =>
{
    if (context.Request.ContentType == "application/grpc")
    {
        context.Response.OnStarting(() =>
        {
            if (context.Response.StatusCode == 404)
            {
                context.Response.ContentType = "application/grpc";
                context.Response.Headers.Add("grpc-status", ((int)StatusCode.NotFound).ToString());
                context.Response.StatusCode = 200; //Помним о том, что HTTP-код должен быть 200 OK
            }
            return Task.CompletedTask;
        });
    }
    await next(context);
});

app.Run();
