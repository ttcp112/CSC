using NSCSC.Shared.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.Settings.Permissions
{
    public class PermissionsDetailModels : BaseModels
    {
        public string ID { get; set; }
        [Required(ErrorMessage = "Name field is required")]
        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public List<PermissionDTO> ListPermission { get; set; }

        public PermissionsDetailModels()
        {

        }
    }
}
