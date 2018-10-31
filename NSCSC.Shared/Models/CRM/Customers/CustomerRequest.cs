using NSCSC.Shared.Models.CRM.OrderReceiptManagement;
using System.Collections.Generic;

namespace NSCSC.Shared.Models.CRM.Customers
{
    public class CustomerRequest : RequestBaseModels
    {
        public CustomerDetailModels CustomerDetail { get; set; }
        public MerchantModels Merchant { get; set; }
        public string CustomerID { get; set; }
        public string AssetID { get; set; }
        public StoreDetailModels StoreInfo { get; set; }

        public string UUID { get; set; }

        public List<SerialNumberDTO> ListSerialNo { get; set; }
        public string StoreID { get; set; }

    }

    public class DoneRefundRequest : RequestBaseModels
    {
        public string OrderID { get; set; }
        public float TotalAmount { get; set; }
        public string Description { get; set; }
        public List<RefundDetailModels> ListDetail { get; set; }
    }

    public class ChangeStatusProductManagementRequest : RequestBaseModels
    {
        public List<string> ListAssetID { get; set; }
    }
}
