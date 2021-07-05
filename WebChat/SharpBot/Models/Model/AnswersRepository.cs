using System.Collections.Generic;
using WebChat.SharpBot.Models.HelpToConnectModel;
using WebChat.SharpBot.Untils;

namespace WebChat.SharpBot.Models.Model
{
    public delegate void MethodRep();

    public enum AnswerCategory
    {
        Name,
        Hello,
        Aphorism,
        Bye,
        Joke
    }

    public class AnswersRepository : IAnswersRepository
    {
        private List<string> botName = new List<string>();
        private List<string> greetings = new List<string>();
        private List<string> aphorisms = new List<string>();
        private List<string> godbyes = new List<string>();
        private List<string> jokes = new List<string>(); 

        private Dictionary<AnswerCategory, IList<string>> Answers = new Dictionary<AnswerCategory, IList<string>>();

        public AnswersRepository()
        {
            botName = FileReader.ReadListFromFiles(@"XmlFiles\Names.xml");
            greetings = FileReader.ReadListFromFiles(@"XmlFiles\Greetings.xml");
            aphorisms = FileReader.ReadListFromFiles(@"XmlFiles\Aphorisms.xml");
            godbyes = FileReader.ReadListFromFiles(@"XmlFiles\Godbyes.xml");
            jokes = FileReader.ReadListFromFiles(@"XmlFiles\Jokes.xml");

            Answers.Add(AnswerCategory.Name, botName);
            Answers.Add(AnswerCategory.Hello, greetings);
            Answers.Add(AnswerCategory.Aphorism, aphorisms);
            Answers.Add(AnswerCategory.Bye, godbyes);
            Answers.Add(AnswerCategory.Joke, jokes);
        }

        public string GetRandomAnswer(AnswerCategory category)
        {
            if (Answers.ContainsKey(category))
            {
                var answers = Answers[category];
                var rndIndex = Randomizer.GetRandomInt(answers.Count);
                return answers[rndIndex];
            }

            return string.Empty;
        }
    }
}
