using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.CRM.LicenseKeyManagement
{
    public class LicenseKeyManagementModels
    {
        public string ID { get; set; }
        public string LicenseKey { get; set; }
        public string ReceiptNo { get; set; }
        public int ProductType { get; set; }
        public int AdditionType { get; set; }
        public bool IsActive { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int NumberOfDevice { get; set; }
        public int NumberOfAccount { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public List<string> ListFunctionName { get; set; }
    }
    public class LicenseKeyManagementViewModels
    {
        public List<LicenseKeyManagementModels> ListItem { get; set; }
        public LicenseKeyManagementViewModels()
        {
            ListItem = new List<LicenseKeyManagementModels>();
        }
    }
}
