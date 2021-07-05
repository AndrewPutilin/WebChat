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
        public IActionResult ReadMessage(int messageId)
        {
            var model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());

            model = _chatService.ReadMessage(model, messageId);

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
            var model = new ChatViewModel();

            if (TempData["OurModel"] is not null)
            {
                 model = JsonSerializer.Deserialize<ChatViewModel>(TempData["OurModel"].ToString());
            }
            else
            {
                ClaimsPrincipal currentUser = this.User;
                var currentUserEmail = currentUser.FindFirst(ClaimTypes.Email).Value;

                model = _chatService.Index(model, currentUserEmail);
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

            model = _chatService.GroupDialogue(model);

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

            model = _chatService.AddUserToChat(model, userName);

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
            var model = new ChatViewModel();

            ClaimsPrincipal currentUser = this.User;
            var currentUserEmail = currentUser.FindFirst(ClaimTypes.Email).Value;

            model = _chatService.CreateGroupChat(model, chatName, currentUserEmail);

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

            model = _chatService.DeleteMessage(model, messageId);
            
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

            model = _chatService.ExitFromChat(model);

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

            model = _chatService.DeleteChat(model);

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

            ClaimsPrincipal currentUser = this.User;
            var currentUserEmail = currentUser.FindFirst(ClaimTypes.Email).Value;

            model.ThisUserEmail = currentUserEmail;

            model = _chatService.SendMessage(model, messegeText, chatId, currentUserEmail);

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
            var model = new ChatViewModel();

            User secondUser = await _userManager.FindByIdAsync(id);

            ClaimsPrincipal currentUser = this.User;
            var currentUserEmail = currentUser.FindFirst(ClaimTypes.Email).Value;

            model = _chatService.CreateChat(model, id, currentUserEmail, secondUser);

            TempData["OurModel"] = JsonSerializer.Serialize(model);

            return RedirectToAction("Dialogue", "Chat");
        }
    }
}
