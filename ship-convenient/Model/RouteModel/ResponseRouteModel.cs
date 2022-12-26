namespace ship_convenient.Model.RouteModel
{
    public class ResponseRouteModel
    {
        public string FromName { get; set; } = string.Empty;
        public double FromLongitude { get; set; }
        public double FromLatitude { get; set; }
        public string ToName { get; set; } = string.Empty;
        public double ToLongitude { get; set; }
        public double ToLatitude { get; set; }
        public bool IsActive { get; set; }
        public Guid InfoUserId { get; set; }
    }
}
