using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using System.Collections.Generic;

/// <summary>
/// Summary description for Chart
/// </summary>
public class chart
{
    public class ChartRender : IHttpHandler
    {
       
        private enum ChartOptionsIndex
        {
            ChartTitle1,
            ChartTitle2,
            ChartTitle3,
            ChartTitle4,
            ChartHeight,
            ChartWidth,
            OtherOption1,
            OtherOption2
        }
        private enum SeriesOptionsIndex
        {
            SeriesName,
            SeriesType,
            SeriesColor,
            SeriesAxis,
            SeriesVisible,
            SeriesHideZeros
        }

        Dictionary<string, Color> Colors = new Dictionary<string, Color>()    {
            {"Completely Satisfied", System.Drawing.ColorTranslator.FromHtml("#33BF33")},
            {"Very Satisfied", System.Drawing.ColorTranslator.FromHtml("#228B22")},
            {"Satisfied", System.Drawing.ColorTranslator.FromHtml("#F1BE48")},
            {"Dissatisfied", System.Drawing.ColorTranslator.FromHtml("#FF0000")}
        };

        private string ErrorMessage(int p_tableRef)
        {
            return ("Title " + p_tableRef.ToString() + " is missing from source table");
        }
        private string GetOptionValue(string p_optionType, string p_arrayValue)
        {
            string returnValue = "";
            returnValue = p_arrayValue;
            returnValue = returnValue.Substring(returnValue.IndexOf(':') + 1);
            switch (p_optionType)
            {
                case "ChartTitle1":
                    if (returnValue == null)
                    {
                        returnValue = "";
                    }
                    return returnValue;

                case "ChartTitle2":
                    if (returnValue == null)
                    {
                        returnValue = "";
                    }
                    return returnValue;

                case "ChartTitle3":
                    if (returnValue == null)
                    {
                        returnValue = "";
                    }
                    return returnValue;

                case "ChartTitle4":
                    if (returnValue == null)
                    {
                        returnValue = "";
                    }
                    return returnValue;

                case "ChartHeight":
                    if (returnValue == null)
                    {
                        returnValue = "400";
                    }
                    return returnValue;

                case "ChartWidth":
                    if (returnValue == null)
                    {
                        returnValue = "600";
                    }
                    return returnValue;

                case "SeriesName":
                    if (returnValue == null)
                    {
                        returnValue = "";
                    }
                    return returnValue;

                case "SeriesType":
                    if ((returnValue == null) | (returnValue == ""))
                    {
                        returnValue = "Spline";
                    }
                    return returnValue;

                case "SeriesColor":
                    if ((returnValue == null) | (returnValue == ""))
                    {
                        returnValue = "Black";
                    }
                    return returnValue;

                case "SeriesAxis":
                    if ((returnValue == null) | (returnValue == ""))
                    {
                        returnValue = "Primary";
                    }
                    return returnValue;

                case "SeriesVisible":
                    if ((returnValue == null) | (returnValue == ""))
                    {
                        returnValue = "True";
                    }
                    return returnValue;

                case "SeriesHideZeros":
                    if ((returnValue == null) | (returnValue == ""))
                    {
                        returnValue = "True";
                    }
                    return returnValue;
            }
            return returnValue;
        }

