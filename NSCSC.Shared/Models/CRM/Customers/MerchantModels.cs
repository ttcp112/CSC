using NSCSC.Shared.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NSCSC.Shared.Models.CRM.Customers
{
    public class MerchantModels
    {
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string ImageURL { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$",ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }
        public string GSTRegNo { get; set; }
        public bool IsActive { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ExpiredDate { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        [Required]
        public String Country { get; set; }
        [Display(Name = "ZIP Code")]
        [Required]
        public string ZipCode { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
       
        public string ContactPhone { get; set; }
        public List<StoreModels> ListStore { get; set; }

        [DataType(DataType.Upload)]
        [FileTypes("jpeg,jpg,png,bmp")]
        public HttpPostedFileBase PictureUpload { get; set; }
        public byte[] PictureByte { get; set; }

        [Required]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid Number")]
        public string Phone { get; set; }
        public string CreatedUser { get; set; }
        public MerchantModels()
        {
            ListStore = new List<StoreModels>();
        }
    }
}
