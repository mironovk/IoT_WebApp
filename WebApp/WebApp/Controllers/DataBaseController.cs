using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;
using WebApp.Models;


namespace WebApp.Controllers
{
    public class DataBaseController : Controller
    {
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

        private DataTable ConvertToDataTable(List<DataBaseItem> Lst)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(DataBaseItem));

            DataTable table = new DataTable();

            foreach (PropertyDescriptor property in properties)
            {
                table.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            foreach (DataBaseItem item in Lst)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor property in properties)
                {
                    row[property.Name] = property.GetValue(item);
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
