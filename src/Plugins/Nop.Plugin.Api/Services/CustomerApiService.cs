using System;
using System.Collections.Generic;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.MVC;
using System.Linq;
using Nop.Core.Domain.Common;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Web.Framework.Kendoui;

namespace Nop.Plugin.Api.Services
{
    public class CustomerApiService : ICustomerApiService
    {
        private const string FirstName = "firstname";
        private const string LastName = "lastname";
        private const string KeyGroup = "customer";

        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;

        public CustomerApiService(IRepository<Customer> customerRepository, IRepository<GenericAttribute> genericAttributeRepository)
        {
            _customerRepository = customerRepository;
            _genericAttributeRepository = genericAttributeRepository;
        }

        public IList<CustomerDto> GetCustomersDtos(string createdAtMin = "", string createdAtMax = "", int limit = Configurations.DefaultLimit,
            int page = 1, int sinceId = 0)
        {
            var query = GetCustomersQuery(createdAtMin, createdAtMax, sinceId);

            // TODO: Check why this does not return some customers i.e Guests
            IList<CustomerDto> result = HandleCustomerGenericAttributes(null, query, limit, page);

            return result;
        }

        public int GetCustomersCount()
        {
            //TODO: Should we return all customers including Guests?
            // Maybe we can have a filter by customer roles
            return _customerRepository.Table.Count();
        }

        // Need to work with dto object so we can map the first and last name from generic attributes table.
        public IList<CustomerDto> Search(Dictionary<string, string> searchParams = null, string order = "Id", int page = 1, int limit = Configurations.DefaultLimit)
        {
            IList<CustomerDto> result = new List<CustomerDto>();

            if (searchParams != null)
            {
                IQueryable<Customer> query = _customerRepository.Table;

                foreach (var searchParam in searchParams)
                {
                    // Skip non existing properties.
                    if (ReflectionHelper.HasProperty(searchParam.Key, typeof(Customer)))
                    {
                        // @0 is a placeholder used by dynamic linq and it is used to prevent possible sql injections.
                        query = query.Where(string.Format("{0} = @0 || {0}.Contains(@0)", searchParam.Key), searchParam.Value);
                    }
                    // The code bellow will search in customer addresses as well.
                    //else if (HasProperty(searchParam.Key, typeof(Address)))
                    //{
                    //    query = query.Where(string.Format("Addresses.Where({0} == @0).Any()", searchParam.Key), searchParam.Value);
                    //}
                }

                result = HandleCustomerGenericAttributes(searchParams, query, limit, page, order);
            }

            return result;
        }

        /// <summary>
        /// The idea of this method is to get the first and last name from the GenericAttribute table and to set them in the CustomerDto object.
        /// </summary>
        /// <param name="searchParams">Search parameters is used to shrinc the range of results from the GenericAttibutes table 
        /// to be only those with specific search parameter (i.e. currently we focus only on first and last name).</param>
        /// <param name="query">Query parameter represents the current customer records which we will join with GenericAttributes table.</param>
        /// <returns></returns>
        private IList<CustomerDto> HandleCustomerGenericAttributes(Dictionary<string, string> searchParams, IQueryable<Customer> query, int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, string order = "Id")
        {
            // Here we join the GenericAttribute records with the customers and making sure that we are working only with the attributes
            // that are in the customers keyGroup and their keys are either first or last name.
            // We are returning a collection with customer record and attribute record. 
            // It will look something like:
            // customer data for customer 1, attribute that contains the first name of customer 1
            // customer data for customer 1, attribute that contains the last name of customer 1
            // customer data for customer 2, attribute that contains the first name of customer 2
            // customer data for customer 2, attribute that contains the last name of customer 2
            // etc.

            var customerAttributesMapping = (from attribute in _genericAttributeRepository.Table
                                             join customer in query on attribute.EntityId equals customer.Id
                                             where attribute.KeyGroup.Equals(KeyGroup, StringComparison.CurrentCultureIgnoreCase) &&
                                                   (attribute.Key.Equals(FirstName, StringComparison.CurrentCultureIgnoreCase) ||
                                                    attribute.Key.Equals(LastName, StringComparison.CurrentCultureIgnoreCase))
                                             orderby customer.Id
                                             select new CustomerAttributeMappingDto()
                                             {
                                                 Customer = customer,
                                                 Attribute = attribute
                                             });

            customerAttributesMapping = GetCustomerAttributesMappingsByKey(searchParams, customerAttributesMapping, FirstName);
            customerAttributesMapping = GetCustomerAttributesMappingsByKey(searchParams, customerAttributesMapping, LastName);

            // Since we will have two records for each customer we need to double the limit.
            int limitCustomerAttributes = limit * 2;

            IList<CustomerDto> result = GetFullCustomerDtos(customerAttributesMapping, page, limitCustomerAttributes, order);

            return result;
        }

