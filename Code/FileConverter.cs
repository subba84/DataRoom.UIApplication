using Amazon.Runtime.Documents;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Font = iTextSharp.text.Font;
using Rectangle = iTextSharp.text.Rectangle;
using Image = iTextSharp.text.Image;
using DataRooms.UI.Models;
using Castle.Components.DictionaryAdapter.Xml;
using DataRooms.UI;
using iTextSharp.tool.xml.html.head;
using iTextSharp.tool.xml.html;
using static iTextSharp.text.pdf.AcroFields;

namespace DataRooms.UI.Code
{
    public class FileConverter
    {
        public byte[] ConverttoPDF(string viewContent, string reportTitle)
        {
            byte[] bytes;
            try
            {
                var ms = new MemoryStream();
                var doc = new iTextSharp.text.Document(PageSize.A4.Rotate());
                var writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();
                doc.Add(new Chunk(""));
                var example_html = viewContent;
                var srHtml = new StringReader(viewContent);
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, srHtml);
                doc.Close();
                bytes = ms.ToArray();
                srHtml.Dispose();
                writer.Dispose();
                doc.Dispose();
                ms.Dispose();

                iTextSharp.text.Font blackFont = FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 18, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font headerFont = new Font(FontFactory.GetFont("Arial", 18, Font.BOLD));
                using (MemoryStream stream = new MemoryStream())
                {
                    PdfReader reader = new PdfReader(bytes);
                    using (PdfStamper stamper = new PdfStamper(reader, stream))
                    {
                        int pages = reader.NumberOfPages;
                        for (int i = 1; i <= pages; i++)
                        {
                            //Header Code
                            PdfPTable tblHeader = new PdfPTable(2);
                            tblHeader = new PdfPTable(2);

                            tblHeader.DefaultCell.Border = Rectangle.NO_BORDER;
                            tblHeader.TotalWidth = PageSize.A4.Height;


                            Paragraph p1 = new Paragraph("SharBox", headerFont);
                            PdfPCell _cell = new PdfPCell();
                            _cell.AddElement(p1);

                            _cell.Border = Rectangle.NO_BORDER;
                            tblHeader.AddCell(_cell);
                            _cell = new PdfPCell();
                            iTextSharp.text.Image myImage = iTextSharp.text.Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~") + "/Content/dist/img/logo-icon.png");
                            //myImage.SpacingBefore = 10f;//15f
                            myImage.ScaleAbsolute(60f, 40f);
                            myImage.Alignment = Image.ALIGN_RIGHT;
                            _cell.AddElement(myImage);
                            _cell.Border = Rectangle.NO_BORDER;
                            tblHeader.AddCell(_cell);

                            // Print the Header
                            ColumnText column = new ColumnText(stamper.GetOverContent(i));
                            Rectangle rectPage1 = new Rectangle(5, 60, 635, 840);//8, 60, 590, 840
                            rectPage1 = new Rectangle(-35, 60, 800, 600);
                            column.SetSimpleColumn(rectPage1);
                            column.AddElement(tblHeader);
                            column.Go();

                            ColumnText.ShowTextAligned(stamper.GetUnderContent(i),
                            @Element.ALIGN_CENTER, new Phrase(reportTitle, titleFont), 400f, 569f, 0);

                            ColumnText.ShowTextAligned(stamper.GetOverContent(i),
                             @Element.ALIGN_LEFT, new Phrase("SharBox", blackFont), 63f, 24f, 0);

                            ColumnText.ShowTextAligned(stamper.GetUnderContent(i),
                            @Element.ALIGN_CENTER, new Phrase("Page " + i.ToString() + " of " + pages, blackFont), 300f, 24f, 0);

                            ColumnText.ShowTextAligned(stamper.GetUnderContent(i),
                            @Element.ALIGN_RIGHT, new Phrase("" + DateTime.Now, blackFont), 549f, 24f, 0);
                        }
                    }

                    bytes = stream.ToArray();

                }//End of page counter


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bytes;
        }



