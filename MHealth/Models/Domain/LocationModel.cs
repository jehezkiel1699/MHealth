using System.ComponentModel.DataAnnotations;

namespace MHealth.Models.Domain
{
    public class LocationModel
    {
        [Key]
        public string Id { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
