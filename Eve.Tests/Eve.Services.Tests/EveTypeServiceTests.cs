using System.Text.Json;
using Eve.Models.EveApi;
using Eve.Repositories.Interfaces.Types;
using Eve.Services.EveApi.EveTypes;
using Eve.Services.Interfaces.EveApi.EveTypes;
using Eve.Services.Interfaces.Wrappers;
using NSubstitute;

namespace Eve.Tests.Eve.Services.Tests;

public class EveTypeServiceTests
{
    private const string FAKE_ACCESS_TOKEN = "FakeAccessToken";

    [Fact]
    public void GetEveType_ExistsInRepository_ReturnEveType()
    {
        int typeId = 1;

        var typeRepository = Substitute.For<ITypeRepository>();
        typeRepository
            .Get(typeId)
            .Returns(new EveType() { TypeId = typeId, Name = "", MarketGroupId = 1 });

    
        var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
        IEveTypeService eveTypeService = new EveTypeService(
            typeRepository,
            httpClientWrapper
            );
        var eveType = eveTypeService.GetEveType(typeId, FAKE_ACCESS_TOKEN);
        Assert.True(eveType is not null);
        Assert.Equal(typeId, eveType.Id);        
    }

    [Fact]
    public async Task GetEveType_DoesNotExistInRepository_ReturnEveType()
    {
        var eveTypeInitial = new EveType() { TypeId = 1, MarketGroupId = 2, Name = "TypeName" };

        var typeRepository = Substitute.For<ITypeRepository>();
        typeRepository
            .Get(eveTypeInitial.TypeId)
            .Returns((EveType?)null);

        var httpContentWrapper = Substitute.For<IHttpContentWrapper>();
        httpContentWrapper
            .ReadAsStringAsync()
            .Returns(JsonSerializer.Serialize(eveTypeInitial));

        var httpResponseMessageWrapper = Substitute.For<IHttpResponseMessageWrapper>();
        httpResponseMessageWrapper
            .Content
            .Returns(httpContentWrapper);
            
        var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
        httpClientWrapper
            .GetAsync(Arg.Any<Uri>())
            .Returns(httpResponseMessageWrapper);

        IEveTypeService eveTypeService = new EveTypeService(
            typeRepository,
            httpClientWrapper);

        var eveType = await eveTypeService.GetEveType(eveTypeInitial.TypeId, FAKE_ACCESS_TOKEN);
        Assert.True(eveType is not null);
        Assert.Equal(eveTypeInitial.TypeId, eveType.TypeId);
        Assert.Equal(eveTypeInitial.Name, eveType.Name);
        Assert.Equal(eveTypeInitial.MarketGroupId, eveType.MarketGroupId);
    }

    [Fact]
    public async Task GetEveType_DoesNotExistInEveApi_ReturnNull()
    {
        var eveTypeInitial = new EveType() { TypeId = 1, MarketGroupId = 2, Name = "TypeName" };

        var typeRepository = Substitute.For<ITypeRepository>();
        typeRepository
            .Get(eveTypeInitial.TypeId)
            .Returns((EveType?)null);

        var httpContentWrapper = Substitute.For<IHttpContentWrapper>();
        httpContentWrapper
            .ReadAsStringAsync()
            .Returns("null");

        var httpResponseMessageWrapper = Substitute.For<IHttpResponseMessageWrapper>();
        httpResponseMessageWrapper
            .Content
            .Returns(httpContentWrapper);
            
        var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
        httpClientWrapper
            .GetAsync(Arg.Any<Uri>())
            .Returns(httpResponseMessageWrapper);

        IEveTypeService eveTypeService = new EveTypeService(
            typeRepository,
            httpClientWrapper);

        await Assert.ThrowsAsync<Exception>(async () => await eveTypeService.GetEveType(eveTypeInitial.TypeId, FAKE_ACCESS_TOKEN));
    }
}