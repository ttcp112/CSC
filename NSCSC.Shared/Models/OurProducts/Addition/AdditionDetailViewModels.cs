using NSCSC.Shared.Models.ClientSite.OurProDucts;
using System.Collections.Generic;

namespace NSCSC.Shared.Models.OurProducts.Addition
{
    public class AdditionDetailViewModels
    {
        public int TypeProductApply { get; set; }
        public OurProDuctsModel AdditionDetail { get; set; }

        public List<ProductApplyAdditionModels> ListProductApply { get; set; }
        public AdditionDetailViewModels()
        {
            AdditionDetail = new OurProDuctsModel();
            ListProductApply = new List<ProductApplyAdditionModels>();
        }
    }
}
