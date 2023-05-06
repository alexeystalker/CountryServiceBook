namespace CountryService.DAL.Database;

public class CountryContext : DbContext
{
    public CountryContext() : base() { }
    public CountryContext(DbContextOptions<CountryContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=CountryService;");
    }

    public DbSet<Country> Countries { get; set; }
    public DbSet<Language> Languages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>()
            .HasMany(e => e.Languages)
            .WithMany(e => e.Countries)
            .UsingEntity<CountryLanguage>(
                l => l.HasOne<Language>().WithMany(lng =>lng.CountryLanguages).HasForeignKey(e => e.LanguageId),
                r => r.HasOne<Country>().WithMany(c => c.CountryLanguages).HasForeignKey(e => e.CountryId)
            );

        modelBuilder.Entity<Language>()
            .HasData(
                new Language {Id = 1, Name = "English"},
                new Language {Id = 2, Name = "French"},
                new Language {Id = 3, Name = "Spanish"});
    }
}