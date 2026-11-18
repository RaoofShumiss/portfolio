using Microsoft.AspNetCore.Mvc;
using PortfolioAPI.Controllers;
using PortfolioAPI.DTOs;

namespace PortfolioAPI.Tests;

public class BlogsControllerTests
{
    [Fact]
    public async Task GetAll_ItemsAvailable_ReturnsOkWithItems()
    {
        // Arrange
        var service = new MockBlogService
        {
            Items = [new BlogResponseDto { Id = 7, Titel = "Eerste" },
                     new BlogResponseDto { Id = 8, Titel = "Tweede" }]
        };
        var controller = new BlogsController(service);

        // Act
        var result = await controller.GetAll(CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsType<List<BlogResponseDto>>(ok.Value);
        Assert.Same(service.Items, items);
        Assert.Equal(2, items.Count);
        Assert.Equal("Eerste", items[0].Titel);
    }

    [Fact]
    public async Task GetAll_NoItems_ReturnsOkWithEmptyList()
    {
        // Arrange
        var service = new MockBlogService();
        var controller = new BlogsController(service);

        // Act
        var result = await controller.GetAll(CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Empty(Assert.IsType<List<BlogResponseDto>>(ok.Value));
    }

    [Theory]
    [InlineData(7, "Eerste")]
    [InlineData(8, "Tweede")]
    public async Task GetById_ExistingId_ReturnsOkWithItem(int id, string title)
    {
        // Arrange
        var service = new MockBlogService
        {
            GetByIdResult = new BlogResponseDto { Id = id, Titel = title }
        };
        var controller = new BlogsController(service);

        // Act
        var result = await controller.GetById(id, CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var item = Assert.IsType<BlogResponseDto>(ok.Value);
        Assert.Equal(id, service.ReceivedId);
        Assert.Same(service.GetByIdResult, item);
        Assert.Equal(title, item.Titel);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        // Arrange
        var service = new MockBlogService { GetByIdResult = null };
        var controller = new BlogsController(service);

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
        var dto = new BlogCreateDto { Titel = "Nieuw", Inhoud = "Inhoud", Publicatiedatum = new DateTime(2026, 11, 18) };
        var service = new MockBlogService
        {
            CreateResult = new BlogResponseDto { Id = 12, Titel = dto.Titel }
        };
        var controller = new BlogsController(service);

        // Act
        var result = await controller.Create(dto, CancellationToken.None);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal(nameof(BlogsController.GetById), created.ActionName);
        Assert.Equal(12, created.RouteValues!["id"]);
        Assert.Same(service.CreateResult, created.Value);
        Assert.Same(dto, service.ReceivedCreateDto);
        Assert.Equal(1, service.WriteCalls);
    }

    [Fact]
    public async Task Update_ExistingId_PassesDtoAndReturnsUpdatedItem()
    {
        // Arrange
        var dto = new BlogUpdateDto { Titel = "Gewijzigd", Inhoud = "Inhoud", Publicatiedatum = new DateTime(2026, 11, 18) };
        var service = new MockBlogService
        {
            UpdateResult = new BlogResponseDto { Id = 12, Titel = dto.Titel }
        };
        var controller = new BlogsController(service);

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
        var dto = new BlogUpdateDto { Titel = "Gewijzigd", Inhoud = "Inhoud", Publicatiedatum = new DateTime(2026, 11, 18) };
        var service = new MockBlogService { UpdateResult = null };
        var controller = new BlogsController(service);

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
        var service = new MockBlogService { DeleteResult = deleted };
        var controller = new BlogsController(service);

        // Act
        var result = await controller.Delete(12, CancellationToken.None);

        // Assert
        var status = Assert.IsAssignableFrom<StatusCodeResult>(result);
        Assert.Equal(expectedStatus, status.StatusCode);
        Assert.Equal(12, service.ReceivedId);
        Assert.Equal(1, service.WriteCalls);
    }
}
