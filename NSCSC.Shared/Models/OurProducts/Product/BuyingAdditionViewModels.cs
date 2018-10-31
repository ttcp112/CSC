using NSCSC.Shared.Models.ClientSite.OurProDucts;
using System.Collections.Generic;

namespace NSCSC.Shared.Models.OurProducts.Product
{
    public class BuyingAdditionViewModels
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<OurProDuctsModel> ListItem { get; set; }

        public BuyingAdditionViewModels()
        {
            ListItem = new List<OurProDuctsModel>();
        }
    }
}
