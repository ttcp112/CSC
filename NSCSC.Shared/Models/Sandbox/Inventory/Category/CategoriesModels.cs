using NSCSC.Shared.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.Sandbox.Inventory.Category
{
    public class CategoriesModels
    {
        public string Id { get; set; }
        public int Type { get; set; }
        public string ImageURL { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        [Range(0, 99999999, ErrorMessage = "Please enter a value greater than or equal to 0.")]
        public int Sequence { get; set; }
        public int NumberOfProduct { get; set; }
        public bool IsActive { get; set; }
        public bool IsFreeTrial { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "The Short Description maximum length 255 characters")]
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        
        public List<SelectListItem> ListType { get; set; }       
        // upload image
        public byte[] PictureByte { get; set; }
        [DataType(DataType.Upload)]
        public HttpPostedFileBase PictureUpload { get; set; }
        // upload file excel
        [DataType(DataType.Upload)]
        //[FileSize(3072000)]
        [FileTypes("xls,xlsx")]
        public HttpPostedFileBase ExcelUpload { get; set; }
        public string VideoURL { get; set; }
        public bool IsConfirm { get; set; }
        public bool IsPopup { get; set; }
        public CategoriesModels()
        {
            ListType = new List<SelectListItem>()
            {
                new SelectListItem() { Text = Commons.EType.NuKiosk.ToString(), Value =  Commons.EType.NuKiosk.ToString("d")},
                new SelectListItem() { Text = Commons.EType.NuPOS.ToString(), Value =  Commons.EType.NuPOS.ToString("d")},
                new SelectListItem() { Text = Commons.EType.POinS_Link_App.ToString(), Value =  Commons.EType.POinS_Link_App.ToString("d")},
                new SelectListItem() { Text = Commons.EType.POZZ.ToString(), Value =  Commons.EType.POZZ.ToString("d")},
                new SelectListItem() { Text = Commons.EType.POZZ_Display.ToString(), Value =  Commons.EType.POZZ_Display.ToString("d")},
                new SelectListItem() { Text = Commons.EType.POZZ_Kiosk.ToString(), Value =  Commons.EType.POZZ_Kiosk.ToString("d")},
                new SelectListItem() { Text = Commons.EType.NuDisplay.ToString(), Value =  Commons.EType.NuDisplay.ToString("d")},
            };
            ListType = ListType.OrderBy(oo => oo.Value).ToList();

        }

    }

    public class Categories
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