        /// <summary>
        /// This method is responsible for getting unique customer dto records with first and last names set from the attribute mappings.
        /// </summary>
        private IList<CustomerDto> GetFullCustomerDtos(IQueryable<CustomerAttributeMappingDto> customerAttributesMapping, int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit, string order = "Id")
        {
            var uniqueMappings = new Dictionary<int, CustomerDto>();

            customerAttributesMapping = customerAttributesMapping.OrderBy("Customer." + order);

            IList<CustomerAttributeMappingDto> customerAttributesMappingsList = new ApiList<CustomerAttributeMappingDto>(customerAttributesMapping, page - 1, limit);

            foreach (var mapping in customerAttributesMappingsList)
            {
                if (!uniqueMappings.ContainsKey(mapping.Customer.Id))
                {
                    CustomerDto record = mapping.Customer.ToDto();

                    uniqueMappings.Add(mapping.Customer.Id, record);
                }

                if (mapping.Attribute.Key.Equals(FirstName, StringComparison.CurrentCultureIgnoreCase))
                {
                    uniqueMappings[mapping.Customer.Id].FirstName = mapping.Attribute.Value;
                }
                else if (mapping.Attribute.Key.Equals(LastName, StringComparison.CurrentCultureIgnoreCase))
                {
                    uniqueMappings[mapping.Customer.Id].LastName = mapping.Attribute.Value;
                }
            }

            return uniqueMappings.Values.ToList();
        }

        private IQueryable<CustomerAttributeMappingDto> GetCustomerAttributesMappingsByKey(Dictionary<string, string> searchParams,
            IQueryable<CustomerAttributeMappingDto> customerAttributesMapping, string key)
        {
            if (searchParams != null && searchParams.ContainsKey(key))
            {
                // We are saving the value from search parameters in variable, because we can not use indexers in Linq.
                string searchParamValue = searchParams[key];

                // Here we filter the customerAttributeMappings to be only the ones that have the passed key parameter as a key.
                var customerAttributesMappingByKey = from mapping in customerAttributesMapping
                                                     where mapping.Attribute.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase) &&
                                                           mapping.Attribute.Value.Equals(searchParamValue, StringComparison.InvariantCultureIgnoreCase)
                                                     select mapping;

                // We need to join the customerAttributesMapping with the collection above so we do not skip the first/last name mappings (depends on the key).
                // Otherwise we will have customers with only the first or last name populated. 
                customerAttributesMapping = from mapping in customerAttributesMapping
                                            join map in customerAttributesMappingByKey on mapping.Attribute.EntityId equals
                                                map.Attribute.EntityId
                                            select mapping;
            }

            return customerAttributesMapping;
        }

        private IQueryable<Customer> GetCustomersQuery(string createdAtMin = "", string createdAtMax = "", int sinceId = 0)
        {
            var query = _customerRepository.Table;
            if (!string.IsNullOrEmpty(createdAtMin))
            {
                var createAtMin = DateTime.Parse(createdAtMin).ToUniversalTime();
                query = query.Where(c => c.CreatedOnUtc > createAtMin);
            }

            if (!string.IsNullOrEmpty(createdAtMax))
            {
                var createAtMax = DateTime.Parse(createdAtMax).ToUniversalTime();
                query = query.Where(c => c.CreatedOnUtc < createAtMax);
            }

            query = query.OrderBy(customer => customer.Id);

            if (sinceId > 0)
            {
                query = query.Where(customer => customer.Id > sinceId);
            }

            return query;
        }

        public CustomerDto GetCustomerById(int id)
        {
            if (id == 0)
                return null;

            Customer customer = _customerRepository.GetById(id);
            if (customer != null)
            {
               return customer.ToDto();
            }

            return null;
        }
    }
}