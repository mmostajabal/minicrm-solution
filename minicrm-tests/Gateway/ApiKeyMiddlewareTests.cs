using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MiniCRM.Gateway.Middleware;
using NSubstitute;
using Xunit;

namespace MiniCRM.Tests.Gateway;

public class ApiKeyMiddlewareTests
{
    private const string ValidKey = "test-gateway-key";

    private static ApiKeyMiddleware CreateMiddleware(RequestDelegate next)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Gateway:GatewayApiKey"] = ValidKey
            })
            .Build();

        var logger = Substitute.For<ILogger<ApiKeyMiddleware>>();
        return new ApiKeyMiddleware(next, config, logger);
    }

    private static DefaultHttpContext CreateContext(string? apiKey = null, string path = "/api/contacts")
    {
        var ctx = new DefaultHttpContext();
        ctx.Request.Path = path;
        if (apiKey is not null)
            ctx.Request.Headers["X-Gateway-Key"] = apiKey;
        ctx.Response.Body = new MemoryStream();
        return ctx;
    }

    [Fact]
    public async Task Invoke_CallsNext_WhenKeyIsValid()
    {
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
        var middleware = CreateMiddleware(next);
        var ctx = CreateContext(apiKey: ValidKey);

        await middleware.InvokeAsync(ctx);

        nextCalled.Should().BeTrue();
        ctx.Response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Invoke_Returns401_WhenKeyIsMissing()
    {
        var next = Substitute.For<RequestDelegate>();
        var middleware = CreateMiddleware(next);
        var ctx = CreateContext(apiKey: null);

        await middleware.InvokeAsync(ctx);

        ctx.Response.StatusCode.Should().Be(401);
        await next.DidNotReceive().Invoke(Arg.Any<HttpContext>());
    }

    [Fact]
    public async Task Invoke_Returns401_WhenKeyIsInvalid()
    {
        var next = Substitute.For<RequestDelegate>();
        var middleware = CreateMiddleware(next);
        var ctx = CreateContext(apiKey: "wrong-key");

        await middleware.InvokeAsync(ctx);

        ctx.Response.StatusCode.Should().Be(401);
        await next.DidNotReceive().Invoke(Arg.Any<HttpContext>());
    }

    [Fact]
    public async Task Invoke_AllowsSwaggerPath_WithoutKey()
    {
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
        var middleware = CreateMiddleware(next);
        var ctx = CreateContext(apiKey: null, path: "/swagger/index.html");

        await middleware.InvokeAsync(ctx);

        nextCalled.Should().BeTrue();
    }
}
