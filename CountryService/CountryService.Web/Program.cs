using System.IO.Compression;
using CountryService.gRPC.Compression;
using CountryService.Web;
using CountryService.Web.Services;
using Grpc.Net.Compression;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 6291456; // 6 Mb
    options.MaxSendMessageSize = 6291456; // 6 Mb
    options.CompressionProviders = new ICompressionProvider[] { new BrotliCompressionProvider(CompressionLevel.Optimal) };
    options.ResponseCompressionAlgorithm = "br"; //задаём grpc-accept-encoding, соответствует указанному в провайдере
    options.ResponseCompressionLevel = CompressionLevel.Optimal;
});
builder.Services.AddGrpcReflection();

builder.Services.AddSingleton<CountryManagementService>();

var app = builder.Build();

app.MapGrpcReflectionService();
app.MapGrpcService<CountryGrpcService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
