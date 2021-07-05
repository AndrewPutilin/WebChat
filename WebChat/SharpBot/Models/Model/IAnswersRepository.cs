namespace WebChat.SharpBot.Models.Model
{
    public interface IAnswersRepository
    {
        string GetRandomAnswer(AnswerCategory category);
    }
}
