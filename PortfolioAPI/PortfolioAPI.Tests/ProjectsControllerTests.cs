using Microsoft.AspNetCore.Mvc;
using PortfolioAPI.Controllers;
using PortfolioAPI.DTOs;

namespace PortfolioAPI.Tests;

public class ProjectsControllerTests
{
    [Fact]
    public async Task GetAll_ItemsAvailable_ReturnsOkWithItems()
    {
        // Arrange
        var service = new MockProjectService
        {
            Items = [new ProjectResponseDto { Id = 7, Title = "Eerste" },
                     new ProjectResponseDto { Id = 8, Title = "Tweede" }]
        };
        var controller = new ProjectsController(service);

        // Act
        var result = await controller.GetAll(CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsType<List<ProjectResponseDto>>(ok.Value);
        Assert.Same(service.Items, items);
        Assert.Equal(2, items.Count);
        Assert.Equal("Eerste", items[0].Title);
    }

    [Fact]
    public async Task GetAll_NoItems_ReturnsOkWithEmptyList()
    {
        // Arrange
        var service = new MockProjectService();
        var controller = new ProjectsController(service);

        // Act
        var result = await controller.GetAll(CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Empty(Assert.IsType<List<ProjectResponseDto>>(ok.Value));
    }

    [Theory]
    [InlineData(7, "Eerste")]
    [InlineData(8, "Tweede")]
    public async Task GetById_ExistingId_ReturnsOkWithItem(int id, string title)
    {
        // Arrange
        var service = new MockProjectService
        {
            GetByIdResult = new ProjectResponseDto { Id = id, Title = title }
        };
        var controller = new ProjectsController(service);

        // Act
        var result = await controller.GetById(id, CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var item = Assert.IsType<ProjectResponseDto>(ok.Value);
        Assert.Equal(id, service.ReceivedId);
        Assert.Same(service.GetByIdResult, item);
        Assert.Equal(title, item.Title);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        // Arrange
        var service = new MockProjectService { GetByIdResult = null };
        var controller = new ProjectsController(service);

        // Act
        var result = await controller.GetById(99, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        Assert.Equal(99, service.ReceivedId);
    }

    [Fact]
    public async Task Create_ValidDto_PassesDtoAndReturnsCreatedItem()
    {
        // Arrange
        var dto = new ProjectCreateDto { Title = "Nieuw", Description = "Beschrijving", Category = "web", Technologies = ["C#"], GitHubUrl = "https://github.com/example/project", Datum = new DateTime(2026, 11, 18) };
        var service = new MockProjectService
        {
            CreateResult = new ProjectResponseDto { Id = 12, Title = dto.Title }
        };
        var controller = new ProjectsController(service);

        // Act
        var result = await controller.Create(dto, CancellationToken.None);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal(nameof(ProjectsController.GetById), created.ActionName);
        Assert.Equal(12, created.RouteValues!["id"]);
        Assert.Same(service.CreateResult, created.Value);
        Assert.Same(dto, service.ReceivedCreateDto);
        Assert.Equal(1, service.WriteCalls);
    }

    [Fact]
    public async Task Update_ExistingId_PassesDtoAndReturnsUpdatedItem()
    {
        // Arrange
        var dto = new ProjectUpdateDto { Title = "Gewijzigd", Description = "Beschrijving", Category = "web", Technologies = ["C#"], GitHubUrl = "https://github.com/example/project", Datum = new DateTime(2026, 11, 18) };
        var service = new MockProjectService
        {
            UpdateResult = new ProjectResponseDto { Id = 12, Title = dto.Title }
        };
        var controller = new ProjectsController(service);

        // Act
        var result = await controller.Update(12, dto, CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(service.UpdateResult, ok.Value);
        Assert.Equal(12, service.ReceivedId);
        Assert.Same(dto, service.ReceivedUpdateDto);
        Assert.Equal(1, service.WriteCalls);
    }

    [Fact]
    public async Task Update_UnknownId_ReturnsNotFound()
    {
        // Arrange
        var dto = new ProjectUpdateDto { Title = "Gewijzigd", Description = "Beschrijving", Category = "web", Technologies = ["C#"], GitHubUrl = "https://github.com/example/project", Datum = new DateTime(2026, 11, 18) };
        var service = new MockProjectService { UpdateResult = null };
        var controller = new ProjectsController(service);

        // Act
        var result = await controller.Update(99, dto, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        Assert.Equal(99, service.ReceivedId);
        Assert.Same(dto, service.ReceivedUpdateDto);
        Assert.Equal(1, service.WriteCalls);
    }

    [Theory]
    [InlineData(true, 204)]
    [InlineData(false, 404)]
    public async Task Delete_ServiceResult_ReturnsExpectedStatus(bool deleted, int expectedStatus)
    {
        // Arrange
        var service = new MockProjectService { DeleteResult = deleted };
        var controller = new ProjectsController(service);

        // Act
        var result = await controller.Delete(12, CancellationToken.None);

        // Assert
        var status = Assert.IsAssignableFrom<StatusCodeResult>(result);
        Assert.Equal(expectedStatus, status.StatusCode);
        Assert.Equal(12, service.ReceivedId);
        Assert.Equal(1, service.WriteCalls);
    }
}
