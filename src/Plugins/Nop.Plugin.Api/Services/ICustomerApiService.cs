using System;
using System.Collections.Generic;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Services
{
    public interface ICustomerApiService
    {
        int GetCustomersCount();

        CustomerDto GetCustomerById(int id);

        IList<CustomerDto> GetCustomersDtos(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
        int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);
        
        IList<CustomerDto> Search(string query = "", string order = "desc", int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit);

        Dictionary<string, string> GetFirstAndLastNameByCustomerId(int customerId);
    }
}