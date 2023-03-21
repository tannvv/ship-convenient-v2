namespace ship_convenient.Model.ReportModel
{
    public class CreateReportPackageModel
    {
        public string Reason { get; set; } = string.Empty;
        public Guid AccountId { get; set; }
        public Guid PackageId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
