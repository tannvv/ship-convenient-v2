﻿using ship_convenient.Model.NotificationModel;

namespace ship_convenient.Entities
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsSend { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        #region Relationship
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }
        #endregion

        ResponseNotificationModel ToResponseModel()
        {
            ResponseNotificationModel model = new();
            model.Id = this.Id;
            model.Title = this.Title;
            model.Content = this.Content;
            model.IsSend = this.IsSend;
            model.CreatedAt = this.CreatedAt;
            return model;
        }
    }
}
