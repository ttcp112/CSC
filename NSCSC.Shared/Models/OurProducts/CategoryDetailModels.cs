using NSCSC.Shared.Factory.ClientSite.OurProducts;
using System.Collections.Generic;

namespace NSCSC.Shared.Models.OurProducts
{
    public class CategoryDetailModels
    {
        public int Index { get; set; }
        public string ID { get; set; }
        public int Type { get; set; }
        public string ImageURL { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Sequence { get; set; }
        public bool IsActive { get; set; }
        public bool IsFreeTrial { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }

        public string UrlEncrypt { get; set; }
    }
}
