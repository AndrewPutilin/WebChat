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
        private OperatorContext context;
        public ChatStorage()
        {
            context = new OperatorContext();
            chats = context.Chats.OrderBy(c => c.Id).ToList();
        }

        public Chat CreateGroupChat(string groupName, List<string> users)
        {
            users.Sort();

            var newChat = new Chat();

            newChat.Users = String.Join(" ", users);
            newChat.Name = groupName;

            context.Chats.Add(newChat);
            context.SaveChanges();

            chats = context.Chats.OrderBy(c => c.Id).ToList();
            newChat.Id = chats.Last().Id;

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

            context.SaveChanges();

            chats = context.Chats.OrderBy(c => c.Id).ToList();
        }

        public void AddUser(Chat chat, User user)
        {
            var changeChat = context.Chats.FirstOrDefault(c => c == chat);
            var listNewChat = changeChat.Users.Split(' ').ToList();

            listNewChat.Add(user.Email);
            listNewChat.Sort();

            changeChat.Users = String.Join(" ", listNewChat);

            context.SaveChanges();

            chats = context.Chats.OrderBy(c => c.Id).ToList();
        }

        public void DeleteUser(Chat chat, string user)
        {
            var changeChat = context.Chats.FirstOrDefault(c => c == chat);
            var listNewChat = changeChat.Users.Split(' ').ToList();

            listNewChat.Remove(user);
            changeChat.Users = String.Join(" ", listNewChat);

            context.SaveChanges();

            chats = context.Chats.OrderBy(c => c.Id).ToList();
        }

        public Chat GetChatById(int soughtChat)
        {
            return chats.FirstOrDefault(c => c.Id == soughtChat);
        }

        public IReadOnlyCollection<Chat> GetAll()
        {
            return chats;
        }
    }
}
