using ship_convenient.Entities;
using ship_convenient.Model.UserModel;

namespace ship_convenient.Model.FeedbackModel
{
    public class ResponseFeedbackModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string FeedbackFor { get; set; } = string.Empty;
        public Guid PackageId { get; set; }
        public Guid AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public ResponseAccountModel? Account { get; set; }
    }
}
