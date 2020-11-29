using System;
using System.Collections.Generic;
using System.Text;

namespace MessageToTelegramAPI.Domain.Abstracts
{
    public abstract class BaseMessage
    {
        public long ChatId { get; set; }
        public string Message { get; set; }
        public int MessageId { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Sent_at { get; set; }

    }
}
