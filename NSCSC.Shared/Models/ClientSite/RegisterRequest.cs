using NSCSC.Shared.Factory;
using NSCSC.Shared.Models.CRM.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NSCSC.Shared.Models.ClientSite
{
    public class RegisterRequest
    {
        
    }
    public class AccountRegisterRequest
    {
        public CustomerDetailDTO CustomerDetail { get; set; }
        public MerchantDTO Merchant { get; set; }
        public string ResellerID { get; set; }
        public string MerchantID { get; set; }
        public string CustomerID { get; set; }
        public bool IsFree { get; set; }
        public int AppType { get; set; }
    }
    public class AccountRegisterCheckEmailRequest
    {
        public string Email { get; set; }
    }
    public class ResponseAccountRegisterModel : NSApiResponseBase
    {
        public string MerchantID { get; set; }
        public string CustomerID { get; set; }
        public bool IsSuccess { get; set; }
        public string msg { get; set; }
    }
}
