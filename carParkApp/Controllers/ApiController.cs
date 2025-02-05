using carParkApp.DB;
using carParkApp.Helpers;
using carParkApp.Models;
using carParkApp.Services;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace carParkApp.Controllers
{
    
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly DBContext _dBContext;
        private readonly CarParkServices _carParkServices;
        private Logger _logger;

        public ApiController(DBContext dBContext)
        {
            _dBContext = dBContext;
            _carParkServices = new CarParkServices(_dBContext);
            _logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        ///     Get all car park header
        /// </summary>
        /// <returns></returns>
        [HttpGet("carparks")]
        public ActionResult carparks()
        {
            try
            {
                var a = _dBContext.car_park_hdr.ToList();
                return Ok(a); 
            }
            catch (Exception ex) 
            {
                _logger.Error(ex);
                return BadRequest("");
            }
            finally
            {

            }
        }
        /// <summary>
        ///     Get nearest car park based on latitude & longitude
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="page"></param>
        /// <param name="per_page"></param>
        /// <returns></returns>
        [HttpGet("carparks/nearest")]
        public async Task<ActionResult> getNearestCarPark(string latitude = "1.37326", string longitude = "103.897", int page = 1, int per_page = 5)
        {
            List<NearestCarPark> nearestCarParks = new List<NearestCarPark>();
            try
            {
                if(string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude))
                {
                    return BadRequest("Missing latitude/longitude");
                }

                nearestCarParks = await _carParkServices.GetNearestCarPark(latitude, longitude);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return BadRequest("");
            }
            finally
            {

            }
            return Ok(PaginateList(nearestCarParks.OrderBy(c => c.distance).ToList(), page, per_page));
        }

        /// <summary>
        ///     Update car park availability
        /// </summary>
        /// <returns></returns>
        [HttpPost("carparks/update")]
        public async Task<ActionResult> UpdateCarParks()
        {
            await _carParkServices.UpdateCarParkData();
            return Ok();
        }


        /* SHARED FUNCTION */
        public List<T> PaginateList<T>(List<T> data, int pageNumber, int pageSize)
        {
            return data.Skip((pageNumber - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();
        }
    }
}
