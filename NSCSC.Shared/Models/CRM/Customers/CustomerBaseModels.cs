using NSCSC.Shared.Models.CRM.Customers.ProductManagement;

namespace NSCSC.Shared.Models.CRM.Customers
{
    public class CustomerBaseModels
    {
        public string CurrencySymbol { get; set; }
        public CustomerDetailModels CustomerDetail { get; set; }
        public MerchantModels MerchantStore { get; set; }
        public ProductManagementModels ProductManagement { get; set; }
        public int IndexTab { get; set; }// 1 : Customer , 2 : Merchant
        public CustomerBaseModels()
        {
            CustomerDetail = new CustomerDetailModels();
            MerchantStore = new MerchantModels();
            ProductManagement = new ProductManagementModels();
            IndexTab = 1;
        }
    }
}
