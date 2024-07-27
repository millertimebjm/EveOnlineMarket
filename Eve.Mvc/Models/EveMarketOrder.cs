
using System.Text.Json.Serialization;

namespace Eve.Mvc.Models;

public class EveMarketOrder
{
    public int Duration {get; set;}
// duration*	integer($int32)
// title: get_characters_character_id_orders_duration
// Number of days for which order is valid (starting from the issued date). An order expires at time issued + duration

    public decimal Escrow {get; set;}
// escrow	number($double)
// title: get_characters_character_id_orders_escrow
// For buy orders, the amount of ISK in escrow

// is_buy_order	boolean
// title: get_characters_character_id_orders_is_buy_order
// True if the order is a bid (buy) order

// is_corporation*	boolean
// title: get_characters_character_id_orders_is_corporation
// Signifies whether the buy/sell order was placed on behalf of a corporation.

    public DateTime Issued {get; set;}
// issued*	string($date-time)
// title: get_characters_character_id_orders_issued
// Date and time when this order was issued

    [JsonPropertyName("location_id")]
    public long LocationId {get; set;}
// location_id*	integer($int64)
// title: get_characters_character_id_orders_location_id
// ID of the location where order was placed

// min_volume	integer($int32)
// title: get_characters_character_id_orders_min_volume
// For buy orders, the minimum quantity that will be accepted in a matching sell order

    [JsonPropertyName("order_id")]
    public long OrderId {get; set;}
// order_id*	integer($int64)
// title: get_characters_character_id_orders_order_id
// Unique order ID

    public decimal Price {get; set;}
// price*	number($double)
// title: get_characters_character_id_orders_price
// Cost per unit for this order

// range*	string
// title: get_characters_character_id_orders_range
// Valid order range, numbers are ranges in jumps
// Enum:
// Array [ 12 ]

// region_id*	integer($int32)
// title: get_characters_character_id_orders_region_id
// ID of the region where order was placed

    [JsonPropertyName("type_id")]
    public int TypeId {get; set;}
// type_id*	integer($int32)
// title: get_characters_character_id_orders_type_id
// The type ID of the item transacted in this order

    [JsonPropertyName("volume_remain")]
    public int VolumeRemain {get; set;}
// volume_remain*	integer($int32)
// title: get_characters_character_id_orders_volume_remain
// Quantity of items still required or offered

    [JsonPropertyName("volume_total")]
    public int VolumeTotal {get; set;}
// volume_total*	integer($int32)
// title: get_characters_character_id_orders_volume_total
// Quantity of items required or offered at time order was placed

    public TimeSpan TimeRemaining
    {
        get
        {
            return Issued.AddDays(Duration) - DateTime.UtcNow;
        }
    }
}