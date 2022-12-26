namespace ship_convenient.Entities
{
    public class Package : BaseEntity
    {
        public string StartAddress { get; set; } = string.Empty;
        public double StartLongitude { get; set; }
        public double StartLatitude { get; set; }
        public string DestinationAddress { get; set; } = string.Empty;
        public double DestinationLongitude { get; set; }
        public double DestinationLatitude { get; set; }
        public double Distance { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public double Volume { get; set; }
        public double Weight { get; set; }
        public string Status { get; set; } = string.Empty;
        public int PriceShip { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; } 
        #region Relationship
        public Guid CreatorId { get; set; }
        public Account? Creator { get; set; }
        public Guid? DeliverId { get; set; }
        public Account? Deliver { get; set; }
        public Guid? DiscountId { get; set; }
        public Discount? Discount { get; set; }

        public List<Transaction> Transactions { get; set; }
        public List<TransactionPackage> TransactionPackages { get; set; }
        public List<Product> Products { get; set; }
        #endregion

        public Package()
        {
            Transactions = new List<Transaction>();
            TransactionPackages = new List<TransactionPackage>();
            Products = new List<Product>();
        }
    }
}
