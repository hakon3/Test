using Microsoft.EntityFrameworkCore;

namespace Second.Data
{
    public class SecondDbContext : DbContext
    {
        public SecondDbContext(DbContextOptions<SecondDbContext> options) : base(options)
        {
        }

        public virtual DbSet<CityWeather> CityWeather { get; set; }
        public virtual DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Country = "Norway", Name = "Oslo", Latitude = "59.91", Longitude = "10.75" },
                new City { Id = 2, Country = "Sweden", Name = "Stockholm", Latitude = "59.33", Longitude = "18.07" },
                new City { Id = 3, Country = "Denmark", Name = "Copenhagen", Latitude = "55.68", Longitude = "12.57" },
                new City { Id = 4, Country = "Finland", Name = "Helsinki", Latitude = "60.17", Longitude = "24.94" },
                new City { Id = 5, Country = "Spain", Name = "Madrid", Latitude = "40.42", Longitude = "-3.70" },
                new City { Id = 6, Country = "USA", Name = "New York", Latitude = "40.71", Longitude = "-74.01" },
                new City { Id = 7, Country = "South Africa", Name = "New York", Latitude = "-33.93", Longitude = "18.42" },
                new City { Id = 8, Country = "Japan", Name = "Tokyo", Latitude = "35.69", Longitude = "139.69" },
                new City { Id = 9, Country = "Afghanistan", Name = "Kabul", Latitude = "34.53", Longitude = "69.17" },
                new City { Id = 10, Country = "Brazil", Name = "Rio de Janeiro", Latitude = "-22.91", Longitude = "-43.18" }
            );
        }
    }

    public class CityWeather
    {
        public int Id { get; set; }
        public DateTimeOffset Time { get; set; }
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public Cloudiness Cloudiness { get; set; }
        public virtual City CityNavigation { get; set; }
    }

    public enum Cloudiness
    {
        Undefined,
        Clear,
        Cloudy,
    }

    public class City
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
