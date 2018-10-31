using System;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
{
    public class ServiceModels
    {
        public string AssetID { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public int AdditionType { get; set; }
        public DateTime? BoughtTime { get; set; }
        public string ReceiptNo { get; set; }
        public int State { get; set; }
        public string Status { get; set; }

        public string sBoughtTime { get; set; }
    }
}
