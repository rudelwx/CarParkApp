using carParkApp.Controllers;
using carParkApp.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Text.Json;
using Xunit;

namespace carParkApp
{
    public class ApiControllerTests
    {
        private DBContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
               .UseMySql(
                    "server=localhost;database=carparkdb;user=test;password=test;",
                    new MySqlServerVersion(new Version(9, 2, 0))
                ).Options;
            return new DBContext(options);
        }

        [Fact]
        public void getNearestCarPark_ReturnsOk()
        {
            DBContext dbContext = GetInMemoryDbContext();

            var controller = new ApiController(dbContext);

            // Act
            var result = controller.getNearestCarPark("1.37326", "103.897", 1, 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var json = JsonSerializer.Serialize(okResult.Value); // Convert response to JSON string
            Assert.StartsWith("[", json);
            Assert.EndsWith("]", json);
        }

        [Fact]
        public void getNearestCarPark_ReturnsBadRequest()
        {

            DBContext dbContext = GetInMemoryDbContext();

            var controller = new ApiController(dbContext);

            // Act
            var result = controller.getNearestCarPark(null, null, 1, 5);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
