using System;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using System.Linq;

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
            Country countryResult = (from country in _countryRepository.Table
                                     where (!string.IsNullOrEmpty(country.Name) && country.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) ||
                                           (!string.IsNullOrEmpty(country.TwoLetterIsoCode) && country.TwoLetterIsoCode.Equals(name, StringComparison.InvariantCultureIgnoreCase)) ||
                                           (!string.IsNullOrEmpty(country.ThreeLetterIsoCode) && country.ThreeLetterIsoCode.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                                     select country).FirstOrDefault();

            return countryResult;
        }
    }
}