using carParkApp.DB;
using carParkApp.DB.Models;
using carParkApp.Helpers;
using carParkApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore.Update.Internal;

using Newtonsoft.Json;
using NLog;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace carParkApp.Services
{
    public class CarParkServices
    {
        private readonly DBContext _dBContext;
        HttpClient _httpClient;
        private Logger _logger;

        public CarParkServices(DBContext dBContext)
        {
            _dBContext = dBContext;
            _httpClient = new HttpClient();
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<List<NearestCarPark>> GetNearestCarPark(string latitude, string longitude)
        {
            List<NearestCarPark> nearestCarParks = new List<NearestCarPark>();
            try
            {
                //convert to SBY21 for distance calculation
                (double x, double y) = CoordinatesConverter.ConvertWGS84ToSVY21(double.Parse(latitude), double.Parse(longitude));

                List<car_park_hdr> carParks = _dBContext.car_park_hdr.ToList();

                //get nearer carpark hdr
                foreach (var carPark in carParks)
                {
                    double distance = GetDistance(x, y, carPark.x_coord, carPark.y_coord);
                    if (distance < Constant.DistanceRadius)
                    {
                        (double carParkLatitude, double carParkLongitude) = CoordinatesConverter.ConvertSVY21ToWGS84(carPark.x_coord, carPark.y_coord);
                        car_park_dtl dtl = _dBContext.car_park_dtl.Where(c => c.car_park_no == carPark.car_park_no && c.carParHdrId == carPark.id).FirstOrDefault();

                        //generate return model
                        if(dtl != null)
                        {
                            NearestCarPark newData = new NearestCarPark()
                            {
                                address = carPark.address,
                                latitude = carParkLatitude,
                                longitude = carParkLongitude,
                                total_lots = int.Parse(dtl.total_lots),
                                available_lots = int.Parse(dtl.lots_available),
                                distance = distance
                            };
                            nearestCarParks.Add(newData);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Error($"An error occurred: {ex.Message}");
            }
            finally
            {

            }
            return nearestCarParks;
        }

        public async Task UpdateCarParkData()
        {
            try
            {
                // Specify the API endpoint
                string url = "https://api.data.gov.sg/v1/transport/carpark-availability";

                // Send the GET request
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read and output the response body as a string
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<wrapper>(responseBody);
                    UpdateAvailability(data);
                }
                else
                {
                    _logger.Error($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred: {ex.Message}");
            }
            
        }

        /* SHARED FUNCTION */
        private double GetDistance(double e1, double n1, double e2, double n2)
        {

            return Math.Sqrt(Math.Pow(e2 - e1, 2) + Math.Pow(n2 - n1, 2));
        }

        private void UpdateAvailability(wrapper data)
        {
            try
            {
                foreach (var i in data.items)
                {
                    foreach (carpark_dataModel carPark in i.carpark_data)
                    {
                        car_park_hdr hdr = _dBContext.car_park_hdr.Where(c => c.car_park_no == carPark.carpark_number).FirstOrDefault();
                        car_park_dtl dtl = _dBContext.car_park_dtl.Where(c => c.car_park_no == carPark.carpark_number).FirstOrDefault();

                        foreach (carpark_infoModel parking in carPark.carpark_info)
                        {

                            if (dtl == null)
                            {
                                if(hdr != null)
                                {
                                    car_park_dtl newDtl = new car_park_dtl()
                                    {
                                        carParHdrId = hdr.id,
                                        car_park_no = carPark.carpark_number,
                                        lot_type = parking.lot_type,
                                        lots_available = parking.lots_available,
                                        total_lots = parking.total_lots,
                                        updateDt = carPark.update_datetime
                                    };
                                    _dBContext.car_park_dtl.Add(newDtl);
                                }
                            }
                            else
                            {
                                dtl.lots_available = parking.lots_available;
                                dtl.total_lots = parking.total_lots;
                                dtl.updateDt = carPark.update_datetime;
                                _dBContext.car_park_dtl.Update(dtl);
                            }

                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
            finally
            {
                _dBContext.SaveChanges();
            }

        }
    }


}
