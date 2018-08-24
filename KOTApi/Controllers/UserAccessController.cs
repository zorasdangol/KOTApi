using Dapper;
using KOTApi.Helpers;
using KOTAppClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KOTApi.Controllers
{
    public class UserVerificationController : ApiController
    {
        public string postuserVerification(User User)
        {
            //string[] tmp = values.Split(new string[] { "<;>" }, StringSplitOptions.RemoveEmptyEntries);
            string USERNAME = User.UserName;
            string PASSWORD = User.Password;
            string UNIQUEID = User.UniqueID;
            string encPassword;
            string key = "AmitLalJoshi";
            encPassword = GlobalClass.Encrypt(PASSWORD, key);
            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            {
                cnMain.Open();
                //dt = new DataTable();
                try
                {
                    registerDevice(UNIQUEID, cnMain);
                    int i = cnMain.ExecuteScalar<int>("SELECT COUNT(*) FROM USERPROFILES WHERE UNAME='" + USERNAME + "' AND PASSWORD='" + encPassword + "'", cnMain);
                    if (i > 0)
                    {
                        i = cnMain.ExecuteScalar<int>("SELECT COUNT(*) FROM RMD_DEVICEVALIDATION WHERE UNIQUEID='" + UNIQUEID + "'", cnMain);
                        if (i > 0)
                        {
                            return "1";
                        }
                    }
                    return "0";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
        }

        public void registerDevice(string UNIQUEID, SqlConnection cnMain)
        {
            //db.executeNonQuery("INSERT INTO RMD_DEVICEVALIDATION (UNIQUEID, SALESTERMINAL,DIVISION,WAREHOUSE) VALUES('" + UNIQUEID + "', (SELECT TOP 1 NAME FROM SALESTERMINAL), 'MMX', (SELECT NAME FROM RMD_WAREHOUSE WHERE ISDEFAULT='T'))", trn, cnMain);
            //cnMain.Execute("INSERT INTO RMD_DEVICEVALIDATION (UNIQUEID, SALESTERMINAL,DIVISION,WAREHOUSE) SELECT'" + UNIQUEID + "', (SELECT TOP 1 NAME FROM SALESTERMINAL), '" + GlobalClass.Division + "', (SELECT NAME FROM RMD_WAREHOUSE WHERE ISDEFAULT='T')");
            if (cnMain.ExecuteScalar<int>("SELECT COUNT(*) FROM RMD_DEVICEVALIDATION WHERE UNIQUEID = @UNIQUEID", new { UNIQUEID = UNIQUEID }) == 0)
                cnMain.Execute("INSERT INTO RMD_DEVICEVALIDATION (UNIQUEID, SALESTERMINAL,DIVISION,WAREHOUSE) SELECT TOP 1 @UNIQUEID DEVICE_ID, ST.NAME TERMINAL, @DIVISION DIVISION, W.NAME WAREHOUSE FROM SALESTERMINAL ST, RMD_WAREHOUSE W ORDER BY ISDEFAULT DESC", new { UNIQUEID = UNIQUEID, DIVISION = GlobalClass.Division });
        }

    }
}
