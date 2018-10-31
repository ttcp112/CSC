using NSCSC.Shared.Models.ClientSite.OurProDucts;
using System.Collections.Generic;

namespace NSCSC.Shared.Models.OurProducts.Package
{
    public class PackageViewModels
    {
        public List<CategoryDetailModels> ListCategories { get; set; }
        public List<OurProDuctsModel> ListProducts { get; set; }

        public PackageViewModels()
        {
            ListCategories = new List<CategoryDetailModels>();
            ListProducts = new List<OurProDuctsModel>();
        }
    }
}
