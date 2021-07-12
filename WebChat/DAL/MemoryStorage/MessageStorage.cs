using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WebChat.DAL.Domain;
using WebChat.Models;

namespace WebChat.DAL.MemoryStorage
{
    public class MessageStorage : IMessageStorage
    {
        private OperatorContext context;

        public MessageStorage()
        {
            context = new OperatorContext();
        }

        public Messаge GetMessаgeById(int messageId)
        {
            var mesWhithCHat = context.Messages.Include("Chat").ToList();
            var message = mesWhithCHat.FirstOrDefault(m => m.Id == messageId);

            return message;
        }

        public void MarkMessageAsRead(int messageId)
        {
            var message = context.Messages.FirstOrDefault(m => m.Id == messageId);

            if (message is not null)
            {
                message.CheckRead = true;

                context.SaveChanges();
            }
        }

        public void AddMessage(Messаge messаge)
        {
            var MessageChat = context.Chats.FirstOrDefault(c => c.Id == messаge.Chat.Id);

            messаge.Chat = MessageChat;
            messаge.Id = null;
            messаge.CheckRead = false;

            context.Messages.Add(messаge);

            context.SaveChanges();
        }

        public void DeleteMessage(Messаge messаge)
        {
            context.Messages.Remove(messаge);

            context.SaveChanges();
        }

        public List<Messаge> GetMessagesByChat(int chatId)
        {
            var chat = context.Chats.FirstOrDefault(c => c.Id == chatId);

            return context.Messages.Where(m => m.Chat == chat).OrderBy(mes => mes.Id).ToList();
        }

        public IReadOnlyCollection<Messаge> GetAll()
        {
            return context.Messages.ToList();
        }
    }
}
