using BraceletPrinter.Domain;
using System.Net.Http.Json;

namespace BraceletPrinter.Services;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(ApiSettings settings)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(settings.BaseUrl)
        };
    }

    public async Task<bool> CheckIfUsersExistAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/users/any");
            return await response.Content.ReadFromJsonAsync<bool>();
        }
        catch
        {
            return false;
        }
    }

        public async Task CreateAdminUserAsync(AdminUser adminUser)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/register", adminUser);
            response.EnsureSuccessStatusCode();
        }
}
