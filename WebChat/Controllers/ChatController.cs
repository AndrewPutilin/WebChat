using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using WebChat.Models;
using WebChat.ViewModels;
using WebChat.Services.Interfaces;

namespace WebChat.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;
        UserManager<User> _userManager;

        public ChatController(ILogger<ChatController> logger, UserManager<User> userManager, IChatService chatService)
        {
            this._chatService = chatService;
            this._logger = logger;
            this._userManager = userManager;
        }

        /// <summary>
        /// Экшн прочитки сообщения
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ReadMessage(int messageId, int chatId)
        {           

            _chatService.MarkMessageAsRead(messageId);

            var model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());

            model.ChatMessаges = _chatService.GetMessаgesByChat(model.NewChat.Id);
            model.CheckLiveBackUsers = _chatService.GetCheckLiveBackUsersByChat(model.NewChat.Id);
            model.NewChat = _chatService.GetChatById(model.NewChat.Id);

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            if (model.NewChat.Name.Contains("#"))
            {
                return RedirectToAction("GroupDialogue", "Chat");
            }

            return RedirectToAction("Dialogue", "Chat");
        }

       /// <summary>
       /// Экшн открытия списка ююзеров для добавления в чат
       /// </summary>
       /// <returns></returns>
        public IActionResult ListUserToAdd() => View(_userManager.Users.ToList());

        /// <summary>
        /// Экшн открытия списка диалога
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            ChatViewModel model;

            var ourModel = TempData["OurModel"]?.ToString();
            if (string.IsNullOrEmpty(ourModel))
            {
                model = new ChatViewModel();
            }
            else
            {
                model = JsonSerializer.Deserialize<ChatViewModel>(ourModel);
            }

            if (model.ThisUserEmail is null)
            {
                var userEmail = this.User.FindFirst(ClaimTypes.Email).Value;

                model.AllChat = _chatService.GetAllGroupChats(userEmail);
                model.ThisUserEmail = userEmail;
            }

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            return View(model);
        }

        /// <summary>
        /// Экшн открытия лички
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Dialogue()
        {
            var model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            return View(model);
        }

        /// <summary>
        /// Экшн открытия диалога
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GroupDialogue()
        {
            var model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());

            model.ChatMessаges = _chatService.GroupDialogue(model.ChatMessаges, model.CheckLiveBackUsers, model.NewChat.Id, model.ThisUserEmail);
            model.CheckLiveBackUsers = _chatService.GetCheckLiveBackUsersByChat(model.NewChat.Id);
            model.NewChat = _chatService.GetChatById(model.NewChat.Id);

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            return View(model);
        }

        /// <summary>
        /// Экшн добавления юзера в чат
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost] 
        public IActionResult AddUserToChat(string userName)
        {
            var model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());

            _chatService.AddUserToChat(model, userName);

            model.ChatMessаges = _chatService.GetMessаgesByChat(model.NewChat.Id);
            model.CheckLiveBackUsers = _chatService.GetCheckLiveBackUsersByChat(model.NewChat.Id);
            model.AllChat = _chatService.GetAllChats();
            model.NewChat = _chatService.GetChatById(model.NewChat.Id);

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            return RedirectToAction("GroupDialogue", "Chat");;
        }

        /// <summary>
        /// Экшн создания группового чата
        /// </summary>
        /// <param name="chatName"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateGroupChat(string chatName)
        {
            var currentUserEmail = this.User.FindFirst(ClaimTypes.Email).Value;

            var model = _chatService.CreateGroupChat(chatName, currentUserEmail);

            TempData["OurModel"] = JsonSerializer.Serialize(model);


            return RedirectToAction("GroupDialogue", "Chat");
        }

        /// <summary>
        /// Экшн удаления сообщений из чата
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteMassage(int messageId)
        {
            var model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());

            _chatService.DeleteMessage(messageId);

            model.ChatMessаges = _chatService.GetMessаgesByChat(model.NewChat.Id);
            model.CheckLiveBackUsers = _chatService.GetCheckLiveBackUsersByChat(model.NewChat.Id);
            
            TempData["OurModel"] = JsonSerializer.Serialize(model);

            if (model.NewChat.Name.Contains("#"))
            {
                return RedirectToAction("GroupDialogue", "Chat");
            }

            return RedirectToAction("Dialogue", "Chat");
        }

        /// <summary>
        /// Экшн выхода из чата
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ExitFromChat()
        {
            var model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());

            _chatService.ExitFromChat(model.NewChat.Id, model.ThisUserEmail);

            model.NewChat = _chatService.GetChatById(model.NewChat.Id);
            model.CheckLiveBackUsers = _chatService.GetCheckLiveBackUsersByChat(model.NewChat.Id);

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            if (model.NewChat.Name.Contains("#"))
            {
                return RedirectToAction("GroupDialogue", "Chat");
            }

            return RedirectToAction("Dialogue", "Chat");
        }

        /// <summary>
        /// Экшен удаления чата
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteChat()
        {
            var model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());

            _chatService.DeleteChat(model.AllChat, model.NewChat.Id);

            model.AllChat = _chatService.GetAllChats();

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            return RedirectToAction("Index","Users");
        }

        /// <summary>
        /// Экшен отправки сообщений
        /// </summary>
        /// <param name="messegeText"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SendMassege(string messegeText, int chatId )
        {
            var model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());

            var currentUserEmail = this.User.FindFirst(ClaimTypes.Email).Value;

            model.ThisUserEmail = currentUserEmail;
            model.Bots=_chatService.SendMessage(model.Bots, model.ChatMessаges, model.CheckLiveBackUsers, messegeText, chatId, currentUserEmail);
            model.ChatMessаges = _chatService.GetMessаgesByChat(model.NewChat.Id);
            model.CheckLiveBackUsers = _chatService.GetCheckLiveBackUsersByChat(model.NewChat.Id);

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            if (model.NewChat.Name.Contains("#"))
            {
                return RedirectToAction("GroupDialogue", "Chat");
            }

            return RedirectToAction("Dialogue", "Chat");
        }

        /// <summary>
        /// Экшен создания лички
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateChat(string id)
        {
            User secondUser = await _userManager.FindByIdAsync(id);

            var currentUserEmail = this.User.FindFirst(ClaimTypes.Email).Value;

            var model = _chatService.CreateChat(id, currentUserEmail, secondUser);

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            return RedirectToAction("Dialogue", "Chat");
        }
    }
}
