using System.Collections.Generic;
using WebChat.DAL.Domain;

namespace WebChat.DAL.MemoryStorage
{
    public interface IMessageStorage
    {
        /// <summary>
        /// Находит сообщение по Id
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        Messаge GetMessаgeById(int messageId);

        /// <summary>
        /// Пометить сообщения как прочитанное
        /// </summary>
        /// <param name="messageId"></param>
        void MarkMessageAsRead(int messageId);

        /// <summary>
        /// Добавляет сообщение в чат
        /// </summary>
        /// <param name="messаge"></param>
        void AddMessage(Messаge messаge);

        /// <summary>
        /// Удаляет сообщение из чата
        /// </summary>
        /// <param name="messаge"></param>
        void DeleteMessage(Messаge messаge);

        /// <summary>
        /// Находит все сообщения в чате
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        List<Messаge> GetMessagesByChat(int chatId);

        /// <summary>
        /// Возвращает все сообщения
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<Messаge> GetAll();
    }
}
