using Eve.Models.EveApi;
using Eve.Services.Interfaces.EveApi.Characters;
using Eve.Services.Interfaces.Wrappers;

namespace Eve.Services.EveApi.Characters;

public class CharacterService : ICharacterService
{
    private readonly IHttpClientWrapper _httpClientWrapper;
    public CharacterService(IHttpClientWrapper httpClientWrapper)
    {
        _httpClientWrapper = httpClientWrapper;
    }
    public async Task<int> GetCharacterId(string accessToken)
    {
        _httpClientWrapper.AddDefaultRequestHeaders("Authorization", $"Bearer {accessToken}");
        var response = await _httpClientWrapper.GetAsync(new Uri($"https://login.eveonline.com/oauth/verify"));
        response.EnsureSuccessStatusCode();
        var character = await response.Content.ReadFromJsonAsync<Character>();
        if (character == null) throw new Exception("character response from Eve Online API is null");
        return character.CharacterId;
    }
}