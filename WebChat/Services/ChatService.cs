using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebChat.DAL.Domain;
using WebChat.DAL.MemoryStorage;
using WebChat.Models;
using WebChat.Services.Interfaces;
using WebChat.SharpBot;
using WebChat.ViewModels;

namespace WebChat.Services
{
    public class ChatService : IChatService
    {
        private IChatStorage _chatStorage;
        private IMessageStorage _messageStorage;

        public ChatService()
        {
            _chatStorage = new ChatStorage();
            _messageStorage = new MessageStorage();
        }

        public List<Chat> GetAllChats()
        {
            return _chatStorage.GetAll().ToList();
        }

        public void MarkMessageAsRead(int messageId)
        {
            _messageStorage.MarkMessageAsRead(messageId);
        }

        public List<Messаge> GetMessages(int chatId)
        {
            return _messageStorage.GetMessagesByChat(chatId);
        }

        public ChatViewModel CreateGroupChat(string chatName, string currentUserEmail)
        {
            var model = new ChatViewModel();

            if (Int32.TryParse(chatName, out int id))
            {
                model.NewChat = _chatStorage.GetChatById(id);
            }

            else
            {
                model.NewChat = _chatStorage.CreateGroupChat(chatName, currentUserEmail);
            }

            model.AllChat = _chatStorage.GetAll();
            model.ChatMessаges = new List<Messаge>();
            model.Bots = new HashSet<ChatBot>();
            model.ChatMessаges = _messageStorage.GetMessagesByChat(model.NewChat.Id);
            model.CurrentChat = model.NewChat.Id.ToString();
            model.ThisUserEmail = currentUserEmail;
            model.CheckLiveBackUsers = _chatStorage.GetAllCheckLives(model.NewChat.Id).ToList();

            return model;
        }

        public Chat GetChatById(int chatId)
        {
            return _chatStorage.GetChatById(chatId);
        }

        public void DeleteMessage(int messageId)
        {
            var deleteMessage = _messageStorage.GetMessаgeById(messageId);

            _messageStorage.DeleteMessage(deleteMessage);
        }

        public void ExitFromChat(int chatId, string userEmail)
        {
            _chatStorage.DeleteUser(_chatStorage.GetChatById(chatId), userEmail);
        }

        public void DeleteChat(IReadOnlyCollection<Chat> chats, int chatId)
        {
            var deleteChat = chats.FirstOrDefault(c => c.Id == chatId);

            _chatStorage.DeleteChat(deleteChat);
        }

        public HashSet<ChatBot> SendMessage(HashSet<ChatBot> bots, List<Messаge> messаges, List<CheckLiveBackUser> checkLiveBackUsers, string messageText, int chatId, string currentUserEmail)
        {
            var currentChat = _chatStorage.GetChatById(chatId);

            if (_chatStorage.GetAllCheckLives(chatId).Where(check => check.UserEmail.Equals(currentUserEmail)).Any(v => v.TimeLeave is null) || !currentChat.Name.Contains("#")) // проверка что пользователь не вышел с чата
            {
                switch (messageText) //проверка что написали команду боту
                {
                    case "/startsharpik":
                        bots.Add(new ChatBot(currentChat, "Sharpik"));
                        return bots;
                    case "/endsharpik":
                        var chatbot = bots.FirstOrDefault(b => b.BotName == "Sharpik" && b.Chat.Id == currentChat.Id);
                        bots.Remove(chatbot);
                        return bots;
                    case "/startzhabist":
                        bots.Add(new ChatBot(currentChat, "Zhabist"));
                        return bots;
                    case "/endzhabist":
                        var chatbot2 = bots.FirstOrDefault(b => b.BotName == "Zhabist" && b.Chat.Id == currentChat.Id);
                        bots.Remove(chatbot2);
                        return bots;
                }

                var newMessage = new Messаge();

                newMessage.MesegeTime = DateTime.Now.ToString();
                newMessage.UserEmail = currentUserEmail;
                newMessage.MessageText = messageText;
                newMessage.Chat = currentChat;

                _messageStorage.AddMessage(newMessage);

                var chatBots = bots.Where(cb => cb.Chat.Id == currentChat.Id).ToList();

                if (chatBots is not null)
                {
                    foreach (var b in chatBots)
                    {
                        if (b.BotName.Equals("Sharpik"))
                        {
                            var botMessege = new Messаge();
                            var sharpAnswer = new StringBuilder();

                            SharpikBot.SharpikStart(messageText, sharpAnswer);

                            botMessege.MessageText = sharpAnswer.ToString();
                            botMessege.Chat = currentChat;
                            botMessege.MesegeTime = DateTime.Now.ToString();
                            botMessege.UserEmail = "Sharpik";
                            botMessege.Id = null;

                            _messageStorage.AddMessage(botMessege);
                        }
                        if (b.BotName.Equals("Zhabist"))
                        {
                            var textFromBot = ZhabistBot.ZhabistStart(messageText);

                            if (textFromBot is not null)
                            {
                                var botMessege = new Messаge();
                                var sharpAnswer = new StringBuilder();

                                botMessege.MessageText = textFromBot;
                                botMessege.Chat = currentChat;
                                botMessege.MesegeTime = DateTime.Now.ToString();
                                botMessege.UserEmail = "Zhabist";
                                botMessege.Id = null;

                                _messageStorage.AddMessage(botMessege);
                            }
                        }
                    }
                }

                messаges = new List<Messаge>();
            }

            return bots;
        }

