using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

public class imageUploader
{
    static public string uploadImage(HttpPostedFileBase image)
    {
        Image newImage = System.Drawing.Image.FromStream(image.InputStream);
        string imageFile = "upload" + DateTime.Now.ToString("ddmmyyyyffff") + ".png";
        newImage.Save(@"\\1125571-ws03\MISApps$\BIDashboard\webroot\img\help\" + imageFile);
        var url = "https://dashboard1-bi.ipsos.com/img/help/" + imageFile;
        return url;
    }
}   