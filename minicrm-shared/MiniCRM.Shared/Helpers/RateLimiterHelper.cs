using System.Collections.Concurrent;
using MiniCRM.Shared.Exceptions;

namespace MiniCRM.Shared.Helpers;

/// <summary>
/// Sliding-window rate limiter: enforces the miniCRM 60 requests/minute limit.
/// Thread-safe, per-instance; create one instance per HttpClient/service.
/// </summary>
public sealed class RateLimiterHelper
{
    private readonly int _maxRequests;
    private readonly TimeSpan _window;
    private readonly ConcurrentQueue<DateTime> _timestamps = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <param name="maxRequests">Maximum requests in the time window (default 60).</param>
    /// <param name="windowSeconds">Window duration in seconds (default 60).</param>
    public RateLimiterHelper(int maxRequests = 60, int windowSeconds = 60)
    {
        _maxRequests = maxRequests;
        _window = TimeSpan.FromSeconds(windowSeconds);
    }

    /// <summary>
    /// Checks if a request can proceed; throws <see cref="MiniCrmRateLimitException"/>
    /// if the limit would be exceeded, or waits up to <paramref name="waitMs"/> ms.
    /// </summary>
    public async Task AcquireAsync(int waitMs = 5000, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            var deadline = DateTime.UtcNow.Add(TimeSpan.FromMilliseconds(waitMs));

            while (true)
            {
                Purge();
                if (_timestamps.Count < _maxRequests)
                {
                    _timestamps.Enqueue(DateTime.UtcNow);
                    return;
                }

                if (DateTime.UtcNow >= deadline)
                    throw new MiniCrmRateLimitException();

                // Wait until the oldest timestamp expires
                _timestamps.TryPeek(out var oldest);
                var delay = oldest.Add(_window) - DateTime.UtcNow;
                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay < TimeSpan.FromMilliseconds(waitMs) ? delay : TimeSpan.FromMilliseconds(500), ct);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void Purge()
    {
        var cutoff = DateTime.UtcNow.Subtract(_window);
        while (_timestamps.TryPeek(out var ts) && ts < cutoff)
            _timestamps.TryDequeue(out _);
    }

    /// <summary>Returns the number of requests used in the current window.</summary>
    public int CurrentCount
    {
        get { Purge(); return _timestamps.Count; }
    }
}
