using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.Sandbox.Employee
{
    public class EmployeeRequest : RequestBaseModels
    {

        public int Level { get; set; }
        public EmployeeDetailModels EmployeeDetail { get; set; }
        public List<string> ListID { get; set; }

        public EmployeeRequest()
        {
            ListID = new List<string>();
        }

        public class EmployeeChangePasswordRequest : RequestBaseModels
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

        public class EmployeeForgotPasswordRequest
        {
            public string Email { get; set; }
        }

        public class DeleteEmployeeRequest : RequestBaseModels
        {

        }
    }
}
