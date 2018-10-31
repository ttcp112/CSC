using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.Settings.Permissions;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory.Settings.Permissions
{
    public class PermissionsFactory : BaseFactory
    {
        private BaseFactory _baseFactory = null;
        public PermissionsFactory()
        {
            _baseFactory = new BaseFactory();
        }

        public List<PermissionDTO> GetListModule()
        {
            List<PermissionDTO> listData = new List<PermissionDTO>();
            try
            {
                GetListPermissionRequest paraBody = new GetListPermissionRequest();
                NSLog.Logger.Info("Permission Get List Module Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.PermissionAPIGet, null, paraBody);
                NSLog.Logger.Info("Permission Get List Module Response", paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListPermission"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<PermissionDTO>>(lstObject);

                NSLog.Logger.Info("RoleGetListPermission", listData);

                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("RoleGetListPermission_Fail", e);
                return listData;
            }
        }

        public List<PermissionsModels> GetListData(int level)
        {
            List<PermissionsModels> listData = new List<PermissionsModels>();
            try
            {
                GetListRoleRequest paraBody = new GetListRoleRequest();
                paraBody.Level = level;
                NSLog.Logger.Info("Permission Get List Role Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.PermissionAPIGetList, null, paraBody);
                NSLog.Logger.Info("Permission Get List Role Response", paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListRole"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<PermissionsModels>>(lstObject);

                NSLog.Logger.Info("RoleGetListData", listData);

                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("RoleGetListData_Fail", e);
                return listData;
            }
        }

        public RoleDetailDTO GetDetail(string ID)
        {
            RoleDetailDTO item = new RoleDetailDTO();
            try
            {
                GetRoleDetailRequest paraBody = new GetRoleDetailRequest();
                paraBody.ID = ID;
                NSLog.Logger.Info("Permission Get Detail Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.PermissionAPIGetDetail, null, paraBody);
                NSLog.Logger.Info("Permission Get Detail Response", paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["RoleDetail"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                item = JsonConvert.DeserializeObject<RoleDetailDTO>(lstObject);

                NSLog.Logger.Info("RoleGetDetail", item);

                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("RoleGetDetail_Fail", e);
                return item;
            }
        }

        public bool CreateOrEdit(RoleDetailDTO model, string userId, ref string msg)
        {
            try
            {
                CreateOrEditRoleRequest paraBody = new CreateOrEditRoleRequest();
                paraBody.RoleDetail = model;
                paraBody.CreatedUser = userId;
                //====================
                NSLog.Logger.Info("Permission Create Or Edit Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.PermissionAPICreateOrEdit, null, paraBody);
                NSLog.Logger.Info("Permission Create Or Edit Response", paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("PermissionCreateOrEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("PermissionCreateOrEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("PermissionCreateOrEdit_Fail", e);
                return false;
            }
        }

        public bool Delete(string ID, string CreateUser, ref string msg, string ReasonDelete = null)
        {
            try
            {
                DeleteRoleRequest paraBody = new DeleteRoleRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ReasonDelete = ReasonDelete;
                paraBody.ID = ID;
                //====================
                NSLog.Logger.Info("Permission Delete Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.PermissionAPIDelete, null, paraBody);
                NSLog.Logger.Info("Permission Delete Response", paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("PermissionDelete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("PermissionDelete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("PermissionDelete_Fail", e);
                return false;
            }
        }
    }
}
