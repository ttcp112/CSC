using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
{
    class StoreAssignProductRequest: RequestBaseModels
    {
        public List<StoreAssignProduct> ListStoreAssignProduct { get; set; }
        public bool IsAssignProductToStore { get; set; }
    }
    public class StoreAssignProduct
    {
        public string StoreID { get; set; }
        public string AssetID { get; set; }

    }
}
