namespace ship_convenient.Model.TransactionPackageModel
{
    public class ResponseTransactionPackageModel
    {
        public string FromStatus { get; set; } = string.Empty;
        public string ToStatus { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid PackageId { get; set; }
    }
}
