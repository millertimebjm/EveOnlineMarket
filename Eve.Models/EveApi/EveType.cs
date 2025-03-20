using System.Text.Json.Serialization;

namespace Eve.Models.EveApi;

public class EveType
{
    // capacity	number($float)
    // title: get_universe_types_type_id_capacity
    // capacity number

    // description*	string
    // title: get_universe_types_type_id_description
    // description string
    // dogma_attributes	get_universe_types_type_id_dogma_attributes[
    // maxItems: 1000
    // title: get_universe_types_type_id_dogma_attributes

    // dogma_attributes array
    // get_universe_types_type_id_dogma_attribute{
    // attribute_id*	integer($int32)
    // title: get_universe_types_type_id_attribute_id

    // attribute_id integer
    // value*	number($float)
    // title: get_universe_types_type_id_value
    // value number
    // }]
    // dogma_effects	get_universe_types_type_id_dogma_effects[
    // maxItems: 1000
    // title: get_universe_types_type_id_dogma_effects

    // dogma_effects array
    // get_universe_types_type_id_dogma_effect{

    // effect_id*	integer($int32)
    // title: get_universe_types_type_id_effect_id

    // effect_id integer
    // is_default*	boolean
    // title: get_universe_types_type_id_is_default

    // is_default boolean
    // }]
    // graphic_id	integer($int32)
    // title: get_universe_types_type_id_graphic_id

    // graphic_id integer

    // group_id*	integer($int32)
    // title: get_universe_types_type_id_group_id

    // group_id integer
    // icon_id	integer($int32)
    // title: get_universe_types_type_id_icon_id

    // icon_id integer

    [JsonPropertyName("market_group_id")]
    public int MarketGroupId { get; set; }
    // market_group_id	integer($int32)
    // title: get_universe_types_type_id_market_group_id
    // This only exists for types that can be put on the market

    // mass	number($float)
    // title: get_universe_types_type_id_mass
    // mass number

    public string Name { get; set; } = "";
    // name*	string
    // title: get_universe_types_type_id_name
    // name string
    // packaged_volume	number($float)
    // title: get_universe_types_type_id_packaged_volume

    // packaged_volume number
    // portion_size	integer($int32)
    // title: get_universe_types_type_id_portion_size

    // portion_size integer

    // published*	boolean
    // title: get_universe_types_type_id_published
    // published boolean
    // radius	number($float)
    // title: get_universe_types_type_id_radius
    // radius number

    [JsonPropertyName("type_id")]
    public int TypeId { get; set; }
    // type_id*	integer($int32)
    // title: get_universe_types_type_id_type_id

    // type_id integer
    // volume	number($float)
    // title: get_universe_types_type_id_volume
    // volume number
    public decimal Volume { get; set; }

    public int? SchematicId { get; set; } = null;

    public override string ToString()
    {
        return $"{TypeId} {Name}";
    }
}