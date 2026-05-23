using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using RestSharp;
using Serilog;
using SeleniumFramework.Core;
using SeleniumFramework.Pages.Models;
using System.Net;

namespace SeleniumFramework.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
[Category("API")]
public class ApiTests
{
    private const string BaseUrl = "https://jsonplaceholder.typicode.com";

    // Shared across parallel test threads — RestClient and Serilog are thread-safe.
    private ILogger _logger = null!;
    private ApiClient _apiClient = null!;

    [OneTimeSetUp]
    public void InitialiseTestRun()
    {
        // Min log level is driven by "Serilog:MinimumLevel" in appsettings.json.
        // Change the value there to adjust verbosity: Debug, Information, Warning, Error.
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/api-tests-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        _logger = Log.Logger;
        _apiClient = new ApiClient(BaseUrl, _logger);

        _logger.Information("API test run initialised — base URL: {BaseUrl}", BaseUrl);
    }

    [SetUp]
    public void SetUp()
    {
        _logger.Information("=== Test start: {TestName} ===", TestContext.CurrentContext.Test.Name);
    }

    [TearDown]
    public void TearDown()
    {
        var result = TestContext.CurrentContext.Result;
        var status = result.Outcome.Status;

        if (status == TestStatus.Failed)
            _logger.Error("Test failed: {Message}", result.Message);

        _logger.Information("=== Test end: {TestName} — {Status} ===",
            TestContext.CurrentContext.Test.Name, status);
    }

    [OneTimeTearDown]
    public void FlushLogging()
    {
        _logger.Information("API test run complete");
        Log.CloseAndFlush();
    }

    [Test]
    public void ValidateListOfUsersCanBeReceivedSuccessfully()
    {
        _logger.Information("Building GET /users request");
        var request = new RequestBuilder("/users", Method.Get).Build();

        _logger.Information("Executing request and deserializing response as List<User>");
        var response = _apiClient.Execute<List<User>>(request);

        _logger.Information("Asserting 200 OK and no network errors");
        Assert.That((int)response.StatusCode, Is.EqualTo(200),
            $"Expected 200 OK but got {response.StatusCode}");
        Assert.That(response.ErrorException, Is.Null,
            $"Unexpected network error: {response.ErrorMessage}");

        _logger.Information("Asserting response body is not empty");
        Assert.That(response.Data, Is.Not.Null.And.Not.Empty,
            "Response body should contain at least one user");

        _logger.Information("Asserting each user contains all required fields");
        foreach (var user in response.Data!)
        {
            Assert.Multiple(() =>
            {
                Assert.That(user.Id, Is.GreaterThan(0),
                    $"User Id should be a positive integer (got {user.Id})");
                Assert.That(user.Name, Is.Not.Empty,
                    $"User {user.Id} Name should not be empty");
                Assert.That(user.Username, Is.Not.Empty,
                    $"User {user.Id} Username should not be empty");
                Assert.That(user.Email, Is.Not.Empty,
                    $"User {user.Id} Email should not be empty");
                Assert.That(user.Address, Is.Not.Null,
                    $"User {user.Id} Address should not be null");
                Assert.That(user.Phone, Is.Not.Empty,
                    $"User {user.Id} Phone should not be empty");
                Assert.That(user.Website, Is.Not.Empty,
                    $"User {user.Id} Website should not be empty");
                Assert.That(user.Company, Is.Not.Null,
                    $"User {user.Id} Company should not be null");
            });
        }

        _logger.Information("All required user fields validated successfully for {Count} user(s)",
            response.Data!.Count);
    }

    [Test]
    public void ValidateContentTypeHeaderForListOfUsers()
    {
        _logger.Information("Building GET /users request to validate Content-Type response header");
        var request = new RequestBuilder("/users", Method.Get).Build();

        _logger.Information("Executing request");
        var response = _apiClient.Execute(request);

        _logger.Information("Asserting 200 OK and no network errors");
        Assert.That((int)response.StatusCode, Is.EqualTo(200),
            $"Expected 200 OK but got {response.StatusCode}");
        Assert.That(response.ErrorException, Is.Null,
            $"Unexpected network error: {response.ErrorMessage}");

        _logger.Information("Locating Content-Type header in response");
        var contentTypeHeader = response.ContentHeaders?
            .FirstOrDefault(h => h.Name?.Equals("Content-Type", StringComparison.OrdinalIgnoreCase) == true);

        _logger.Information("Asserting Content-Type header is present");
        Assert.That(contentTypeHeader, Is.Not.Null,
            "Content-Type header should be present in the response");

        var actualValue = contentTypeHeader!.Value?.ToString();
        _logger.Information(
            "Asserting Content-Type value is 'application/json; charset=utf-8' (actual: {Actual})",
            actualValue);
        Assert.That(actualValue, Is.EqualTo("application/json; charset=utf-8"),
            $"Content-Type header value mismatch — got: {actualValue}");

        _logger.Information("Content-Type header validated successfully");
    }

