using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Data.SqlClient;

public class KPIxml
{
    public KPIxml()
    {

    }
   
    public  XmlDocument LoadData(string userKey, string url)
    {
        XmlReader xr;
        SqlConnection dashboard = new SqlConnection(db.GetConnectionString());

        string proc = "UI_GetXML " + userKey + ", '" + url + "'";
        dashboard.Open();
        SqlCommand custCMD = new SqlCommand(proc, dashboard);
        custCMD.CommandTimeout = 90;

        xr = custCMD.ExecuteXmlReader();
        
        XmlDocument x = new XmlDocument();
        x.Load(xr);
        xr.Close();

        dashboard.Close();
        return x;
    }

    

public string GetFileName(string userKey)
    {

    HttpContext p_context = HttpContext.Current;
    p_context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

    
    string path = @"/App_Data/xml/";
    string fileName = @"kpidata_" + userKey + ".xml";
    fileName = path + fileName;
        string fullpath = p_context.Server.MapPath(fileName);
    return fullpath;
}

   

}
