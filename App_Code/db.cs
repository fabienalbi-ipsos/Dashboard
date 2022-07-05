using System;
using System.Web;
using WebMatrix.Data;
using System.Configuration;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;

using System.Security.Cryptography;
using System.Text;
using System.Xml;

/// <summary>
/// Summary description for db
/// </summary>
public class db
{
    public db()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public class Constants
    {
        public const string dbName = "Dashboard";
        public const string providerName = "System.Data.SqlClient";

    
    }

    public static string GetConnectionString()
    {
        
        //this doesn't work because of the commandtimeout setting which is not supported in Database.Open
        string connectionString = ConfigurationManager.ConnectionStrings[Constants.dbName].ConnectionString ; 
        return connectionString;
    }

    public static string UI_AuthenticateUser_From_BIAuthentication(string userName, string password)
    {
       var db = Database.Open(Constants.dbName);
        
       var userKey = db.QueryValue("UI_AuthenticateUser_From_BIAuthentication @0, @1", userName, password);
        db.Dispose();
        return userKey;
       

    }


    public static string BI_Decrypt(string p_input)
    {

        //jIggrqblc3+iFVo4f03WvVpcND7ixpzz2dyAIezHQlRaGY2ZCVeRFw==;

        string v_results = null;
        string p_key = "__XTIMEZONE";

        {
            // prepare decryption
            TripleDESCryptoServiceProvider v_des = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider v_hash = new MD5CryptoServiceProvider();
            v_des.Key = v_hash.ComputeHash(Encoding.ASCII.GetBytes(p_key));
            v_hash = null;
            v_des.Mode = CipherMode.ECB;
            v_des.Padding = PaddingMode.ISO10126;
            byte[] v_input = Convert.FromBase64String(p_input);
            v_results = Encoding.ASCII.GetString(v_des.CreateDecryptor().TransformFinalBlock(v_input, 0, v_input.Length));
        }
        return v_results;
    }

    public static void setImpersonation(string userKey)
    {
        if (userKey == "0")
        {
            System.Web.HttpContext.Current.Session["impersonation"] = null;
        }
        else
        {
            System.Web.HttpContext.Current.Session["impersonation"] = userKey;
        }
    }

    public static string GetUserKey()
    {
        string userKey = "";
        if (System.Web.HttpContext.Current.Session["impersonation"] != null)
        {
            userKey = System.Web.HttpContext.Current.Session["impersonation"].ToString();

        }
        else if (System.Web.HttpContext.Current.Session["userKey"] != null)
        {
            userKey = System.Web.HttpContext.Current.Session["userKey"].ToString();
            
        }

        else { 
        string sessionKey = GetSessionKey();

        var db = Database.Open(Constants.dbName);
        userKey = db.QueryValue("UI_GetUserKey_FromSessionKey @0", sessionKey);


        System.Web.HttpContext.Current.Session["userKey"] = userKey;

            db.Dispose();
        }

       

        //HttpCookie Cookie = System.Web.HttpContext.Current.Request.Cookies["userKey"];

        //if (Cookie != null)
        //{
        //    userKey = Cookie.Value.ToString();
        //}

        return userKey;
        
    }


    public static string GetSessionKey()
    {
        string sessionKey = "";
        HttpCookie Cookie = System.Web.HttpContext.Current.Request.Cookies["sessionKey"];
        if (Cookie != null)
        {
            sessionKey = Cookie.Value.ToString();
        }
        return sessionKey;
    }


    public static string UI_MetaData_Get(string DataType, string DataID)
    {

        var db = Database.Open(Constants.dbName);
        var dataValue = db.QueryValue("UI_MetaData_Get @0, @1", DataType, DataID);
        db.Dispose();
        return dataValue;
       
    }

    public static string UI_GetUserKey_FromUserName(string userName)
    {
        var db = Database.Open(Constants.dbName);
        var userKey = db.QueryValue("UI_GetUserKey_FromUserName @0", userName);
        db.Dispose();
        return userKey;
    }

    public static string UI_GetUserKey_FromAN8(string AN8)
    {
        var db = Database.Open(Constants.dbName);
        var userKey = db.QueryValue("UI_GetUserKey_FromAN8 @0", AN8);
        db.Dispose();
        return userKey;
    }

