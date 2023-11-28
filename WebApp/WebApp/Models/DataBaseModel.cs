using MySqlConnector;
using System.Configuration;
using System.Runtime.CompilerServices;
using WebApp.Controllers;

namespace WebApp.Models
{
    public class DataBaseModel
    {
        String connectionString = "Server=192.168.31.229;User ID=reader;Password=reader;Database=SENSOR_DB";
        
        public DataBaseModel() 
        {
            //this.connectionString = Microsoft.Extensions.Configuration.JsonConfigurationExtensions
            //this.
            //this.connectionString = Configuration.GetConnectionString
            //this.connectionString = Configuration["ConnectionStrings:Default"];
            //this.connectionString = IConfiguration.GetValue<string>("ConnectionStrings:Default");
            //this.connectionString = ConfigurationPath.GetSectionKey("ConnectionStrings:Default");
            //this.connectionString = ConfigurationPath.GetParentPath("ConnectionStrings:Default");
        }  
        public List<DataBaseItem> GetItems()
        {
            List<DataBaseItem> resultList = new List<DataBaseItem>();

            using (MySqlConnection connection = new MySqlConnection(this.connectionString))
            {
                connection.Open();
                String selectSqlCmd = "SELECT * FROM " + "SENSOR_DATA_TABLE;";
                MySqlCommand cmd = new MySqlCommand(selectSqlCmd, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DataBaseItem item = new DataBaseItem();

                        item.ID = Convert.ToInt32(reader["ID"]);
                        item.SensorName = Convert.ToString(reader["SensorName"]);
                        item.DataType = Convert.ToString(reader["DataType"]);
                        item.Position = Convert.ToString(reader["Position"]);
                        item.Value = Convert.ToString(reader["Value"]);
                        item.Date = Convert.ToString(reader["Date"]);

                        resultList.Add(item);
                    }
                }
            }

            return resultList;
        }

        public Int32 GetNumberOfItems()
        {
            Int32 numOfItems = 0;

            using (MySqlConnection connection = new MySqlConnection(this.connectionString))
            {
                connection.Open();

                String selectCmd = "SELECT COUNT(*) FROM " + "SENSOR_DATA_TABLE;";

                MySqlCommand cmd = new MySqlCommand(selectCmd, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        numOfItems = Convert.ToInt32(reader[0]);
                    }
                }
            }
            return numOfItems;
        }

        public List<DataBaseItem> GetSensorItems(String SensorName, String DataType, String Position)
        {
            List<DataBaseItem> resultList = new List<DataBaseItem>();

            using (MySqlConnection connection = new MySqlConnection(this.connectionString))
            {
                connection.Open();
                String selectSqlCmd = "SELECT * FROM " + "SENSOR_DATA_TABLE" +
                                        " WHERE (SensorName = '" + SensorName + "')" +
                                        " AND (DataType = '" + DataType + "')" +
                                        " AND (Position = '" + Position + "');";

                MySqlCommand cmd = new MySqlCommand(selectSqlCmd, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DataBaseItem item = new DataBaseItem();

                        item.ID = Convert.ToInt32(reader["ID"]);
                        item.SensorName = Convert.ToString(reader["SensorName"]);
                        item.DataType = Convert.ToString(reader["DataType"]);
                        item.Position = Convert.ToString(reader["Position"]);
                        item.Value = Convert.ToString(reader["Value"]);
                        item.Date = Convert.ToString(reader["Date"]);

                        resultList.Add(item);
                    }
                }
            }

            return resultList;
        }
    }
}
