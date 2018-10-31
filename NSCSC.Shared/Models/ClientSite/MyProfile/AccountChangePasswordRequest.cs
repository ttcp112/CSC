using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ClientSite.MyProfile
{
   public class AccountChangePasswordRequest:RequestBaseModels
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
