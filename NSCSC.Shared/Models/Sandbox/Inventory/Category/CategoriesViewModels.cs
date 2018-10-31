using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSCSC.Shared.Models.Sandbox.Inventory;

namespace NSCSC.Shared.Models.Sandbox.Inventory.Category
{
    public class CategoriesViewModels
    {
        public string  StoreId { get; set; }
        public List<CategoriesModels> ListCategory { get; set; }
        public CategoriesViewModels()
        {
            ListCategory = new List<CategoriesModels>();
        }
    }
}
