﻿using ship_convenient.Entities;

namespace ship_convenient.Model.FeedbackModel
{
    public class CreateFeedbackModel
    {
        public string Content { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string TypeOfFeedback { get; set; } = string.Empty;
        public Guid PackageId { get; set; }
        public Guid AccountId { get; set; }

        public Feedback ToEntity() {
            Feedback feedback = new Feedback();
            feedback.Content = Content;
            feedback.Rating = Rating;
            feedback.TypeOfFeedback = TypeOfFeedback;
            feedback.PackageId = PackageId;
            feedback.AccountId = AccountId;
            return feedback;
        }
    }
}