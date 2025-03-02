using System.Text.Json;
using System.Threading.Tasks;
using Eve.Models.EveApi;
using Eve.Services.EveApi.Characters;
using Eve.Services.Interfaces.EveApi.Characters;
using Eve.Services.Interfaces.Wrappers;
using NSubstitute;

namespace Eve.Tests.Eve.Services.Tests;

public class CharacterTests
{
    private const string FAKE_ACCESS_TOKEN = "FakeAccessToken";

    [Fact]
    public async Task GetCharacterId_ReturnsInt()
    {
        var characterInitial = new Character()
        {
            CharacterId = 5,
        };

        var httpContentWrapper = Substitute.For<IHttpContentWrapper>();
        httpContentWrapper
            .ReadFromJsonAsync<Character>()
            .Returns(characterInitial);

        var httpResponseMessageWrapper = Substitute.For<IHttpResponseMessageWrapper>();
        httpResponseMessageWrapper
            .Content
            .Returns(httpContentWrapper);
            
        var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
        httpClientWrapper
            .GetAsync(Arg.Any<Uri>())
            .Returns(httpResponseMessageWrapper);

        ICharacterService characterService = new CharacterService(httpClientWrapper);
        var characterId = await characterService.GetCharacterId(FAKE_ACCESS_TOKEN);
        Assert.Equal(characterInitial.CharacterId, characterId);
    }

    [Fact]
    public async Task GetCharacterId_ReturnsNull_ThrowsException()
    {
        var characterInitial = new Character()
        {
            CharacterId = 5,
        };

        var httpContentWrapper = Substitute.For<IHttpContentWrapper>();
        httpContentWrapper
            .ReadFromJsonAsync<Character>()
            .Returns((Character?)null);

        var httpResponseMessageWrapper = Substitute.For<IHttpResponseMessageWrapper>();
        httpResponseMessageWrapper
            .Content
            .Returns(httpContentWrapper);
            
        var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
        httpClientWrapper
            .GetAsync(Arg.Any<Uri>())
            .Returns(httpResponseMessageWrapper);

        ICharacterService characterService = new CharacterService(httpClientWrapper);
        await Assert.ThrowsAsync<Exception>(async () => await characterService.GetCharacterId(FAKE_ACCESS_TOKEN));
    }
}