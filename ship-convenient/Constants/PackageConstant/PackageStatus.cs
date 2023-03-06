namespace unitofwork_core.Constant.Package
{
    public static class PackageStatus
    {
        public const string WAITING = "WAITING";
        public const string APPROVED = "APPROVED";
        public const string REJECT = "REJECT";
        public const string DELIVER_PICKUP = "DELIVER_PICKUP";
        public const string DELIVER_CANCEL = "DELIVER_CANCEL";
        public const string SENDER_CANCEL = "SENDER_CANCEL";
        public const string DELIVERY = "DELIVERY";
        public const string DELIVERED = "DELIVERED";
        public const string DELIVERY_FAILED = "DELIVERY_FAILED";
       /* public const string SENDER_CONFIRM_DELIVERED = "SENDER_CONFIRM_DELIVERED";
        public const string SENDER_CONFIRM_DELIVERED_FAILED = "SENDER_CONFIRM_DELIVERED_FAILED";*/
        public const string REFUND_SUCCESS = "REFUND_SUCCESS";
        public const string REFUND_FAILED = "REFUND_FAILED";
        public const string NOT_EXIST = "NOT_EXIST";
    }
}
