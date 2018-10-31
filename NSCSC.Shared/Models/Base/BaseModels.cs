using NSCSC.Shared.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NSCSC.Shared.Models.Base
{
    public class BaseModels
    {
        [DataType(DataType.Upload)]
        //[FileSize(10072000)]
        //[FileSize(300000)]
        [FileTypes("jpeg,jpg,png,bmp")]
        public HttpPostedFileBase PictureUpload { get; set; }

        public string ImageURL { get; set; }
        public byte[] PictureByte { get; set; }

        //========
        public string CreateUser { get; set; }
    }
}
