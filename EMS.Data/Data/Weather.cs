namespace EMS.Data.Data
{
    public class Weather
    {
        public int CurrentTemperature { get; set; }
        public int AverageHighTemperature { get; set; }
        public int AverageLowTemperature { get; set; }
        public string Country { get; set; }
        public string Continent { get; set; }
        public string City { get; set; }
        public string WeatherDescription { get; set; }
        public int Humidity { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
}