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
        private  IChatStorage _chatStorage;
        private  IMessageStorage _messageStorage;

        public ChatService()
        {
            _chatStorage = new ChatStorage();
            _messageStorage = new MessageStorage();
        }

        public ChatViewModel ReadMessage(ChatViewModel chatViewModel, int id)
        {
            _messageStorage.ReadMessage(id);

            chatViewModel.ChatMessаges = _messageStorage.GetMesegesByChat(_messageStorage.GetMessаgeById(id).Chat.Id);

            return chatViewModel;
        }

        public ChatViewModel CreateGroupChat(ChatViewModel chatViewModel, string chatName, string currentUserEmail)
        {
            if (Int32.TryParse(chatName, out int id))
            {
                chatViewModel.NewChat = _chatStorage.GetChatById(id);
            }

            else
            {
                chatViewModel.NewChat = _chatStorage.CreateGroupChat(chatName, currentUserEmail);
            }


            chatViewModel.AllChat = _chatStorage.GetAll();
            chatViewModel.ChatMessаges = new List<Messаge>();
            chatViewModel.Bots = new HashSet<ChatBot>();
            chatViewModel.ChatMessаges = _messageStorage.GetMesegesByChat(chatViewModel.NewChat.Id);
            chatViewModel.CurrentChat = chatViewModel.NewChat.Id.ToString();
            chatViewModel.ThisUserEmail = currentUserEmail;
            chatViewModel.CheckLiveBackUsers = _chatStorage.GetAllCheckLives(chatViewModel.NewChat.Id);

            return chatViewModel;
        }

        public ChatViewModel DeleteMessage(ChatViewModel chatViewModel, int messageId)
        {
            var deleteMessage = _messageStorage.GetMessаgeById(messageId);
            var mesChat = deleteMessage.Chat;

            _messageStorage.DeleteMessage(deleteMessage);

            chatViewModel.ChatMessаges = new List<Messаge>();

            var listMessages = _messageStorage.GetMesegesByChat(mesChat.Id);

            if (listMessages is not null)
            {
                chatViewModel.ChatMessаges = listMessages;
            }
            else
            {
                chatViewModel.ChatMessаges = new List<Messаge>();
            }

            chatViewModel.CheckLiveBackUsers = _chatStorage.GetAllCheckLives(chatViewModel.NewChat.Id);


            return chatViewModel;
        }

        public ChatViewModel ExitFromChat(ChatViewModel chatViewModel)
        {
            chatViewModel.AllChat = _chatStorage.GetAll();
            chatViewModel.ChatMessаges = _messageStorage.GetMesegesByChat(chatViewModel.NewChat.Id);
            chatViewModel.CheckLiveBackUsers = _chatStorage.GetAllCheckLives(chatViewModel.NewChat.Id);

            _chatStorage.DeleteUser(chatViewModel.NewChat, chatViewModel.ThisUserEmail);

            chatViewModel.AllChat = _chatStorage.GetAll();
            chatViewModel.ChatMessаges = _messageStorage.GetMesegesByChat(chatViewModel.NewChat.Id);
            chatViewModel.CheckLiveBackUsers = _chatStorage.GetAllCheckLives(chatViewModel.NewChat.Id);

            return chatViewModel;
        }

        public ChatViewModel DeleteChat(ChatViewModel chatViewModel)
        {
            var deleteChat = chatViewModel.AllChat.FirstOrDefault(c => c.Id == chatViewModel.NewChat.Id);

            _chatStorage.DeleteChat(deleteChat);

            chatViewModel.AllChat = _chatStorage.GetAll();

            return chatViewModel;
        }

        public ChatViewModel SendMessage(ChatViewModel chatViewModel, string messageText, int chatId, string currentUserEmail)
        {
            if (_chatStorage.GetAllCheckLives(chatViewModel.NewChat.Id).Where(check => check.UserEmail.Equals(chatViewModel.ThisUserEmail)).Any(v => v.TimeLeave is not null)) // проверка что пользователь не вышел с чата
            {
                var maxCheckLive = _chatStorage
                    .GetAllCheckLives(chatViewModel.NewChat.Id)
                    .Where(check => check.UserEmail.Equals(chatViewModel.ThisUserEmail))
                    .OrderByDescending(c => c.TimeLeave)
                    .First();
                var maxCheckCome = _chatStorage
                    .GetAllCheckLives(chatViewModel.NewChat.Id)
                    .Where(check => check.UserEmail.Equals(chatViewModel.ThisUserEmail))
                    .OrderByDescending(c => c.TimeComeIn)
                    .First();

                if (maxCheckLive.TimeLeave > maxCheckCome.TimeComeIn)
                {
                    return chatViewModel;
                }
            }

            var currentChat = _chatStorage.GetChatById(chatId);

            switch (messageText) //проверка что написали команду боту
            {
                case "/startsharpik":
                    chatViewModel.Bots.Add(new ChatBot(currentChat, "Sharpik"));
                    return chatViewModel;
                case "/endsharpik":
                    var chatbot = chatViewModel.Bots.FirstOrDefault(b => b.BotName == "Sharpik" && b.Chat.Id == currentChat.Id);
                    chatViewModel.Bots.Remove(chatbot);
                    return chatViewModel;
                case "/startzhabist":
                    chatViewModel.Bots.Add(new ChatBot(currentChat, "Zhabist"));
                    return chatViewModel;
                case "/endzhabist":
                    var chatbot2 = chatViewModel.Bots.FirstOrDefault(b => b.BotName == "Zhabist" && b.Chat.Id == currentChat.Id);
                    chatViewModel.Bots.Remove(chatbot2);
                    return chatViewModel;
            }

            var newMessage = new Messаge();

            newMessage.MesegeTime = DateTime.Now.ToString();
            newMessage.UserEmail = currentUserEmail;
            newMessage.MessageText = messageText;
            newMessage.Chat = currentChat;

            _messageStorage.AddMessage(newMessage);

            var chatBots = chatViewModel.Bots.Where(cb => cb.Chat.Id == currentChat.Id).ToList();

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

            chatViewModel.ChatMessаges = new List<Messаge>();

            var listMessages = _messageStorage.GetMesegesByChat(newMessage.Chat.Id);

            if (listMessages is not null)
            {
                chatViewModel.ChatMessаges = listMessages;
            }
            else
            {
                chatViewModel.ChatMessаges = new List<Messаge>();
            }

            chatViewModel.CheckLiveBackUsers = _chatStorage.GetAllCheckLives(chatViewModel.NewChat.Id);

            return chatViewModel;
        }

        public ChatViewModel CreateChat(ChatViewModel chatViewModel, string userId, string currentUserEmail, User secondUser)
        {
            chatViewModel.AllChat = _chatStorage.GetAll();

            var newCHat = new Chat();

            newCHat.Name = "Чат " + secondUser.Email + " и " + currentUserEmail;

            newCHat.Users = currentUserEmail + " " + secondUser.Email;

            _chatStorage.AddChat(newCHat);

            chatViewModel.ChatMessаges = new List<Messаge>();
            chatViewModel.Bots = new HashSet<ChatBot>();
            chatViewModel.ChatMessаges = _messageStorage.GetMesegesByChat(newCHat.Id);

            chatViewModel.NewChat = _chatStorage.GetChatByUsers(newCHat.Users);
            chatViewModel.CurrentChat = userId;
            chatViewModel.ThisUserEmail = currentUserEmail;

            return chatViewModel;
        }

        public ChatViewModel AddUserToChat(ChatViewModel chatViewModel, string userName)
        {
            var chat = chatViewModel.AllChat.FirstOrDefault(c => c.Id == chatViewModel.NewChat.Id);

            if (!chat.Users.Contains(userName))
            {
                _chatStorage.AddUser(chat, userName);

                chatViewModel.NewChat.Users = chatViewModel.NewChat.Users + " " + userName;
            }
            else
            {
                var checkLives = _chatStorage.GetAllCheckLives(chatViewModel.NewChat.Id).Where(ch => ch.Chat.Id == chat.Id && ch.UserEmail == userName).OrderByDescending(c => c.TimeComeIn).First();

                if (checkLives.TimeLeave is not null)
                {
                    _chatStorage.AddCheckLive(chatViewModel.NewChat, userName);

                    var systemMessage = new Messаge();

                    systemMessage.MessageText = "Пользователь " + userName + " вернулся в чат";
                    systemMessage.Chat = _chatStorage.GetChatById(chat.Id);
                    systemMessage.MesegeTime = DateTime.Now.ToString();
                    systemMessage.UserEmail = "System";
                    systemMessage.Id = null;

                    _messageStorage.AddMessage(systemMessage);
                }
            }
            chatViewModel.ChatMessаges = _messageStorage.GetMesegesByChat(chatViewModel.NewChat.Id);
            chatViewModel.AllChat = _chatStorage.GetAll();
            chatViewModel.CheckLiveBackUsers = _chatStorage.GetAllCheckLives(chatViewModel.NewChat.Id);

            return chatViewModel;
        }

        public ChatViewModel GroupDialogue(ChatViewModel chatViewModel)
        {
            chatViewModel.CheckLiveBackUsers = _chatStorage.GetAllCheckLives(chatViewModel.NewChat.Id);

            var chatChecks = chatViewModel.CheckLiveBackUsers.Where(ch => ch.Chat.Id == chatViewModel.NewChat.Id && ch.UserEmail.Equals(chatViewModel.ThisUserEmail)).ToList();

            var newChatMessages = new List<Messаge>();

            foreach (var chatCheck in chatChecks)
            {
                if (chatCheck.TimeLeave is not null)
                {
                    newChatMessages.AddRange(chatViewModel.ChatMessаges.Where(cm => DateTime.Parse(cm.MesegeTime) > chatCheck.TimeComeIn && DateTime.Parse(cm.MesegeTime) < chatCheck.TimeLeave).ToList());
                }
                else
                {
                    newChatMessages.AddRange(chatViewModel.ChatMessаges.Where(cm => DateTime.Parse(cm.MesegeTime) > chatCheck.TimeComeIn).ToList());
                }
            }

            chatViewModel.ChatMessаges = newChatMessages;

            return chatViewModel;
        }

        public ChatViewModel Index(ChatViewModel chatViewModel, string currentUserEmail)
        {
            chatViewModel.ThisUserEmail = currentUserEmail;
            chatViewModel.AllChat = _chatStorage.GetAll();

            return chatViewModel;
        }
    }
}
