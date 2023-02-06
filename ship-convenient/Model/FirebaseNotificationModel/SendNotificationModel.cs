namespace ship_convenient.Model.FirebaseNotificationModel
{
    public class SendNotificationModel
    {
        public Guid AccountId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
