using NSCSC.Shared.Models.ClientSite.MyProfile;
using System.Collections.Generic;

namespace NSCSC.Shared.Models.ClientSite.YourCart
{
    public class AddItemToOrderModels
    {
        public string CusID { get; set; }
        public string OrderID { get; set; }
        public List<Item> ListItems { get; set; }
        public string Note { get; set; }
        public bool IsFree { get; set; }
        public AddItemToOrderModels()
        {
            ListItems = new List<Item>();
        }
    }
}
