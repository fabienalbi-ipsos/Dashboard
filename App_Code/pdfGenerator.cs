using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System;
using PDF_Tests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

/// <summary>
/// Summary description for pdfGenerator
/// </summary>
public class pdfGenerator
{
    private string userKey;
    private PdfContentByte cb;
    private byte[] pdfBytes;
    private float[] widths;
    private PdfPTable table;
    private PdfPCell cell;
    private Paragraph title, subtitle, KPIName, KPIValue;
    private Phrase rowTitle;
    private Image graph, dot;
    private Document PdfDashboard;
    private Boolean noData, noGraph;
    private JObject kpiSubtitle;
    private IEnumerable<JObject> kpiTableHeader, kpiTableContent;
    private JArray jsonData;

    private Font MainTitleFont = FontFactory.GetFont("Helvetica", 26);
    private Font KPITitleFont = FontFactory.GetFont("Helvetica", 16);
    private Font KPISubtitleFont = FontFactory.GetFont("Helvetica", 12);
    private Font SectionTitleFont = FontFactory.GetFont("Helvetica", 18, Font.BOLD);

    public pdfGenerator(List<string> kpis, dynamic data, string userID, string server)
    {
        jsonData = JArray.Parse(JsonConvert.SerializeObject(data));
        userKey = userID;
        using (MemoryStream ms = new MemoryStream())
        {
            PdfDashboard = new Document(PageSize.LETTER, 36f, 36f, 18f, 18f);
            PdfWriter writer = PdfWriter.GetInstance(PdfDashboard, ms);
            
            HeaderFooter PageEventHandler = new HeaderFooter();
            writer.PageEvent = PageEventHandler;

            PdfDashboard.Open();
            cb = writer.DirectContent;

            Image logo = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "/img/IpsosLogo.png");
            logo.SetAbsolutePosition(90, 720);
            logo.ScaleAbsolute(40f, 40f);
            logo.Alignment = Image.UNDERLYING;
            PdfDashboard.Add(logo);

            title = new Paragraph("BI Business Dashboard", MainTitleFont);
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingAfter = 50;
            PdfDashboard.Add(title);

            PdfDashboard.Add(addParams());

            addSectionTitle("Headlines");
            addHeadlines();

            foreach (string kpi in kpis)
            {
                noData = false;
                noGraph = false;
                if (kpi == "61" || kpi == "71" || kpi == "81" || kpi == "91") { addSectionTitle(kpi); }
                PdfDashboard.Add(addTitle(kpi));
                addSubtitle(kpi);
                if (!noData) { PdfDashboard.Add(addTable(kpi)); } //show the P&L table even without revenue
                if (kpi != "TP" && !noData && !noGraph) { addGraph(kpi,server); }
                if (kpi == "91") {
                    Paragraph final = new Paragraph("For more information, please log into BI at: ");
                    Phrase biURL = new Phrase("https://bi.ipsos.com", new Font(Font.FontFamily.HELVETICA, 12, Font.UNDERLINE));
                    final.Add(biURL);
                    final.Alignment = Element.ALIGN_CENTER;
                    final.SpacingBefore = 100;
                    PdfDashboard.Add(final);
                }
                PdfDashboard.NewPage();
            }
            PdfDashboard.Close();
            pdfBytes = ms.ToArray();
        }
    }

    public byte[] retrieveBytes()
    {
        return pdfBytes;
    }

    private string getString(JObject jsonObject, string columnName)
    {
        return jsonObject.GetValue(columnName).ToString();
    }

    private void addHeadlines()
    {
        table = new PdfPTable(5);
        widths = new float[] { 2f, 1f, 1f, 2f, 1f };
        table.SetWidths(widths);

        var headlines = jsonData.Children<JObject>().Where(p => p["RowKey"].ToString() == "500");
        string RowDescription, Value, Rounding, Icon, KPIKey;
        foreach (var headline in headlines)
        {
            RowDescription = headline.GetValue("RowDescription").ToString();
            Value = headline.GetValue("Value").ToString();
            Rounding = headline.GetValue("Rounding").ToString();
            Icon = headline.GetValue("Icon").ToString();
            KPIKey = headline.GetValue("KPIKey").ToString();

            KPIName = new Paragraph(RowDescription);
                if (Value == "")
                {
                    KPIValue = new Paragraph("N/A", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD));
                } else
                {
                    KPIValue = new Paragraph(Value + " " + Rounding, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD));
                }
                KPIName.Alignment = Element.ALIGN_LEFT;
                KPIValue.Alignment = Element.ALIGN_LEFT;

                cell = new PdfPCell();
                cell.AddElement(KPIValue);
                cell.AddElement(KPIName);
                cell.BorderWidth = 0;
                table.AddCell(cell);

                Image logo = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "/img/headlines/" + Icon + ".png");
                logo.ScaleAbsolute(50f, 50f);
                cell = new PdfPCell(logo);
                cell.BorderWidth = 0;
                cell.FixedHeight = 75f;
                table.AddCell(cell);

                if (KPIKey == "61" || KPIKey == "66" || KPIKey == "73")
                {
                    table.AddCell(leftNoBorderCell(""));
                }
        }
        PdfDashboard.Add(table);
    }

    private Paragraph addTitle(string kpi)
    {
        noData = true;
        var kpiTitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "100");
        if (kpiTitle != null)
        {
            noData = false;
            title = new Paragraph(kpiTitle.GetValue("RowDescription").ToString(), KPITitleFont);
            if (kpi == "63") {  title = new Paragraph(kpiTitle.GetValue("ParameterValueName").ToString(), KPITitleFont); }
            if (kpi == "TP") { title.SpacingBefore = 50; }
        }
        if (noData == true) { title = new Paragraph(""); }
        title.Alignment = Element.ALIGN_CENTER;
        return title;
    }

    private void addGraph(string kpi, string server)
    {
        int width = 540;
        int height = 257;
        if (kpi == "81") { height = 240; }
        else if (kpi == "91") { height = 440; }

        //graph = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "/content/Charts/chartimage_" + userKey + "_chartData" + kpi + ".png");
        graph = Image.GetInstance("https://" + server + "/content/Charts/chartimage_" + userKey + "_chartData" + kpi + ".png");
        graph.ScaleAbsolute(width, height);
        PdfDashboard.Add(graph);
        
        if (kpi == "81" || ((kpi == "611" || kpi == "612") && getString(kpiSubtitle, "Col4") != "-"))
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/content/Charts/chartimage_" + userKey + "_chartData" + kpi + "0.png"))
            {
                graph = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "/content/Charts/chartimage_" + userKey + "_chartData" + kpi + "0.png");
                graph.ScaleAbsolute(width, height);
                PdfDashboard.Add(graph);
            }
        }
    }

    private void addSectionTitle(string kpi)
    {
        string section = "";
        switch (kpi)
        {
            case "61":
                section = "Client";
                break;
            case "71":
                section = "People";
                break;
            case "81":
                section = "Profit & Loss";
                break;
            case "91":
                section = "Innovation";
                break;
            case "Headlines":
                section = "Headlines";
                break;
        }
        if (section != "")
        {
            table = new PdfPTable(1);
            Phrase sectionTitle = new Phrase(section, SectionTitleFont);
            cell = new PdfPCell(sectionTitle);
            cell.UseAscender = true;
            cell.Padding = 5;
            cell.VerticalAlignment = 1;
            table.AddCell(cell);
            if (section == "Headlines") { table.SpacingAfter = 15; }
            else { table.WidthPercentage = 100f; }
            PdfDashboard.Add(table);
        }
    }

    private PdfPCell setBorderWidth (PdfPCell cell, string borderClass, Boolean top, Boolean bottom, Boolean left, Boolean right)
    {
        switch (borderClass)
        {
            case "NoBottomBorderRow NoTopBorderRow":
                top = false; bottom = false; break;
            case "NoBottomBorderRow":
                bottom = false; break;
            case "NoTopBorderRow":
                top = false; break;
            case "NoSideBorders":
                left = false; right = false; break;
        }
        if (top) { cell.BorderWidthTop = 0.1f; } else { cell.BorderWidthTop = 0; }
        if (bottom) { cell.BorderWidthBottom = 0.1f; } else { cell.BorderWidthBottom = 0; }
        if (left) { cell.BorderWidthLeft = 0.1f; } else { cell.BorderWidthLeft = 0; }
        if (right) { cell.BorderWidthRight = 0.1f; } else { cell.BorderWidthRight = 0; }
        return cell;
    }

    private PdfPCell rightCell(string content, string borderClass = "", Boolean top = true, Boolean bottom = true, Boolean left = true, Boolean right = true)
    {
        cell = new PdfPCell(new Phrase(content, new Font(Font.FontFamily.HELVETICA, 8)));
        cell = setBorderWidth(cell, borderClass, top, bottom, left, right);
        cell.HorizontalAlignment = 2;
        return cell;
    }

    private PdfPCell rightBoldCell(string content, string borderClass = "", Boolean top = true, Boolean bottom = true, Boolean left = true, Boolean right = true)
    {
        cell = new PdfPCell(new Phrase(content, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        cell.HorizontalAlignment = 2;
        cell = setBorderWidth(cell, borderClass, top, bottom, left, right);
        return cell;
    }

    private PdfPCell leftNoBorderCell(string content)
    {
        cell = new PdfPCell(new Phrase(content, new Font(Font.FontFamily.HELVETICA, 8)));
        cell.BorderWidth = 0;
        return cell;
    }

    private PdfPCell columnHeadCell(string content, string borderClass = "", Boolean top = true, Boolean bottom = true, Boolean left = true, Boolean right = true)
    {
        cell = new PdfPCell(new Phrase(content, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        cell.HorizontalAlignment = 2;
        cell.BackgroundColor = new BaseColor(230, 230, 230);
        cell = setBorderWidth(cell, borderClass, top, bottom, left, right);
        return cell;
    }

    private PdfPCell firstTopLeftCell(string content, string borderClass = "", Boolean top = true, Boolean bottom = true, Boolean left = true, Boolean right = false)
    {
        cell = new PdfPCell(new Phrase(content, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        cell.BackgroundColor = new BaseColor(230, 230, 230);
        cell = setBorderWidth(cell, borderClass, top, bottom, left, right);
        return cell;
    }

    private PdfPCell secondTopLeftCell(string content, string borderClass = "", Boolean top = true, Boolean bottom = true, Boolean left = false, Boolean right = true)
    {
        cell = new PdfPCell(new Phrase(content, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        cell.BackgroundColor = new BaseColor(230, 230, 230);
        cell = setBorderWidth(cell, borderClass, top, bottom, left, right);
        return cell;
    }

    private PdfPCell trafficLightCell(string kpiColor = "", string borderClass = "", Boolean top = true, Boolean bottom = true, Boolean left = false, Boolean right = true)
    {
        Phrase trafficLightPhrase = new Phrase("");
        if (kpiColor != "" && kpiColor != null && kpiColor != "kpiGrey")
        {
            trafficLightPhrase.Add(new Chunk(dotColor(kpiColor, "rowTitle"), -6, -7));
        }
        cell = new PdfPCell(trafficLightPhrase);
        cell = setBorderWidth(cell, borderClass, top, bottom, left, right);
        return cell;
    } 

    private PdfPCell rowTitleCell(string content, string borderClass = "", Boolean top = true, Boolean bottom = true, Boolean left = true, Boolean right = false)
    {
        rowTitle = new Phrase(content, new Font(Font.FontFamily.HELVETICA, 8));
        cell = new PdfPCell(rowTitle);
        cell = setBorderWidth(cell, borderClass, top, bottom, left, right);
        return cell;
    }

    private PdfPCell rowTitleBoldCell(string content, string borderClass = "", Boolean top = true, Boolean bottom = true, Boolean left = true, Boolean right = false)
    {
        rowTitle = new Phrase(content, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD));
        cell = new PdfPCell(rowTitle);
        cell = setBorderWidth(cell, borderClass, top, bottom, left, right);
        return cell;
    }

    private PdfPCell rowTitleNoBorderBoldCell(string content)
    {
        cell = new PdfPCell(new Phrase(content, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        cell.BorderWidth = 0;
        return cell;
    }

    private PdfPTable addParams ()
    {
        var parameters = jsonData.Children<JObject>().Where(p => p["RecordType"].ToString() == "parameter");
        PdfPTable paramsTable = new PdfPTable(3);
        widths = new float[] { 0.5f, 1.7f, 1.9f };
        paramsTable.SetWidths(widths);
        foreach (var param in parameters)
        {
            paramsTable.AddCell(leftNoBorderCell(""));
            if (getString(param, "ParameterType") == "0") { paramsTable.AddCell(rowTitleNoBorderBoldCell("Generated by")); }
            else { paramsTable.AddCell(rowTitleNoBorderBoldCell(getString(param, "ParameterName"))); }
            paramsTable.AddCell(leftNoBorderCell(getString(param, "ParameterValueName")));
        }
        paramsTable.SpacingAfter = 75;
        paramsTable.WidthPercentage = 50f;
        return paramsTable;
    }

    private Image dotColor (string kpiColor, string type = "subtitle")
    {
        string color = "";
        if (kpiColor.Trim() == "fa fa-circle kpiGreen") { color = "green"; }
        else if (kpiColor.Trim() == "fa fa-circle kpiYellow") { color = "yellow"; }
        else if (kpiColor.Trim() == "fa fa-circle kpiRed") { color = "red"; }
        else { color = "white"; }
        dot = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "/img/" + color + "-dot.png");
        if (type == "subtitle") { dot.ScaleAbsolute(12, 12); }
        else { dot.ScaleAbsolute(6, 6); }
        return dot;
    }

    private void addSubtitle(string kpi)
    {   switch(kpi)
        {
            case "61":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "4");
                subtitle = new Paragraph("");
                noData = true;
                if (kpiSubtitle != null)
                {
                    noData = false;
                    subtitle = new Paragraph("Value YTD: " + getString(kpiSubtitle, "Col1") + " " + getString(kpiSubtitle, "Rounding") + " (" + getString(kpiSubtitle, "Col3") + ")", KPISubtitleFont);
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            case "62":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "1");
                subtitle = new Paragraph("");
                noData = true;
                if (kpiSubtitle != null)
                {
                    noData = false;
                    subtitle = new Paragraph("Value YTD: " + getString(kpiSubtitle, "Col4") + " (" + getString(kpiSubtitle, "Col6") + ")", KPISubtitleFont);
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            case "63":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "1");
                subtitle = new Paragraph("");
                noData = true;
                noGraph = true;
                if (kpiSubtitle != null)
                {
                    if (getString(kpiSubtitle, "Col1") + getString(kpiSubtitle, "Col2") + getString(kpiSubtitle, "Col3") + getString(kpiSubtitle, "Col4") + getString(kpiSubtitle, "Col5") + getString(kpiSubtitle, "Col6") != "------")
                    { noGraph = false; }
                    noData = false;
                    subtitle = new Paragraph("Compliance YTD: " + getString(kpiSubtitle, "Col4") + " (" + getString(kpiSubtitle, "Col6") + ")", KPISubtitleFont);
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            case "65":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "1");
                subtitle = new Paragraph("");
                noData = true;
                noGraph = true;
                if (kpiSubtitle != null)
                {
                    if (getString(kpiSubtitle, "Col1") + getString(kpiSubtitle, "Col2") + getString(kpiSubtitle, "Col3") + getString(kpiSubtitle, "Col4") + getString(kpiSubtitle, "Col5") + getString(kpiSubtitle, "Col6") != "------")
                    { noGraph = false; }
                    noData = false;
                    subtitle = new Paragraph("Compliance YTD: " + getString(kpiSubtitle, "Col4") + " (" + getString(kpiSubtitle, "Col6") + ")", KPISubtitleFont);
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            case "66":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "2");
                subtitle = new Paragraph("");
                noData = true;
                if (kpiSubtitle != null)
                {
                    noData = false;
                    subtitle = new Paragraph("Total AOT YTD: " + getString(kpiSubtitle, "Col4") + " " + getString(kpiSubtitle, "Rounding") + " (" + getString(kpiSubtitle, "Col6") + ")", KPISubtitleFont);
                    subtitle.Add(new Chunk(dotColor(getString(kpiSubtitle, "KPIColor")), 4, -2));
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;

                PdfDashboard.Add(subtitle);

                break;
            case "67":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "1");
                subtitle = new Paragraph("");
                noData = true;
                if (kpiSubtitle != null)
                {
                    noData = false;
                    subtitle = new Paragraph("YTD: " + getString(kpiSubtitle, "Col4") + " (" + getString(kpiSubtitle, "Col6") + ")", KPISubtitleFont);
                    subtitle.Add(new Chunk(dotColor(getString(kpiSubtitle, "KPIColor")), 4, -2));
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            case "69":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "1");
                subtitle = new Paragraph("No data found");
                noData = true;
                noGraph = true;
                if (kpiSubtitle != null)
                {
                    noData = false;
                    subtitle = new Paragraph("N/A");
                }
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "7");
                if (kpiSubtitle != null)
                {
                    subtitle = new Paragraph("Total: " + getString(kpiSubtitle, "Col1") + " " + getString(kpiSubtitle, "Rounding") + " (" + getString(kpiSubtitle, "Col3") + ")", KPISubtitleFont);
                    noGraph = false;
                }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            case "610":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "7");
                subtitle = new Paragraph("");
                noData = true;
                if (kpiSubtitle != null)
                {
                    noData = false;
                    subtitle = new Paragraph("Total: " + getString(kpiSubtitle, "Col1") + " " + getString(kpiSubtitle, "Rounding") + " (" + getString(kpiSubtitle, "Col3") + ")", KPISubtitleFont);
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            case "611":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "11");
                subtitle = new Paragraph("");
                noData = true;
                if (kpiSubtitle != null)
                {
                    noData = false;
                    subtitle = new Paragraph("Average Satisfaction Score: " + getString(kpiSubtitle, "Col4") + " (" + getString(kpiSubtitle, "Col6") + ")", KPISubtitleFont);
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            case "612":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "11");
                subtitle = new Paragraph("");
                noData = true;
                if (kpiSubtitle != null)
                {
                    noData = false;
                    subtitle = new Paragraph("Average Satisfaction Score: " + getString(kpiSubtitle, "Col4") + " (" + getString(kpiSubtitle, "Col6") + ")", KPISubtitleFont);
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            case "71":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "5");
                subtitle = new Paragraph("");
                noData = true;
                if (kpiSubtitle != null)
                {
                    noData = false;
                    subtitle = new Paragraph("Total Closing YTD: " + getString(kpiSubtitle, "Col5") + " (" + getString(kpiSubtitle, "Col6") + ")", KPISubtitleFont);
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            case "72":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "5");
                subtitle = new Paragraph("");
                noData = true;
                if (kpiSubtitle != null)
                {
                    noData = false;
                    subtitle = new Paragraph("Total Turnover: " + getString(kpiSubtitle, "Col9") + " (" + getString(kpiSubtitle, "Col11") + ")", KPISubtitleFont);
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            case "73":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "5");
                subtitle = new Paragraph("");
                noData = true;
                if (kpiSubtitle != null)
                {
                    noData = false;
                    subtitle = new Paragraph("Total: " + getString(kpiSubtitle, "Col4") + " (" + getString(kpiSubtitle, "Col6") + ")", KPISubtitleFont);
                    subtitle.Add(new Chunk(dotColor(getString(kpiSubtitle, "KPIColor")), 4, -2));
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            case "74":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "5");
                subtitle = new Paragraph("");
                noData = true;
                if (kpiSubtitle != null)
                {
                    noData = false;
                    subtitle = new Paragraph("Total: " + getString(kpiSubtitle, "Col4"), KPISubtitleFont);
                    subtitle.Add(new Chunk(dotColor(getString(kpiSubtitle, "KPIColor")), 4, -2));
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            case "81":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "1");
                noData = true;
                noGraph = true;
                subtitle = new Paragraph("N/A");
                if (kpiSubtitle != null)
                {
                    noData = false;
                }
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "10");
                if (kpiSubtitle != null)
                {
                    noGraph = false;
                    subtitle = new Paragraph("% OPBBGC/Total Revenue: " + getString(kpiSubtitle, "Col6") + " (" + getString(kpiSubtitle, "Col9") + ")", KPISubtitleFont);
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            case "91":
                kpiSubtitle = jsonData.Children<JObject>().FirstOrDefault(p => p["KPIKey"].ToString() == kpi && p["RowKey"].ToString() == "24");
                subtitle = new Paragraph("");
                noData = true;
                if (kpiSubtitle != null)
                {
                    noData = false;
                    subtitle = new Paragraph("YTD: " + getString(kpiSubtitle, "Col4") + " " + getString(kpiSubtitle, "Rounding") + " (" + getString(kpiSubtitle, "Col6") + ")", KPISubtitleFont);
                }
                if (noData == true) { subtitle = new Paragraph("No data found"); }
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 15;
                PdfDashboard.Add(subtitle);
                break;
            default:
                subtitle = new Paragraph("");
                subtitle.SpacingAfter = 25;
                PdfDashboard.Add(subtitle);
                break;
        }
    }

    private PdfPTable addTable(string kpi)
    {
        kpiTableHeader = jsonData.Children<JObject>().Where(p => p["RecordType"].ToString() == "columnTitles" && p["KPIKey"].ToString() == kpi);
        kpiTableContent = jsonData.Children<JObject>().Where(p => p["RecordType"].ToString() == "tableData" && p["KPIKey"].ToString() == kpi);
        switch (kpi)
        {
            case "TP":
                table = new PdfPTable(2);
                widths = new float[] { 4f, 1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell("Client"));
                table.AddCell(columnHeadCell("Amount"));
                foreach (JObject row in kpiTableContent)
                {
                    table.AddCell(rowTitleCell(getString(row, "RowDescription")));
                    table.AddCell(rightCell((getString(row, "Col1"))));
                }
                table.WidthPercentage = 70f;
                table.SpacingAfter = 15;
                return table;
            case "61":
                table = new PdfPTable(5);
                widths = new float[] { 1.5f, 0.1f, 1f, 1f, 1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    table.AddCell(rowTitleCell(getString(row, "RowDescription")));
                    table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                    table.AddCell(rightCell(getString(row, "Col1")));
                    table.AddCell(rightCell(getString(row, "Col2")));
                    table.AddCell(rightCell(getString(row, "Col3")));
                }
                table.WidthPercentage = 100;
                table.SpacingAfter = 15;
                return table;
            case "62":
                table = new PdfPTable(8);
                widths = new float[] { 2f, 0.1f, 1f, 1f, 1f, 1f, 1f, 1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    table.AddCell(rowTitleCell(getString(row, "RowDescription")));
                        table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                        table.AddCell(rightCell(getString(row, "Col1")));
                        table.AddCell(rightCell(getString(row, "Col2")));
                        table.AddCell(rightCell(getString(row, "Col3")));
                        table.AddCell(rightCell(getString(row, "Col4")));
                        table.AddCell(rightCell(getString(row, "Col5")));
                        table.AddCell(rightCell(getString(row, "Col6")));
                }
                table.WidthPercentage = 100f;
                table.SpacingAfter = 15;
                return table;
            case "63":
                table = new PdfPTable(8);
                widths = new float[] { 1.5f, 0.1f, 1f, 1f, 1f, 1f, 1f, 1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    table.AddCell(rowTitleCell(getString(row, "RowDescription")));
                    table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                    table.AddCell(rightCell(getString(row, "Col1")));
                    table.AddCell(rightCell(getString(row, "Col2")));
                    table.AddCell(rightCell(getString(row, "Col3")));
                    table.AddCell(rightCell(getString(row, "Col4")));
                    table.AddCell(rightCell(getString(row, "Col5")));
                    table.AddCell(rightCell(getString(row, "Col6")));
                }
                table.WidthPercentage = 100f;
                table.SpacingAfter = 15;
                return table;
            case "65":
                table = new PdfPTable(8);
                widths = new float[] { 1.5f, 0.1f, 1f, 1f, 1f, 1f, 1f, 1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    table.AddCell(rowTitleCell(getString(row, "RowDescription")));
                    table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                    table.AddCell(rightCell(getString(row, "Col1")));
                    table.AddCell(rightCell(getString(row, "Col2")));
                    table.AddCell(rightCell(getString(row, "Col3")));
                    table.AddCell(rightCell(getString(row, "Col4")));
                    table.AddCell(rightCell(getString(row, "Col5")));
                    table.AddCell(rightCell(getString(row, "Col6")));
                }
                table.WidthPercentage = 100f;
                table.SpacingAfter = 15;
                return table;
            case "66":
                table = new PdfPTable(13);
                widths = new float[] { 2f, 0.1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    table.AddCell(rowTitleCell(getString(row, "RowDescription")));
                    table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                    table.AddCell(rightCell(getString(row, "Col1")));
                    table.AddCell(rightCell(getString(row, "Col2")));
                    table.AddCell(rightCell(getString(row, "Col3")));
                    table.AddCell(rightCell(getString(row, "Col4")));
                    table.AddCell(rightCell(getString(row, "Col5")));
                    table.AddCell(rightCell(getString(row, "Col6")));
                    table.AddCell(rightCell(getString(row, "Col7")));
                    table.AddCell(rightCell(getString(row, "Col8")));
                    table.AddCell(rightCell(getString(row, "Col9")));
                    table.AddCell(rightCell(getString(row, "Col10")));
                    table.AddCell(rightCell(getString(row, "Col11")));
                }
                table.WidthPercentage = 100f;
                table.SpacingAfter = 15;
                return table;
            case "67":
                table = new PdfPTable(9);
                widths = new float[] { 1.5f, 0.1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    table.AddCell(rowTitleCell(getString(row, "RowDescription")));
                    table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                    table.AddCell(rightCell(getString(row, "Col1")));
                    table.AddCell(rightCell(getString(row, "Col2")));
                    table.AddCell(rightCell(getString(row, "Col3")));
                    table.AddCell(rightCell(getString(row, "Col4")));
                    table.AddCell(rightCell(getString(row, "Col5")));
                    table.AddCell(rightCell(getString(row, "Col6")));
                    table.AddCell(rightCell(getString(row, "Col7")));
                }
                table.WidthPercentage = 100f;
                table.SpacingAfter = 15;
                return table;
            case "69":
                table = new PdfPTable(7);
                widths = new float[] { 3f, 0.1f, 1f, 1f, 1f, 1f, 1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    if (getString(row, "RowKey") == "7") {
                        table.AddCell(rowTitleBoldCell(getString(row, "RowDescription")));
                        table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                        table.AddCell(rightBoldCell(getString(row, "Col1")));
                        table.AddCell(rightBoldCell(getString(row, "Col2")));
                        table.AddCell(rightBoldCell(getString(row, "Col3")));
                        table.AddCell(rightBoldCell(getString(row, "Col4")));
                        table.AddCell(rightBoldCell(getString(row, "Col5")));
                    }
                    else {
                        table.AddCell(rowTitleCell(getString(row, "RowDescription"), getString(row, "Class")));
                        table.AddCell(trafficLightCell(getString(row, "KPIColor"), getString(row, "Class")));
                        table.AddCell(rightCell(getString(row, "Col1"), getString(row, "Class")));
                        table.AddCell(rightCell(getString(row, "Col2"), getString(row, "Class")));
                        table.AddCell(rightCell(getString(row, "Col3"), getString(row, "Class")));
                        table.AddCell(rightCell(getString(row, "Col4"), getString(row, "Class")));
                        table.AddCell(rightCell(getString(row, "Col5"), getString(row, "Class")));
                    }
                }
                table.WidthPercentage = 100f;
                table.SpacingAfter = 15;
                return table;
            case "610":
                table = new PdfPTable(7);
                widths = new float[] { 2f, 0.1f, 1f, 1f, 1f, 1f, 1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    if (getString(row, "RowKey") == "6")
                    {
                        table.AddCell(rowTitleBoldCell(getString(row, "RowDescription")));
                        table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                        table.AddCell(rightBoldCell(getString(row, "Col1")));
                        table.AddCell(rightBoldCell(getString(row, "Col2")));
                        table.AddCell(rightBoldCell(getString(row, "Col3")));
                        table.AddCell(rightBoldCell(getString(row, "Col4")));
                        table.AddCell(rightBoldCell(getString(row, "Col5")));
                    }
                    else
                    {
                        table.AddCell(rowTitleCell(getString(row, "RowDescription"), getString(row, "Class")));
                        table.AddCell(trafficLightCell(getString(row, "KPIColor"), getString(row, "Class")));
                        table.AddCell(rightCell(getString(row, "Col1"), getString(row, "Class")));
                        table.AddCell(rightCell(getString(row, "Col2"), getString(row, "Class")));
                        table.AddCell(rightCell(getString(row, "Col3"), getString(row, "Class")));
                        table.AddCell(rightCell(getString(row, "Col4"), getString(row, "Class")));
                        table.AddCell(rightCell(getString(row, "Col5"), getString(row, "Class")));
                    }
                }
                table.WidthPercentage = 100f;
                table.SpacingAfter = 15;
                return table;
            case "611":
                table = new PdfPTable(9);
                widths = new float[] { 2.6f, 0.12f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    table.AddCell(rowTitleCell(getString(row, "RowDescription"), getString(row, "Class")));
                    table.AddCell(trafficLightCell(getString(row, "KPIColor"), getString(row, "Class")));
                    table.AddCell(rightCell(getString(row, "Col1"), getString(row, "Class")));
                    table.AddCell(rightCell(getString(row, "Col2"), getString(row, "Class")));
                    table.AddCell(rightCell(getString(row, "Col3"), getString(row, "Class")));
                    table.AddCell(rightCell(getString(row, "Col4"), getString(row, "Class")));
                    table.AddCell(rightCell(getString(row, "Col5"), getString(row, "Class")));
                    table.AddCell(rightCell(getString(row, "Col6"), getString(row, "Class")));
                    table.AddCell(rightCell(getString(row, "Col7"), getString(row, "Class")));
                }
                table.WidthPercentage = 100f;
                table.SpacingAfter = 15;
                return table;
            case "612":
                table = new PdfPTable(9);
                widths = new float[] { 2.5f, 0.12f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    table.AddCell(rowTitleCell(getString(row, "RowDescription"), getString(row, "Class")));
                    table.AddCell(trafficLightCell(getString(row, "KPIColor"), getString(row, "Class")));
                    table.AddCell(rightCell(getString(row, "Col1"), getString(row, "Class")));
                    table.AddCell(rightCell(getString(row, "Col2"), getString(row, "Class")));
                    table.AddCell(rightCell(getString(row, "Col3"), getString(row, "Class")));
                    table.AddCell(rightCell(getString(row, "Col4"), getString(row, "Class")));
                    table.AddCell(rightCell(getString(row, "Col5"), getString(row, "Class")));
                    table.AddCell(rightCell(getString(row, "Col6"), getString(row, "Class")));
                    table.AddCell(rightCell(getString(row, "Col7"), getString(row, "Class")));
                }
                table.WidthPercentage = 100f;
                table.SpacingAfter = 15;
                return table;
            case "71":
                table = new PdfPTable(8);
                widths = new float[] { 2f, 0.1f, 1f, 1f, 1f, 1f, 1f, 1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    if (getString(row, "RowKey") == "5") {
                        table.AddCell(rowTitleBoldCell(getString(row, "RowDescription")));
                        table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                        table.AddCell(rightBoldCell(getString(row, "Col1")));
                        table.AddCell(rightBoldCell(getString(row, "Col2")));
                        table.AddCell(rightBoldCell(getString(row, "Col3")));
                        table.AddCell(rightBoldCell(getString(row, "Col4")));
                        table.AddCell(rightBoldCell(getString(row, "Col5")));
                        table.AddCell(rightBoldCell(getString(row, "Col6")));
                    }
                    else
                    {
                        table.AddCell(rowTitleCell(getString(row, "RowDescription")));
                        table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                        table.AddCell(rightCell(getString(row, "Col1")));
                        table.AddCell(rightCell(getString(row, "Col2")));
                        table.AddCell(rightCell(getString(row, "Col3")));
                        table.AddCell(rightCell(getString(row, "Col4")));
                        table.AddCell(rightCell(getString(row, "Col5")));
                        table.AddCell(rightCell(getString(row, "Col6")));
                    }
                }
                table.WidthPercentage = 100f;
                table.SpacingAfter = 15;
                return table;
            case "72":
                table = new PdfPTable(13);
                widths = new float[] { 1.3f, 0.1f, 1f, 1f, 1f, 1.1f, 1.1f, 1.1f, 1f, 1f, 1f, 1f, 1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    if (getString(row, "RowKey") == "5")
                    {
                        table.AddCell(rowTitleBoldCell(getString(row, "RowDescription")));
                        table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                        table.AddCell(rightBoldCell(getString(row, "Col1")));
                        table.AddCell(rightBoldCell(getString(row, "Col2")));
                        table.AddCell(rightBoldCell(getString(row, "Col3")));
                        table.AddCell(rightBoldCell(getString(row, "Col4")));
                        table.AddCell(rightBoldCell(getString(row, "Col5")));
                        table.AddCell(rightBoldCell(getString(row, "Col6")));
                        table.AddCell(rightBoldCell(getString(row, "Col7")));
                        table.AddCell(rightBoldCell(getString(row, "Col8")));
                        table.AddCell(rightBoldCell(getString(row, "Col9")));
                        table.AddCell(rightBoldCell(getString(row, "Col10")));
                        table.AddCell(rightBoldCell(getString(row, "Col11")));
                    }
                    else
                    {
                        table.AddCell(rowTitleCell(getString(row, "RowDescription")));
                        table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                        table.AddCell(rightCell(getString(row, "Col1")));
                        table.AddCell(rightCell(getString(row, "Col2")));
                        table.AddCell(rightCell(getString(row, "Col3")));
                        table.AddCell(rightCell(getString(row, "Col4")));
                        table.AddCell(rightCell(getString(row, "Col5")));
                        table.AddCell(rightCell(getString(row, "Col6")));
                        table.AddCell(rightCell(getString(row, "Col7")));
                        table.AddCell(rightCell(getString(row, "Col8")));
                        table.AddCell(rightCell(getString(row, "Col9")));
                        table.AddCell(rightCell(getString(row, "Col10")));
                        table.AddCell(rightCell(getString(row, "Col11")));
                    }
                }
                table.WidthPercentage = 100f;
                table.SpacingAfter = 15;
                return table;
            case "73":
                table = new PdfPTable(9);
                widths = new float[] { 1f, 0.1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f};
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    if (getString(row, "RowKey") == "5")
                    {
                        table.AddCell(rowTitleBoldCell(getString(row, "RowDescription")));
                        table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                        table.AddCell(rightBoldCell(getString(row, "Col1")));
                        table.AddCell(rightBoldCell(getString(row, "Col2")));
                        table.AddCell(rightBoldCell(getString(row, "Col3")));
                        table.AddCell(rightBoldCell(getString(row, "Col4")));
                        table.AddCell(rightBoldCell(getString(row, "Col5")));
                        table.AddCell(rightBoldCell(getString(row, "Col6")));
                        table.AddCell(rightBoldCell(getString(row, "Col7")));
                    }
                    else
                    {
                        table.AddCell(rowTitleCell(getString(row, "RowDescription")));
                        table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                        table.AddCell(rightCell(getString(row, "Col1")));
                        table.AddCell(rightCell(getString(row, "Col2")));
                        table.AddCell(rightCell(getString(row, "Col3")));
                        table.AddCell(rightCell(getString(row, "Col4")));
                        table.AddCell(rightCell(getString(row, "Col5")));
                        table.AddCell(rightCell(getString(row, "Col6")));
                        table.AddCell(rightCell(getString(row, "Col7")));
                    }
                }
                table.WidthPercentage = 100f;
                table.SpacingAfter = 15;
                return table;
            case "74":
                table = new PdfPTable(8);
                widths = new float[] { 1f, 0.1f, 1f, 1f, 1f, 1f, 1f, 1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    if (getString(row, "RowKey") == "5")
                    {
                        table.AddCell(rowTitleBoldCell(getString(row, "RowDescription")));
                        table.AddCell(trafficLightCell());
                        table.AddCell(rightBoldCell(getString(row, "Col1")));
                        table.AddCell(rightBoldCell(getString(row, "Col2")));
                        table.AddCell(rightBoldCell(getString(row, "Col3")));
                        table.AddCell(rightBoldCell(getString(row, "Col4")));
                        table.AddCell(rightBoldCell(getString(row, "Col5")));
                        table.AddCell(rightBoldCell(getString(row, "Col6")));
                    }
                    else
                    {
                        table.AddCell(rowTitleCell(getString(row, "RowDescription")));
                        table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                        table.AddCell(rightCell(getString(row, "Col1")));
                        table.AddCell(rightCell(getString(row, "Col2")));
                        table.AddCell(rightCell(getString(row, "Col3")));
                        table.AddCell(rightCell(getString(row, "Col4")));
                        table.AddCell(rightCell(getString(row, "Col5")));
                        table.AddCell(rightCell(getString(row, "Col6")));
                    }
                }
                table.WidthPercentage = 100f;
                table.SpacingAfter = 15;
                return table;
            case "81":
                table = new PdfPTable(13);
                widths = new float[] { 3f, 0.2f, 1f, 1f, 1f, 1f, 1f, 1.1f, 1f, 1f, 1f, 1f, 1.1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell("", "NoSideBorders"));
                table.AddCell(secondTopLeftCell("", "NoSideBorders"));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription"), "NoSideBorders"));
                }
                foreach (JObject row in kpiTableContent)
                {
                    if (getString(row, "RowKey") == "2" || getString(row, "RowKey") == "4" || getString(row, "RowKey") == "7" || getString(row, "RowKey") == "10")
                    {
                        table.AddCell(rowTitleBoldCell(getString(row, "RowDescription"), "", true, true, false, false));
                        table.AddCell(trafficLightCell(getString(row, "KPIColor"), "", true, true, false, false));
                        table.AddCell(rightBoldCell(getString(row, "Col1"), "", true, true, false, false));
                        table.AddCell(rightBoldCell(getString(row, "Col2"), "", true, true, false, false));
                        table.AddCell(rightBoldCell(getString(row, "Col3"), "", true, true, false, false));
                        table.AddCell(rightBoldCell(getString(row, "Col4"), "", true, true, false, false));
                        table.AddCell(rightBoldCell(getString(row, "Col5"), "", true, true, false, false));
                        table.AddCell(rightBoldCell(getString(row, "Col6"), "", true, true, false, false));
                        table.AddCell(rightBoldCell(getString(row, "Col7"), "", true, true, false, false));
                        table.AddCell(rightBoldCell(getString(row, "Col8"), "", true, true, false, false));
                        table.AddCell(rightBoldCell(getString(row, "Col9"), "", true, true, false, false));
                        table.AddCell(rightBoldCell(getString(row, "Col10"), "", true, true, false, false));
                        table.AddCell(rightBoldCell(getString(row, "Col11"), "", true, true, false, false));
                    }
                    else
                    {
                        table.AddCell(rowTitleCell(getString(row, "RowDescription"), "", true, true, false, false));
                        table.AddCell(trafficLightCell(getString(row, "KPIColor"), "", true, true, false, false));
                        table.AddCell(rightCell(getString(row, "Col1"), "", true, true, false, false));
                        table.AddCell(rightCell(getString(row, "Col2"), "", true, true, false, false));
                        table.AddCell(rightCell(getString(row, "Col3"), "", true, true, false, false));
                        table.AddCell(rightCell(getString(row, "Col4"), "", true, true, false, false));
                        table.AddCell(rightCell(getString(row, "Col5"), "", true, true, false, false));
                        table.AddCell(rightCell(getString(row, "Col6"), "", true, true, false, false));
                        table.AddCell(rightCell(getString(row, "Col7"), "", true, true, false, false));
                        table.AddCell(rightCell(getString(row, "Col8"), "", true, true, false, false));
                        table.AddCell(rightCell(getString(row, "Col9"), "", true, true, false, false));
                        table.AddCell(rightCell(getString(row, "Col10"), "", true, true, false, false));
                        table.AddCell(rightCell(getString(row, "Col11"), "", true, true, false, false));
                    }
                };
                table.WidthPercentage = 100f;
                table.SpacingAfter = 5;
                return table;
            case "91":
                table = new PdfPTable(13);
                widths = new float[] { 3f, 0.1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1.1f, 1.1f };
                table.SetWidths(widths);
                table.AddCell(firstTopLeftCell(""));
                table.AddCell(secondTopLeftCell(""));
                foreach (JObject column in kpiTableHeader)
                {
                    table.AddCell(columnHeadCell(getString(column, "RowDescription")));
                }
                foreach (JObject row in kpiTableContent)
                {
                    if (getString(row, "RowKey") == "24" && getString(row, "Col4") == "0")
                    {
                        noGraph = true;
                    }
                    table.AddCell(rowTitleCell(getString(row, "RowDescription")));
                    table.AddCell(trafficLightCell(getString(row, "KPIColor")));
                    table.AddCell(rightCell(getString(row, "Col1")));
                    table.AddCell(rightCell(getString(row, "Col2")));
                    table.AddCell(rightCell(getString(row, "Col3")));
                    table.AddCell(rightCell(getString(row, "Col4")));
                    table.AddCell(rightCell(getString(row, "Col5")));
                    table.AddCell(rightCell(getString(row, "Col6")));
                    table.AddCell(rightCell(getString(row, "Col7")));
                    table.AddCell(rightCell(getString(row, "Col8")));
                    table.AddCell(rightCell(getString(row, "Col9")));
                    table.AddCell(rightCell(getString(row, "Col10")));
                    table.AddCell(rightCell(getString(row, "Col11")));
                };
                table.WidthPercentage = 100f;
                table.SpacingAfter = 15;
                return table;
            default:
                table = new PdfPTable(4);
                return table;
        }
    }
}