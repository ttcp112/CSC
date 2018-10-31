using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.Sandbox.Employee
{
    public class EmployeeModels
    {
        public string ID { get; set; }
        public string ImageURL { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int RoleLevel { get; set; }
        public bool IsActive { get; set; }

        /*For Chat*/
        /*
            = 0 : Block
            = 1 : Online
            = 2 : Offline
        */
        public int Status { get; set; }

        public int ReceivedMessages { get; set; }
    }
    public class EmployeeViewModels
    {
        public string SearchString { get; set; }
        public List<EmployeeModels> ListItem { get; set; }
        public EmployeeViewModels()
        {
            ListItem = new List<EmployeeModels>();
        }
    }
}
