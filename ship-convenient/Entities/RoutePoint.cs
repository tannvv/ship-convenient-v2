using ship_convenient.Model.RouteModel;

namespace ship_convenient.Entities
{
    public class RoutePoint : BaseEntity
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Index { get; set; }
        public string DirectionType { get; set; } = string.Empty;
        public Guid RouteId { get; set; }
        public Route? Route { get; set; }

        public ResponseRoutePointModel ToResponseModel()
        {
            ResponseRoutePointModel model = new();
            model.Latitude = this.Latitude;
            model.Longitude = this.Longitude;
            model.Index = this.Index;
            model.DirectionType = this.DirectionType;
            model.RouteId = this.RouteId;
            return model;
        }

    }
}
