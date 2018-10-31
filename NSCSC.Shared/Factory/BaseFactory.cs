
using ClosedXML.Excel;
using NSCSC.Shared.Models.CommonModels;
using NSCSC.Shared.Models.CRM.Customers;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NSCSC.Shared.Factory
{
    public class BaseFactory
    {
        public static bool GetBoolValue(string columnName, string rowCount, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
                //throw new Exception(string.Format("Data in row #{0} is not valid, {1} cannot be null, cannot import this row", rowCount, columnName));
            }
            value = value.Trim().ToLower();

            if (value == "true")
                return true;
            if (value == "false")
                return false;
            if (value == "1")
                return true;
            if (value == "0")
                return false;
            if (value == "yes")
                return true;
            if (value == "no")
                return false;
            if (value == "active")
                return true;
            if (value == "inactive")
                return false;
            if (value == "male")
                return true;
            if (value == "female")
                return false;

            throw new Exception(string.Format("Data in row #{0} is not valid, {1} must be a boolean value, cannot import this row", rowCount, columnName));
        }

        public static bool GetBoolValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            value = value.Trim().ToLower();

            if (value == "true")
                return true;
            if (value == "false")
                return false;
            if (value == "1")
                return true;
            if (value == "0")
                return false;
            if (value == "yes")
                return true;
            if (value == "no")
                return false;
            if (value == "active")
                return true;
            if (value == "inactive")
                return false;
            if (value == "male")
                return true;
            if (value == "female")
                return false;
            throw new Exception();
        }

        public static void FormatExcelExport(IXLWorksheet ws, int lastRow, int lastCol)
        {
            // Format Excel
            ws.Range(1, 1, 1, lastCol).Style.Font.SetBold(true);
            ws.Range(1, 1, lastRow, lastCol).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
            ws.Range(1, 1, lastRow, lastCol).Style.Alignment.Vertical = ClosedXML.Excel.XLAlignmentVerticalValues.Center;

            ws.Range(1, 1, lastRow - 1, lastCol).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(1, 1, lastRow - 1, lastCol).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Columns(1, lastCol).AdjustToContents();
        }

        public List<T> GetListObject<T>(DataTable dt) where T : new()
        {
            List<T> lst = new List<T>();
            for (int rowCount = 0; rowCount < dt.Rows.Count; rowCount++)
            {
                T obj = new T();
                InforImportModels model = new InforImportModels();
                DataRow cells = dt.Rows[rowCount];
                PropertyInfo[] props = obj.GetType().GetProperties();

                // Check Row Nulls
                string text = "";
                for (int i = 0; i < cells.Table.Columns.Count; i++)
                    text += cells[i].ToString();

                if (string.IsNullOrEmpty(text))
                    continue;

                for (int cellCount = 0; cellCount < cells.Table.Columns.Count; cellCount++)
                {
                    try
                    {
                        MapProperty(ref obj, props[cellCount].Name, cells[cellCount].ToString());
                    }
                    catch
                    {
                        model.Errors.Add(cells.Table.Columns[cellCount].ColumnName + " is not valid");
                        model.IsValidRow = false;
                        continue;
                    }
                }
                obj.GetType().GetProperty("Infor").SetValue(obj, model, null);
                obj.GetType().GetProperty("RowCount").SetValue(obj, (rowCount + 1), null);
                lst.Add(obj);
            }
            return lst;
        }

        protected void MapProperty<T>(ref T obj, string propName, string value) where T : new()
        {
            PropertyInfo prop = obj.GetType().GetProperty(propName);
            string propType = prop.PropertyType.ToString();
            try
            {
                switch (propType)
                {
                    case "System.Boolean":
                        prop.SetValue(obj, GetBoolValue(value), null);
                        break;
                    case "System.Double":
                        prop.SetValue(obj, GetDoubleValue(value), null);
                        break;
                    case "System.Decimal":
                    case "System.Nullable`1[System.Decimal]":
                        prop.SetValue(obj, GetDecimalValue(value), null);
                        break;
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Int16":
                        prop.SetValue(obj, GetIntValue(value), null);
                        break;
                    case "System.DateTime":
                        prop.SetValue(obj, GetDateTimeValue(value), null);
                        break;
                    default:
                        prop.SetValue(obj, Regex.Replace(value, @"\s+", " "), null);
                        break;
                }
            }
            catch
            {
                throw;
            }
        }

        public static double GetDoubleValue(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    return 0;
                return double.Parse(value);
            }
            catch
            {
                throw;
            }
        }

        public static decimal GetDecimalValue(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    return 0;
                return decimal.Parse(value);
            }
            catch
            {
                throw;
            }
        }

        public static int GetIntValue(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    return 0;
                return int.Parse(value);
            }
            catch
            {
                throw;
            }
        }
        public static DateTime GetDateTimeValue(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value) || value.Trim().ToLower() == "never")
                    value = Commons.NeverDate;
                return DateTime.Parse(value);
            }
            catch
            {
                throw;
            }
        }

        /*Factory Extension*/
        /*===========Set Menu*/
        public DataTable ReadExcelFile(string path, string sheetName)
        {
            DataTable dt = new DataTable();
            using (OleDbConnection conn = new OleDbConnection())
            {
                conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";" + "Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandText = "SELECT * FROM [" + sheetName + "$]";
                cmd.Connection = conn;
                OleDbDataAdapter da = new OleDbDataAdapter();
                da.SelectCommand = cmd;
                try
                {
                    conn.Open();
                    da.Fill(dt);
                }
                finally
                {
                    da.Dispose();
                    cmd.Dispose();
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.Dispose();
                }
                return dt;
            }
        }

        public List<CountryModel> GetListCountry()
        {
            List<CountryModel> listData = new List<CountryModel>();
            //check session
            if (System.Web.HttpContext.Current.Session["ContriesLibSessionCSC"] != null)
                listData = (List<CountryModel>)System.Web.HttpContext.Current.Session["ContriesLibSessionCSC"];
            else
            {
                string APICountry = ConfigurationManager.AppSettings["RestContriesApi"] ?? "";
                listData = (List<CountryModel>)ApiResponse.GetWithoutHostConfig<List<CountryModel>>(APICountry, null, null);
                //write to session

                System.Web.HttpContext.Current.Session.Add("ContriesLibSessionCSC", listData);

            }
            if (listData == null)
            {
                listData = new List<CountryModel>();
            }
            return listData;
        }
    }
}
