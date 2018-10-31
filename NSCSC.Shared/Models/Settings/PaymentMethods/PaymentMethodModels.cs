using NSCSC.Shared.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NSCSC.Shared.Models.Settings.PaymentMethods
{
    public class PaymentMethodModels
    {
        public string ID { get; set; }


        [DataType(DataType.Upload)]
        //[FileSize(10072000)]
        //[FileSize(300000)]
        [FileTypes("jpeg,jpg,png")]
        public HttpPostedFileBase PictureUpload { get; set; }

        public byte[] PictureByte { get; set; }
        public string ImageURL { get; set; }
        public string ParentID { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string IntegrationCode { get; set; }
        public int Code { get; set; }
        public string ErrorSubPayment { get; set; }
        public List<PaymentMethodModels> ListChild { get; set; }

        public int OffSet { get; set; }
        public byte Status { get; set; }
        public bool IsDefault { get; set; }
        public int Payment { get; set; }
        public PaymentMethodModels()
        {
            ListChild = new List<PaymentMethodModels>();
            IsActive = true;
        }
    }
}
