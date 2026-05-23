using RestSharp;
using Serilog;

namespace SeleniumFramework.Core;

public class ApiClient
{
    private readonly RestClient _client;
    private readonly ILogger _logger;

    public ApiClient(string baseUrl, ILogger logger)
    {
        _client = new RestClient(baseUrl);
        _logger = logger;
    }

    public RestResponse<T> Execute<T>(RestRequest request)
    {
        var uri = _client.BuildUri(request);
        _logger.Information("Sending {Method} request to {Uri}", request.Method, uri);

        var response = _client.Execute<T>(request);

        // RestSharp 107+ sets ErrorException for any non-2xx status code.
        // We clear it when a valid HTTP response was received so callers can distinguish
        // genuine transport failures (ErrorException set, StatusCode == 0) from expected
        // HTTP error responses (ErrorException null, StatusCode == 4xx/5xx).
        if ((int)response.StatusCode > 0)
            response.ErrorException = null;

        _logger.Information("Received {StatusCode} ({StatusDescription})",
            (int)response.StatusCode, response.StatusDescription);

        if (response.ErrorException is not null)
            _logger.Error(response.ErrorException, "Transport error: {ErrorMessage}", response.ErrorMessage);

        return response;
    }

    public RestResponse Execute(RestRequest request)
    {
        var uri = _client.BuildUri(request);
        _logger.Information("Sending {Method} request to {Uri}", request.Method, uri);

        var response = _client.Execute(request);

        if ((int)response.StatusCode > 0)
            response.ErrorException = null;

        _logger.Information("Received {StatusCode} ({StatusDescription})",
            (int)response.StatusCode, response.StatusDescription);

        if (response.ErrorException is not null)
            _logger.Error(response.ErrorException, "Transport error: {ErrorMessage}", response.ErrorMessage);

        return response;
    }
}
