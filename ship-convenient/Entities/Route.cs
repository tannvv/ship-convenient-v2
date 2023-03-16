using ship_convenient.Model.RouteModel;

namespace ship_convenient.Entities
{
    public class Route : BaseEntity
    {
        public bool IsActive { get; set; }
        public string FromName { get; set; } = string.Empty;
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }
        public string ToName { get; set; } = string.Empty;
        public double Distance { get; set; }
        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }

        #region Relationship
        public Guid InfoUserId { get; set; }
        public InfoUser? InfoUser { get; set; }
        public List<RoutePoint> RoutePoints { get; set; }
        #endregion

        public Route()
        {
            RoutePoints = new List<RoutePoint>();
        }

        public ResponseRouteModel ToResponseModel()
        {
            ResponseRouteModel model = new ResponseRouteModel();
            model.Id = this.Id;
            model.FromName = this.FromName;
            model.FromLongitude = this.FromLongitude;
            model.FromLatitude = this.FromLatitude;
            model.ToName = this.ToName;
            model.ToLongitude = this.ToLongitude;
            model.ToLatitude = this.ToLatitude;
            model.IsActive = this.IsActive;
            model.InfoUserId = this.InfoUserId;
            return model;
        }
    }
}
