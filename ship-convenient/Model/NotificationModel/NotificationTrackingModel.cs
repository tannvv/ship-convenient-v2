namespace ship_convenient.Model.NotificationModel
{
    public class NotificationTrackingModel
    {
        public Guid DeliverId { get; set; }
        public string Title { get; set; } = string.Empty;
        public IReadOnlyDictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
    }
}
