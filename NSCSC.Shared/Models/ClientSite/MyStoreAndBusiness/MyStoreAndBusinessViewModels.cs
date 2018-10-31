using System.Collections.Generic;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
{
    public class MyStoreAndBusinessViewModels
    {
        public MerchantModels MerchantInfo { get; set; }
        public StoreDetailModels StoreInfo { get; set; }
        public List<HardwareModels> ListHardware { get; set; }
        public List<ServiceModels> ListService { get; set; }
        public List<AccountManagementModels> ListAccount { get; set; }
        public List<AdditionFunctionModels> ListFunction { get; set; }
        public string IsTabStore { get; set; }
        public List<StoreModels> ListStore { get; set; }
        public List<ProductApplyStoreModels> ListProductApply { get; set; }
        public int TabProductAndPackage { get; set; }
        public bool isShow { get; set; }
        public bool IsReseller { get; set; }
        public MyStoreAndBusinessViewModels()
        {
            MerchantInfo = new MerchantModels();
            StoreInfo = new StoreDetailModels();
            ListHardware = new List<HardwareModels>();
            ListService = new List<ServiceModels>();
            ListAccount = new List<AccountManagementModels>();
            ListFunction = new List<AdditionFunctionModels>();
            ListStore = new List<StoreModels>();
            ListProductApply = new List<ProductApplyStoreModels>();
            TabProductAndPackage = 1; // 1 : Material - 2 : On going -3 : expired
        }
    }

    public class MyStoreAndBusinessMerchantViewModels
    {
        public List<MerchantModels> ListMerchant { get; set; }
        public MyStoreAndBusinessMerchantViewModels()
        {
            ListMerchant = new List<MerchantModels>();
        }
    }
}
