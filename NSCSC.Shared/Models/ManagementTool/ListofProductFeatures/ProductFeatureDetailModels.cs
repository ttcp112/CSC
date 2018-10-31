using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.ManagementTool.ListofProductFeatures
{
    public class ProductFeatureDetailModels
    {
        public string ID { get; set; }
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        [Required]
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public byte[] ImageURLPictureByte { get; set; }
        [DataType(DataType.Upload)]
        public HttpPostedFileBase ImageURLUpload { get; set; }
        public string IconURL { get; set; }
        public byte[] IconURLPictureByte { get; set; }
        [DataType(DataType.Upload)]
        public HttpPostedFileBase IconURLUpload { get; set; }
        [Range(0, 99999999, ErrorMessage = "Please enter a value greater than or equal to 0.")]
        public int Sequence { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public List<SelectListItem> ListCategory { get; set; }
        public ProductFeatureDetailModels()
        {
            ListCategory = new List<SelectListItem>();
        }
    }

    
}
