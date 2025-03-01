using System.Text.Json;
using Eve.Models.EveApi;
using Eve.Services.Interfaces.Orders;
using Eve.Services.Interfaces.Wrappers;
using Eve.Services.Orders;
using NSubstitute;

namespace Eve.Tests.Eve.Services.Tests;

public class OrdersServiceTests
{

    private const string FAKE_ACCESS_TOKEN = "FakeAccessToken";

    [Fact]
    public async Task GetMarketOrders_GetSingleOrder_IsEqual()
    {
        var orderInitial = new Order()
        {
            Duration = 0,
            Escrow = 0,
            IsBuyOrder = false,
            Issued = new DateTime(2025, 1, 1),
            LocationId = 1,
            OrderId = 2,
            Price = 3,
            TypeId = 4,
            VolumeRemain = 100,
            VolumeTotal = 101,
        };

        var httpContentWrapper = Substitute.For<IHttpContentWrapper>();
        httpContentWrapper
            .ReadAsStringAsync()
            .Returns(JsonSerializer.Serialize(new List<Order>{orderInitial}));

        var httpResponseMessageWrapper = Substitute.For<IHttpResponseMessageWrapper>();
        httpResponseMessageWrapper
            .Content
            .Returns(httpContentWrapper);
            
        var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
        httpClientWrapper
            .GetAsync(Arg.Any<Uri>())
            .Returns(httpResponseMessageWrapper);

        IOrdersService ordersService = new OrdersService(httpClientWrapper);
        var marketOrders = await ordersService.GetMarketOrders(0, FAKE_ACCESS_TOKEN);
        Assert.True(marketOrders.Count == 1);
        var marketOrder = marketOrders.Single();
        Assert.Equal(marketOrder.Price, orderInitial.Price);
        Assert.Equal(marketOrder.Issued, orderInitial.Issued);
        Assert.Equal(marketOrder.LocationId, orderInitial.LocationId);
        Assert.Equal(marketOrder.TimeRemaining, orderInitial.TimeRemaining);
        Assert.Equal(marketOrder.VolumeTotal, orderInitial.VolumeTotal);
        Assert.Equal(marketOrder.VolumeRemain, orderInitial.VolumeRemain);
        Assert.Equal(marketOrder.Duration, orderInitial.Duration);
        Assert.Equal(marketOrder.Escrow, orderInitial.Escrow);
        Assert.Equal(marketOrder.IsBuyOrder, orderInitial.IsBuyOrder);
        Assert.Equal(marketOrder.OrderId, orderInitial.OrderId);
        Assert.Equal(marketOrder.TypeId, orderInitial.TypeId);
    }

    [Fact]
    public async Task GetMarketOrders_GetSingleOrder_ReturnsException()
    {
        var orderInitial = new Order()
        {
            Duration = 0,
            Escrow = 0,
            IsBuyOrder = false,
            Issued = new DateTime(2025, 1, 1),
            LocationId = 1,
            OrderId = 2,
            Price = 3,
            TypeId = 4,
            VolumeRemain = 100,
            VolumeTotal = 101,
        };

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

        IOrdersService ordersService = new OrdersService(httpClientWrapper);
        await Assert.ThrowsAsync<JsonException>(async () => await ordersService.GetMarketOrders(0, FAKE_ACCESS_TOKEN));
    }

    [Fact]
    public async Task GetMarketOrderIds_ReturnsIds()
    {
        var orderInitial = new Order()
        {
            Duration = 0,
            Escrow = 0,
            IsBuyOrder = false,
            Issued = new DateTime(2025, 1, 1),
            LocationId = 1,
            OrderId = 2,
            Price = 3,
            TypeId = 4,
            VolumeRemain = 100,
            VolumeTotal = 101,
        };

        var httpContentWrapper = Substitute.For<IHttpContentWrapper>();
        httpContentWrapper
            .ReadAsStringAsync()
            .Returns(JsonSerializer.Serialize(new List<Order>{orderInitial}));

        var httpResponseMessageWrapper = Substitute.For<IHttpResponseMessageWrapper>();
        httpResponseMessageWrapper
            .Content
            .Returns(httpContentWrapper);
            
        var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
        httpClientWrapper
            .GetAsync(Arg.Any<Uri>())
            .Returns(httpResponseMessageWrapper);

        IOrdersService ordersService = new OrdersService(httpClientWrapper);
        var marketOrderIds = await ordersService.GetMarketOrderIds(0, FAKE_ACCESS_TOKEN);
        Assert.True(marketOrderIds.Count == 1);
        var marketOrder = marketOrderIds.Single();
        Assert.Equal(marketOrder, orderInitial.OrderId);
    }

    [Fact]
    public async Task GetBuySellOrders_GetSingleOrder_IsEqual()
    {
        var orderInitial = new Order()
        {
            Duration = 0,
            Escrow = 0,
            IsBuyOrder = false,
            Issued = new DateTime(2025, 1, 1),
            LocationId = 1,
            OrderId = 2,
            Price = 3,
            TypeId = 4,
            VolumeRemain = 100,
            VolumeTotal = 101,
        };

        var httpContentWrapper = Substitute.For<IHttpContentWrapper>();
        httpContentWrapper
            .ReadAsStringAsync()
            .Returns(JsonSerializer.Serialize(new List<Order>{orderInitial}));

        var httpResponseMessageWrapper = Substitute.For<IHttpResponseMessageWrapper>();
        httpResponseMessageWrapper
            .Content
            .Returns(httpContentWrapper);
            
        var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
        httpClientWrapper
            .GetAsync(Arg.Any<Uri>())
            .Returns(httpResponseMessageWrapper);

        IOrdersService ordersService = new OrdersService(httpClientWrapper);
        var marketOrders = await ordersService.GetBuySellOrders(orderInitial.TypeId, FAKE_ACCESS_TOKEN);
        Assert.True(marketOrders.Count == 1);
        var marketOrder = marketOrders.Single();
        Assert.Equal(marketOrder.Price, orderInitial.Price);
        Assert.Equal(marketOrder.Issued, orderInitial.Issued);
        Assert.Equal(marketOrder.LocationId, orderInitial.LocationId);
        Assert.Equal(marketOrder.TimeRemaining, orderInitial.TimeRemaining);
        Assert.Equal(marketOrder.VolumeTotal, orderInitial.VolumeTotal);
        Assert.Equal(marketOrder.VolumeRemain, orderInitial.VolumeRemain);
        Assert.Equal(marketOrder.Duration, orderInitial.Duration);
        Assert.Equal(marketOrder.Escrow, orderInitial.Escrow);
        Assert.Equal(marketOrder.IsBuyOrder, orderInitial.IsBuyOrder);
        Assert.Equal(marketOrder.OrderId, orderInitial.OrderId);
        Assert.Equal(marketOrder.TypeId, orderInitial.TypeId);
    }
}