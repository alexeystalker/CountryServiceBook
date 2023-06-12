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
}