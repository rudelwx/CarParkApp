using System.ComponentModel.DataAnnotations;

namespace carParkApp.DB.Models
{
    public class car_park_hdr
    {
        [Key]
        public int id { get; set; }
        public string car_park_no {  get; set; }
        public string address { get; set; }
        public float x_coord { get; set; }
        public float y_coord { get; set; }
        public string car_park_type { get; set; }
        public string type_of_parking_system { get; set; }
        public string short_term_parking { get; set; }
        public string free_parking { get; set; }
        public bool night_parking { get; set; }
        public int car_park_decks { get; set; }
        public double gantry_height { get; set; }
        public bool car_park_basement { get; set; }

    }
}
