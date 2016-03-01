using Nop.Core.Domain.Directory;

namespace Nop.Plugin.Api.Services
{
    public interface ICountryApiService
    {
        Country GetCountryByName(string name);
    }
}