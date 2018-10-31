using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.CRM.LicenseKeyManagement
{
    public class LicenseKeyManagementRequest
    {
    }

    public class GetListLicenseRequest : RequestBaseModels { }

    public class GetLicenseDetailRequest : RequestBaseModels { }
    public class UpdateLicenseKeyRequest : RequestBaseModels
    {
        public bool IsActive { get; set; }
        public List<LicenseItemDTO> ListItem { get; set; }
    }
    public class ChangeStatusLicenseKeyRequest : RequestBaseModels { }
}
