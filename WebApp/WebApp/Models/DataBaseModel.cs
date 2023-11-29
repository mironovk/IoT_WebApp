using MySqlConnector;
using System.Configuration;
using System.Runtime.CompilerServices;
using WebApp.Controllers;
using System.Text.Json;
using GetSettingsConnect;

namespace GetSettingsConnect
{
    public class ConnectionSettings
    {
        public string ip { get; set; }
        public string user { get; set; }
        public string passwd { get; set; }
        public string database { get; set; }
    }
    public class LogLvl
    {
        public string Default { get; set; }
        public string Microsoft_AspNetCore { get; set; }
    }
    public class Logging
    {
        public LogLvl LogLevel { get; set; }
    }
    public class JsonFields
    {
        public ConnectionSettings ConnectionStrings { get; set; }
        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; }
    }

    public class ReadAndParseJson
    {
        public string file_path;
        public ReadAndParseJson()
        {
            this.file_path = "";
        }
        public ReadAndParseJson(string path)
        {
            this.file_path = path;
        }
        public ConnectionSettings readjson()
        {
            ConnectionSettings fields = new ConnectionSettings();

            try
            {
                using (StreamReader reader = new StreamReader(this.file_path))
                {
                    var json = reader.ReadToEnd();
                    JsonFields all_fields_json = JsonSerializer.Deserialize<JsonFields>(json);
                    fields = all_fields_json.ConnectionStrings;
                }
                
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
            return fields;
        }

    }
}
namespace WebApp.Models
{
    public class DataBaseModel
    {
        String connectionString;
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

        public void get_connetction_string()
        {
            ReadAndParseJson parser = new ReadAndParseJson(".\\appsettings.json");
            ConnectionSettings connection_settings = new ConnectionSettings();
            connection_settings = parser.readjson();
            this.connectionString = "Server=" + connection_settings.ip + ";User ID=" + connection_settings.user +
                                    ";Password=" + connection_settings.passwd + ";Database=" + connection_settings.database;
        }
        public List<DataBaseItem> GetItems()
        {
            get_connetction_string();

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
            get_connetction_string();

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
            get_connetction_string();

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
