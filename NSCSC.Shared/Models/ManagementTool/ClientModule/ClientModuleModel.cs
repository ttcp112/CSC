using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ManagementTool.ClientModule
{
    public class ClientModuleModel
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
        public bool IsActive { get; set; }
    }
    public class ClientPackageSession
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
    }
}
