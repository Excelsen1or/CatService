using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CatService.Controllers;

[ApiController]
[Route("[controller]")]
public class Controller : ControllerBase {
    private readonly HttpClient _httpClient = new();
    private readonly IMemoryCache _memoryCache;

    public Controller(IMemoryCache memoryCache) {
        _memoryCache = memoryCache;
    }

    [HttpGet]
    public async Task<ActionResult> Get(string fromUrl) {
        try {
            HttpResponseMessage response = await _httpClient.GetAsync(fromUrl);
            byte[]? catPicture = await GetCatPicture(response.StatusCode);
            if (catPicture is null) return Problem();
            return File(catPicture, "image/jpeg");
        } catch (Exception) {
            return BadRequest("Значение FromUrl не является допустимым. Url должен быть абсолютным URI существующего домена.");
        }
    }

    private async Task<byte[]?> GetCatPicture(HttpStatusCode statusCode) {
        string url = $"https://http.cat/{(int)statusCode}";
        return await _memoryCache.GetOrCreateAsync(statusCode, cacheEntry => {
            cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(3);
            return _httpClient.GetByteArrayAsync(url);
        });
    }
}