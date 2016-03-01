using Nop.Core.Data;
using Nop.Core.Domain.Directory;

namespace Nop.Plugin.Api.Services
{
    public class CountryApiService : ICountryApiService
    {
        private readonly IRepository<Country> _countryRepository;

        public CountryApiService(IRepository<Country> countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public Country GetCountryByName(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}