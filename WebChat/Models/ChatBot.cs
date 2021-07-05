using WebChat.DAL.Domain;

namespace WebChat.Models
{
    public class ChatBot
    {
        public Chat Chat { get; set; }
        public string BotName { get; set; }
        public ChatBot(Chat chat, string botName)
        {
            this.Chat = chat;
            this.BotName = botName;
        }
    }
}
