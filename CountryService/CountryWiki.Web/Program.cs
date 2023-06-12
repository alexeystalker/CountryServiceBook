var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICountryServices, CountryServices>();
builder.Services.AddScoped<ICountryFileUploadValidatorService, 
    CountryFileUploadValidatorService>();
builder.Services.AddSingleton<ISyncCountriesChannel, SyncCountriesChannel>();
builder.Services.AddHostedService<SyncUploadedCountriesBackgroundService>();
builder.Services.AddSingleton(new GlobalOptions {ProcessingUpload = false});

var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Trace);
});

builder.Services.AddCountryServiceClient(
    loggerFactory, 
    builder.Configuration.GetValue<string>("CountryServiceUri"));

//-- начиная с этой строки, весь код ниже взят напрямую из шаблона
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
