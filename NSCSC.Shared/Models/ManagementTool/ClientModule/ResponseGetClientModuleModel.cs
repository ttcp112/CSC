using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ManagementTool.ClientModule
{
    public class ResponseGetClientModuleModel:NSApiResponseBase
    {
        public List<ClientModuleModel> ListModule { get; set; }

        public ResponseGetClientModuleModel()
        {
            ListModule = new List<ClientModuleModel>();
        }
    }
}
