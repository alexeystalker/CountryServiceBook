using CountryService.Web.gRPC.v1;
using Google.Protobuf.WellKnownTypes;

namespace CountryService.Web
{
    public class CountryManagementService
    {
        private readonly List<CountryReply> _countries = new();

        public CountryManagementService()
        {
            _countries.Add(new CountryReply
            {
                Id = 1,
                Name = "Canada",
                Description = "Canada has at least 32 000 lakes",
                CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2021, 1, 2), DateTimeKind.Utc))
            });
            _countries.Add(new CountryReply
            {
                Id = 2,
                Name = "USA",
                Description = "Yellowstone has 300 to 500 geysers",
                CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2021, 1, 2), DateTimeKind.Utc))
            });
            _countries.Add(new CountryReply
            {
                Id = 3,
                Name = "Mexico",
                Description = "Mexico is crossed by Sierra Madre Oriental and Sierra Madre Occidental mountains",
                CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2021, 1, 2), DateTimeKind.Utc))
            });
        }

        public  Task<IEnumerable<CountryReply>> GetAllAsync()
        {
            return Task.FromResult(_countries.AsEnumerable());
        }

        public Task<CountryReply?> GetAsync(CountryIdRequest idRequest)
        {
            return Task.FromResult(_countries.FirstOrDefault(c => c.Id == idRequest.Id));
        }

        public Task DeleteAsync(IEnumerable<CountryIdRequest> idRequests)
        {
            var ids = idRequests.Select(r => r.Id).ToHashSet();
            _countries.RemoveAll(c => ids.Contains(c.Id));
            return Task.CompletedTask;
        }

        public Task UpdateAsync(CountryUpdateRequest updateRequest)
        {
            var countryToUpdate = _countries.FirstOrDefault(c => c.Id == updateRequest.Id);
            if (countryToUpdate != null)
            {
                countryToUpdate.Description = updateRequest.Description;
                countryToUpdate.UpdateDate = updateRequest.UpdateDate;
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<CountryCreationReply>> CreateAsync(IEnumerable<CountryCreationRequest> creationRequests)
        {
            var countriesCount = _countries.Count;
            var newCountries = creationRequests
                .Where(cr => _countries.All(c => c.Name != cr.Name))
                .Select(cr => new CountryReply
                {
                    Id = ++countriesCount,
                    Name = cr.Name,
                    Description = cr.Description,
                    Flag = cr.Flag,
                    CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc))
                }).ToList();
            _countries.AddRange(newCountries);
            var result = newCountries.Select(nc => new CountryCreationReply
            {
                Id = nc.Id,
                Name = nc.Name
            });
            return Task.FromResult(result);
        }
    }
}
