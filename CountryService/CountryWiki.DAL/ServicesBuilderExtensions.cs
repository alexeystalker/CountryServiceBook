namespace CountryWiki.DAL;

public static class ServicesBuilderExtensions
{
    public static IServiceCollection AddCountryServiceClient(this IServiceCollection services,
        ILoggerFactory loggerFactory, string countryServiceUri)
    {
        services.AddGrpcClient<CountryServiceClient>(o =>
            {
                o.Address = new Uri(countryServiceUri);
            })
            .AddInterceptor(() => new TracerInterceptor(loggerFactory.CreateLogger<TracerInterceptor>()))
            .ConfigureChannel(o =>
            {
                o.CompressionProviders = new List<ICompressionProvider>
                {
                    new BrotliCompressionProvider()
                };
                o.MaxReceiveMessageSize = 6291456;
                o.MaxSendMessageSize = 6291456;
            });

        return services;
    }

    public static IServiceCollection AddCountryServiceGrpcWebAspNetCoreClient(
        this IServiceCollection services,
        ILoggerFactory loggerFactory,
        string countryServiceUri)
    {
        services.AddGrpcClient<CountryServiceClient>(o =>
            {
                o.Address = new Uri(countryServiceUri);
            })
            //Добавляем GrpcWebHandler здесь
            .ConfigurePrimaryHttpMessageHandler(() => new GrpcWebHandler(new HttpClientHandler()))
            .AddInterceptor(() => new TracerInterceptor(loggerFactory.CreateLogger<TracerInterceptor>()))
            .ConfigureChannel(o =>
            {
                o.CompressionProviders = new List<ICompressionProvider>
                {
                    new BrotliCompressionProvider()
                };
                o.MaxReceiveMessageSize = 6291456;
                o.MaxSendMessageSize = 6291456;
            });

        return services;
    }

    public static IServiceCollection AddCountryServiceGrpcWebClient(this IServiceCollection services,
        ILoggerFactory loggerFactory, string countryServiceUri)
    {
        var channel = GrpcChannel.ForAddress(new Uri(countryServiceUri), new GrpcChannelOptions
        {
            LoggerFactory = loggerFactory,
            CompressionProviders = new List<ICompressionProvider>
            {
                new BrotliCompressionProvider()
            },
            MaxReceiveMessageSize = 6291456,
            MaxSendMessageSize = 6291456,
            
            HttpHandler = new GrpcWebHandler
            {
                HttpVersion = new Version("1.1"),
                GrpcWebMode = GrpcWebMode.GrpcWebText,
                InnerHandler = new HttpClientHandler()
            }
        });
        // Просто создадим клиента
        // var client = new CountryServiceClient(channel);
        
        // Добавим перехватчик
        var client = new CountryServiceClient(channel
            .Intercept(new TracerInterceptor(loggerFactory.CreateLogger<TracerInterceptor>())));
        //Для простоты добавим синглтоном.
        services.AddSingleton(client);

        return services;
    }
}