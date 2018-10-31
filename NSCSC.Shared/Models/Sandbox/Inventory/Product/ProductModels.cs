using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.Sandbox.Inventory.Product
{
    public class ProductModels
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int ProductType { get; set; }
        public int AdditionType { get; set; }
        public string CategoryName { get; set; }
        public int Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublic { get; set; }
        public double StartPrice { get; set; }
        public bool IsSelect { get; set; }
        public int? Quantity { get; set; }
        public int? Sequence { get; set; }
        public List<string> ListProductName { get; set; }
        public bool Ischeck{get;set;}
        public byte Status { get; set; }
        public int OffSet { get; set; }
        public ProductModels()
        {
            ListProductName = new List<string>();
        }
    }

    public class ProductViewModels
    {
        public string SearchString { get; set; }
        //public int ProductType { get; set; }
        public List<ProductModels> ListItem { get; set; }
        public ProductViewModels()
        {
            ListItem = new List<ProductModels>();
        }
    }
}
