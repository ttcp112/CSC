using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.CRM.OrderReceiptManagement
{
    public class OrderReceiptManagementRequest
    {
    }
    public class GetListReceiptRequest : RequestBaseModels { }
    public class GetReceiptDetailRequest : RequestBaseModels { }
    public class AddOrRemoveDeviceSerialNoRequest : RequestBaseModels
    {
        public List<SerialNumberDTO> ListSerialNo { get; set; }
    }    
}
