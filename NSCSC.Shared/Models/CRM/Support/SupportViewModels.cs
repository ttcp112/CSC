using NSCSC.Shared.Models.Sandbox.Employee;
using System.Collections.Generic;

namespace NSCSC.Shared.Models.CRM.Support
{
    public class SupportViewModels
    {
        public UserSession User { get; set; }
        public List<EmployeeModels> ListEmployee { get; set; }
        public List<MessageModels> ListMessage { get; set; }
        public SupportViewModels()
        {
            User = new UserSession();
            ListEmployee = new List<EmployeeModels>();
            ListMessage = new List<MessageModels>();
        }
    }
}
