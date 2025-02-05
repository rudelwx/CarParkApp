using carParkApp.DB;
using carParkApp.Services;
using Microsoft.EntityFrameworkCore;
using NLog;
using Quartz;

namespace carParkApp.Jobs
{
    public class CarParkJob : IJob
    {

        private readonly DBContext _dBContext;
        private CarParkServices _carParkServices;
        private Logger _logger;

        public CarParkJob(DBContext dBContext)
        {
            _dBContext = dBContext;
            _carParkServices = new CarParkServices(_dBContext);
            _logger = LogManager.GetCurrentClassLogger(); 
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.Info($"Job Running at {DateTime.Now}");
            await _carParkServices.UpdateCarParkData();
            _logger.Info($"Job Completed  at {DateTime.Now}");
            await Task.CompletedTask;
        }
    }
}
