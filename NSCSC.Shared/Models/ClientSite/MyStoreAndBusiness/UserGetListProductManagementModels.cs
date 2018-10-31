using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
{
    public class UserGetListProductManagementModels
    {
        public List<HardwareModels> ListHardware { get; set; }
        public List<ServiceModels> ListService { get; set; }
        public List<AccountManagementModels> ListAccount { get; set; }
        public List<AdditionFunctionModels> ListFunction { get; set; }
        public List<ProductPackageUserModels> ListProductPackage { get; set; }

        public UserGetListProductManagementModels()
        {
            ListHardware = new List<HardwareModels>();
            ListService = new List<ServiceModels>();
            ListAccount = new List<AccountManagementModels>();
            ListFunction = new List<AdditionFunctionModels>();
            ListProductPackage = new List<ProductPackageUserModels>();
        }
    }
}
