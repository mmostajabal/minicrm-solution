using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniCRM.ProjectService.Controllers;
using MiniCRM.Shared.DTOs;
using MiniCRM.Shared.Exceptions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace MiniCRM.Tests.ProjectService;

public class ProjectsControllerTests
{
    private readonly IProjectApiClient _api = Substitute.For<IProjectApiClient>();
    private readonly ILogger<ProjectsController> _logger = Substitute.For<ILogger<ProjectsController>>();
    private readonly ProjectsController _sut;

    public ProjectsControllerTests()
    {
        _sut = new ProjectsController(_api, _logger);
    }

    // --- Search ---

    [Fact]
    public async Task Search_ReturnsOk_WithResults()
    {
        var expected = new ProjectListResponse { Count = 2 };
        _api.SearchAsync(Arg.Any<ProjectSearchParams>(), Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await _sut.Search(new ProjectSearchParams(), CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(expected);
    }

    [Fact]
    public async Task Search_ReturnsStatusCode_WhenMiniCrmExceptionThrown()
    {
        _api.SearchAsync(Arg.Any<ProjectSearchParams>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new MiniCrmAuthException("Unauthorized"));

        var result = await _sut.Search(new ProjectSearchParams(), CancellationToken.None);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(401);
    }

    // --- Get ---

    [Fact]
    public async Task Get_ReturnsOk_WhenProjectFound()
    {
        var project = new ProjectDto { Id = 1, Name = "Test Project" };
        _api.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(project);

        var result = await _sut.Get(1, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(project);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenProjectMissing()
    {
        _api.GetByIdAsync(99, Arg.Any<CancellationToken>())
            .ThrowsAsync(new MiniCrmNotFoundException("Project", 99));

        var result = await _sut.Get(99, CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // --- Create ---

    [Fact]
    public async Task Create_ReturnsCreated_WhenSuccessful()
    {
        var dto = new ProjectDto { Name = "New Deal" };
        var created = new ProjectDto { Id = 20, Name = "New Deal" };
        _api.CreateAsync(dto, Arg.Any<CancellationToken>()).Returns(created);

        var result = await _sut.Create(dto, CancellationToken.None);

        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().Be(created);
    }

    // --- ChangeStatus ---

    [Fact]
    public async Task ChangeStatus_ReturnsOk_WhenSuccessful()
    {
        var updated = new ProjectDto { Id = 5, StatusId = 3, StatusName = "Closed" };
        _api.ChangeStatusAsync(5, 3, Arg.Any<CancellationToken>()).Returns(updated);

        var result = await _sut.ChangeStatus(5, new ProjectStatusChangeRequest { StatusId = 3 }, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(updated);
    }

    [Fact]
    public async Task ChangeStatus_ReturnsNotFound_WhenProjectMissing()
    {
        _api.ChangeStatusAsync(99, Arg.Any<int>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new MiniCrmNotFoundException("Project", 99));

        var result = await _sut.ChangeStatus(99, new ProjectStatusChangeRequest { StatusId = 1 }, CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ChangeStatus_ReturnsStatusCode_WhenRateLimitHit()
    {
        _api.ChangeStatusAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new MiniCrmRateLimitException());

        var result = await _sut.ChangeStatus(1, new ProjectStatusChangeRequest { StatusId = 2 }, CancellationToken.None);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(429);
    }
}