        public string SaveReport(string flag, string viewContent, string reportTitle)
        {
            string filePath = string.Empty;
            try
            {
                byte[] bytes;
                if (flag == "PDF")
                {
                    bytes = ConverttoPDF(viewContent, reportTitle);
                    Guid obj = Guid.NewGuid();
                    filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~") + "/Temp/", "Report_ " + obj.ToString() + ".pdf");
                }
                else
                {
                    bytes = Encoding.ASCII.GetBytes(viewContent);
                    Guid obj = Guid.NewGuid();
                    filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~") + "/Temp/", "Report_" + obj.ToString() + ".csv");
                }
                System.IO.File.WriteAllBytes(filePath, bytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return filePath;
        }

        public string GetHtmlContent(int itemtrackerid)
        {
            StringBuilder StrExport = new StringBuilder();
            try
            {
                var itemtrackercontrols = DataCache.ItemTrackerControls.Where(x => x.ItemTrackerId == itemtrackerid);
                var itemtrackerdata = DataCache.ItemTrackerData.Where(x => x.ItemTrackerId == itemtrackerid);
                StrExport.Append("<table border='1' cellpadding='1' cellspacing='0' style='width:100%;margin-top:30px;'>");
                List<string> columns = itemtrackercontrols.OrderBy(x=>x.Id).Select(x => x.ControlName).Distinct().ToList();
                int colcount = 0;
                if (columns != null && columns.Count > 0)
                {
                    StrExport.Append("<thead>");
                    StrExport.Append("<tr class='tbl-header'>");
                    //StrExport.Append("<th colspan='3'>Actions</th>");
                    if (columns != null && columns.Count() > 0)
                    {
                        foreach (var column in columns)
                        {
                            StrExport.Append("<th>" + column + "</th>");
                            colcount++;
                        }
                    }
                    StrExport.Append("</tr>");
                    StrExport.Append("</thead>");
                    StrExport.Append("<tbody>");
                    if (itemtrackerdata != null && itemtrackerdata.Count() > 0)
                    {
                        List<string> distinctRows = itemtrackerdata.OrderBy(x=>x.Id).Select(x => x.RowGuid).Distinct().ToList();
                        if (distinctRows != null && distinctRows.Count() > 0)
                        {
                            foreach (var row in distinctRows)
                            {
                                int rowcolcount = 0;
                                var rowData = itemtrackerdata.OrderBy(x => x.Id).Where(x => x.RowGuid == row);
                                if (rowData != null && rowData.Count() > 0)
                                {
                                    var columnData = rowData.Select(x => x.ControlGuid).ToList();
                                    StrExport.Append("<tr>");
                                    if (columnData != null && columnData.Count() > 0)
                                    {
                                        foreach (var item in columnData)
                                        {
                                            if (itemtrackercontrols.Select(x => x.ControlGuid).ToList().Contains(item))
                                            {
                                                int controlType = 0;
                                                string controlData = string.Empty;
                                                var data = rowData.Where(x => x.ControlGuid == item);
                                                if (data != null && data.Count() > 0)
                                                {
                                                    controlType = data.First().ControlTypeId;
                                                    controlData = data.First().ControlDataName;
                                                }
                                                switch (controlType)
                                                {
                                                    case ControlType.TextBox:
                                                        StrExport.Append("<td>" + controlData + "</td>");
                                                        break;
                                                    case ControlType.Dropdown:

                                                        StrExport.Append("<td>" + controlData + "</td>");
                                                        break;
                                                    case ControlType.TwoLevelDropDown:
                                                        StrExport.Append("<td>" + controlData + "</td>");
                                                        break;
                                                    case ControlType.ThreeLevelDropdown:
                                                        StrExport.Append("<td>" + controlData + "</td>");
                                                        break;
                                                    case ControlType.FileUpload:
                                                        StrExport.Append("<td>--</td>");
                                                        break;
                                                    case ControlType.DateControl:
                                                        StrExport.Append("<td>" + controlData + "</td>");
                                                        break;
                                                }
                                                rowcolcount++;
                                            }
                                        }
                                    }
                                }
                                if (colcount > rowcolcount)
                                {
                                    int diifcolcount = colcount - rowcolcount;
                                    for (int i = 0; i < diifcolcount; i++)
                                    {
                                        StrExport.Append("<td></td>");
                                    }
                                }
                                StrExport.Append("</tr>");
                            }
                        }
                    }
                }                
                StrExport.Append("</tbody>");
                StrExport.Append("</table>");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return StrExport.ToString();
        }

        public string GetPlainContent(int itemtrackerid)
        {
            StringBuilder StrExport = new StringBuilder();
            try
            {
                var itemtrackercontrols = DataCache.ItemTrackerControls.OrderBy(x=>x.Id).Where(x => x.ItemTrackerId == itemtrackerid);
                var itemtrackerdata = DataCache.ItemTrackerData.OrderBy(x => x.Id).Where(x => x.ItemTrackerId == itemtrackerid);
                List<string> columns = itemtrackercontrols.Select(x => x.ControlName).Distinct().ToList();
                int colcount = 0;
                if (columns != null && columns.Count > 0)
                {
                    for (int i = 0; i < columns.Count; i++)
                    {
                        var name = columns[i];
                        StrExport.Append(name + ",");
                        colcount++;
                    }
                    StrExport.Append("\r\n");
                    if (itemtrackerdata != null && itemtrackerdata.Count() > 0)
                    {
                        List<string> distinctRows = itemtrackerdata.Select(x => x.RowGuid).Distinct().ToList();
                        if (distinctRows != null && distinctRows.Count() > 0)
                        {
                            foreach (var row in distinctRows)
                            {
                                int rowcolcount = 0;
                                    var rowData = itemtrackerdata.Where(x => x.RowGuid == row);
                                if (rowData != null && rowData.Count() > 0)
                                {
                                    var columnData = rowData.Select(x => x.ControlGuid).ToList();
                                    //StrExport.Append("\r\n");
                                    if (columnData != null && columnData.Count() > 0)
                                    {
                                        foreach (var item in columnData)
                                        {
                                            if (itemtrackercontrols.Select(x => x.ControlGuid).ToList().Contains(item))
                                            {
                                                int controlType = 0; int controlDataId = 0;
                                                string controlData = string.Empty;
                                                var data = rowData.Where(x => x.ControlGuid == item);
                                                if (data != null && data.Count() > 0)
                                                {
                                                    controlType = data.First().ControlTypeId;
                                                    controlData = data.First().ControlDataName;
                                                }
                                                switch (controlType)
                                                {
                                                    case ControlType.TextBox:
                                                        //StrExport.Append("<td>" + controlData + "</td>");
                                                        StrExport.Append(controlData + ",");
                                                        break;
                                                    case ControlType.Dropdown:
                                                        //StrExport.Append("<td>" + controlData + "</td>");
                                                        StrExport.Append(controlData + ",");
                                                        break;
                                                    case ControlType.TwoLevelDropDown:
                                                        //StrExport.Append("<td>" + controlData + "</td>");
                                                        StrExport.Append(controlData + ",");
                                                        break;
                                                    case ControlType.ThreeLevelDropdown:
                                                        //StrExport.Append("<td>" + controlData + "</td>");
                                                        StrExport.Append(controlData + ",");
                                                        break;
                                                    case ControlType.FileUpload:
                                                        //StrExport.Append("<td>--</td>");
                                                        StrExport.Append(controlData + ",");
                                                        break;
                                                    case ControlType.DateControl:
                                                        //StrExport.Append("<td>" + controlData + "</td>");
                                                        StrExport.Append(controlData + ",");
                                                        break;
                                                }
                                                rowcolcount++;
                                            }
                                            
                                        }
                                        
                                        
                                    }
                                }
                                if (colcount > rowcolcount)
                                {
                                    int diifcolcount = colcount - rowcolcount;
                                    for (int i = 0; i < diifcolcount; i++)
                                    {
                                        StrExport.Append(",");
                                    }
                                }
                                StrExport.Append("\r\n");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return StrExport.ToString();
        }
    }
}