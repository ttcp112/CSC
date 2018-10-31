using NSCSC.Shared.Models.ClientSite.OurProDucts;
using System.Collections.Generic;

namespace NSCSC.Shared.Models.ClientSite.OurProducts
{
    public class OurProductRequest
    {
        public byte EType { get; set; }
        public bool IsPackage { get; set; }

        //for Price
        public int ProductType { get; set; }
        public int AdditionType { get; set; }
        public string CategoryID { get; set; }
    }
    public class ProductGetRequest : RequestBaseModels
    {
        public string AdditionID { get; set; }
        public int ProductType { get; set; }
        public List<int> ListType { get; set; }
        public List<string> ListProductID { get; set; }
        public List<string> ListCateID { get; set; }
        //updated by Trongntn
        public string CustomerID { get; set; }
    }
    public class ResponseProductGetListDetail : NSApiResponseBase
    {
        public List<OurProDuctsModel> ListProduct { get; set; }
    }
    public class ResponseGetListCategory : NSApiResponseBase
    {
        public List<CategoryDTO> ListCategory { get; set; }
    }
    public class ProductGetCateRequest : RequestBaseModels
    {
        public bool IsActive { get; set; }
        public int Type { get; set; }
    }
}
