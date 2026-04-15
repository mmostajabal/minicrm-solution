using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniCRM.Shared.DTOs;
using MiniCRM.Shared.Exceptions;
using MiniCRM.Shared.Services;
using MiniCRM.TodoService.Controllers;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace MiniCRM.Tests.TodoService;

public class TodosControllerTests
{
    private readonly ITodoApiClient _api = Substitute.For<ITodoApiClient>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly ILogger<TodosController> _logger = Substitute.For<ILogger<TodosController>>();
    private readonly TodosController _sut;

    public TodosControllerTests()
    {
        // Cache always returns null (miss) so tests exercise the real API path
        _cache.GetAsync<TodoListResponse>(Arg.Any<string>(), Arg.Any<CancellationToken>())
              .Returns((TodoListResponse?)null);
        _sut = new TodosController(_api, _cache, _logger);
    }

    // --- GetList ---

    [Fact]
    public async Task GetList_ReturnsOk_WithTodos()
    {
        var expected = new TodoListResponse { Count = 3 };
        _api.GetListAsync(10, Arg.Any<CancellationToken>()).Returns(expected);

        var result = await _sut.GetList(10, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(expected);
    }

    [Fact]
    public async Task GetList_ReturnsStatusCode_WhenMiniCrmExceptionThrown()
    {
        _api.GetListAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new MiniCrmRateLimitException());

        var result = await _sut.GetList(1, CancellationToken.None);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(429);
    }

    // --- Create ---

    [Fact]
    public async Task Create_ReturnsCreated_WhenSuccessful()
    {
        var req = new TodoCreateRequest { ProjectId = 5, Comment = "Follow up" };
        var created = new TodoDto { Id = 100, Comment = "Follow up" };
        _api.CreateAsync(req, Arg.Any<CancellationToken>()).Returns(created);

        var result = await _sut.Create(req, CancellationToken.None);

        result.Should().BeOfType<CreatedResult>()
            .Which.Value.Should().Be(created);
    }

    [Fact]
    public async Task Create_ReturnsStatusCode_WhenMiniCrmExceptionThrown()
    {
        var req = new TodoCreateRequest { ProjectId = 1 };
        _api.CreateAsync(req, Arg.Any<CancellationToken>())
            .ThrowsAsync(new MiniCrmApiException("Validation error", 422));

        var result = await _sut.Create(req, CancellationToken.None);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(422);
    }
}
