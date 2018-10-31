using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
{
    public class MyStoreAndBusinessRequest
    {
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string AssetID { get; set; }

        public MerchantModels MerchantInfo { get; set; }
        public StoreDetailModels StoreInfo { get; set; }
        public string StoreID { get; set; }
        public List<string> ListAssetID { get; set; }
    }
}
