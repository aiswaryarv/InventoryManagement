using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Controllers;
using InventoryManagement.Models;
using InventoryManagement.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;


public class AuthControllerTests
{
    private readonly AuthController _controller;
    private readonly InventoryContext _context;

    public AuthControllerTests()
    {
        var options = new DbContextOptionsBuilder<InventoryContext>()
            .UseInMemoryDatabase(databaseName: "TestDb") // Use in-memory DB for testing
            .Options;

     
        _context = new InventoryContext(options);

      
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.SetupGet(config => config["Jwt:Issuer"]).Returns("https://localhost");
        mockConfiguration.SetupGet(config => config["Jwt:Audience"]).Returns("your-audience");
        mockConfiguration.SetupGet(config => config["Jwt:SecretKey"]).Returns("your-secret-key");

        _controller = new AuthController(mockConfiguration.Object, _context); // Pass correct dependencies
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenUserIsValid()
    {
        // Arrange
        var user = new User { Username = "testuser", Password = "password" };

        // Act
        var result = await _controller.Register(user);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}