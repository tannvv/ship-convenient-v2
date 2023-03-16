using ship_convenient.Entities;

namespace ship_convenient.Model.RouteModel
{
    public class CreateRoutePointModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Index { get; set; }
        public string DirectionType { get; set; } = string.Empty;

        public RoutePoint ToEntity() {
            RoutePoint entity = new();
            entity.Latitude = this.Latitude;
            entity.Longitude = this.Longitude;
            entity.Index = this.Index;
            entity.DirectionType = this.DirectionType;
            return entity;
        }
    }
}