        public void ProcessRequest(HttpContext context)
        {
            //System.Diagnostics.Debug.WriteLine("class file called");
          

            string v_userKey = context.Request.QueryString["userKey"];
            string v_chartID = context.Request.QueryString["chartID"];
            string v_dataArg = context.Request.Unvalidated.QueryString["data"];
            

            string v_yFormat = "##,0";
            string v_yFormatAverage = "#0.00";
            string v_yFormatPercent = v_yFormat;
            Chart v_chart = new Chart();
            v_chart.ImageStorageMode = ImageStorageMode.UseImageLocation;
            v_chart.ImageType = ChartImageType.Png;
            v_chart.BackColor = Color.FromArgb(200, 200, 200);
            v_chart.BackGradientStyle = GradientStyle.TopBottom;
            BorderSkin v_skin = new BorderSkin();
            v_skin.SkinStyle = BorderSkinStyle.None;
            v_chart.BorderSkin = v_skin;
            v_chart.AntiAliasing = AntiAliasingStyles.All;
            v_chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
            string[] v_chartOptions = v_dataArg.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            v_chartOptions[0] = v_chartOptions[0].Split(new char[] { '~' })[0];
            string[] v_chartOptionsArray = v_chartOptions[0].Split(new char[] { '|' });
            int index = 0;
            string v_Title1 = GetOptionValue("ChartTitle1", v_chartOptionsArray[0]);
            if (v_Title1 != "")
            {
                v_chart.Titles.Add(v_Title1);
                v_chart.Titles[index].Font = new Font("Calibri", 16f, FontStyle.Bold);
                v_chart.Titles[index].Alignment = ContentAlignment.MiddleCenter;
                v_chart.Titles[index].ForeColor = Color.FromKnownColor(KnownColor.Black);
            }
            string v_Title2 = GetOptionValue("ChartTitle2", v_chartOptionsArray[1]);
            if (v_Title2 != "")
            {
                v_chart.Titles.Add(v_Title2);
                index = v_chart.Titles.Count - 1;
                v_chart.Titles[index].Font = new Font("Calibri", 11f, FontStyle.Regular);
                v_chart.Titles[index].ForeColor = Color.FromKnownColor(KnownColor.Black);
                
            }
            string v_Title3 = GetOptionValue("ChartTitle3", v_chartOptionsArray[2]);
            string v_Title4 = GetOptionValue("ChartTitle4", v_chartOptionsArray[3]);
           
            string v_height = GetOptionValue("ChartHeight", v_chartOptionsArray[4]);
            string v_width = GetOptionValue("ChartWidth", v_chartOptionsArray[5]);
            v_chart.Width = Unit.Pixel(Convert.ToInt32(v_width));
            v_chart.Height = Unit.Pixel(Convert.ToInt32(v_height));
            string[] v_dataRows = v_dataArg.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] v_data_xval = null;
            Boolean hasTwoLinesLegends = false;
            Dictionary<string, double> pieChartData = new Dictionary<string, double>();
            List<Color> colorPalette = new List<Color>();
            string v_firstValue = string.Empty;
            for (int rows = 0; rows < v_dataRows.Length; rows++)
            {
                double[] v_data_yval = null;
                if (rows == 0)
                {
                    v_firstValue = v_dataRows[rows].Split(new char[] { '~' })[0];
                    v_dataRows[rows] = v_dataRows[rows].Remove(0, v_firstValue.Length + 1);
                    v_data_xval = v_dataRows[rows].Split(new char[] { '~' });
                }
                else
                {
                    string[] v_string_yval = v_dataRows[rows].Split(new char[] { '~' });
                    v_data_yval = new double[v_string_yval.Length - 1];
                    string v_seriesName = string.Empty;
                    string v_seriesType = string.Empty;
                    string v_seriesColour = string.Empty;
                    string v_seriesAxis = string.Empty;
                    bool v_seriesVisible = true;
                    bool v_seriesHideZeros = true;
                    for (int cells = 0; cells < v_string_yval.Length; cells++)
                    {
                        if (cells == 0)
                        {
                            string[] v_seriesArray = v_string_yval[cells].Split(new char[] { '|' });
                            v_seriesName = GetOptionValue("SeriesName", v_seriesArray[0]);
                            if (v_seriesName == "Total Revenue Var. % LY YTD" || v_seriesName == "Unbilled Ext. Revenue > 90 Days Overdue") { hasTwoLinesLegends = true; }
                            if (string.IsNullOrEmpty(v_seriesName))
                            {
                                v_seriesName = "undefined";
                            }
                            try
                            {
                                v_chart.Legends.Add(v_seriesName);
                            }
                            catch (ArgumentException)
                            {
                            }
                            v_chart.Legends[cells].BackColor = Color.Transparent;
                            v_chart.Legends[cells].Font = new Font("Calibri", 10.5f, FontStyle.Regular);
                            v_chart.Legends[cells].Docking = Docking.Bottom;
                            v_chart.Legends[cells].Alignment = StringAlignment.Center;
                            v_seriesType = GetOptionValue("SeriesType", v_seriesArray[1]);
                            v_seriesColour = GetOptionValue("SeriesColor", v_seriesArray[2]);
                            v_seriesAxis = GetOptionValue("SeriesAxis", v_seriesArray[3]);
                            v_seriesVisible = bool.Parse(GetOptionValue("SeriesVisible", v_seriesArray[4]));
                            v_seriesHideZeros = bool.Parse(GetOptionValue("SeriesHideZeros", v_seriesArray[5]));
                        }
                        else
                        {
                            string v_value = v_string_yval[cells];
                            if (string.IsNullOrEmpty(v_value))
                            {
                                v_value = "0";
                            }
                            v_data_yval[cells - 1] = Convert.ToDouble(v_value);
                            if (v_seriesName.IndexOf("Percent", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                v_data_yval[cells - 1] /= 100.0;
                            }
                            else if (v_seriesName.IndexOf("Average", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                if (v_data_yval[cells - 1] > 100.0)
                                {
                                    v_data_yval[cells - 1] /= 100.0;
                                }
                                if (v_data_yval[cells - 1] == 0.0)
                                {
                                    v_data_yval[cells - 1] = double.NaN;
                                }
                            }
                            if ((v_seriesType == "Doughnut" || v_seriesType == "Pie") && v_data_yval[cells - 1] != 0.0)
                            {
                                pieChartData.Add(v_seriesName, v_data_yval[cells - 1]);
                                if (v_seriesType == "Doughnut") { colorPalette.Add(Colors[v_seriesName]); }
                            }
                        }
                    }
                    Color v_color = new Color();
                        v_color = ColorTranslator.FromHtml(v_seriesColour);
                    v_chart.Series.Add(new Series(v_seriesName));
                    v_chart.Series[rows - 1].XValueType = ChartValueType.Double;
                    v_chart.Series[rows - 1].YValueType = ChartValueType.Double;
                    if (v_seriesType.Equals("Spline", StringComparison.InvariantCultureIgnoreCase))
                    {
                        v_chart.Series[rows - 1].ChartType = SeriesChartType.Spline;
                        v_chart.Series[rows - 1].MarkerStyle = MarkerStyle.Diamond;
                        v_chart.Series[rows - 1].MarkerSize = 8;
                        v_chart.Series[rows - 1].BorderWidth = 4;
                    }
                    else if (v_seriesType.Equals("Spline2", StringComparison.InvariantCultureIgnoreCase))
                    {
                        v_chart.Series[rows - 1].ChartType = SeriesChartType.Spline;
                        v_chart.Series[rows - 1].MarkerStyle = MarkerStyle.None;
                        v_chart.Series[rows - 1].MarkerSize = 8;
                        v_chart.Series[rows - 1].BorderWidth = 2;
                        v_chart.Series[rows - 1].ShadowColor = Color.FromKnownColor(KnownColor.Black);
                        v_chart.Series[rows - 1].ShadowOffset = 1;
                    }
                    else if (v_seriesType.Equals("SplinePercent", StringComparison.InvariantCultureIgnoreCase))
                    {
                        v_chart.Series[rows - 1].ChartType = SeriesChartType.Spline;
                        v_chart.Series[rows - 1].MarkerStyle = MarkerStyle.None;
                        v_chart.Series[rows - 1].MarkerSize = 8;
                        v_chart.Series[rows - 1].BorderWidth = 2;
                        v_chart.Series[rows - 1].ShadowColor = Color.FromKnownColor(KnownColor.Black);
                        v_chart.Series[rows - 1].ShadowOffset = 1;
                        v_chart.Series[rows - 1].EmptyPointStyle.BorderWidth = 1;
                        v_chart.Series[rows - 1].EmptyPointStyle.BorderDashStyle = ChartDashStyle.Dash;
                        v_chart.Series[rows - 1].EmptyPointStyle.MarkerStyle = MarkerStyle.Square;
                    }
                    else if (v_seriesType.Equals("SplineQuantity", StringComparison.InvariantCultureIgnoreCase))
                    {
                        v_chart.Series[rows - 1].ChartType = SeriesChartType.Spline;
                        v_chart.Series[rows - 1].MarkerStyle = MarkerStyle.None;
                        v_chart.Series[rows - 1].MarkerSize = 8;
                        v_chart.Series[rows - 1].BorderWidth = 4;
                        v_chart.Series[rows - 1].ShadowColor = Color.FromKnownColor(KnownColor.Black);
                        v_chart.Series[rows - 1].ShadowOffset = 1;
                    }
                    else if (v_seriesType.Equals("SplineAmount", StringComparison.InvariantCultureIgnoreCase))
                    {
                        v_chart.Series[rows - 1].ChartType = SeriesChartType.Spline;
                        v_chart.Series[rows - 1].MarkerStyle = MarkerStyle.None;
                        v_chart.Series[rows - 1].MarkerSize = 8;
                        v_chart.Series[rows - 1].BorderWidth = 4;
                        v_chart.Series[rows - 1].ShadowColor = Color.FromKnownColor(KnownColor.Black);
                        v_chart.Series[rows - 1].ShadowOffset = 1;
                    }
                    else if (v_seriesType.Equals("Bar", StringComparison.InvariantCultureIgnoreCase))
                    {
                        v_chart.Series[rows - 1].ChartType = SeriesChartType.Bar;
                    }
                    else if (v_seriesType.Equals("Column", StringComparison.InvariantCultureIgnoreCase))
                    {
                        v_chart.Series[rows - 1].ChartType = SeriesChartType.Column;
                        v_chart.Series[rows - 1]["DrawingStyle"] = "Default";
                        v_chart.Series[rows - 1]["PointWidth"] = "0.4";
                    }
                    else if (v_seriesType.Equals("StackedColumn", StringComparison.InvariantCultureIgnoreCase))
                    {
                        v_chart.Series[rows - 1].ChartType = SeriesChartType.StackedColumn;
                        v_chart.Series[rows - 1]["DrawingStyle"] = "Default";
                        v_chart.Series[rows - 1]["PointWidth"] = "0.6";
                    }

                    else if (v_seriesType.Equals("Stacked", StringComparison.InvariantCultureIgnoreCase))
                    {
                        v_chart.Series[rows - 1].ChartType = SeriesChartType.StackedColumn;
                        v_chart.Series[rows - 1]["DrawingStyle"] = "Default";
                        v_chart.Series[rows - 1]["PointWidth"] = "0.6";
                    }

                    else if (v_seriesType.Equals("StackedColumn100", StringComparison.InvariantCultureIgnoreCase))
                    {
                        v_chart.Series[rows - 1].ChartType = SeriesChartType.StackedColumn100;
                        v_chart.Series[rows - 1]["DrawingStyle"] = "Default";
                        v_chart.Series[rows - 1]["PointWidth"] = "0.6";
                    }
                    else if (v_seriesType.Equals("Pie", StringComparison.InvariantCultureIgnoreCase))
                    {
                        v_chart.Series[rows - 1].ChartType = SeriesChartType.Pie;
                        colorPalette.Add(System.Drawing.ColorTranslator.FromHtml("#1b365d"));
                        colorPalette.Add(System.Drawing.ColorTranslator.FromHtml("#007681"));
                        colorPalette.Add(System.Drawing.ColorTranslator.FromHtml("#71b2b9"));
                        colorPalette.Add(System.Drawing.ColorTranslator.FromHtml("#c8c9c7"));
                        colorPalette.Add(System.Drawing.ColorTranslator.FromHtml("#e87722"));
                        colorPalette.Add(System.Drawing.ColorTranslator.FromHtml("#82c992"));
                    }
                    else if (v_seriesType.Equals("Doughnut", StringComparison.InvariantCultureIgnoreCase))
                    {
                        v_chart.Series[rows - 1].ChartType = SeriesChartType.Doughnut; //SeriesChartType.Doughnut;
                    }
                    else
                    {
                        v_chart.Series[rows - 1].ChartType = SeriesChartType.Spline;
                    }
                    if (v_seriesAxis.Equals("Secondary", StringComparison.InvariantCultureIgnoreCase))
                    {
                        v_chart.Series[rows - 1].YAxisType = AxisType.Secondary;
                    }
                    else
                    {
                        v_chart.Series[rows - 1].YAxisType = AxisType.Primary;
                    }
                    if (!v_seriesVisible)
                    {
                        v_chart.Series[rows - 1].Enabled = false;
                    }
                    v_chart.Series[rows - 1].Font = new Font("Calibri", 11f, FontStyle.Regular);
                    if (v_seriesName.IndexOf("Percent", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        v_yFormatPercent = "##" + "%";
                    }
                    if (!v_seriesType.Equals("Doughnut", StringComparison.InvariantCultureIgnoreCase) && !v_seriesType.Equals("Pie", StringComparison.InvariantCultureIgnoreCase))
                    {
                        v_chart.Series[rows - 1].Points.DataBindXY(v_data_xval, new IEnumerable[] { v_data_yval });
                        v_chart.Series[rows - 1].Color = v_color;
                    }
                     
                    if (v_seriesHideZeros)
                    {
                        double criteriaValue = 0.0;
                        v_chart.DataManipulator.FilterMatchedPoints = true;
                        v_chart.DataManipulator.Filter(CompareMethod.EqualTo, criteriaValue, v_chart.Series[rows - 1], v_chart.Series[rows - 1], "Y");
                    }
                }
            }
            v_chart.ChartAreas.Add("ChartArea1");
            v_chart.ChartAreas[0].Position.Auto = false;
            v_chart.ChartAreas[0].Position.X = 1;
            v_chart.ChartAreas[0].Position.Y = 20;
            if (hasTwoLinesLegends) { v_chart.ChartAreas[0].Position.Height = 65; }
            else { v_chart.ChartAreas[0].Position.Height = 70; }
            v_chart.ChartAreas[0].Position.Width = 98;

            if (pieChartData.Count > 0)
            {
                v_chart.Series[0].Points.DataBindXY(pieChartData.Keys, pieChartData.Values);
                v_chart.Series[0]["PieLabelStyle"] = "outside";
                v_chart.Series[0]["PieLineColor"] = "black";
                v_chart.Series[0]["PieStartAngle"] = "180";
                if (pieChartData.Count > 6)
                {
                    v_chart.Series[0]["CollectedStyle"] = "SingleSlice";
                    v_chart.Series[0]["CollectedThresholdUsePercent"] = "true";
                    v_chart.Series[0]["CollectedThreshold"] = "5";
                    v_chart.Series[0]["CollectedLabel"] = "Others";
                }
                v_chart.Series[0].IsVisibleInLegend = false;
                v_chart.ChartAreas[0].Position.Width = 100;
                v_chart.Palette = ChartColorPalette.None;
                v_chart.PaletteCustomColors = colorPalette.ToArray();
            }
            v_chart.ChartAreas[0].BorderColor = Color.FromArgb(0x40, 0x40, 0x40, 0x40);
            v_chart.ChartAreas[0].BackSecondaryColor = Color.Transparent;
            v_chart.ChartAreas[0].BackColor = Color.FromArgb(0x40, 0xa5, 0xbf, 0xe4);
            v_chart.ChartAreas[0].ShadowColor = Color.Transparent;
            v_chart.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
            v_chart.ChartAreas[0].InnerPlotPosition.Auto = true;
            v_chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(0x40, 0x40, 0x40, 0x40);
            v_chart.ChartAreas[0].AxisX.LineColor = Color.FromArgb(0x40, 0x40, 0x40, 0x40);
            v_chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Calibri", 8f, FontStyle.Regular);
            v_chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(0x40, 0x40, 0x40, 0x40);
            v_chart.ChartAreas[0].AxisX.Interval = 1.0;

            v_chart.ChartAreas[0].AxisY.Title = v_Title3;
            v_chart.ChartAreas[0].AxisY.LabelStyle.Format = "##,#";
            v_chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Calibri", 8.25f, FontStyle.Regular);
            v_chart.ChartAreas[0].AxisY.LineColor = Color.FromArgb(0x80, 0x80, 0x80, 0x80);
            v_chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(0x80, 0x80, 0x80, 0x80);
            v_chart.ChartAreas[0].AxisY.IsStartedFromZero = false;

            v_chart.ChartAreas[0].AxisY2.Title = v_Title4;
            v_chart.ChartAreas[0].AxisY2.LabelStyle.Font = new Font("Calibri", 8.25f, FontStyle.Regular);
            v_chart.ChartAreas[0].AxisY2.LabelStyle.Format = "##,#";// v_yFormat;
            v_chart.ChartAreas[0].AxisY2.LineColor = Color.FromArgb(0x80, 0x80, 0x80, 0x80);
            v_chart.ChartAreas[0].AxisY2.MajorGrid.LineColor = Color.FromArgb(0, 0, 0, 0);
            v_chart.ChartAreas[0].AxisY2.IsMarginVisible = false;

            if (v_Title4.ToLower().Contains("mean"))
            {
                v_chart.ChartAreas[0].AxisY2.Maximum = 10.0;
            }

            v_chart.ChartAreas[0].AxisY2.LabelStyle.Format = v_yFormatPercent;

            if (v_Title4.ToLower().Contains("average"))
            {
                v_chart.ChartAreas[0].AxisY2.ScaleBreakStyle.Enabled = true;
                v_chart.ChartAreas[0].AxisY2.ScaleBreakStyle.BreakLineStyle = BreakLineStyle.None;
                v_chart.ChartAreas[0].AxisY2.ScaleBreakStyle.Spacing = 0.0;
                v_chart.ChartAreas[0].AxisY2.IntervalAutoMode = IntervalAutoMode.VariableCount;
                v_chart.ChartAreas[0].AxisY2.LabelStyle.Format = v_yFormatAverage;
                v_chart.ChartAreas[0].AxisY2.Maximum = double.NaN;
                v_chart.ChartAreas[0].AxisY2.Minimum = double.NaN;
                v_chart.ChartAreas[0].AxisY2.ScaleBreakStyle.CollapsibleSpaceThreshold = 15;
                v_chart.ChartAreas[0].AxisY2.ScaleBreakStyle.StartFromZero = StartFromZero.Auto;
                v_chart.ChartAreas[0].AxisY2.ScaleBreakStyle.MaxNumberOfBreaks = 2;
            }

           
            v_chart.SaveImage(context.Server.MapPath(@"/content/charts/chartimage_" + v_userKey + "_" + v_chartID + ".png"));
            
            context.Response.Clear();
            context.Response.ContentType = "image/png";

            //using (MemoryStream v_stream = new MemoryStream())
            //{
            //    v_chart.SaveImage(v_stream, ChartImageFormat.Png);
            //    v_stream.WriteTo(context.Response.OutputStream);
            //}

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}