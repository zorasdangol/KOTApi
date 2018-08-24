using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace KOTApi.Helpers
{
    public class GlobalClass
    {
        public static string Division = "MMX";
        public static string DataConnectionString { get { return ConfigurationManager.ConnectionStrings["DBSETTING"].ConnectionString; } }

        public static string Encrypt(string Text, string Key)
        {
            int i;
            string TEXTCHAR;
            string KEYCHAR;
            string encoded = string.Empty;
            for (i = 0; i < Text.Length; i++)
            {
                TEXTCHAR = Text.Substring(i, 1);
                var keysI = ((i + 1) % Key.Length);
                KEYCHAR = Key.Substring(keysI);
                var encrypted = Microsoft.VisualBasic.Strings.Asc(TEXTCHAR) ^ Microsoft.VisualBasic.Strings.Asc(KEYCHAR);
                encoded += Microsoft.VisualBasic.Strings.Chr(encrypted);
            }
            return encoded;
        }
    }
}