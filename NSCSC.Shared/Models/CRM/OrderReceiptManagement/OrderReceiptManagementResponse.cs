using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.CRM.OrderReceiptManagement
{
    public class OrderReceiptManagementResponse
    {
    }
    public class ResponseGetListReceipt : NSApiResponseBase
    {
        public List<ReceiptDTO> ListReceipt { get; set; }
    }

    public class ResponseGetReceiptDetail : NSApiResponseBase
    {
        public ReceiptDetailDTO ReceiptDetail { get; set; }
    }

    public class ReceiptDTO
    {
        public string ID { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int ReceiptStatus { get; set; }
    }
    public class ReceiptDetailDTO
    {
        public string ID { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string errorItem { get; set; }
        public List<HardwareServiceDTO> ListAdditionItem { get; set; }
        public List<SerialNumberDTO> ListSerialNo { get; set; }
        public ReceiptDetailDTO()
        {
            ListAdditionItem = new List<HardwareServiceDTO>();
            ListSerialNo = new List<SerialNumberDTO>();
        }
    }

    public class HardwareServiceDTO
    {
        public string AssetID { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public int AdditionType { get; set; }
        public string AdditionName { get; set; }
        public DateTime? BoughtTime { get; set; }
        public string SerialNo { get; set; }
        public string SerialNoView { get; set; }
        public string ReceiptNo { get; set; }
        public int State { get; set; }
        public int StateChange { get; set; }
        public string StatusName { get; set; }
        public int Update { get; set; }
        public int Offset { get; set; }
    }
    public class SerialNumberDTO
    {
        public string AssetID { get; set; }
        public string SerialNo { get; set; }
        public int State { get; set; }
    }
}
