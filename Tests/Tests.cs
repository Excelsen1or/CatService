using System.Net;
using Xunit;

namespace CatService.Tests;

public class Tests {
    private readonly HttpClient _httpClient = new() {
        BaseAddress = new Uri("https://localhost:5053")
    };

    [Fact]
    public async Task Get_ReturnsBadRequest_OnEmptyRequestParameter() {
        var response = await _httpClient.GetAsync("Cat");
        Assert.Equal(response.StatusCode, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_OnInvalidUrl() {
        var response = await _httpClient.GetAsync("Cat?fromUrl=helloworld");
        Assert.Equal(response.StatusCode, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_OnNonExistentDomain() {
        var response = await _httpClient.GetAsync("Cat?fromUrl=http://verycoolwebsite.com/");
        Assert.Equal(response.StatusCode, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_ReturnsCatPicture_OnValidUrl() {
        var response = await _httpClient.GetAsync("Cat?fromUrl=https://httpstat.us/Random/200,201,500-504");
        var expectedStatusCodes = new List<HttpStatusCode> {
            HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.InternalServerError,
            HttpStatusCode.NotImplemented, HttpStatusCode.BadGateway, HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout
        };

        Assert.Contains(response.StatusCode, expectedStatusCodes);
    }
}