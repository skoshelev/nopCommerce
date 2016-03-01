using System.Collections.Generic;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Services
{
    public interface ICustomerApiService
    {
        IList<CustomerDto> GetCustomersDtos(string createdAtMin = "", string createdAtMax = "",
          byte limit = Configurations.DefaultLimit, int page = 1, int sinceId = 0);

        int GetCustomersCount();
        IList<CustomerDto> Search(Dictionary<string, string> query = null, string order = "desc", int page = 1, byte limit = Configurations.DefaultLimit);
    }
}