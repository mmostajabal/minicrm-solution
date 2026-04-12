namespace MiniCRM.Shared.Exceptions;

/// <summary>
/// Base exception for all miniCRM integration errors.
/// </summary>
public class MiniCrmException : Exception
{
    public int? StatusCode { get; }

    public MiniCrmException(string message, int? statusCode = null)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public MiniCrmException(string message, Exception innerException, int? statusCode = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}

/// <summary>
/// Thrown when the miniCRM API returns a 401 / 403 response.
/// </summary>
public class MiniCrmAuthException : MiniCrmException
{
    public MiniCrmAuthException(string message)
        : base(message, 401) { }
}

/// <summary>
/// Thrown when a requested resource is not found (404).
/// </summary>
public class MiniCrmNotFoundException : MiniCrmException
{
    public MiniCrmNotFoundException(string resource, object id)
        : base($"{resource} with id '{id}' was not found.", 404) { }
}

/// <summary>
/// Thrown when the 60 req/min rate limit is hit.
/// </summary>
public class MiniCrmRateLimitException : MiniCrmException
{
    public MiniCrmRateLimitException()
        : base("miniCRM API rate limit reached (60 requests/minute). Please wait and retry.", 429) { }
}

/// <summary>
/// Thrown when the API returns unexpected or malformed data.
/// </summary>
public class MiniCrmApiException : MiniCrmException
{
    public MiniCrmApiException(string message, int statusCode)
        : base(message, statusCode) { }
}
