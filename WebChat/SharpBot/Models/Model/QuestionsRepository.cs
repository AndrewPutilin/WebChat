using System.Collections.Generic;
using System.Linq;
using WebChat.SharpBot.Models.HelpToConnectModel;

namespace WebChat.SharpBot.Models.Model
{
    public enum CategoryQestion
    {
        Name,
        Hello,
        Time,
        Bye,
        Joke
    }

    public class QuestionsRepository : IQuestionsRepository
    {
        private List<string> questHello = new List<string>();
        private List<string> questName = new List<string>();
        private List<string> questJokes = new List<string>();
        private List<string> questTime = new List<string>();
        private List<string> questBye = new List<string>();

        private Dictionary<CategoryQestion, IList<string>> questions = new Dictionary<CategoryQestion, IList<string>>();

        public QuestionsRepository()
        {
            questHello = FileReader.ReadListFromFiles(@"XmlFiles\HelloesTrigger.xml");
            questName = FileReader.ReadListFromFiles(@"XmlFiles\NamesTrigger.xml");
            questJokes = FileReader.ReadListFromFiles(@"XmlFiles\JokesTrigger.xml");
            questTime = FileReader.ReadListFromFiles(@"XmlFiles\TimesTrigger.xml");
            questBye = FileReader.ReadListFromFiles(@"XmlFiles\ByesTrigger.xml");

            questions.Add(CategoryQestion.Name, questName);
            questions.Add(CategoryQestion.Hello, questHello);
            questions.Add(CategoryQestion.Time, questTime);
            questions.Add(CategoryQestion.Bye, questBye);
            questions.Add(CategoryQestion.Joke, questJokes);
        }

        public IList<string> GetQuestionsByCategory(CategoryQestion category) => questions[category].ToList();
    }
}
