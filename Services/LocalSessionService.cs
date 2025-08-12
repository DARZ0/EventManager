using System.Text.Json;
using Microsoft.JSInterop;

public class LocalSessionService
{
    public bool IsUserLoggedIn { get; set; } = false;
    private readonly IJSRuntime js;

    public LocalSessionService(IJSRuntime js)
    {
        this.js = js;
    }

    public async Task SetItemAsync(string key, string value)
    {
        await js.InvokeVoidAsync("sessionStorage.setItem", key, value);
    }

    public async Task<string> GetItemAsync(string key)
    {
        return await js.InvokeAsync<string>("sessionStorage.getItem", key);
    }

    public async Task RemoveItemAsync(string key)
    {
        await js.InvokeVoidAsync("sessionStorage.removeItem", key);
    }

    public async Task SetObjectAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        await SetItemAsync(key, json);
    }

    public async Task<T?> GetObjectAsync<T>(string key)
    {
        var json = await GetItemAsync(key);
        return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
    }

    public async Task<string?> GetUserName()
    {
        var userName = await GetItemAsync("userName");
        return userName;
    }
}
