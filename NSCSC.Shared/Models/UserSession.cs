using NSCSC.Shared.Models.SandBox.Inventory.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models
{
    public class UserSession
    {
        public bool IsAuthenticated { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public string PinCode { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string IntegrationNuwebLink { get; set; }
        public string IntegrationPozzwebLink { get; set; }

        //public string MerchantName { get; set; }       
        //public List<StoreWebDTO> ListStores { get; set; }
        public string StoreId { get; set; } //
       
        public List<string> ListStoreID { get; set; }// written by 4/3/2017
        public List<int> ListIndustry { get; set; }
        public List<PermissionDTO> ListPermission { get; set; }
        public string CurrencySymbol { get; set; }
        public string QRCodeSizeId { get; set; }
        public bool IsIntergrate { get; set; }
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public int RoleLevel { get; set; }

        public string PublicImages { get; set; }
        public string FTPWebImage { get; set; }
        public string FTPUser { get; set; }
        public string FTPPassword { get; set; }

        //Edit
        public string OrderID { get; set; }

        //Update Trongntn
        public bool IsReseller { get; set; }

        //Update 04/09/2018
        public List<ProductDetailModels> ListProduct { get; set; }

        public UserSession()
        {
            //ListStores = new List<StoreWebDTO>();
            QRCodeSizeId = "2";
        }
    }
}
