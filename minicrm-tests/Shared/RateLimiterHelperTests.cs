using FluentAssertions;
using MiniCRM.Shared.Helpers;
using Xunit;

namespace MiniCRM.Tests.Shared;

public class RateLimiterHelperTests
{
    [Fact]
    public async Task AcquireAsync_Succeeds_WhenUnderLimit()
    {
        // Allow 5 requests per second for fast testing
        var limiter = new RateLimiterHelper(5, 1);

        var tasks = Enumerable.Range(0, 5)
            .Select(_ => limiter.AcquireAsync());

        // Should all complete without throwing
        await Task.WhenAll(tasks);
    }

    [Fact]
    public async Task AcquireAsync_DoesNotThrow_WhenLimitReached()
    {
        var limiter = new RateLimiterHelper(2, 1);

        // Exhaust the limit
        await limiter.AcquireAsync();
        await limiter.AcquireAsync();

        // 3rd request should wait but eventually succeed (not throw)
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var act = async () => await limiter.AcquireAsync(ct: cts.Token);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AcquireAsync_ThrowsOperationCanceled_WhenCancelled()
    {
        var limiter = new RateLimiterHelper(1, 60);

        // Exhaust the limit
        await limiter.AcquireAsync();

        // Cancel immediately
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await limiter.AcquireAsync(ct: cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
