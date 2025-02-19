    namespace Eve.Mvc.Models;
    public class PlanetaryInteractionHeaderModel
    {
        public DateTime last_update { get; set; }
        public int num_pins { get; set; }
        public int owner_id { get; set; }
        public int planet_id { get; set; }
        public string planet_type { get; set; } = "";
        public int solar_system_id { get; set; }
        public int upgrade_level { get; set; }
    }