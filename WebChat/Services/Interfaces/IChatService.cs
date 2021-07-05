using WebChat.Models;
using WebChat.ViewModels;

namespace WebChat.Services.Interfaces
{
    public interface IChatService
    {
        ChatViewModel GroupDialogue(ChatViewModel chatViewModel);
        ChatViewModel AddUserToChat(ChatViewModel chatViewModel, string userName);
        ChatViewModel CreateGroupChat(ChatViewModel chatViewModel, string chatName, string currentUserEmail);
        ChatViewModel ExitFromChat(ChatViewModel chatViewModel);
        ChatViewModel DeleteChat(ChatViewModel chatViewModel);
        ChatViewModel ReadMessage(ChatViewModel chatViewModel, int id);
        ChatViewModel DeleteMessage(ChatViewModel chatViewModel, int messageId);
        ChatViewModel SendMessage(ChatViewModel chatViewModel, string messageText, int chatId, string currentUserEmail);
        ChatViewModel CreateChat(ChatViewModel chatViewModel, string userId, string currentUserEmail, User secondUser);
        ChatViewModel Index(ChatViewModel chatViewModel, string currentUserEmail);
    }
}
