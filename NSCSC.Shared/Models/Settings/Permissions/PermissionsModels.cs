using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.Settings.Permissions
{
    public class PermissionsModels
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEdit { get; set; }
        public bool IsActive { get; set; }
    }

    public class PermissionsViewModels
    {
        public List<PermissionsModels> ListItem { get; set; }
    }
}
