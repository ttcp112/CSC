using System.Collections.Generic;

namespace NSCSC.Shared.Models.CRM.Customers.ProductManagement
{
    public class ProductManagementModels
    {
        public List<HardwareServiceModels> ListHardwareService { get; set; }
        public List<ProductPackageModels> ListProductPackage { get; set; }
        public List<AdditionFunctionModels> ListFunction { get; set; }
        public List<AccountManagementModels> ListAccount { get; set; }

        public ProductManagementModels()
        {
            ListHardwareService = new List<HardwareServiceModels>();
            ListProductPackage = new List<ProductPackageModels>();
            ListFunction = new List<AdditionFunctionModels>();
            ListAccount = new List<AccountManagementModels>();
        }
    }
}
