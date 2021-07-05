using System.Collections.Generic;
using WebChat.DAL.Domain;

namespace WebChat.DAL.MemoryStorage
{
    public interface IMessageStorage
    {
        Messаge GetMessаgeById(int messageId);
        void AddMessage(Messаge messаge);
        void DeleteMessage(Messаge messаge);
        List<Messаge> GetMesegesByChat(int chatId);
        IReadOnlyCollection<Messаge> GetAll();
    }
}
