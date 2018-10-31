using NSCSC.Shared.Models;
using NSCSC.Shared.Models.OurProducts;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace NSCSC.Shared
{
    public static class Commons
    {
        #region LINK API FOR HOSTPOS
        //========== TOPIC =================
        public const string TopicAPICreateOrEdit = "api/admin/managementtool/topic/createoredit";
        public const string TopicAPIDelete = "api/admin/managementtool/topic/delete";
        public const string TopicAPIGetList = "api/admin/managementtool/topic/get";
        //========== END TOPIC =================

        //========== EMPLOYEE =================
        //public const string Login = "api/link/employee/login/web";
        public const string Login = "api/admin/sandbox/employee/login";
        public const string EmployeeAPIChangePass = "api/admin/sandbox/employee/change/password";
        public const string EmployeeAPICreateOrEdit = "api/admin/sandbox/employee/createoredit";
        public const string EmployeeAPIDelete = "api/admin/sandbox/employee/delete";
        public const string EmployeeAPIGetList = "api/admin/sandbox/employee/get/list";
        public const string EmployeeAPIGetDetail = "api/admin/sandbox/employee/get/detail";
        public const string EmployeeAPIExport = "api/admin/sandbox/employee/export";
        public const string ChangePassword = "api/admin/sandbox/employee/password/change";
        public const string ForgotPassword = "api/admin/sandbox/employee/password/forgot";
        //========== END EMPLOYEE =================

        //------------User Client-------------
        public const string LoginClient = "api/user/account/login";
        public const string ForgotClient = "api/user/account/password/forgot";
        public const string ChangeClient = "api/user/account/password/change";
        //-------------end ------------------------

        //========== DISCOUNT =================
        public const string DiscountAPICreateOrEdit = "api/admin/sandbox/inventory/discount/createoredit";
        public const string DiscountAPIDelete = "api/admin/sandbox/inventory/discount/delete";
        public const string DiscountAPIGetList = "api/admin/sandbox/inventory/discount/get/list";
        public const string DiscountAPIGetDetail = "api/admin/sandbox/inventory/discount/get/detail";
        public const string DiscountAPIGenerate = "api/admin/sandbox/inventory/discount/code/generate";
        public const string DiscountAPIBlockOrUnblock = "api/admin/sandbox/inventory/discount/code/blockorunblock";
        public const string DiscountAPIExport = "api/admin/sandbox/inventory/discount/export";
        //========== END DISCOUNT =================
        //==========   Prefix Settings ============
        public const string PrefixSettingsAPIEdit = "api/admin/setting/update";
        public const string PrefixSettingsAPIGetList = "api/admin/setting/get";

        //========== TAX =================
        public const string TaxAPICreateOrEdit = "api/admin/setting/tax/createoredit";
        public const string TaxAPIDelete = "api/admin/setting/tax/delete";
        public const string TaxAPIGetList = "api/admin/setting/tax/get";
        //========== END TAX =================

        //========== CURRENCY =================
        public const string CurrencyAPICreateOrEdit = "api/admin/setting/currency/createoredit";
        public const string CurrencyAPIDelete = "api/admin/setting/currency/delete";
        public const string CurrencyAPIGetList = "api/admin/setting/currency/get";
        //========== END CURRENCY =================

        //========== CATEGORY =================
        public const string CategoryAPICreateOrEdit = "api/admin/sandbox/inventory/category/createoredit";
        public const string CategoryAPIDelete = "api/admin/sandbox/inventory/category/delete";
        public const string CategoryAPIGetList = "api/admin/sandbox/inventory/category/get/list";
        public const string CategoryAPIGetDetail = "api/admin/sandbox/inventory/category/get/detail";
        public const string CategoryAPIImport = "api/admin/sandbox/inventory/category/import";
        public const string CategoryAPIExport = "api/admin/sandbox/inventory/category/export";
        //========== END CATEGORY =================

        //========== PRODUCT =================
        public const string ProductAPICreateOrEdit = "api/admin/sandbox/inventory/product/createoredit";
        public const string ProductAPIDelete = "api/admin/sandbox/inventory/product/delete";
        public const string ProductAPIGetList = "api/admin/sandbox/inventory/product/get/list";
        public const string ProductAPIGetDetail = "api/admin/sandbox/inventory/product/get/detail";
        public const string ProductAPIImport = "api/admin/sandbox/inventory/category/import";
        public const string ProductAPIExport = "api/admin/sandbox/inventory/category/export";
        public const string ProductAPIGetFunction = "api/admin/sandbox/inventory/product/get/function";
        //========== END PRODUCT =================

        //========== FEEDBACK =================
        public const string FeedbackAPICreateOrEdit = "api/admin/managementtool/feedback/createoredit";
        public const string FeedbackAPIDelete = "api/admin/managementtool/feedback/delete";
        public const string FeedbackAPIGetList = "api/admin/managementtool/feedback/get";
        public const string FeedbackAPIExport = "api/admin/managementtool/feedback/export";
        //========== END FEEDBACK =================

        //========== PRODUCTFEATURE =================
        public const string ProductFeatureAPICreateOrEdit = "api/admin/managementtool/feature/createoredit";
        public const string ProductFeatureAPIDelete = "api/admin/managementtool/feature/delete";
        public const string ProductFeatureAPIGetList = "api/admin/managementtool/feature/get/list";
        public const string ProductFeatureAPIGetDetail = "api/admin/managementtool/feature/get/detail";
        public const string ProductFeatureAPIImport = "api/admin/managementtool/feature/import";
        public const string ProductFeatureAPIExport = "api/admin/managementtool/feature/export";
        public const string ProductFeatureAPIAction = "api/admin/managementtool/feature/action";
        //========== END PRODUCTFEATURE =================

        //========== FAQ =================
        public const string FAQAPICreateOrEdit = "api/admin/managementtool/faq/createoredit";
        public const string FAQAPIDelete = "api/admin/managementtool/faq/delete";
        public const string FAQAPIGetList = "api/admin/managementtool/faq/get";
        //========== END FAQ =================

        //========== Customer/Receipt =================
        public const string CustomerReceiptGetDetail = "api/admin/crm/customer/receipt/get/detail";
        public const string CustomerReceiptGetList = "api/admin/crm/customer/receipt/get/list";
        public const string CustomerAssetAddserialnumber = "api/admin/crm/customer/asset/serialnumber/add";
        //========== END Customer/Receipt =================

        //========== Customer/License =================
        public const string CustomerLicenseGetDetail = "api/admin/crm/customer/license/get/detail";
        public const string CustomerLicenseGetList = "api/admin/crm/customer/license/get/list";
        public const string CustomerLicenseUpdate = "api/admin/crm/customer/license/update";
        public const string CustomerLicenseChangeStatus = "api/admin/crm/customer/license/change/status";
        //========== END Customer/License =================

        //========== PERMISSION =================
        public const string PermissionAPICreateOrEdit = "api/admin/setting/role/createoredit";
        public const string PermissionAPIDelete = "api/admin/setting/role/delete";
        public const string PermissionAPIGetList = "api/admin/setting/role/get/list";
        public const string PermissionAPIGetDetail = "api/admin/setting/role/get/detail";
        public const string PermissionAPIGet = "api/admin/setting/permission/get";
        //========== END PERMISSION =================

        //=======================================================
        //Promotion
        //=======================================================
        public const string PromotionAPIGetList = "api/admin/sandbox/inventory/promotion/get/list";
        public const string PromotionAPIGetDetail = "api/admin/sandbox/inventory/promotion/get/detail";
        public const string PromotionAPICreateOrEdit = "api/admin/sandbox/inventory/promotion/createoredit";
        public const string PromotionAPIDelete = "api/admin/sandbox/inventory/promotion/delete";
        //================CRM=====================
        public const string CRMAPIGetList = "api/admin/crm/rnc/get";
        public const string CRMAPIShowHide = "api/admin/crm/rnc/action";
        public const string CRMAPIDelete = "api/admin/crm/rnc/delete";


        //========== CUSTOMER =================
        public const string customerAPICreateOrEdit = "api/admin/crm/customer/createoredit";
        public const string customerAPIDelete = "api/admin/crm/customer/delete";
        public const string customerAPIGetList = "api/admin/crm/customer/get/list";
        public const string customerAPIGetDetail = "api/admin/crm/customer/get/detail";

        public const string customerAPIMerchantCreateOrEdit = "api/admin/crm/customer/merchant/createoredit";
        public const string customerAPIMerchantGet = "api/admin/crm/customer/merchant/get";
        public const string customerAPICreateStore = "api/admin/crm/customer/store/create";
        public const string customerAPIEditStore = "api/admin/crm/customer/store/edit";
        public const string customerAPIStoreGet = "api/admin/crm/customer/store/get";
        public const string customerAPIStoreCreateOrEdit = "api/admin/crm/customer/store/createoredit";
        public const string customerAPIStoreDelete = "api/admin/crm/customer/store/delete";
        public const string customerAPIorderAdd = "api/admin/crm/customer/order/add";

        public const string customerAPIAssetAddDevice = "api/admin/crm/customer/asset/add/device";

        public const string customerAPIProductsManagement = "api/admin/crm/customer/asset/get/list/productmanagement";
        public const string customerAPIProductsManagementChangeStatusHardwareService = "api/admin/crm/customer/asset/change/status/hardwareservice";
        public const string customerAPIProductsManagementChangeStatusProductPackage = "api/admin/crm/customer/asset/change/status/productpackage";
        public const string customerAPIProductsManagementChangeStatusAccount = "api/admin/crm/customer/asset/change/status/account";
        public const string customerAPIProductsManagementChangeStatusFunction = "api/admin/crm/customer/asset/change/status/function";
        public const string customerAPIProductsManagementChangeBlockFunction = "api/admin/crm/customer/asset/change/block/function";
        public const string customerAPIProductPackage = "api/admin/crm/customer/asset/get/productpackage";
        //========== END CUSTOMER =================

        //============================================ Payment Method ===========================
        public const string paymentMethodAPICreateOrEdit = "api/admin/setting/paymentmethod/createoredit";
        public const string paymentMethodAPIGet = "api/admin/setting/paymentmethod/get";
        public const string paymentMethodAPIDelete = "api/admin/setting/paymentmethod/delete";

        #endregion
        #region API for Account
        public const string ClientSideToRegister = "api/user/account/register";
        public const string ClientSideAccountToVerify = "api/user/account/verify";
        public const string ClientSideAccountToResendEmail = "api/user/account/resendmail";
        public const string ClientSideAccountCheckEmail = "api/user/account/register/check/email";
        #endregion
        #region API for CSC Customer Site

        //============= My Store and Business
        public const string ClientSiteStoreBusinessMerchantGetList = "api/user/merchant/get/list";
        public const string ClientSiteStoreBusinessMerchantGetInfo = "api/user/merchant/get/info";
        public const string ClientSiteStoreBusinessMerchantUpdateInfo = "api/user/merchant/edit/info";
        public const string ClientSiteStoreBusinessStoreGetList = "api/user/store/get/list";
        public const string ClientSiteStoreBusinessStoreGetInfo = "api/user/store/get/info";
        public const string ClientSiteStoreBusinessStoreUpdateInfo = "api/user/store/edit/info";
        public const string ClientSiteStoreBusinessStoreCreate = "api/user/store/add";
        public const string ClientSiteStoreBusinessStoreAssignProduct = "api/user/store/assignproduct";



        public const string ClientSiteStoreBusinessPriceList = "api/user/product/get/pricelist";

        public const string ClientSiteStoreBusinessHardwareServiceAccountAddition = "api/user/management/get/list/product";
        public const string ClientSiteStoreBusinessGetProductPackageDetail = "api/user/management/get/productpackage";
        public const string ClientSiteStoreBusinessChangeStatusDevice = "api/user/management/change/status/device";
        public const string ClientSiteStoreBusinessChangeStatusFunction = "api/user/management/change/status/function";
        public const string ClientSiteStoreBusinessChangeStatusAccount = "api/user/management/change/status/account";
        public const string ClientSiteStoreBusinessGetListProductApplyStore = "api/user/product/get/applystore";
        //============= End My Store and Business

        //============= Your Cart - Order
        public const string ClientSiteYourCartAddItems = "api/admin/crm/customer/order/add/items";
        public const string ClientSiteYourCartGetOrderDetail = "api/admin/crm/customer/order/get";
        public const string ClientSiteYourCartOrderPay = "api/admin/crm/customer/order/pay";
        public const string ClientSiteYourCartOrderCancelPay = "api/admin/crm/customer/order/cancelpay";
        public const string ClientSiteYourCartOrderDonePay = "api/admin/crm/customer/order/donepay";
        public const string ClientSiteYourCartDeleteOrder = "api/admin/crm/customer/order/delete";
        public const string ClientSiteYourCartCouponCode = "api/admin/sandbox/inventory/discount/code/check";
        public const string ClientSiteInputStoreCreateStoreTemp = "api/user/store/add/temp";
        public const string CRMCustomerDoneRefund = "api/admin/crm/customer/donerefund";
        public const string CRMCustomerOrderMerge = "api/admin/crm/customer/order/merge";
        //============= End Your Cart


        //============= My Profile
        public const string ClientSideMyProfileGetInfo = "api/user/account/get/info";
        public const string ClientSideMyProfileEdit = "api/user/account/edit/info";
        public const string ClientSideMyProfileChangePassword = "api/user/account/password/change";
        public const string PayOrder = "api/admin/crm/customer/order/pay";

        //============= End My Profile

        #region "OurProduct"
        public const string ClientSideOurProductGetPromo = "api/user/ourproduct/get/promotion";
        public const string ClientSideOurProductGet = "api/user/ourproduct/get";
        public const string ClientSideOurProductGetCate = "api/user/ourproduct/get/cate";

        public const string ClientSiteOurProductGetPriceList = "api/user/ourproduct/get/pricelist";
        public const string ClientSiteOurProductGetProductApplyAddition = "api/user/product/get/applyaddition";
        #endregion
        //========================Manager client site============================
        public const string ClientModuleSetting = "api/admin/setting/clientmodule/get";
        #endregion End API for CSC Customer Site

        #region VARIABLE FOR PAGE
        public static string _MsgDoesNotMatchFileExcel = "File import does not match file excel template. Please check file excel.";
        public static string _MsgAllowedSizeImg = "Your Photo is too large, maximum allowed size is : 300KB";
        public static int _MaxSizeFileUploadImg = 300000;

        public const string NoExpiryDate = "No expiry date";
        public const string MaritalStatusSingle = "Single";
        public const string MaritalStatusMarried = "Married";

        public const string GenderstatusMale = "Male";
        public const string GenderstatusFemale = "Female";

        public const string Image100_50 = "https://dummyimage.com/100x50";
        public const string Image100_100 = "https://dummyimage.com/100x100";
        public const string Image200_100 = "https://dummyimage.com/200x100";
        public const string Image200_200 = "https://dummyimage.com/200x200";
       
        public static DateTime MinDate = new DateTime(1900, 01, 01, 00, 00, 00, DateTimeKind.Utc);
        public static DateTime MaxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        public static string NeverDate = "30/12/9999";
        public static string FTPHost
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["User"] != null)
                    return ((UserSession)System.Web.HttpContext.Current.Session["User"]).FTPWebImage + "/";
                else
                    return "";
            }
        }
        public static string FTPUser
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["User"] != null)
                    return ((UserSession)System.Web.HttpContext.Current.Session["User"]).FTPUser;
                else
                    return "";
            }
        }
        public static string FTPPassword
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["User"] != null)
                    return ((UserSession)System.Web.HttpContext.Current.Session["User"]).FTPPassword;
                else
                    return "";
            }
        }
        public static string PublicImages
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["User"] != null)
                    return ((UserSession)System.Web.HttpContext.Current.Session["User"]).PublicImages;
                else
                    return "";
            }
        }
        // currency symbol

        public const string FormatDate = "dd/MM/yyyy";
        public const string FormatTime = "HH:mm";
        //public const string FormatDateTime = "dd/MM/yyy - HH:mm";
        // Format DateTime for Csc Client
        public const string FormatDateTime = "dd/MM/yyyy - hh:mm tt";

        // corlor for Export
        public const string BgColorHeader = "#d9d9d9";
        public const string BgColorDataRow = "#d9d9d9";
        public const string BgColorStore = "#eeece1";

        public const string TierFromLastTransaction = "From Last Transaction";
        public const string TierAfterReachThisTier = "After Reach This Tier";

        public const string PeriodTypeNone = "None";//0
        public const string PeriodTypeDay = "Day";//1
        public const string PeriodTypeMonth = "Month";//2
        public const string PeriodTypeYear = "Year";//3
        public const string PeriodTypeOneTime = "One Time";//4

        public const string ValueTypePercent = "Percent";
        public const string ValueTypeCurrency = "Value";

        public const string DiscountApplyTypeTotal = "Total order";
        public const string DiscountApplyTypeItem = "Specific items";

        public static string EarnTypeSpentItem = "Spent Item"; //2
        public static string EarnTypeSpecificItem = "Specific Item";//3
        public static string EarnTypeTotalBill = "Total Bill";//4

        public static string DiscountPercent = "Discount %";
        public static string DiscountValue = "Discount Value";

        public static string SpendTypeBuyItem = "Buy Item"; //1
        public static string SpendTypeSpendMoney = "Spend Money"; //2

        public static string SpendOnTypeAnyItem = "Any Item"; //1
        public static string SpendOnTypeSpecificItem = "Specific Item";//2

        public static string DiscountPeriodTypeNever = "Never";
        public static string DiscountPeriodTypeTime = "Period of time (months)";
        public static string DiscountPeriodTypeDay = "Specific day";

        public static string FunctionProductTypeNuPOS = "NuPOS";
        public static string FunctionProductTypeNuKiosk = "NuKiosk";
        public static string FunctionProductTypeNuDisplay = "NuDisplay";
        public static string FunctionProductTypePOZZ = "POZZ";
        public static string FunctionProductTypePOZZKiosk = "POZZ Kiosk";
        public static string FunctionProductTypePOZZDisplay = "POZZ Display";
        public static string FunctionProductTypePOinSLinkApp = "POinS Link App";

        public static string TypeTaxAddOn = "Add On";
        public static string TypeTaxInclusive = "Inclusive";

        public static string StoreTypeFnB = "FnB";
        public static string StoreTypeBeauty = "Retailer & Beauty";

        public static string MsgErrorForClient = "System or network error. Unable to add this item to your cart. Please try again.";

        // Receipt Status
        public const string ReceiptStatusNew = "New";
        public const string ReceiptStatusOnProgress = "On Progress";
        public const string ReceiptStatusCompleted = "Completed";
        public const string ReceiptStatusRecalled = "Recalled";

        public const string KEYENCRYPTANDDECRYPT = "NewsteadCSC@#2018";

        #endregion VARIABLE FOR PAGE

        #region Messages
        public static string ErrorMsg = "Unable to create/edit at  this time. Please check system setup or network connection.";
        public const string ErrorMsgFilterImage = "Invalid file type. Only the following types jpeg, jpg, png are supported.";
        public static string[] ImageExtendFiler = new string[]
                    {
                        "image/jpeg",
                        "image/jpg",
                        "image/png"
                    };
        #endregion

        #region ENUM FOR PAGE

        public enum EStoreStatus
        {
            None = 0,
            InUse = 1,
            Expired = 2,
            Temporary = 3,
        }

        public enum EDiscountPeriodType
        {
            Never = 0,
            Time = 1,
            Day = 2,
        }

        public enum EModuleCode
        {
            Sandbox = 100,
            Sand_Employees = 110,
            Sand_Inventory = 120,
            Sand_Inv_Categories = 121,
            Sand_Inv_Products = 122,
            Sand_Inv_Packages = 123,
            Sand_Inv_Additions = 124,
            Sand_Inv_Promotions = 125,
            Sand_Inv_Discount = 126,
            CRM = 200,
            CRM_Customers = 210,
            CRM_OrderReceiptMana = 220,
            CRM_LiscenseKeyMana = 230,
            Setting = 300,
            Set_Tax = 310,
            Set_Prefix = 320,
            Set_Permission = 330,
            Set_PaymentMethod = 340,
            Set_Currency = 350,
            ManagementTool = 400,
            Mana_ListFeature = 410,
            Mana_Feedback = 420,
            Mana_FAQ = 430,
            Mana_Report = 440,
            Mana_ClientSite = 450
        }

        public enum EAdditionType
        {
            Hardware = 1,
            Software = 2,
            Service = 3,
            Location = 4,
            Account = 5,
            Function = 6,
        }

        public enum EProductType
        {
            Product = 1,
            Addition = 2,
            //3
            Package = 4,
            Discount = 5,
            Promotion = 6,
            //7
            //8
            //9
        }

        public enum EFeedbackType
        {
            Support = 1,
            Purchase = 2
        }

        public enum EPeriodType
        {
            None = 0,
            Day = 1,
            Month = 2,
            Year = 3,
            OneTime = 4,
        }

        public enum EType
        {
            //NuKiosk = 1,
            //POZZ = 2,
            //POZZ_Kiosk = 3,
            //POinS_Link_App = 4,
            //NuPOS_Display = 5,
            //POZZ_Display = 6,
            NuPOS = 1,
            NuKiosk = 2,
            NuDisplay = 3,
            POZZ = 4,
            POZZ_Kiosk = 5,
            POZZ_Display = 6,
            POinS_Link_App = 7,
            Web_Nupos = 8,
            Web_PoZZ = 9,
            Web_PoinS = 10
        }


        public enum EStoreType
        {
            FnB = 1,
            Beauty = 2,
        }

        public enum EValueType
        {
            Percent = 0,
            Currency = 1,
        }

        public enum EDiscountApplyType
        {
            Total = 0,
            Item = 1,
        }
        public enum EStatus
        {
            Actived = 1,
            Deleted = 9,
            Refund = 4
        }
        //for promotion
        public enum EOperatorType
        {
            All = 0,
            And = 1,
            Or = 2
        }

        public enum ESpendType
        {
            BuyItem = 1,
            SpendMoney = 2,
        }

        public enum ESpendOnType
        {
            AnyItem = 3,
            SpecificItem = 4,
            Category = 5,
            TotalBill = 6,
            SingleReceipt = 7,
            AccumulativeReceipt = 8
        }

        public enum EEarnType
        {
            TotalBill = 1,
            SpentItem = 2,
            SpecificItem = 3,
            Category = 4,
            Point = 5,
            Money = 6,
        }

        public enum EDiscountCodeState
        {
            Active = 1,
            Block = 2,
            Used = 3,
            Expired = 4
        }
        //Tax
        public enum ETaxType
        {
            TaxExempt = 1,
            Inclusive = 2,
            AddOn = 3
        }
        //GeneralSetting
        public enum EGeneralSetting
        {
            Email = 1,
            Password = 2,
            OrderPrefix = 3,
            RoundingOption = 4,
            ReceiptPrefix = 5,
            StartNumber = 6,
        }

        public enum EServiceAssetStatus
        {
            New = 0,
            Done = 1,
            Recalled = 2,
        }
        public enum EChangeStatus
        {
            Done = 0,
            Recall = 1,
            Edit = 2,
        }

        public enum EStatusAccountFunction
        {
            Expired,
            Inactive,
            Active,
            Blocked
        }
        public enum ENumberofDevices
        {
            Unlimited = -1
        }

        public enum EClientModule
        {
            Overview = 0,
            Package = 1,
            StartBusiness = 2
        }

        public enum ECustomerType
        {
            Consumer = 0,
            Customer = 1,
            Reseller = 2,
        }

        #endregion END ENUM FOR PAGE

        #region ENUM FOR CSC Client
        public enum EOrderStatus
        {
            Completed = 1,
            Half_Refunded = 2,
            Full_Refunded = 3,
        }

        public enum EItemGroup
        {
            Parent = 0,
            Composite = 1,
            Addition = 2,
        }

        public enum EPaymentCode
        {
            Parent = 0,
            Cash = 1,
            Redeem = 2,
            External = 3,
            GiftCard = 4
        }
        public enum EReceiptStatus
        {
            New = 0,
            OnProgress = 1,
            Completed = 2,
            Recalled = 3,
        }

        public enum EPayment
        {
            PayPal = 1,
            SecureAcceptance = 2,
            eNETS = 3
        }

        #endregion

        public static string FTPHostClient = ConfigurationManager.AppSettings["FTPWebImage"] ?? "";
        public static string FTPUserClient = ConfigurationManager.AppSettings["FTPUser"] ?? "";
        public static string FTPPasswordClient = ConfigurationManager.AppSettings["FTPPassword"] ?? "";
        public static string PublicImagesClient = ConfigurationManager.AppSettings["PublicImages"] ?? "";

        public static List<CategoryDetailModels> ListItem = new List<CategoryDetailModels>();

        public static bool IsChat
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["IsChat"]))
                    return bool.Parse(ConfigurationManager.AppSettings["IsChat"]);
                else
                    return false;
            }
        }

        #region VARIABLE FOR ENETS
        public static string URLENetsCSC
        {
            get
            {
                return ConfigurationManager.AppSettings["URLENetsCSC"] == null ? "" : ConfigurationManager.AppSettings["URLENetsCSC"];
            }
        }
        public static string URLENetsApp
        {
            get
            {
                return ConfigurationManager.AppSettings["URLENetsApp"] == null ? "" : ConfigurationManager.AppSettings["URLENetsApp"];
            }
        }

        public static string NetsMid
        {
            get
            {
                return ConfigurationManager.AppSettings["NetsMid"] == null ? "" : ConfigurationManager.AppSettings["NetsMid"];
            }
        }
        public static string SubmissionMode
        {
            get
            {
                return ConfigurationManager.AppSettings["SubmissionMode"] == null ? "" : ConfigurationManager.AppSettings["SubmissionMode"];
            }
        }
        public static string PaymentType
        {
            get
            {
                return ConfigurationManager.AppSettings["PaymentType"] == null ? "" : ConfigurationManager.AppSettings["PaymentType"];
            }
        }
        public static string MerchantTimeZone
        {
            get
            {
                return ConfigurationManager.AppSettings["MerchantTimeZone"] == null ? "" : ConfigurationManager.AppSettings["MerchantTimeZone"];
            }
        }
        public static string ClientType
        {
            get
            {
                return ConfigurationManager.AppSettings["ClientType"] == null ? "" : ConfigurationManager.AppSettings["ClientType"];
            }
        }
        public static string NetsMidIndicator
        {
            get
            {
                return ConfigurationManager.AppSettings["NetsMidIndicator"] == null ? "" : ConfigurationManager.AppSettings["NetsMidIndicator"];
            }
        }
        public static string IpAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["IpAddress"] == null ? "" : ConfigurationManager.AppSettings["IpAddress"];
            }
        }
        public static string Language
        {
            get
            {
                return ConfigurationManager.AppSettings["Language"] == null ? "" : ConfigurationManager.AppSettings["Language"];
            }
        }
        public static string SS
        {
            get
            {
                return ConfigurationManager.AppSettings["SS"] == null ? "" : ConfigurationManager.AppSettings["SS"];
            }
        }
        public static string SecretKey
        {
            get
            {
                return ConfigurationManager.AppSettings["SecretKey"] == null ? "" : ConfigurationManager.AppSettings["SecretKey"];
            }
        }
        #endregion

    }
}
