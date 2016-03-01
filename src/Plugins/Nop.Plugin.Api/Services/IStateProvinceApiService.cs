using Nop.Core.Domain.Directory;

namespace Nop.Plugin.Api.Services
{
    public interface IStateProvinceApiService
    {
        StateProvince GetStateProvinceByName(string name);
    }
}