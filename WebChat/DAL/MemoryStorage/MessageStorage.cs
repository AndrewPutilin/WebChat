using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WebChat.DAL.Domain;
using WebChat.Models;

namespace WebChat.DAL.MemoryStorage
{
    public class MessageStorage : IMessageStorage
    {
        private List<Messаge> messаges;
        private OperatorContext context;

        public MessageStorage()
        {
            context = new OperatorContext();
            this.messаges = context.Messages.OrderBy(c => c.MesegeTime).ToList();
        }

        public Messаge GetMessаgeById(int messageId)
        {
            var mesWhithCHat = context.Messages.Include("Chat").ToList();
            var message = mesWhithCHat.FirstOrDefault(m => m.Id == messageId);

            return message;
        }

        public void ReadMessage(int messageId)
        {
            var readMessage = context.Messages.FirstOrDefault(m=>m.Id == messageId);

            readMessage.CheckRead = true;

            context.SaveChanges();

            messаges = context.Messages.OrderBy(m => m.MesegeTime).ToList();
        }

        public void AddMessage(Messаge messаge)
        {
            var MessageChat = context.Chats.FirstOrDefault(c => c.Id == messаge.Chat.Id);

            messаge.Chat = MessageChat;
            messаge.Id = null;
            messаge.CheckRead = false;

            context.Messages.Add(messаge);

            context.SaveChanges();

            messаges = context.Messages.OrderBy(m => m.MesegeTime).ToList();
        }

        public void DeleteMessage(Messаge messаge)
        {
            context.Messages.Remove(messаge);

            context.SaveChanges();

            messаges = context.Messages.OrderBy(m => m.MesegeTime).ToList();
        }

        public List<Messаge> GetMesegesByChat(int chatId)
        {
                var chat = context.Chats.FirstOrDefault(c => c.Id == chatId);

                return context.Messages.Where(m => m.Chat == chat).OrderBy(mes => mes.Id).ToList();
        }

        public IReadOnlyCollection<Messаge> GetAll()
        {
            return messаges;
        }
    }
}
