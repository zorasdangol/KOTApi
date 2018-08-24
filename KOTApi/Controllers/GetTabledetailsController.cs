using KOTApi.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dapper;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data;
using KOTAppClassLibrary.Models;

namespace KOTApi.Controllers
{
    public class GetTableDetailsController : ApiController
    {    

        public String getdayCloseTable(string dayCloseTable)
        {
            try
            {
                //return "0";
                using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    DateTime ServerDate = cnMain.ExecuteScalar<DateTime>("SELECT GETDATE()");
                    TimeSpan KotActiveTime = cnMain.ExecuteScalar<TimeSpan>("SELECT ISNULL(KotActiveTime, '00:00:00.000') FROM SETTING");
                    foreach (DateTime KotTime in cnMain.Query<DateTime>("SELECT TRNDATE + CAST(TRNTIME AS DATETIME) FROM RMD_KOTMAIN KM JOIN RMD_KOTMAIN_STATUS KMS ON KM.KOTID=KMS.KOTID WHERE KMS.STATUS='ACTIVE'"))
                    {
                        DateTime KotAllowdTime = ServerDate.Date.Add(KotActiveTime);
                        if (ServerDate > KotAllowdTime && KotTime < KotAllowdTime)
                        {
                            return "1";
                        }
                    }
                    return "0";
                    //return (cnMain.ExecuteScalar<int>("SELECT COUNT(TABLENO) FROM RMD_KOTMAIN WHERE TRNDATE < CONVERT(VARCHAR, GETDATE(), 101)") > 0) ? "1" : "0";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public IEnumerable<TableDetail> getTable(string GetTable)
        {
            try
            {
                using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    return (cnMain.Query<TableDetail>("SELECT DISTINCT(TABLENO),LayoutName,ImageName FROM TABLELIST ORDER BY TABLENO"));
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<TableDetail> getTableNo(string getTableNo)
        {
            IEnumerable<TableDetail> PackTables = new List<TableDetail>();
            string saveSetting = "";
            
            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            {
                cnMain.Open();
                try
                {
                    var dtt = cnMain.ExecuteScalar<string>("SELECT USERMD FROM AND_SETTING", cnMain);
                    if (dtt != null)
                    {
                        saveSetting = dtt.ToString();
                    }
                    if (saveSetting == "1")
                    {
                        PackTables = cnMain.Query<TableDetail>("SELECT KM.TABLENO FROM RMD_KOTMAIN KM JOIN RMD_KOTMAIN_STATUS KMS ON KM.KOTID=KMS.KOTID WHERE KMS.STATUS='ACTIVE'", cnMain);
                    }
                    else if (saveSetting == "0")
                    {
                        PackTables = cnMain.Query<TableDetail>("SELECT TABLENO FROM KOTMAIN", cnMain);
                    }
                    return PackTables;

                    //if (dt.Rows.Count > 0)
                    //{
                    //    for (int i = 0; i < dt.Rows.Count; i++)
                    //    {
                    //        str.Add(dt.Rows[i][0].ToString());
                    //    }
                    //}
                    //return str;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
