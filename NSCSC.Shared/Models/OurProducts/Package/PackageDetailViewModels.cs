using NSCSC.Shared.Models.ClientSite.OurProDucts;
using System.Collections.Generic;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.OurProducts.Package
{
    public class PackageDetailViewModels
    {
        public string CurrencySymbol { get; set; }

        public OurProDuctsModel PackageDetail { get; set; }

        // For apply a addition for a product in package
        public List<ProductApplyAdditionPackage> ListApplyAdditionProduct { get; set; }
        public List<OurProDuctsModel> ListAdditionBuy { get; set; }

        public PackageDetailViewModels()
        {
            ListApplyAdditionProduct = new List<ProductApplyAdditionPackage>();
            ListAdditionBuy = new List<OurProDuctsModel>();
        }
    }

    public class ProductApplyAdditionPackage
    {
        public string ProductName { get; set; }
        public string ProductId { get; set; }
        public string ProductCateId { get; set; }
    }
}
