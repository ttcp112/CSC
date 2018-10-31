using NSCSC.Shared.Utilities;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.CRM.Customers
{
    public class CustomerModels
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string MerchantName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public bool IsActive { get; set; }
        public int CustomerType { get; set; }
        public string CountItem { get; set; }
        public string Type { get; set; }
        public bool Parent { get; set; }
        public List<CustomerModels> ListCus { get; set; }
        public CustomerModels()
        {
            ListCus = new List<CustomerModels>();
        }
    }

    public class CustomerViewModels
    {
        public int CustomerType { get; set; }
        public List<SelectListItem> ListCusType { get; set; }
        public List<SelectListItem> ListCustomerType { get; set; }
        public List<int> ListCustomerTypeChoese { get; set; }
        public List<CustomerModels> ListItem { get; set; }

        public CustomerViewModels()
        {
            ListCustomerTypeChoese = new List<int>();
            ListItem = new List<CustomerModels>();
            ListCusType = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "All customers", Value = Commons.ECustomerType.Customer.ToString("d")},
                new SelectListItem() { Text = "Consumers", Value = Commons.ECustomerType.Consumer.ToString("d")},
            };
            ListCustomerType = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "All customers", Value = Commons.ECustomerType.Customer.ToString("d")},
                new SelectListItem() { Text = "Consumers", Value = Commons.ECustomerType.Consumer.ToString("d")},
                 new SelectListItem() { Text = "Reseller", Value = Commons.ECustomerType.Reseller.ToString("d")},
            };
        }
    }

    //////
    public class CountryModel
    {
        public string Name { get; set; }
        public string Callingcode { get; set; }
        public string Region { get; set; }
        public List<string> TimeZones { get; set; }
        public string Alpha2Code { get; set; }
        public string NumericCode { get; set; }
        public List<CurrencyModel> Currencies { get; set; }
        public CountryModel()
        {
            TimeZones = new List<string>();
            Currencies = new List<CurrencyModel>();
        }
    }

    public class CurrencyModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
    }
}
