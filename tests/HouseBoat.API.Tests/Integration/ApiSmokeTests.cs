using HouseBoat.API.Tests.Factories;
using HouseBoat.API.Tests.Helpers;
using HouseBoat.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Xunit;

namespace HouseBoat.API.Tests.Integration;

public class ApiSmokeTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ApiSmokeTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHouseBoats_ShouldReturnSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/HouseBoats");

        // Assert
        Assert.True(
            response.IsSuccessStatusCode,
            $"Expected success but received: {response.StatusCode}");
    }

    [Fact]
    public async Task GetHouseBoats_ShouldReturnJsonResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/HouseBoats");

        // Assert
        Assert.Equal(
            "application/json",
            response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task AdminTest_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/Bookings/admin-test");

        // Assert
        Assert.Equal(
            System.Net.HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task AdminTest_WithCustomerToken_ShouldReturnForbidden()
    {
        // Get the actual JWT authentication options used by the API.
        var jwtBearerOptions = _factory.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        var validationParameters =
            jwtBearerOptions.TokenValidationParameters;

        // Retrieve the exact signing key used by the middleware.
        var signingKey = Assert.IsType<SymmetricSecurityKey>(
            validationParameters.IssuerSigningKey);

        var key = Encoding.UTF8.GetString(signingKey.Key);

        var token = JwtTokenHelper.GenerateToken(
            "Customer",
            validationParameters.ValidIssuer!,
            validationParameters.ValidAudience!,
            key);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync(
            "/api/Bookings/admin-test");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminTest_WithAdminToken_ShouldReturnSuccess()
    {
        // Arrange
        var jwtBearerOptions = _factory.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        var validationParameters =
            jwtBearerOptions.TokenValidationParameters;

        var signingKey = Assert.IsType<SymmetricSecurityKey>(
            validationParameters.IssuerSigningKey);

        var key = Encoding.UTF8.GetString(signingKey.Key);

        var token = JwtTokenHelper.GenerateToken(
            "Admin",
            validationParameters.ValidIssuer!,
            validationParameters.ValidAudience!,
            key);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync(
            "/api/Bookings/admin-test");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateBooking_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var jwtBearerOptions = _factory.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        var validationParameters =
            jwtBearerOptions.TokenValidationParameters;

        var signingKey = Assert.IsType<SymmetricSecurityKey>(
            validationParameters.IssuerSigningKey);

        var key = Encoding.UTF8.GetString(signingKey.Key);

        var token = JwtTokenHelper.GenerateToken(
            "Customer",
            validationParameters.ValidIssuer!,
            validationParameters.ValidAudience!,
            key);

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/Bookings");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var bookingRequest = new
        {
            houseBoatId = Guid.Parse(
                "a90f39cc-eb6e-40ed-acc3-e8998d830979"),
            checkInDate = DateTime.UtcNow.Date.AddDays(10),
            checkOutDate = DateTime.UtcNow.Date.AddDays(12),
            guestCount = 2
        };

        request.Content = JsonContent.Create(bookingRequest);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseBody = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.True(responseBody.TryGetProperty("id", out var bookingId));

        Assert.NotEqual(Guid.Empty, bookingId.GetGuid());

        Assert.Equal(
            "Booking created successfully.",
            responseBody.GetProperty("message").GetString());
    }

    [Fact]
    public async Task GetMyBookings_AfterCreatingBooking_ShouldReturnCustomerBooking()
    {
        // Arrange
        var token = TestAuthHelper.GenerateToken(
            _factory,
            "Customer");

        var checkInDate = DateTime.UtcNow.Date.AddDays(30);
        var checkOutDate = checkInDate.AddDays(2);

        var bookingRequest = new
        {
            houseBoatId = Guid.Parse(
                "a90f39cc-eb6e-40ed-acc3-e8998d830979"),
            checkInDate,
            checkOutDate,
            guestCount = 2
        };

        // Create booking
        using var createRequest = TestAuthHelper.CreateRequest(
            "POST",
            "/api/Bookings",
            token);

        createRequest.Content = JsonContent.Create(bookingRequest);

        var createResponse = await _client.SendAsync(createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        // Act: Get customer's bookings
        using var getRequest = TestAuthHelper.CreateRequest(
            "GET",
            "/api/Bookings/my",
            token);

        var getResponse = await _client.SendAsync(getRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var bookings = await getResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(JsonValueKind.Array, bookings.ValueKind);
        Assert.True(bookings.GetArrayLength() > 0);
    }

    [Fact]
    public async Task GetBookingById_AfterCreatingBooking_ShouldReturnBooking()
    {
        // Arrange: use the same customer token for both requests
        var token = TestAuthHelper.GenerateToken(_factory, "Customer");

        var checkInDate = DateTime.UtcNow.Date.AddDays(45);

        var bookingRequest = new
        {
            houseBoatId = Guid.Parse(
                "a90f39cc-eb6e-40ed-acc3-e8998d830979"),
            checkInDate,
            checkOutDate = checkInDate.AddDays(2),
            guestCount = 2
        };

        // Act 1: Create booking
        var createRequest = TestAuthHelper.CreateRequest(
            "POST",
            "/api/Bookings",
            token);

        createRequest.Content = JsonContent.Create(bookingRequest);

        var createResponse = await _client.SendAsync(createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        // Extract the newly created booking ID
        var createdBooking = await createResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var bookingId = createdBooking
            .GetProperty("id")
            .GetGuid();

        // Act 2: Get booking by ID
        var getRequest = TestAuthHelper.CreateRequest(
            "GET",
            $"/api/Bookings/{bookingId}",
            token);

        var getResponse = await _client.SendAsync(getRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var booking = await getResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            bookingId,
            booking.GetProperty("id").GetGuid());
    }

    [Fact]
    public async Task GetBookingById_WithDifferentCustomerToken_ShouldReturnNotFound()
    {
        // Customer A creates a booking
        var customerAToken =
            TestAuthHelper.GenerateToken(_factory, "Customer");

        var checkInDate = DateTime.UtcNow.Date.AddDays(60);

        var bookingRequest = new
        {
            houseBoatId = Guid.Parse(
                "a90f39cc-eb6e-40ed-acc3-e8998d830979"),
            checkInDate,
            checkOutDate = checkInDate.AddDays(2),
            guestCount = 2
        };

        var createRequest = TestAuthHelper.CreateRequest(
            "POST", "/api/Bookings", customerAToken);

        createRequest.Content = JsonContent.Create(bookingRequest);

        var createResponse = await _client.SendAsync(createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdBooking = await createResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var bookingId = createdBooking.GetProperty("id").GetGuid();

        // Customer B tries to access Customer A's booking
        var customerBToken =
            TestAuthHelper.GenerateToken(_factory, "Customer");

        var getRequest = TestAuthHelper.CreateRequest(
            "GET",
            $"/api/Bookings/{bookingId}",
            customerBToken);

        var getResponse = await _client.SendAsync(getRequest);

        // Customer B should not be able to access it
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task AdminCancelBooking_WithValidBooking_ShouldReturnSuccess()
    {
        // Arrange: Customer creates a booking
        var customerToken =
            TestAuthHelper.GenerateToken(_factory, "Customer");

        var checkInDate = DateTime.UtcNow.Date.AddDays(75);

        var bookingRequest = new
        {
            houseBoatId = Guid.Parse(
                "a90f39cc-eb6e-40ed-acc3-e8998d830979"),
            checkInDate,
            checkOutDate = checkInDate.AddDays(2),
            guestCount = 2
        };

        var createRequest = TestAuthHelper.CreateRequest(
            "POST",
            "/api/Bookings",
            customerToken);

        createRequest.Content = JsonContent.Create(bookingRequest);

        var createResponse = await _client.SendAsync(createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdBooking = await createResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var bookingId = createdBooking.GetProperty("id").GetGuid();

        // Act: Admin cancels the booking
        var adminToken =
            TestAuthHelper.GenerateToken(_factory, "Admin");

        var cancelRequest = TestAuthHelper.CreateRequest(
            "PUT",
            $"/api/Bookings/admin/{bookingId}/cancel",
            adminToken);

        var cancelResponse = await _client.SendAsync(cancelRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);

        var responseBody = await cancelResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            "Booking cancelled successfully.",
            responseBody.GetProperty("message").GetString());
    }

    [Fact]
    public async Task AdminCancelBooking_WithCustomerToken_ShouldReturnForbidden()
    {
        // Arrange: Create a booking as Customer A
        var customerToken =
            TestAuthHelper.GenerateToken(_factory, "Customer");

        var checkInDate = DateTime.UtcNow.Date.AddDays(90);

        var bookingRequest = new
        {
            houseBoatId = Guid.Parse(
                "a90f39cc-eb6e-40ed-acc3-e8998d830979"),
            checkInDate,
            checkOutDate = checkInDate.AddDays(2),
            guestCount = 2
        };

        var createRequest = TestAuthHelper.CreateRequest(
            "POST", "/api/Bookings", customerToken);

        createRequest.Content = JsonContent.Create(bookingRequest);

        var createResponse = await _client.SendAsync(createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdBooking = await createResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var bookingId = createdBooking.GetProperty("id").GetGuid();

        // Act: Customer tries to access the Admin endpoint
        var cancelRequest = TestAuthHelper.CreateRequest(
            "PUT",
            $"/api/Bookings/admin/{bookingId}/cancel",
            customerToken);

        var cancelResponse = await _client.SendAsync(cancelRequest);

        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            cancelResponse.StatusCode);
    }

    [Fact]
    public async Task AdminConfirmBooking_WithValidBooking_ShouldReturnSuccess()
    {
        // Arrange: Customer creates a booking
        var customerToken =
            TestAuthHelper.GenerateToken(_factory, "Customer");

        var checkInDate = DateTime.UtcNow.Date.AddDays(105);

        var bookingRequest = new
        {
            houseBoatId = Guid.Parse(
                "a90f39cc-eb6e-40ed-acc3-e8998d830979"),
            checkInDate,
            checkOutDate = checkInDate.AddDays(2),
            guestCount = 2
        };

        var createRequest = TestAuthHelper.CreateRequest(
            "POST", "/api/Bookings", customerToken);

        createRequest.Content = JsonContent.Create(bookingRequest);

        var createResponse = await _client.SendAsync(createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdBooking = await createResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var bookingId = createdBooking.GetProperty("id").GetGuid();

        // Act: Admin confirms the booking
        var adminToken =
            TestAuthHelper.GenerateToken(_factory, "Admin");

        var confirmRequest = TestAuthHelper.CreateRequest(
            "PUT",
            $"/api/Bookings/admin/{bookingId}/confirm",
            adminToken);

        var confirmResponse = await _client.SendAsync(confirmRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);
    }

    [Fact]
    public async Task AdminCompleteBooking_WithValidBooking_ShouldReturnSuccess()
    {
        // Arrange: Customer creates a booking
        var customerToken =
            TestAuthHelper.GenerateToken(_factory, "Customer");

        var checkInDate = DateTime.UtcNow.Date.AddDays(120);

        var bookingRequest = new
        {
            houseBoatId = Guid.Parse(
                "a90f39cc-eb6e-40ed-acc3-e8998d830979"),
            checkInDate,
            checkOutDate = checkInDate.AddDays(2),
            guestCount = 2
        };

        var createRequest = TestAuthHelper.CreateRequest(
            "POST", "/api/Bookings", customerToken);

        createRequest.Content = JsonContent.Create(bookingRequest);

        var createResponse = await _client.SendAsync(createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdBooking = await createResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var bookingId = createdBooking.GetProperty("id").GetGuid();

        // Act: Admin confirms the booking first
        var adminToken =
            TestAuthHelper.GenerateToken(_factory, "Admin");

        var confirmRequest = TestAuthHelper.CreateRequest(
            "PUT",
            $"/api/Bookings/admin/{bookingId}/confirm",
            adminToken);

        var confirmResponse = await _client.SendAsync(confirmRequest);

        Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);

        // Admin completes the booking
        var completeRequest = TestAuthHelper.CreateRequest(
            "PUT",
            $"/api/Bookings/admin/{bookingId}/complete",
            adminToken);

        var completeResponse = await _client.SendAsync(completeRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, completeResponse.StatusCode);
    }

    [Fact]
    public async Task AdminGetAllBookings_WithAdminToken_ShouldReturnSuccess()
    {
        // Arrange
        var adminToken =
            TestAuthHelper.GenerateToken(_factory, "Admin");

        var request = TestAuthHelper.CreateRequest(
            "GET",
            "/api/Bookings/admin/all",
            adminToken);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseBody = await response.Content
            .ReadAsStringAsync();

        Assert.False(string.IsNullOrWhiteSpace(responseBody));
    }

    [Fact]
    public async Task AdminGetAllBookings_WithCustomerToken_ShouldReturnForbidden()
    {
        // Arrange
        var customerToken =
            TestAuthHelper.GenerateToken(_factory, "Customer");

        var request = TestAuthHelper.CreateRequest(
            "GET",
            "/api/Bookings/admin/all",
            customerToken);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    //[Fact]
    //public async Task AdminGetBookingById_WithAdminToken_ShouldReturnSuccess()
    //{
    //    // Arrange: Customer creates a booking
    //    var customerToken =
    //        TestAuthHelper.GenerateToken(_factory, "Customer");

    //    var checkInDate = DateTime.UtcNow.Date.AddDays(135);

    //    var bookingRequest = new
    //    {
    //        houseBoatId = Guid.Parse(
    //            "a90f39cc-eb6e-40ed-acc3-e8998d830979"),
    //        checkInDate,
    //        checkOutDate = checkInDate.AddDays(2),
    //        guestCount = 2
    //    };

    //    var createRequest = TestAuthHelper.CreateRequest(
    //        "POST", "/api/Bookings", customerToken);

    //    createRequest.Content = JsonContent.Create(bookingRequest);

    //    var createResponse = await _client.SendAsync(createRequest);

    //    Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

    //    var createdBooking = await createResponse.Content
    //        .ReadFromJsonAsync<JsonElement>();

    //    var bookingId = createdBooking.GetProperty("id").GetGuid();

    //    // Act: Admin retrieves the booking
    //    var adminToken =
    //        TestAuthHelper.GenerateToken(_factory, "Admin");

    //    var getRequest = TestAuthHelper.CreateRequest(
    //        "GET",
    //        $"/api/Bookings/admin/{bookingId}",
    //        adminToken);

    //    var getResponse = await _client.SendAsync(getRequest);

    //    // Assert
    //    var responseBody = await getResponse.Content.ReadAsStringAsync();

    //    Console.WriteLine($"Admin Get Booking Status: {getResponse.StatusCode}");
    //    Console.WriteLine($"Admin Get Booking Response: {responseBody}");

    //    Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

    //    var booking = await getResponse.Content
    //        .ReadFromJsonAsync<JsonElement>();

    //    Assert.Equal(bookingId, booking.GetProperty("id").GetGuid());
    //}
}