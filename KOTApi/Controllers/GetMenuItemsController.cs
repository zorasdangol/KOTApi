using Dapper;
using KOTApi.Helpers;
using KOTAppClassLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KOTApi.Controllers
{
    public class GetMenuItemsController : ApiController
    {
        public FunctionResponse getMenuItem_New()
        {
            try
            {
                IEnumerable<dynamic> MenuList = null;
                byte KotMenuDisplay;
                using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    KotMenuDisplay = cnMain.ExecuteScalar<byte>("SELECT ISNULL(KotMenuDisplay, 0) FROM SETTING");
                    if (KotMenuDisplay == 0)
                    {
                        MenuList = cnMain.Query<MenuItem>(@"SELECT DISTINCT MI.MCODE, MI.MENUCODE, MI.DESCA, MI.PARENT, MI.PTYPE, ISNULL(MI.BASEUNIT,'EMPTY') AS BASEUNIT, MI.RATE_A, MI.TYPE, MI.IsBarItem, MI.MGROUP, 0 IsUnknown FROM MENUITEM MI JOIN MENUITEM C ON MI.MCODE = C.PARENT WHERE C.PTYPE IN(0,10,11) AND C.DISCONTINUE = 0 AND C.TYPE = 'A'
                                                    UNION ALL
                                                    SELECT MCODE, MENUCODE, DESCA, PARENT, PTYPE, ISNULL(BASEUNIT, 'EMPTY') AS BASEUNIT, RATE_A, TYPE, IsBarItem, MGROUP, ISNULL(IsUnknown, 0) IsUnknown FROM MENUITEM WHERE PTYPE IN(0, 10, 11) AND DISCONTINUE = 0 AND TYPE = 'A' ORDER BY DESCA");
                    }
                    else
                    {
                        MenuList = cnMain.Query<MenuItem>(@"SELECT 'Undefined' MCODE, 'Undefined' MENUCODE, 'Undefined' DESCA, '' PARENT, 0 PTYPE, 'EMPTY' BASEUNIT, 0 RATE_A, 'G' TYPE, 0 IsBarItem, '' MGROUP, 0 IsUnknown
                                                    UNION ALL
                                                    SELECT DISTINCT MGroupName MCODE, MGroupName MENUCODE, MGroupName DESCA, '' PARENT, 0 PTYPE, 'EMPTY' BASEUNIT, 0 RATE_A, 'G' TYPE, 0 IsBarItem, '' MGROUP, 0 IsUnknown FROM KOT_MENUMAPPING
                                                    UNION ALL
                                                    SELECT MI.MCODE, MENUCODE, DESCA, ISNULL(MGroupName, 'Undefined') PARENT, PTYPE, BASEUNIT , RATE_A, TYPE, IsBarItem, MGROUP, ISNULL(IsUnknown,0) IsUnknown FROM MenuItem MI LEFT JOIN KOT_MENUMAPPING MAP ON MI.MCODE = MAP.MCODE WHERE TYPE = 'A' AND PTYPE NOT IN (1) AND DISCONTINUE = 0 AND ISNULL([Disabled],0) = 0 ORDER BY DESCA");
                    }
                    return (new FunctionResponse() { status = "ok" , result = MenuList });
                }
            }
            catch (Exception e)
            {
                return (new FunctionResponse() { status = "error", Message = e.Message });
            }
        }

    }
}
