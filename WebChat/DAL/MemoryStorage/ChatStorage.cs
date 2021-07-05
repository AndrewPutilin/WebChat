using System;
using System.Collections.Generic;
using System.Linq;
using WebChat.DAL.Domain;
using WebChat.Models;

namespace WebChat.DAL.MemoryStorage
{
    public class ChatStorage : IChatStorage
    {
        private List<Chat> chats;
        private List<CheckLiveBackUser> checkBacks;
        private OperatorContext context;

        public ChatStorage()
        {
            context = new OperatorContext();
            chats = context.Chats.OrderBy(c => c.Id).ToList();
            checkBacks = context.CheckLiveBackUsers.OrderBy(cb => cb.Id).ToList();
        }

        public Chat CreateGroupChat(string groupName, string creatorUser)
        {
            var newChat = new Chat();

            newChat.Name = groupName;
            newChat.Users = creatorUser;

            context.Chats.Add(newChat);
            context.SaveChanges();

            chats = context.Chats.OrderBy(c => c.Id).ToList();
            newChat.Id = chats.Last().Id;

            AddCheckLive(newChat, creatorUser);

            checkBacks = context.CheckLiveBackUsers.OrderBy(ch => ch.Id).ToList();

            return newChat;
        }

        public Chat GetChatByUsers(string users)
        {
            return chats.Where(c => c.Users.Equals(users)).FirstOrDefault();
        }

        public void AddChat(Chat chat)
        {
            var listNewChat = chat.Users.Split(' ');

            Array.Sort(listNewChat);

            chat.Users = String.Join(" ", listNewChat);

            var checkChat = true;

            if (context.Chats is not null)
            {
                foreach (var c in chats)
                {
                    if (c.Users is not null && chat.Users is not null)
                    {
                        var list1 = c.Users.Split(' ');

                        Array.Sort(list1);

                        if (list1.SequenceEqual(listNewChat))
                        {
                            checkChat = false;
                            chat.Id = c.Id;
                        }
                    }
                }
                if (checkChat)
                {
                    context.Chats.Add(chat);

                    context.SaveChanges();

                    chats = context.Chats.OrderBy(c => c.Id).ToList();

                    chat.Id = chats.Last().Id;
                }
            }
            else
            {
                context.Chats.Add(chat);

                context.SaveChanges();

                chats = context.Chats.OrderBy(c => c.Id).ToList();

                chat.Id = chats.Last().Id;
            }

        }

        public void DeleteChat(Chat chat)
        {
            var deleteChat = context.Chats.FirstOrDefault(c => c.Id == chat.Id);

            context.Chats.Remove(deleteChat);

            var listChecks = context.CheckLiveBackUsers.Where(ch=>ch.Chat.Id== deleteChat.Id).ToList();

            context.CheckLiveBackUsers.RemoveRange(listChecks);

            context.SaveChanges();

            chats = context.Chats.OrderBy(c => c.Id).ToList();

            checkBacks = context.CheckLiveBackUsers.OrderBy(ch => ch.Id).ToList();
        }

        public void AddCheckLive(Chat chat, string userEmail)
        {
            var newCheckLive = new CheckLiveBackUser();

            newCheckLive.Id = null;
            newCheckLive.Chat = context.Chats.FirstOrDefault(c => c.Id == chat.Id);
            newCheckLive.TimeLeave = null;
            newCheckLive.TimeComeIn = DateTime.Now;
            newCheckLive.UserEmail = userEmail;

            context.CheckLiveBackUsers.Add(newCheckLive);

            context.SaveChanges();

            chats = context.Chats.OrderBy(c => c.Id).ToList();

            checkBacks = context.CheckLiveBackUsers.OrderBy(ch => ch.Id).ToList();
        }

        public void AddUser(Chat chat, string userEmail)
        {
            var changeChat = context.Chats.FirstOrDefault(c => c.Id == chat.Id);
            var listNewChat = changeChat.Users.Split(' ').ToList();

            listNewChat.Add(userEmail);
            listNewChat.Sort();

            changeChat.Users = String.Join(" ", listNewChat);

            AddCheckLive(chat, userEmail);

            var systemMessage = new Messаge();

            systemMessage.MessageText = "Пользователь " + userEmail + " вошел в чат";
            systemMessage.Chat = GetChatById(chat.Id);
            systemMessage.MesegeTime = DateTime.Now.ToString();
            systemMessage.UserEmail = "System";
            systemMessage.Id = null;

            context.Messages.Add(systemMessage);

            context.SaveChanges();

            chats = context.Chats.OrderBy(c => c.Id).ToList();

            checkBacks = context.CheckLiveBackUsers.OrderBy(ch => ch.Id).ToList();
        }

        public void DeleteUser(Chat chat, string user)
        {
            var maxCheckCome = checkBacks.Where(check => check.UserEmail.Equals(user) && check.Chat.Id==chat.Id).OrderByDescending(c => c.TimeComeIn).FirstOrDefault();

            if (maxCheckCome.TimeLeave is null)
            {
                context.CheckLiveBackUsers.Where(check => check.Chat.Id == chat.Id && check.UserEmail.Equals(user)).OrderByDescending(c => c.TimeComeIn).First().TimeLeave = DateTime.Now;

                var systemMessage = new Messаge();

                systemMessage.MessageText = "Пользователь " + user + " вышел из чата";
                systemMessage.Chat = GetChatById(chat.Id);
                systemMessage.MesegeTime = DateTime.Now.ToString();
                systemMessage.UserEmail = "System";
                systemMessage.Id = null;

                context.Messages.Add(systemMessage);

                context.SaveChanges();

                chats = context.Chats.OrderBy(c => c.Id).ToList();

                checkBacks = context.CheckLiveBackUsers.OrderBy(ch => ch.Id).ToList();
            }
        }

        public Chat GetChatById(int soughtChat)
        {
            return context.Chats.FirstOrDefault(c => c.Id == soughtChat);
        }

        public IReadOnlyCollection<CheckLiveBackUser> GetAllCheckLives(int chatId)
        {
            if (checkBacks is not null) return checkBacks.Where(c => c.Chat.Id == chatId).ToList();

            else return null;
        }

        public IReadOnlyCollection<Chat> GetAll()
        {
            return chats;
        }
    }
}
