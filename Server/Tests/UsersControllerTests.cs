using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Server.Controllers;
using Server.Data;
using Server.Data.Models;
using Xunit;

public class UsersControllerTests
{
    [Fact]
    public async Task GetUsers_ReturnsAllUsers()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        // Insert test data into the in-memory database
        using (var context = new ApplicationDbContext(options))
        {
            context.Users.Add(new User 
            { 
                Email = "test@abv.bg",
                Username = "tonsan1",
                Profile = new Profile
                {
                    FirstName = "Tony",
                    LastName = "K",
                    DateOfBirth = new DateTime()
                }
            });
            context.Users.Add(new User 
            { 
                Email = "test2@abv.bg",
                Username = "tonsan2",
                Profile = new Profile
                {
                    FirstName = "Tony",
                    LastName = "KJJ",
                    DateOfBirth = new DateTime()
                }
            });
            context.SaveChanges();
        }

        // Use a separate instance of the context to verify correct data retrieval
        using (var context = new ApplicationDbContext(options))
        {
            var mockLogger = new Mock<ILogger<UsersController>>();
            var controller = new UsersController(context, mockLogger.Object);

            // Act
            var result = await controller.GetUsers();


            // Assert
            Assert.IsType<ActionResult<IEnumerable<User>>>(result);
            var okResult = Assert.IsAssignableFrom<ActionResult<IEnumerable<User>>>(result).Result as OkObjectResult;
            Assert.NotNull(okResult);
            var model = Assert.IsType<List<User>>(okResult.Value);
            Assert.Equal(2, model.Count());
        }
    }
}
