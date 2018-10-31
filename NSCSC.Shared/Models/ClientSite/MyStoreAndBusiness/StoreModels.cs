using System;
using System.Collections.Generic;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
{
    public class StoreModels
    {
        public string ID { get; set; }
        public string ImageURL { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int StoreStatus { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<ApplyProductOnStoreModels> ListApplyProduct { get; set; }
        public bool IsTemp { get; set; }

        public string sExpiredDate { get; set; }
        public string sStatus { get; set; }
        public string LocalHQWeb { get; set; }
        public int StoreType { get; set; }
        public bool IsAppliedProduct { get; set; }
        public DateTime? ActivatedDate { get; set; }

        public StoreModels()
        {
            ListApplyProduct = new List<ApplyProductOnStoreModels>();
        }
    }
}
