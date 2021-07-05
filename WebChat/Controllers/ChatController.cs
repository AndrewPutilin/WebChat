using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using WebChat.DAL.Domain;
using WebChat.DAL.MemoryStorage;
using WebChat.Models;
using WebChat.ViewModels;
using WebChat.SharpBot;
using System.Text;

namespace WebChat.Controllers
{
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IChatStorage _chatStorage;
        public readonly IMessageStorage _messageStorage;
        UserManager<User> _userManager;

        public ChatController(ILogger<ChatController> logger, IChatStorage chatStorage, UserManager<User> userManager, IMessageStorage messageStorage)
        {
            _logger = logger;
            _chatStorage = chatStorage;
            _userManager = userManager;
            _messageStorage = messageStorage;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Dialogue()
        {
            var model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddGroupChat(string chatName, List<string> users)
        {
            var model = new ChatViewModel();

            model.AllChat = _chatStorage.GetAll();

            var newCHat = new Chat();

            ClaimsPrincipal currentUser = this.User;
            var currentUserEmail = currentUser.FindFirst(ClaimTypes.Email).Value;
            var user = await _userManager.FindByEmailAsync(currentUserEmail);
            newCHat = _chatStorage.CreateGroupChat(chatName, users);

            _chatStorage.AddChat(newCHat);

            model.ChatMessаges = new List<Messаge>();
            model.Bots = new HashSet<ChatBot>();
            model.ChatMessаges = _messageStorage.GetMesegesByChat(newCHat.Id);

            model.NewChat = newCHat;
            model.CurrentChat = newCHat.Id.ToString();
            model.ThisUserEmail = currentUserEmail;

            TempData["OurModel"] = JsonSerializer.Serialize(model);


            return View();
        }

        [HttpPost]
        public IActionResult DeleteMassage(int messageId)
        {
            var model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());

            var deleteMessage = _messageStorage.GetMessаgeById(messageId);
            var mesChat = deleteMessage.Chat;

            _messageStorage.DeleteMessage(deleteMessage);

            model.ChatMessаges = new List<Messаge>();

            var listMessages = _messageStorage.GetMesegesByChat(mesChat.Id);

            if (listMessages is not null)
            {
                model.ChatMessаges = listMessages;
            }
            else
            {
                model.ChatMessаges = new List<Messаge>();
            }

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            return RedirectToAction("Dialogue", "Chat");
        }

        [HttpPost]
        public IActionResult ExitFromChat()
        {
            var model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());

            var chat = model.AllChat.FirstOrDefault(c => c.Id == model.NewChat.Id);

            ClaimsPrincipal currentUser = this.User;
            var currentUserEmail = currentUser.FindFirst(ClaimTypes.Email).Value;

            _chatStorage.DeleteUser(chat, currentUserEmail);

            var newListUsers = model.NewChat.Users.Replace(currentUserEmail, "");

            model.NewChat.Users = newListUsers;

            model.AllChat = _chatStorage.GetAll();

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            return RedirectToAction("Dialogue", "Chat");
        }


        [HttpPost]
        public IActionResult DeleteChat()
        {
            var model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());

            var deleteChat = model.AllChat.FirstOrDefault(c=>c.Id==model.NewChat.Id);

            _chatStorage.DeleteChat(deleteChat);

            model.AllChat = _chatStorage.GetAll();

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            return RedirectToAction("Index","Users");
        }

        [HttpPost]
        public IActionResult SendMassege(string messegeText, int chatId )
        {
            var model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            var currentChat = _chatStorage.GetChatById(chatId);

            switch (messegeText)
            {
                case "/startsharpik":
                    model.Bots.Add( new ChatBot(currentChat, "Sharpik"));
                    TempData["OurModel"] = JsonSerializer.Serialize(model);

                    return RedirectToAction("Dialogue", "Chat");
                case "/endsharpik":
                    var chatbot = model.Bots.FirstOrDefault(b=>b.BotName== "Sharpik" && b.Chat.Id == currentChat.Id);
                    model.Bots.Remove(chatbot);
                    TempData["OurModel"] = JsonSerializer.Serialize(model);

                    return RedirectToAction("Dialogue", "Chat");
                case "/startzhabist":
                    model.Bots.Add(new ChatBot(currentChat, "Zhabist"));
                    TempData["OurModel"] = JsonSerializer.Serialize(model);

                    return RedirectToAction("Dialogue", "Chat");
                case "/endzhabist":
                    var chatbot2 = model.Bots.FirstOrDefault(b => b.BotName == "Zhabist" && b.Chat.Id == currentChat.Id);
                    model.Bots.Remove(chatbot2);
                    TempData["OurModel"] = JsonSerializer.Serialize(model);

                    return RedirectToAction("Dialogue", "Chat");
            }

            ClaimsPrincipal currentUser = this.User;
            var currentUserEmail = currentUser.FindFirst(ClaimTypes.Email).Value;
            
            var newMessage = new Messаge();
            
            newMessage.MesegeTime = DateTime.Now.ToString();
            newMessage.UserEmail = currentUserEmail;
            newMessage.MessageText = messegeText;
            newMessage.Chat = currentChat;

            _messageStorage.AddMessage(newMessage);

            var chatBots = model.Bots.Where(cb=>cb.Chat.Id==currentChat.Id).ToList();

            if (chatBots is not null)
            {
                foreach(var b in chatBots)
                {
                    if (b.BotName.Equals("Sharpik"))
                    {
                        var botMessege = new Messаge();
                        var sharpAnswer = new StringBuilder();

                        SharpikBot.SharpikStart(messegeText, sharpAnswer);

                        botMessege.MessageText = sharpAnswer.ToString();
                        botMessege.Chat = currentChat;
                        botMessege.MesegeTime = DateTime.Now.ToString();
                        botMessege.UserEmail = "Sharpik";
                        botMessege.Id = null;

                        _messageStorage.AddMessage(botMessege);
                    }
                    if (b.BotName.Equals("Zhabist"))
                    {
                        var textFromBot = ZhabistBot.ZhabistStart(messegeText);
                        
                        if(textFromBot is not null)
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

            model.ChatMessаges = new List<Messаge>();

            var listMessages = _messageStorage.GetMesegesByChat(newMessage.Chat.Id);

            if (listMessages is not null)
            {
                model.ChatMessаges = listMessages;
            }
            else
            {
                model.ChatMessаges = new List<Messаge>();
            }

            TempData["OurModel"] = JsonSerializer.Serialize(model);


            return RedirectToAction("Dialogue", "Chat");
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat(string id)
        {
            var model = new ChatViewModel();

            model.AllChat = _chatStorage.GetAll();

            var newCHat = new Chat();

            User secondUser = await _userManager.FindByIdAsync(id);

            ClaimsPrincipal currentUser = this.User;
            var currentUserEmail = currentUser.FindFirst(ClaimTypes.Email).Value;
            var user = await _userManager.FindByEmailAsync(currentUserEmail);

            newCHat.Name = "Чат " + secondUser.Email + " и " + currentUserEmail;

            newCHat.Users = user.Email + " " + secondUser.Email;

            _chatStorage.AddChat(newCHat);

            model.ChatMessаges = new List<Messаge>();
            model.Bots = new HashSet<ChatBot>();
            model.ChatMessаges = _messageStorage.GetMesegesByChat(newCHat.Id);

            model.NewChat = _chatStorage.GetChatByUsers(newCHat.Users);
            model.CurrentChat = id;
            model.ThisUserEmail = currentUserEmail;

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            return RedirectToAction("Dialogue", "Chat");
        }
    }
}
