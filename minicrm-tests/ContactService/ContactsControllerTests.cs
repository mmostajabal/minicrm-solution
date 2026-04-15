using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniCRM.ContactService.Controllers;
using MiniCRM.Shared.DTOs;
using MiniCRM.Shared.Exceptions;
using MiniCRM.Shared.Services;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace MiniCRM.Tests.ContactService;

public class ContactsControllerTests
{
    private readonly IContactApiClient _api = Substitute.For<IContactApiClient>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly ILogger<ContactsController> _logger = Substitute.For<ILogger<ContactsController>>();
    private readonly ContactsController _sut;

    public ContactsControllerTests()
    {
        // Cache always returns null (miss) so tests exercise the real API path
        _cache.GetAsync<ContactListResponse>(Arg.Any<string>(), Arg.Any<CancellationToken>())
              .Returns((ContactListResponse?)null);
        _cache.GetAsync<ContactDto>(Arg.Any<string>(), Arg.Any<CancellationToken>())
              .Returns((ContactDto?)null);
        _sut = new ContactsController(_api, _cache, _logger);
    }

    // --- Search ---

    [Fact]
    public async Task Search_ReturnsOk_WithResults()
    {
        var expected = new ContactListResponse { Count = 1, Results = new() { ["1"] = new ContactDto { Id = 1, FirstName = "John" } } };
        _api.SearchAsync(Arg.Any<ContactSearchParams>(), Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await _sut.Search(new ContactSearchParams(), CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(expected);
    }

    [Fact]
    public async Task Search_ReturnsStatusCode_WhenMiniCrmExceptionThrown()
    {
        _api.SearchAsync(Arg.Any<ContactSearchParams>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new MiniCrmAuthException("Unauthorized"));

        var result = await _sut.Search(new ContactSearchParams(), CancellationToken.None);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(401);
    }

    // --- Get ---

    [Fact]
    public async Task Get_ReturnsOk_WhenContactFound()
    {
        var contact = new ContactDto { Id = 42, FirstName = "Jane" };
        _api.GetByIdAsync(42, Arg.Any<CancellationToken>()).Returns(contact);

        var result = await _sut.Get(42, CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(contact);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenContactMissing()
    {
        _api.GetByIdAsync(99, Arg.Any<CancellationToken>())
            .ThrowsAsync(new MiniCrmNotFoundException("Contact", 99));

        var result = await _sut.Get(99, CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Get_ReturnsStatusCode_WhenMiniCrmExceptionThrown()
    {
        _api.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .ThrowsAsync(new MiniCrmRateLimitException());

        var result = await _sut.Get(1, CancellationToken.None);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(429);
    }

    // --- Create ---

    [Fact]
    public async Task Create_ReturnsCreated_WhenSuccessful()
    {
        var dto = new ContactDto { FirstName = "New", LastName = "Contact" };
        var created = new ContactDto { Id = 10, FirstName = "New", LastName = "Contact" };
        _api.CreateAsync(dto, Arg.Any<CancellationToken>()).Returns(created);

        var result = await _sut.Create(dto, CancellationToken.None);

        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().Be(created);
    }

    [Fact]
    public async Task Create_ReturnsStatusCode_WhenMiniCrmExceptionThrown()
    {
        var dto = new ContactDto { FirstName = "Test" };
        _api.CreateAsync(dto, Arg.Any<CancellationToken>())
            .ThrowsAsync(new MiniCrmApiException("Bad request", 400));

        var result = await _sut.Create(dto, CancellationToken.None);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    // --- Update ---

    [Fact]
    public async Task Update_ReturnsOk_WhenSuccessful()
    {
        var dto = new ContactDto { FirstName = "Updated" };
        var updated = new ContactDto { Id = 5, FirstName = "Updated" };
        _api.UpdateAsync(5, Arg.Any<ContactDto>(), Arg.Any<CancellationToken>()).Returns(updated);

        var result = await _sut.Update(5, dto, CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(updated);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenContactMissing()
    {
        _api.UpdateAsync(99, Arg.Any<ContactDto>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new MiniCrmNotFoundException("Contact", 99));

        var result = await _sut.Update(99, new ContactDto(), CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
