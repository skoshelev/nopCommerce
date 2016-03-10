using System;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using System.Linq;

namespace Nop.Plugin.Api.Services
{
    public class StateProvinceApiService : IStateProvinceApiService
    {
        private readonly IRepository<StateProvince> _stateProvinceRepository;

        public StateProvinceApiService(IRepository<StateProvince> stateProvinceRepository)
        {
            _stateProvinceRepository = stateProvinceRepository;
        }

        public StateProvince GetStateProvinceByName(string name)
        {
            StateProvince stateProvinceResult = (from stateProvince in _stateProvinceRepository.Table
                                                 where (!string.IsNullOrEmpty(stateProvince.Name) && stateProvince.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                                                 select stateProvince).FirstOrDefault();

            return stateProvinceResult;
        }
    }
}