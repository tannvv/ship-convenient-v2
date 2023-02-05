using ship_convenient.Model.FeedbackModel;

namespace ship_convenient.Entities
{
    public class Feedback : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public double Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string TypeOfFeedback { get; set; } = string.Empty;
        public Guid PackageId { get; set; }
        public Package? Package { get; set; }
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }


        public ResponseFeedbackModel ToResponseModel()
        {
            ResponseFeedbackModel model = new();
            model.Id = this.Id;
            model.Content = this.Content;
            model.Rating = this.Rating;
            model.TypeOfFeedback = this.TypeOfFeedback;
            model.PackageId = this.PackageId;
            model.AccountId = this.AccountId;
            model.CreatedAt = this.CreatedAt;
            model.ModifiedAt = this.ModifiedAt;
            if (Account != null)
            {
                model.Account = this.Account.ToResponseModel();
            }
            return model;
        }
    }
}
