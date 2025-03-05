using System.Text.Json;
using Eve.Models.EveApi;
using Eve.Repositories.Interfaces.Planets;
using Eve.Services.EveApi.Planets;
using Eve.Services.Interfaces.EveApi.Planets;
using Eve.Services.Interfaces.Wrappers;
using NSubstitute;

namespace Eve.Tests.Eve.Services.Tests;

public class PlanetTests
{
    private const string FAKE_ACCESS_TOKEN = "FakeAccessToken";

    // Task<List<PlanetaryInteraction>> GetPlanetaryInteractions(long userId, string accessToken);
    // Task<Planet> GetPlanet(int planetId, string accessToken);
    // Task<List<Planet>> GetPlanets(List<int> planetIds, string accessToken);
    [Fact]
    public async Task GetPlanet_ExistsInRepository_ReturnsPlanet()
    {
        var planetInitial = new Planet()
        {
            PlanetId = 5,
            Name = "Haajinen VI",
        };

        var planetRepository = Substitute.For<IPlanetRepository>();
        planetRepository
            .Get(planetInitial.PlanetId)
            .Returns(planetInitial);

        var httpClientWrapper = Substitute.For<IHttpClientWrapper>();

        IPlanetService planetService = new PlanetService(httpClientWrapper, planetRepository);
        var planet = await planetService.GetPlanet(planetInitial.PlanetId, FAKE_ACCESS_TOKEN);
        Assert.Equal(planetInitial.PlanetId, planet.PlanetId);
        Assert.Equal(planetInitial.Name, planet.Name);
    }

    [Fact]
    public async Task GetPlanet_NotInRepository_ReturnsPlanet()
    {
        var planetInitial = new Planet()
        {
            PlanetId = 5,
            Name = "Haajinen VI",
        };

        var planetRepository = Substitute.For<IPlanetRepository>();
        planetRepository
            .Get(planetInitial.PlanetId)
            .Returns((Planet?)null);

        var httpContentWrapper = Substitute.For<IHttpContentWrapper>();
        httpContentWrapper
            .ReadAsStringAsync()
            .Returns(JsonSerializer.Serialize(planetInitial));

        var httpResponseMessageWrapper = Substitute.For<IHttpResponseMessageWrapper>();
        httpResponseMessageWrapper
            .Content
            .Returns(httpContentWrapper);
            
        var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
        httpClientWrapper
            .GetAsync(Arg.Any<Uri>())
            .Returns(httpResponseMessageWrapper);

        IPlanetService planetService = new PlanetService(httpClientWrapper, planetRepository);
        var planet = await planetService.GetPlanet(planetInitial.PlanetId, FAKE_ACCESS_TOKEN);
        Assert.Equal(planetInitial.PlanetId, planet.PlanetId);
        Assert.Equal(planetInitial.Name, planet.Name);
    }

    [Fact]
    public async Task GetPlanet_ReturnsNull_ThrowsException()
    {
        var planetInitial = new Planet()
        {
            PlanetId = 5,
            Name = "Haajinen VI",
        };

        var planetRepository = Substitute.For<IPlanetRepository>();
        planetRepository
            .Get(planetInitial.PlanetId)
            .Returns((Planet?)null);

        var httpContentWrapper = Substitute.For<IHttpContentWrapper>();
        httpContentWrapper
            .ReadAsStringAsync()
            .Returns(JsonSerializer.Serialize("null"));

        var httpResponseMessageWrapper = Substitute.For<IHttpResponseMessageWrapper>();
        httpResponseMessageWrapper
            .Content
            .Returns(httpContentWrapper);
            
        var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
        httpClientWrapper
            .GetAsync(Arg.Any<Uri>())
            .Returns(httpResponseMessageWrapper);

        IPlanetService planetService = new PlanetService(httpClientWrapper, planetRepository);
        await Assert.ThrowsAsync<JsonException>(async () => await planetService.GetPlanet(planetInitial.PlanetId, FAKE_ACCESS_TOKEN));
    }

    [Fact]
    public async Task GetPlanetaryInteractions_GetItems()
    {
        long userId = 100;
        var planetaryInteractionInitial = new PlanetaryInteraction();

        var planetRepository = Substitute.For<IPlanetRepository>();

        var httpContentWrapper = Substitute.For<IHttpContentWrapper>();
        httpContentWrapper
            .ReadAsStringAsync()
            .Returns(JsonSerializer.Serialize(planetaryInteractionInitial));

        var httpResponseMessageWrapper = Substitute.For<IHttpResponseMessageWrapper>();
        httpResponseMessageWrapper
            .Content
            .Returns(httpContentWrapper);
            
        var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
        httpClientWrapper
            .GetAsync(Arg.Any<Uri>())
            .Returns(httpResponseMessageWrapper);

        IPlanetService planetService = new PlanetService(httpClientWrapper, planetRepository);
        await planetService.GetPlanetaryInteractions(userId, FAKE_ACCESS_TOKEN);
    }
}