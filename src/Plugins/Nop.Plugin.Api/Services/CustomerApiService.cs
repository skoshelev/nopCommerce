using System.Collections.Generic;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Services
{
    public class CustomerApiService : ICustomerApiService
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomerApiService(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public IList<CustomerDto> GetCustomersDtos(string createdAtMin = "", string createdAtMax = "", byte limit = Configurations.DefaultLimit,
            int page = 1, int sinceId = 0)
        {
            throw new System.NotImplementedException();
        }

        public int GetCustomersCount()
        {
            throw new System.NotImplementedException();
        }

        public IList<CustomerDto> Search(Dictionary<string, string> query = null, string order = "desc", int page = 1, byte limit = Configurations.DefaultLimit)
        {
            throw new System.NotImplementedException();
        }
    }
}