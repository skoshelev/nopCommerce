using System.Collections.Generic;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Services
{
    public interface ICustomerApiService
    {
        IList<CustomerDto> GetCustomersDtos(string createdAtMin = "", string createdAtMax = "",
        int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);

        CustomerDto GetCustomerById(int id);
        int GetCustomersCount();
        IList<CustomerDto> Search(string query = "", string order = "desc", int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit);
    }
}