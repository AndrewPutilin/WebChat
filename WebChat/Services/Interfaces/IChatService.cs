using System.Collections.Generic;
using WebChat.Models;
using WebChat.ViewModels;
using WebChat.DAL.Domain;

namespace WebChat.Services.Interfaces
{
    public interface IChatService
    {
        List<Messаge> GroupDialogue(List<Messаge> chatMessages, List<CheckLiveBackUser> checkLiveBackUsers, int chatId, string userEmail);

        void AddUserToChat(ChatViewModel model, string userName);

        ChatViewModel CreateGroupChat(string chatName, string currentUserEmail);

        void ExitFromChat(int chatId, string userEmail);

        void DeleteChat(IReadOnlyCollection<Chat> chats, int chatId);

        List<Messаge> GetMessages(int chatId);

        void MarkMessageAsRead(int messageId);

        void DeleteMessage(int messageId);

        HashSet<ChatBot> SendMessage(HashSet<ChatBot> bots, List<Messаge> messаges, List<CheckLiveBackUser> checkLiveBackUsers, string messageText, int chatId, string currentUserEmail);

        ChatViewModel CreateChat(string userId, string currentUserEmail, User secondUser);

        Chat GetChatById(int chatId);

        List<Chat> GetAllChats();

        List<Chat> GetAllGroupChats(string userEmail);

        List<Messаge> GetMessаgesByChat(int chatId);

        List<CheckLiveBackUser> GetCheckLiveBackUsersByChat(int chatId);
    }
}
