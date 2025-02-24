namespace Eve.Models.EveApi;

public class Content
    {
        public int amount { get; set; }
        public int type_id { get; set; }
    }

    public class ExtractorDetails
    {
        public int cycle_time { get; set; }
        public double head_radius { get; set; }
        public List<Head> heads { get; set; } = new List<Head>();
        public int product_type_id { get; set; }
        public int qty_per_cycle { get; set; }
    }

    public class Head
    {
        public int head_id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class Link
    {
        public long destination_pin_id { get; set; }
        public int link_level { get; set; }
        public long source_pin_id { get; set; }
    }

    public class Pin
    {
        public List<Content> contents { get; set; } = new List<Content>();
        public DateTime last_cycle_start { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public long pin_id { get; set; }
        public int schematic_id { get; set; }
        public int type_id { get; set; }
        public DateTime? expiry_time { get; set; }
        public ExtractorDetails? extractor_details { get; set; }
        public DateTime? install_time { get; set; }
    }

    public class PlanetaryInteraction
    {
        public PlanetaryInteractionHeader? Header { get; set; }
        public List<Link> links { get; set; } = new List<Link>();
        public List<Pin> pins { get; set; } = new List<Pin>();
        public List<Route> routes { get; set; } = new List<Route>();
    }

    public class Route
    {
        public int content_type_id { get; set; }
        public long destination_pin_id { get; set; }
        public double quantity { get; set; }
        public int route_id { get; set; }
        public long source_pin_id { get; set; }
        public List<object>? waypoints { get; set; }
    }