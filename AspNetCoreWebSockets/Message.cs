using System;

namespace AspNetCoreWebSockets
{
    public class Message
    {
        public DateTime Date { get; set; }
        public string Content { get; set; }
    }

    public class MessageViewModel
    {
        public string Content { get; set; }

    }
}
