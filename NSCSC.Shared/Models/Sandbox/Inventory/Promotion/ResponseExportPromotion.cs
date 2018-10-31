using NSCSC.Shared.Models.Sandbox.Inventory.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models
{
    public class ResponseExportPromotion : NSApiResponseBase
    {
        public List<PromotionModels> ListPromo { get; set; }
    }
   
}
