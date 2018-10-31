using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.CRM.Customers
{
    public class StoreModels
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ImageURL { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public int StoreStatus { get; set; }
        public string StoreStatusName { get; set; }
        public string Address { get; set; }
        public int StoreType { get; set; }
        public List<ApplyProductOnStoreModels> ListApplyProduct { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public StoreModels()
        {
            ListApplyProduct = new List<ApplyProductOnStoreModels>();
        }
    }
}
