using System.Collections.Generic;
using WebChat.DAL.Domain;
using WebChat.Models;

namespace WebChat.DAL.MemoryStorage
{
    public interface IChatStorage
    {
        void AddChat(Chat chat);
        Chat CreateGroupChat(string groupName, List<string> users);
        void DeleteChat(Chat chat);
        void AddUser(Chat chat, User user);
        void DeleteUser(Chat chat, string user);
        Chat GetChatById(int soughtChat);
        Chat GetChatByUsers(string users);
        IReadOnlyCollection<Chat> GetAll();
    }
}
