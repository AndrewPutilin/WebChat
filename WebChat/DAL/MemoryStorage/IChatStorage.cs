using System.Collections.Generic;
using WebChat.DAL.Domain;

namespace WebChat.DAL.MemoryStorage
{
    public interface IChatStorage
    {
        /// <summary>
        ///Добавление чата
        /// </summary>
        /// <param name="chat"></param>
        void AddChat(Chat chat);
        /// <summary>
        /// Добавление группового чата
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="creatorUser"></param>
        /// <returns></returns>
        Chat CreateGroupChat(string groupName, string creatorUser);
        /// <summary>
        /// Удаление чата
        /// </summary>
        /// <param name="chat"></param>
        void DeleteChat(Chat chat);
        /// <summary>
        /// Добавление юзера в чат
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="userEmail"></param>
        void AddUser(Chat chat, string userEmail);
        /// <summary>
        /// Удаление юзера из чата
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="user"></param>
        void DeleteUser(Chat chat, string user);
        /// <summary>
        /// Нахождение чата по Id
        /// </summary>
        /// <param name="soughtChat"></param>
        /// <returns></returns>
        Chat GetChatById(int soughtChat);
        /// <summary>
        /// Нахождение чата по списку юзеров
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        Chat GetChatByUsers(string users);
        /// <summary>
        /// Добавление класса проверки не вышел ли юзер из чата
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="userEmail"></param>
        void AddCheckLive(Chat chat, string userEmail);
        /// <summary>
        /// Возвращает все чаты
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<Chat> GetAll();
        /// <summary>
        /// Возвращает все выходы\входы в чат
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<CheckLiveBackUser> GetAllCheckLives(int chatId);
    }
}
