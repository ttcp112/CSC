using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSCSC.Shared.Models.OurProducts;

namespace NSCSC.Shared.Models.OverView
{
    public class OverViewModels
    {
        public List<PromotionDetailModels> ListPromotions { get; set; }
        public bool IsIpSingapore { get; set; }
        public string LeftTitle { get; set; }
        public string LeftDescription { get; set; }
        public string BusinessTitle { get; set; }
        public string BusinessDescription { get; set; }

        public OverViewModels()
        {
            ListPromotions = new List<PromotionDetailModels>();
            //Check session
        }
       

    }
}