        public ChatViewModel CreateChat(string userId, string currentUserEmail, User secondUser)
        {
            var model = new ChatViewModel();
            model.AllChat = _chatStorage.GetAll();

            var newCHat = new Chat();

            newCHat.Name = "Чат " + secondUser.Email + " и " + currentUserEmail;

            newCHat.Users = currentUserEmail + " " + secondUser.Email;

            _chatStorage.AddChat(newCHat);

            model.ChatMessаges = new List<Messаge>();
            model.Bots = new HashSet<ChatBot>();
            model.ChatMessаges = _messageStorage.GetMessagesByChat(newCHat.Id);

            model.NewChat = _chatStorage.GetChatByUsers(newCHat.Users);
            model.CurrentChat = userId;
            model.ThisUserEmail = currentUserEmail;

            return model;
        }

        public void AddUserToChat(ChatViewModel model, string userName)
        {
            var chatId = model.NewChat.Id;
            var chat = model.AllChat.FirstOrDefault(c => c.Id == chatId);

            if (!chat.Users.Contains(userName))
            {
                _chatStorage.AddUser(chat, userName);
            }
            else
            {
                var checkLives = _chatStorage.GetAllCheckLives(chatId).Where(ch => ch.Chat.Id == chat.Id && ch.UserEmail == userName).OrderByDescending(c => c.TimeComeIn).First();

                if (checkLives.TimeLeave is not null)
                {
                    _chatStorage.AddCheckLive(chat, userName);

                    var systemMessage = new Messаge();

                    systemMessage.MessageText = "Пользователь " + userName + " вернулся в чат";
                    systemMessage.Chat = _chatStorage.GetChatById(chatId);
                    systemMessage.MesegeTime = DateTime.Now.ToString();
                    systemMessage.UserEmail = "System";
                    systemMessage.Id = null;

                    _messageStorage.AddMessage(systemMessage);
                }
            }
        }

        public List<Messаge> GroupDialogue(List<Messаge> chatMessages, List<CheckLiveBackUser> checkLiveBackUsers, int chatId, string userEmail)
        {
            checkLiveBackUsers = _chatStorage.GetAllCheckLives(chatId).ToList();

            var chatChecks = checkLiveBackUsers.Where(ch => ch.Chat.Id == chatId && ch.UserEmail.Equals(userEmail)).ToList();

            var newChatMessages = new List<Messаge>();

            foreach (var chatCheck in chatChecks)
            {
                if (chatCheck.TimeLeave is not null)
                {
                    newChatMessages.AddRange(chatMessages.Where(cm => DateTime.Parse(cm.MesegeTime) > chatCheck.TimeComeIn && DateTime.Parse(cm.MesegeTime) < chatCheck.TimeLeave).ToList());
                }
                else
                {
                    newChatMessages.AddRange(chatMessages.Where(cm => DateTime.Parse(cm.MesegeTime) > chatCheck.TimeComeIn).ToList());
                }
            }

            return newChatMessages;
        }

        public List<Chat> GetAllGroupChats(string userEmail)
        {
            return _chatStorage.GetAll(userEmail).ToList();
        }

        public List<Messаge> GetMessаgesByChat(int chatId) 
        {
            return _messageStorage.GetMessagesByChat(chatId);
        }

        public List<CheckLiveBackUser> GetCheckLiveBackUsersByChat(int chatId)
        {
            return _chatStorage.GetAllCheckLives(chatId).ToList();
        }
    }
}
