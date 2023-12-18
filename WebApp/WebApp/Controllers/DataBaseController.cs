using ClosedXML;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using WebApp.Models;


namespace WebApp.Controllers
{
    public class DataBaseController : Controller
    {
        private readonly IConfiguration configuration;
        public DataBaseController(IConfiguration config)
        {
            configuration = config;
        }
        public IActionResult Index()
        {
            DataBaseModel db = HttpContext.RequestServices.GetService(typeof(WebApp.Models.DataBaseModel)) as DataBaseModel;

            return View(db.GetNumberOfItems());
        }
        public IActionResult ShowDataTable()
        {
            DataBaseModel db = HttpContext.RequestServices.GetService(typeof(WebApp.Models.DataBaseModel)) as DataBaseModel;
            return View(db.GetItems());
        }
        public IActionResult ShowDataTable2(DataBaseItem item) 
        {
            return View(item);
        }

        private DataTable ConvertToDataTable(List<DataBaseItem> Lst)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(DataBaseItem));

            DataTable table = new DataTable();

            foreach (PropertyDescriptor property in properties)
            {
                if (property.Name == "StartDate" || property.Name == "EndDate" || property.Name == "Save")
                {
                    continue;
                }
                else
                {
                    table.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }
            }

            foreach (DataBaseItem item in Lst)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor property in properties)
                {
                    if (property.Name == "StartDate" || property.Name == "EndDate" || property.Name == "Save")
                    {
                        continue;
                    }
                    else
                    {
                        row[property.Name] = property.GetValue(item);
                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }

        ///////////////////////////////////////////// Save to Exel /////////////////////////////////////////////
        private Byte[] GetExcelFileBinaryContent(DataBaseModel db, int full, DataBaseItem item)
        {
            List<DataBaseItem> lstItems;
            if (full == 1)
            {
                lstItems = db.GetItems();
            }
            else
            {
                lstItems = db.GetSensorItems(item.SensorName, item.DataType, item.Position, item.StartDate, item.EndDate);
            }

            DataTable dt = this.ConvertToDataTable(lstItems);

            XLWorkbook wb = new XLWorkbook();

            wb.Worksheets.Add(dt, "Sensors");

            wb.Worksheet(1).Cells(true).Style.Font.FontColor = XLColor.FromTheme(XLThemeColor.Text1);
            wb.Worksheet(1).Cells(false).Style.Font.FontColor = XLColor.FromTheme(XLThemeColor.Text1);

            wb.Worksheet(1).Cells(true).Style.Fill.BackgroundColor = XLColor.White;
            wb.Worksheet(1).Cells(false).Style.Fill.BackgroundColor = XLColor.White;

            wb.Worksheet(1).Cells(true).Style.Fill.PatternColor = XLColor.Black;
            wb.Worksheet(1).Cells(false).Style.Fill.PatternColor = XLColor.Black;

            wb.Worksheet(1).Cells(true).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            wb.Worksheet(1).Cells(true).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            wb.Worksheet(1).Cells(false).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            wb.Worksheet(1).Cells(false).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            wb.Worksheet(1).Cells(true).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            wb.Worksheet(1).Cells(false).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            wb.Worksheet(1).Tables.FirstOrDefault().ShowAutoFilter = false;

            wb.Worksheet(1).Columns().AdjustToContents();

            MemoryStream ms = new MemoryStream();
            wb.SaveAs(ms);

            return ms.ToArray();
        }

        public IActionResult SaveToExcelFile() 
        {
            DataBaseModel db  = HttpContext.RequestServices.GetService(typeof(WebApp.Models.DataBaseModel)) as DataBaseModel;
            DataBaseItem item = new DataBaseItem();
            Byte[] data = this.GetExcelFileBinaryContent(db, 1, item);

            return File(data, "application/xlsx", "SensorsData.xlsx"); 
        }

		///////////////////////////////////////////// Save to XML /////////////////////////////////////////////
		public IActionResult SaveToXmlFile()
        {
            DataBaseModel db = HttpContext.RequestServices.GetService(typeof(WebApp.Models.DataBaseModel)) as DataBaseModel;

            List<DataBaseItem> lst = db.GetItems();

            using (StringWriter sw = new StringWriter(new StringBuilder()))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<DataBaseItem>));
                xmlSerializer.Serialize(sw, lst);
                Byte[] data = Encoding.Unicode.GetBytes(sw.ToString());
                return File(data, "application/xml", "SensorsData.xml");
            }
        }

        ///////////////////////////////////////////// Select from DB /////////////////////////////////////////////
        [HttpGet]
        public IActionResult Select()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Select(DataBaseItem item)
        {
            if (item.Save == "Show сhart")
            {
                return RedirectToAction("ShowChart", item);
            }
            else if (item.Save == "Save in Excel-file")
            {
                DataBaseModel db = HttpContext.RequestServices.GetService(typeof(WebApp.Models.DataBaseModel)) as DataBaseModel;
                Byte[] data = this.GetExcelFileBinaryContent(db, 0, item);
                return File(data, "application/xlsx", "SensorsData.xlsx");
            }
            else if (item.Save == "Show table")
            {
                DataBaseModel db = HttpContext.RequestServices.GetService(typeof(WebApp.Models.DataBaseModel)) as DataBaseModel;
                List<DataBaseItem> lst = db.GetSensorItems(item.SensorName, item.DataType, item.Position, item.StartDate, item.EndDate);
                return View("ShowDataTable2", lst);
            }
            else
            {
                DataBaseModel db = HttpContext.RequestServices.GetService(typeof(WebApp.Models.DataBaseModel)) as DataBaseModel;
                List<DataBaseItem> lst = db.GetSensorItems(item.SensorName, item.DataType, item.Position, item.StartDate, item.EndDate);
                using (StringWriter sw = new StringWriter(new StringBuilder()))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<DataBaseItem>));
                    xmlSerializer.Serialize(sw, lst);
                    Byte[] data = Encoding.Unicode.GetBytes(sw.ToString());
                    return File(data, "application/xml", "SensorsData.xml");
                }
            }
        }

        public IActionResult ShowChart(DataBaseItem item)
        {
            CultureInfo ci = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            DataBaseModel db = HttpContext.RequestServices.GetService(typeof(DataBaseModel)) as DataBaseModel;

            return View(db.GetSensorItems(item.SensorName, item.DataType, item.Position, item.StartDate, item.EndDate));
        }

    }
}
