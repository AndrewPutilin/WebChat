using System.Collections.Generic;

namespace WebChat.SharpBot.Models.Model
{
    public interface IQuestionsRepository
    {
        IList<string> GetQuestionsByCategory(CategoryQestion category);
    }
}