    [Test]
    public void ValidateResponseBodyContainsTenUsersWithValidData()
    {
        _logger.Information("Building GET /users request to validate full response body");
        var request = new RequestBuilder("/users", Method.Get).Build();

        _logger.Information("Executing request and deserializing response");
        var response = _apiClient.Execute<List<User>>(request);

        _logger.Information("Asserting 200 OK and no network errors");
        Assert.That((int)response.StatusCode, Is.EqualTo(200),
            $"Expected 200 OK but got {response.StatusCode}");
        Assert.That(response.ErrorException, Is.Null,
            $"Unexpected network error: {response.ErrorMessage}");

        _logger.Information("Asserting response body contains exactly 10 users");
        Assert.That(response.Data, Has.Count.EqualTo(10),
            "Response body should contain exactly 10 users");

        _logger.Information("Asserting all user IDs are unique");
        var ids = response.Data!.Select(u => u.Id).ToList();
        Assert.That(ids, Is.Unique, "Every user should have a distinct ID");

        _logger.Information("Asserting each user has non-empty Name, Username and Company Name");
        foreach (var user in response.Data!)
        {
            Assert.Multiple(() =>
            {
                Assert.That(user.Name, Is.Not.Empty,
                    $"User {user.Id} Name should not be empty");
                Assert.That(user.Username, Is.Not.Empty,
                    $"User {user.Id} Username should not be empty");
                Assert.That(user.Company, Is.Not.Null,
                    $"User {user.Id} Company should not be null");
                Assert.That(user.Company.Name, Is.Not.Empty,
                    $"User {user.Id} Company Name should not be empty");
            });
        }

        _logger.Information(
            "Response body fully validated: 10 users with unique IDs and valid data");
    }

    [Test]
    public void ValidateThatUserCanBeCreated()
    {
        var newUser = new CreateUserRequest { Name = "John Doe", Username = "johndoe" };

        _logger.Information("Building POST /users request with Name: '{Name}', Username: '{Username}'",
            newUser.Name, newUser.Username);

        var request = new RequestBuilder("/users", Method.Post)
            .WithJsonBody(newUser)
            .Build();

        _logger.Information("Executing POST request");
        var response = _apiClient.Execute<User>(request);

        _logger.Information("Asserting 201 Created and no network errors");
        Assert.That((int)response.StatusCode, Is.EqualTo(201),
            $"Expected 201 Created but got {response.StatusCode}");
        Assert.That(response.ErrorException, Is.Null,
            $"Unexpected network error: {response.ErrorMessage}");

        _logger.Information("Asserting response is not empty and contains a valid ID");
        Assert.That(response.Data, Is.Not.Null,
            "Response body should not be empty after user creation");
        Assert.That(response.Data!.Id, Is.GreaterThan(0),
            "Created user should have a positive ID in the response");

        _logger.Information("User created successfully — returned ID: {Id}", response.Data.Id);
    }

    [Test]
    public void ValidateThatUserIsNotifiedIfResourceDoesNotExist()
    {
        _logger.Information("Building GET /invalidendpoint request to validate 404 handling");
        var request = new RequestBuilder("/invalidendpoint", Method.Get).Build();

        _logger.Information("Executing request to non-existent endpoint");
        var response = _apiClient.Execute(request);

        _logger.Information("Asserting 404 Not Found and no network-level errors");
        Assert.That((int)response.StatusCode, Is.EqualTo(404),
            $"Expected 404 Not Found but got {response.StatusCode}");
        Assert.That(response.ErrorException, Is.Null,
            "No network error should be raised for a standard 404 response");

        _logger.Information(
            "404 Not Found response validated — resource correctly reported as missing");
    }
}
