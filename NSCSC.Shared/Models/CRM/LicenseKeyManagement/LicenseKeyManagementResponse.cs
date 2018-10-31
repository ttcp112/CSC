using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.CRM.LicenseKeyManagement
{
    public class LicenseKeyManagementResponse
    {
    }

    public class ResponseGetListLicense : NSApiResponseBase
    {
        public List<LicenseDTO> ListLicense { get; set; }
    }

    public class ResponseGetLicenseDetail : NSApiResponseBase
    {
        public LicenseDetailDTO LicenseDetail { get; set; }
    }

    public class LicenseDTO
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
        public List<string> ListFunctionName { get; set; }
    }

    public class LicenseDetailDTO
    {
        public string ID { get; set; }
        public string LicenseKey { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public string ReceiptNo { get; set; }
        public int ProductType { get; set; }
        public int AdditionType { get; set; }
        public bool IsActive { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int NumberOfDevice { get; set; }
        public int NumberOfAccount { get; set; }
        public string NameFunction { get; set; }
        public List<string> ListFunctionName { get; set; }
        public List<LicenseItemDTO> ListItem { get; set; }
        public DateTime PaidTime { get; set; }
    }

    public class LicenseItemDTO
    {
        public string ID { get; set; }
        public string ItemName { get; set; }
        public DateTime? ActiveTime { get; set; }
        public bool IsActive { get; set; }
        public int AdditionType { get; set; }
        public string StatusName { get; set; }
        public int Update { get; set; }
        public int Offset { get; set; }
    }
}
