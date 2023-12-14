namespace WebApp.Models
{
    public class DataBaseItem
    {
        public int ID { get; set; }
        public string SensorName { get; set; }
        public string DataType { get; set; }
        public String Position { get; set; }
        public string Value { get; set; }
        public string Date { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Save { get; set; }
    }
}
