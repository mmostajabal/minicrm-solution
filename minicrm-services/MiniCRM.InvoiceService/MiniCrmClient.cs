using System.Net.Http.Headers;
using System.Text;
using MiniCRM.Shared.Exceptions;
using MiniCRM.Shared.Helpers;

namespace MiniCRM.InvoiceService;

public abstract class MiniCrmClientBase
{
    protected readonly HttpClient Http;
    protected readonly ILogger Logger;
    private readonly RateLimiterHelper _rateLimiter = new(60, 60);

    protected MiniCrmClientBase(HttpClient http, IConfiguration config, ILogger logger)
    {
        Http   = http;
        Logger = logger;

        var systemId = config["MiniCRM:SystemId"] ?? throw new InvalidOperationException("MiniCRM:SystemId missing");
        var apiKey   = config["MiniCRM:ApiKey"]   ?? throw new InvalidOperationException("MiniCRM:ApiKey missing");
        var baseUrl  = config["MiniCRM:BaseUrl"]  ?? "https://r3.minicrm.hu";

        Http.BaseAddress = new Uri(baseUrl);
        var token = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{systemId}:{apiKey}"));
        Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
        Http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        Http.Timeout = TimeSpan.FromSeconds(30);
    }

    protected async Task AcquireRateLimitAsync(CancellationToken ct)
        => await _rateLimiter.AcquireAsync(ct: ct);

    protected static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;
        var body = await response.Content.ReadAsStringAsync();
        var code = (int)response.StatusCode;
        throw code switch
        {
            401 or 403 => new MiniCrmAuthException($"Authentication failed ({code}): {body}"),
            404        => new MiniCrmApiException($"Not found: {body}", 404),
            429        => new MiniCrmRateLimitException(),
            _          => new MiniCrmApiException($"miniCRM API error {code}: {body}", code)
        };
    }
}
