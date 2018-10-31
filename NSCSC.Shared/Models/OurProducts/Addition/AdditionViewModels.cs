using NSCSC.Shared.Models.ClientSite.OurProDucts;
using System.Collections.Generic;

namespace NSCSC.Shared.Models.OurProducts.Addition
{
    public class AdditionViewModels
    {
        public List<CategoryDetailModels> ListCategory { get; set; }
        public List<OurProDuctsModel> ListProduct { get; set; }

        public AdditionViewModels()
        {
            ListCategory = new List<CategoryDetailModels>();
            ListProduct = new List<OurProDuctsModel>();
        }
    }
}
