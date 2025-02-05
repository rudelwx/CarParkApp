using System.ComponentModel.DataAnnotations;

namespace carParkApp.DB.Models
{
    public class car_park_dtl
    {
        [Key]
        public int id { get; set; }
        public int carParHdrId { get; set; }
        public string car_park_no { get; set; }
        public string lot_type { get; set; }
        public string total_lots { get; set; }
        public string lots_available { get; set; }
        public DateTime updateDt { get; set; }
    }
}
