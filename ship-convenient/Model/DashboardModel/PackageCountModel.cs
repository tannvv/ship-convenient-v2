namespace ship_convenient.Model.DashboardModel
{
    public class PackageCountModel
    {
        public int All { get; set; }
        public int Waiting { get; set; }
        public int Approved { get; set; }
        public int Reject { get; set; }
        public int DeliverPickup { get; set; }
        public int DeliverCancel { get; set; }
        public int SenderCancel { get; set; }
        public int Delivery { get; set; }
        public int Delivered { get; set; }
        public int DeliveryFailed { get; set; }
        public int RefundSuccess { get; set; }
        public int RefundFailed { get; set; }
        public int NotExist { get; set; }
    }
}
