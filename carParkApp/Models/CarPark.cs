namespace carParkApp.Models
{
    public class NearestCarPark
    {
        public string address { get; set; }
        public double latitude {  get; set; }
        public double longitude { get; set; }
        public int total_lots { get; set; }
        public int available_lots { get; set; }
        public double distance { get; set; }
    }

    public class NearerCarpark
    {
        public int id { get; set; }
        public double distance { get; set; }
    }

    public class wrapper
    {
        public List<AvailabilityCarParkWrapper> items { get; set; }
    }
    public class AvailabilityCarParkWrapper
    {
        public string timestamp { get; set; }
        public List<carpark_dataModel> carpark_data { get; set; }
    }

    public class carpark_dataModel
    {
        public string carpark_number { get; set; }
        public DateTime update_datetime { get; set; }
        public List<carpark_infoModel> carpark_info { get; set; }
    }

    public class carpark_infoModel
    {
        public string total_lots { get; set; }
        public string lot_type { get; set; }
        public string lots_available { get; set; }
    }
}
