using NSCSC.Shared.Models.ClientSite.MyProfile;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NSCSC.Shared.Models.SandBox.Inventory.Product
{
    public class ProductCompositeModels
    {
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductType { get; set; }
        public int AdditionType { get; set; }
        //[Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to 1.")]
        public int Quantity { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to 0.")]
        public int Sequence { get; set; }
        public int AmountOfUnit { get; set; }
        public string sAmountOfUnit { get; set; }
        public List<ItemFunction> ListFunction { get; set; }
        public string ImageURL { get; set; }
        public List<ProductCategoryModels> ListCategory { get; set; }
        /**** Client ****/
        public string CategoryName { get; set; }
        public string TypeName { get; set; }
        public bool IsUnlimited { get; set; }
        public int OffSet { get; set; }
        public int Status { get; set; }
        public bool IsSelect { get; set; }
        public int Type { get; set; }
        public ProductCompositeModels()
        {
            Quantity = 1;
            Sequence = 0;
            ListFunction = new List<ItemFunction>();
            ListCategory = new List<ProductCategoryModels>();
        }
    }
}
