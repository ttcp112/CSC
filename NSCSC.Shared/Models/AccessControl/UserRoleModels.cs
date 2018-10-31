using NSCSC.Shared.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.AccessControl
{
    public class UserRoleModels
    {
        private BaseFactory _baseFactory = null;

        public UserRoleModels()
        {
            _baseFactory = new BaseFactory();
        }
    }
    public class UserModule
    {
        public string ModuleName { get; set; }

        public string Controller { get; set; }
        public string StoreId { get; set; }
        public bool IsView { get; set; }
        public bool IsAction { get; set; }
        public bool IsActive { get; set; }
        public string RoleId { get; set; }
    }
}
