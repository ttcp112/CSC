using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ClientSite.Account.Verify
{
    public class RequestVerify
    {
    }
    public class AccountVerifyRequest
    {
        public string Email { get; set; }
        public string VerifyCode { get; set; }
    }
}
