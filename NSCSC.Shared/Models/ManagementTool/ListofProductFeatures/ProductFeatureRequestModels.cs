using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ManagementTool.ListofProductFeatures
{
    public class ProductFeatureRequestModels : RequestBaseModels
    {
        public ProductFeatureDetailModels ProductFeatureDetail { get; set; }
        public List<ProductFeatureDetailModels> ListProductFeature { get; set; }

    }
}
