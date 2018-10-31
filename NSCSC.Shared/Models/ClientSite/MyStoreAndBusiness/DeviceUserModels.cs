using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
{
    public class DeviceUserModels
    {
        public string ID { get; set; }
        public string UUID { get; set; }
        public DateTime? ActiveTime { get; set; }
        public string LicenseKey { get; set; }
        public int ProductType { get; set; }
        public bool IsActive { get; set; }
        public string ProductID { get; set; }
        public string AssetID { get; set; }
    }
}
