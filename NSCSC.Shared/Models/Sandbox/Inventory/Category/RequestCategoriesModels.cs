using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.Sandbox.Inventory.Category
{
    public class RequestCategoriesModels : RequestBaseModels
    {
        public CategoriesModels CategoryDetail { get; set; }
        public List <CategoriesModels> ListCategory { get; set; }
        public bool IsConfirm { get; set; }
    }
}
