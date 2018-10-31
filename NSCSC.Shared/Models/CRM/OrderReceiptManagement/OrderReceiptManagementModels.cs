using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.CRM.OrderReceiptManagement
{
    public class OrderReceiptManagementModels
    {
        public string ID { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int ReceiptStatus { get; set; }
    }
    public class OrderReceiptManagementViewModels
    {
        public List<OrderReceiptManagementModels> ListItem { get; set; }
        public OrderReceiptManagementViewModels()
        {
            ListItem = new List<OrderReceiptManagementModels>();
        }
    }
}