    public static void UI_UserParameter_Set(string parameterType, string parameterValue)
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Execute("UI_UserParameter_Set @0, @1, @2", userKey, parameterType, parameterValue);
        db.Dispose();
    }

    public static void UI_PageLoad_Log(string parameterType, string PageName)
    {
        CompilationSection compilationSection = (CompilationSection)System.Configuration.ConfigurationManager.GetSection(@"system.web/compilation");

        // check the DEBUG attribute on the <compilation> element
        bool isDebugEnabled = compilationSection.Debug;
        if (!isDebugEnabled){

            //if (PageName =="Index" || PageName == "Load XML") { 
        if (System.Web.HttpContext.Current.Session["impersonation"] == null)
        {
            var userKey = GetUserKey();
            var db = Database.Open(Constants.dbName);
            var records = db.Execute("UI_PageLoad_Log @0, @1, @2", userKey, parameterType, PageName);
            db.Dispose();
            db.Close();
        }
        //}
        }
        else
        {
            if (System.Web.HttpContext.Current.Session["impersonation"] == null)
            {
                var userKey = GetUserKey();
                var db = Database.Open(Constants.dbName);
                var records = db.Execute("UI_PageLoad_Log @0, @1, @2", userKey, parameterType, PageName);
                db.Dispose();
                db.Close();
            }
        }
        
    }

    public static void UI_JavascriptError_Log(string errorMessage)
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Execute("UI_JavascriptError_Log @0, @1", userKey, errorMessage);
        db.Dispose();
    }

    public static void UI_UserActivityLog_Set(string ActivityType, string ActivityName)
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Execute("UI_UserActivityLog_Set @0, @1, @2", userKey, ActivityType, ActivityName);
        db.Dispose();
    }

    //this version takes a parameter value of userKey to capture email click-throughs
    public static void UI_UserActivityLog_Set(string userKey, string ActivityType, string ActivityName)
    {
        var db = Database.Open(Constants.dbName);
        var records = db.Execute("UI_UserActivityLog_Set @0, @1, @2", userKey, ActivityType, ActivityName);
        db.Dispose();
    }

    public static dynamic UI_UserActivityLog_Get(string userKey)
    {
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_UserActivityLog_Get @0", userKey);
        db.Dispose();
        return records;
    }

    public static dynamic UI_UserActivityLog_GetAll()
    {
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_UserActivityLog_GetAll");
        db.Dispose();
        return records;
    }

    public static dynamic UI_Session_Start(string userKey)
    {
        //var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var sessionKey = db.QueryValue("UI_Session_Start @0", userKey);

        //clear the session object values
        System.Web.HttpContext.Current.Session.Remove("userKey");
        System.Web.HttpContext.Current.Session.Remove("tree");
        db.Dispose();
        return sessionKey;
    }

    public static dynamic DB_ReportGet(string ReportKey)
    {
        var userID = GetUserKey();
        var db = Database.Open(Constants.dbName);

        if(ReportKey == "All")
        {
            db.Execute("DB_ReportGet @0, @1", userID, ReportKey);
            UI_Data_GetAll(userID);
            return 1;
        }

        else { 
        var records = db.Query("DB_ReportGet @0, @1", userID, ReportKey);
        db.Dispose();
        return records;
        }


    }
    public static dynamic DB_ReportGet_ADO(string ReportKey)
    {
        using (SqlConnection connection = new SqlConnection(Constants.dbName))
        {
            var userID = GetUserKey();
            SqlCommand command = new SqlCommand("DB_ReportGet", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 120;
            command.Parameters.AddWithValue("UserKey", userID);
            command.Parameters.AddWithValue("ReportKey", ReportKey);
            connection.Open();
             var records = command.ExecuteReader();
            return records;
        }

        

    }

    public static dynamic GetTreeByUser()
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_Tree_GetByUser @0", userKey);
        db.Dispose();
        return records;

    }

    public static dynamic UI_Currencies_GetAll(string userID = "")
    {
        if (userID == "")
        {
            userID = GetUserKey();
        }
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_Currencies_GetAll @0", userID);
        db.Dispose();
        return records;

    }

    public static dynamic UI_Parameters_GetAll(string userID = "")
    {
        if (userID == "")
        {
            userID = GetUserKey();
        }
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_Parameters_GetAll @0", userID);
        db.Dispose();
        return records;

    }

    public static dynamic UI_Rounding_GetAll()
    {
        var userID = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_Rounding_GetAll @0", userID);
        db.Dispose();
        return records;

    }

    public static dynamic UI_HelpContent_Get(string helpKey)
    {
        var userID = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.QuerySingle("UI_HelpContent_Get @0, @1", userID, helpKey);
        db.Dispose();
        return records;

    }

    public static string UI_HelpTitle_Get(string helpKey)
    {
        var userID = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.QuerySingle("UI_HelpTitle_Get @0, @1", userID, helpKey);
        db.Dispose();
        return records[0].ToString();
    }

    public static dynamic UI_LastUpdate_Get(string helpKey)
    {
        var userID = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.QuerySingle("UI_LastUpdate_Get @0, @1", userID, helpKey);
        db.Dispose();
        return records;

    }

    public static void UI_HelpContent_Delete(string helpKey)
    {
        var userID = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.QuerySingle("UI_HelpContent_Delete @0, @1", userID, helpKey);
        db.Dispose();
    }

    public static dynamic UI_HelpContent_GetAll(string CategoryKey = "0") //if no categorykey given load all help contents
    {
        var userID = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_HelpContent_GetAll @0, @1", userID, CategoryKey);
        db.Dispose();
        return records;

    }

    public static void UI_HelpContent_Set(string userKey, string HelpKey, string Helptitle, string HelpContent, string CategoryKey)
    {//updates or inserts a help content
        //new records are passed with helpKey = 0

        //var userID = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Execute("UI_HelpContent_Set @0, @1, @2, @3, @4", userKey, HelpKey, Helptitle, HelpContent, CategoryKey);
        db.Dispose();

        HttpContext p_context = HttpContext.Current;

        string host = p_context.Request.Url.Host.ToLower();
        string path = "/img/help/";
        string fileName = @"help_" + HelpKey + ".html";

        if (host.Contains("bidev3") || host.Contains("dashboard-bidev"))
        {
            path = @"\\1237447-WSDEV01\MISApps$\BIDev3.Ipsos\webroot\img\help\";
            using (System.IO.FileStream fs4 = new System.IO.FileStream(path + fileName, System.IO.FileMode.Create))
            {
                using (System.IO.StreamWriter w4 = new System.IO.StreamWriter(fs4))
                { w4.WriteLine(HelpContent); }
            }
        }
        else if (host.Contains("bidashboardtest.ipsos") || host.Contains("dashboard-bitest"))
        {
            path = @"\\1237449-WSTST01\MISAPPS$\Dashboard\webroot\img\help\";
            using (System.IO.FileStream fs1 = new System.IO.FileStream(path + fileName, System.IO.FileMode.Create))
            {
                using (System.IO.StreamWriter w1 = new System.IO.StreamWriter(fs1))
                { w1.WriteLine(HelpContent); }
            }
        }
        else if (host.Contains("bidashboard.ipsos") || host.Contains("dashboard-bi") || host.Contains("dashboard1") || host.Contains("dashboard2") || host.Contains("dashboard3"))
        {
            path = @"\\1125571-ws03\MISApps$\BIDashboard\webroot\img\help\";
            using (System.IO.FileStream fs1 = new System.IO.FileStream(path + fileName, System.IO.FileMode.Create))
            {
                using (System.IO.StreamWriter w1 = new System.IO.StreamWriter(fs1))
                { w1.WriteLine(HelpContent); }
            }
            path = @"\\1125573-WS04\MISApps$\BIDashboard\webroot\img\help\";
            using (System.IO.FileStream fs3 = new System.IO.FileStream(path + fileName, System.IO.FileMode.Create))
            {
                using (System.IO.StreamWriter w3 = new System.IO.StreamWriter(fs3))
                { w3.WriteLine(HelpContent); }
            }
            path = @"\\1125574-WS05\MISApps$\BIDashboard\webroot\img\help\";
            using (System.IO.FileStream fs2 = new System.IO.FileStream(path + fileName, System.IO.FileMode.Create))
            {
                using (System.IO.StreamWriter w2 = new System.IO.StreamWriter(fs2))
                { w2.WriteLine(HelpContent); }
            }
        }
    }


    public static dynamic UI_UserParameters_Get(string userKey)
    {
        if (userKey == "")
        {
            userKey = GetUserKey();
        }
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_UserParameters_Get @0", userKey);
        db.Dispose();
        return records;

    }

    public static dynamic UI_LogHits_Get(string userKey)
    {
        var db = Database.Open(Constants.dbName);
        var records = db.QuerySingle("UI_LogHits_Get @0", userKey);
        db.Dispose();
        return records;
    }

    public static dynamic UI_LogHits_GetAll()
    {
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_LogHits_GetAll");
        db.Dispose();
        return records;
    }

    public static dynamic UI_LogHits_DB_Get()
    {
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_LogHits_DB_Get");
        db.Dispose();
        return records;
    }

    public static dynamic UI_Errors_Javascript_Get()
    {
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_Errors_Javascript_Get");
        db.Dispose();
        return records;
    }

    public static dynamic UI_Periods_GetAll()
    {
        var userID = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_Periods_GetAll @0", userID);
        db.Dispose();
        return records;

    }

    public static dynamic UI_RateTypes_GetAll()
    {
        var userID = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_RateTypes_GetAll @0", userID);
        db.Dispose();
        return records;

    }

    public static dynamic UI_User_Get (string userKey)
    {

        var db = Database.Open(Constants.dbName);
        var records = db.QuerySingle("UI_User_Get @0", userKey);
        db.Dispose();
        return records;

    }

    public static dynamic UI_UserDefaults_Get(string userKey)
    {

        var db = Database.Open(Constants.dbName);
        var records = db.QuerySingle("UI_UserDefaults_Get @0", userKey);
        db.Dispose();
        return records;

    }

    public static void UI_User_Set(string UserUserKey, string currency, string scope, string role)
    {

        var AdminUserKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Execute("UI_User_Set @0, @1, @2, @3, @4", AdminUserKey, UserUserKey, currency, scope, role);
        db.Dispose();
    }

    public static dynamic UI_User_GetAll()
    {

        var userID = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_User_GetAll @0", userID);
        db.Dispose();
        return records;

    }

    public static dynamic UI_Defaults_Get(string userKey, string groupType)
    {

        var userID = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_Defaults_Get @0, @1, @2", userID, userKey, groupType);
        db.Dispose();
        return records;

    }

    public static dynamic UI_GetUserBySearch(string searchTerm)
    {

        var userID = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_GetUserBySearch @0, @1", userID, searchTerm);
        db.Dispose();
        return records;

    }

    public static dynamic UI_Years_GetAll()
    {
        var userID = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_Years_GetAll @0", userID);
        db.Dispose();
        return records;

    }

    public static dynamic UI_ChartData_Get(string chartKey)
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_ChartData_Get @0, @1",userKey, chartKey);
        db.Dispose();
        return records;

    }

    public static dynamic UI_ChartData_Offline_Get(string chartKey)
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_ChartData_Offline_Get @0, @1", userKey, chartKey);
        db.Dispose();
        return records;

    }

    public static dynamic UI_ChartTitles_Get(string chartKey)
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_ChartTitles_Get @0, @1", userKey, chartKey);
        db.Dispose();
        return records;

    }

    public static dynamic UI_ChartTitles_GetAll()
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_ChartTitles_GetAll @0", userKey);
        db.Dispose();
        return records;

    }

    public static dynamic UI_LinkedReports_Get(string KPIKey)
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_LinkedReports_Get @0, @1", userKey, KPIKey);
        db.Dispose();
        return records;

    }

    public static dynamic UI_LinkedReports_GetAll()
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_LinkedReports_GetAll @0", userKey);
        db.Dispose();
        return records;

    }

    public static dynamic UI_AlertReports_GetAll()
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_AlertReports_GetAll @0", userKey);
        db.Dispose();
        return records;
    }

    public static dynamic GetPageLog(string userKey)
    {

        var db = Database.Open(Constants.dbName);
        var records = db.Query("GetPageLog @0", userKey);
        db.Dispose();
        return records;

    }

    public static dynamic GetPageLog_byDate(string date)
    {

        var db = Database.Open(Constants.dbName);
        var records = db.Query("GetPageLog_ByDate @0", date);
        db.Dispose();
        return records;

    }

    public static dynamic UI_Roles_GetAll()
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_Roles_GetAll @0", userKey);
        db.Dispose();
        return records;
    }

    public static dynamic UI_Roles_Get()
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_Roles_Get @0", userKey);
        db.Dispose();
        return records;
    }

    public static dynamic DB_Alerts_GetAll()
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("DB_Alerts_GetAll @0", userKey);
        db.Dispose();
        return records;
    }

    public static dynamic UI_Help_Categories_GetAll()
    {
        var userKey = GetUserKey();
        var db = Database.Open(Constants.dbName);
        var records = db.Query("UI_Help_Categories_GetAll @0", userKey);
        db.Dispose();
        return records;
    }

    public static dynamic DashboardData { get; set; }
    public static dynamic UI_Data_GetAll(string userKey = null)
    {
        if (userKey == null) { userKey = GetUserKey(); }
        var db = Database.Open(Constants.dbName);
        dynamic records = null;
        if (System.Web.HttpContext.Current.Session["impersonation"] != null)
        {
            records = db.Query("UI_Data_GetAll @0, @1", userKey, 1);
        } else
        {
            records = db.Query("UI_Data_GetAll @0", userKey);
        }
            
        db.Dispose();

        
        DashboardData = records;

        return records;
    }

    public static System.Xml.XmlReader UI_GetXML (string content)
    {
        System.Xml.XmlReader xr;

        SqlConnection dashboard = new SqlConnection(GetConnectionString());

            string proc = "UI_GetXML " + GetUserKey() + ", '" + content + "'";
            dashboard.Open();
            SqlCommand custCMD = new SqlCommand(proc, dashboard);

             xr = custCMD.ExecuteXmlReader();

        return xr;

    }

    public static string AttributeIsNull(XmlNode nodeName, string attributeName, string replaceWith)
    {
        string returnValue = "";
        //HttpContext.Current.Trace.Warn("AttributeIsNull method called: " + attributeName);

        /*
            This will check if an attribute exists in an xml node, 
            and if the attribute doesn't exist, the method will return the value of the replaceWith parameter

         */
        try { 
        if (nodeName == null)
        {
            HttpContext.Current.Trace.Warn("node is null for " + attributeName);
        }

        if (nodeName.Attributes == null)
        {
            HttpContext.Current.Trace.Warn("attributes is null for " + attributeName);
        }

        
        string TestValue = null;

        if (nodeName.Attributes[attributeName] != null)
        {
            TestValue = nodeName.Attributes[attributeName].Value;
            returnValue = TestValue;
        }

        if (TestValue == null) { returnValue = replaceWith; }
        
        }
        catch
        {
            UI_PageLoad_Log("0", "Error trying AttributesIsNull");

            HttpContext.Current.Server.TransferRequest("/Index.cshtml");
        }
        return returnValue;

    }

    public static dynamic Admin_Upload_UserSetup()
    {
        var db = Database.Open(Constants.dbName);
        var records = db.Query("Admin_Upload_UserSetup");
        db.Dispose();

        return records;
    }

    public static dynamic Admin_UserSetup_Comment_Get(string userKey)
    {
        var db = Database.Open(Constants.dbName);
        var record = db.QuerySingle("Admin_UserSetup_Comment_Get @0", userKey);
        db.Dispose();

        return record;
    }

    public static void Admin_UserSetup_Comment_Set(string userKey, string comment, string adminkey, bool validated, bool setupvalidated)
    {
        var db = Database.Open(Constants.dbName);
        var record = db.Execute("Admin_UserSetup_Comment_Set @0, @1, @2, @3, @4", userKey, comment, adminkey, validated, setupvalidated);
        db.Dispose();
    }

    public static dynamic Admin_Users_Security_Report()
    {
        var db = Database.Open(Constants.dbName);
        var record = db.Query("Admin_Users_Security_Report");
        db.Dispose();

        return record;
    }

    public static dynamic Admin_GetUsersByTreeNode()
    {
        var db = Database.Open(Constants.dbName);
        var record = db.Query("Admin_GetUsersByTreeNode");
        db.Dispose();

        return record;
    }

    public static void Admin_UserRoles_Set(string userKey, string role)
    {
        var db = Database.Open(Constants.dbName);
        var record = db.Execute("Admin_UserRoles_Set @0, @1", userKey, role);
        db.Dispose();
    }

    public static dynamic UI_UserActivityLog_GetByType(string type, string activity)
    {
        var db = Database.Open(Constants.dbName);
        var record = db.Query("UI_UserActivityLog_GetByType @0, @1", type, activity);
        db.Dispose();

        return record;
    }

    public static dynamic PM_GetPending(string userKey)
    {
        var db = Database.Open(Constants.dbName);
        var records = db.Query("PM_GetPending @0", userKey);
        db.Dispose();

        return records;
    }

    public static dynamic PM_GetWinning(string userKey)
    {
        var db = Database.Open(Constants.dbName);
        var records = db.Query("PM_GetWinning @0", userKey);
        db.Dispose();

        return records;
    }
}