namespace CountryService.Domain.Models;

public record UpdateCountryModel
{
    public int Id { get; set; }
    public string Description { get; set; }
    public DateTime UpdateDate { get; set; }
}