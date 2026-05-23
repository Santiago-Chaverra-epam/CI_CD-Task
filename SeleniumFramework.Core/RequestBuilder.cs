using RestSharp;

namespace SeleniumFramework.Core;

public class RequestBuilder
{
    private readonly RestRequest _request;

    public RequestBuilder(string resource, Method method = Method.Get)
    {
        _request = new RestRequest(resource, method);
    }

    public RequestBuilder WithJsonBody<T>(T body) where T : class
    {
        _request.AddJsonBody(body);
        return this;
    }

    public RequestBuilder WithHeader(string name, string value)
    {
        _request.AddHeader(name, value);
        return this;
    }

    public RequestBuilder WithQueryParameter(string name, string value)
    {
        _request.AddQueryParameter(name, value);
        return this;
    }

    public RestRequest Build() => _request;
}
