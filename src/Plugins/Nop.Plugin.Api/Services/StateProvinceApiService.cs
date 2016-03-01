using Nop.Core.Data;
using Nop.Core.Domain.Directory;

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
            throw new System.NotImplementedException();
        }
    }
}