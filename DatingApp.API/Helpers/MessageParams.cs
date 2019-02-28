using System;

namespace DatingApp.API.Helpers
{
    public class MessageParams
    {
        private const int MaxPageSize = 50;
        private int pageSize = 5;
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => pageSize;
            set => pageSize = Math.Min(MaxPageSize, value);
        }

        public int UserId { get; set; }
        public string MessageContainer { get; set; } = "Unread";
    }
}