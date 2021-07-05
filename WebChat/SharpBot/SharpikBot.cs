using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebChat.SharpBot.Models.Model;

namespace WebChat.SharpBot
{
    public static class SharpikBot
    {
        delegate void CommandHandler(ref StringBuilder outText);

        private static readonly IAnswersRepository botAnswers;
        private static readonly IQuestionsRepository botQuestions;

        private static Dictionary<CategoryQestion, CommandHandler> ActionMap =>
         new Dictionary<CategoryQestion, CommandHandler>() {
                    { CategoryQestion.Name, SayName},
                    { CategoryQestion.Hello, SayGreetings},
                    { CategoryQestion.Time, SayTime},
                    { CategoryQestion.Joke, SayJokes},
                    { CategoryQestion.Bye, SayGodbyes},
            };

        static SharpikBot()
        {
            botAnswers = new AnswersRepository();
            botQuestions = new QuestionsRepository();
        }
        public static void SharpikStart(string commandText, StringBuilder botText)
        {
            bool checkAphor = true; // переменная для проверки, если не нашли никакие фразы вывести афоризмы

            CommandHandler commandHandler;
            commandHandler = StartPhrase;
            if (!String.IsNullOrEmpty(commandText))
            {
                foreach (CategoryQestion category in Enum.GetValues(typeof(CategoryQestion)))
                {
                    var qestions = botQuestions.GetQuestionsByCategory(category);

                    if (CheckWord(qestions, commandText))
                    {

                        commandHandler += ActionMap[category];
                        checkAphor = false;
                    }
                }

                if (checkAphor) // проверка на афоризмы
                    commandHandler += SayAphorisms;

                commandHandler(ref botText);
            }

        }

        public static void StartPhrase(ref StringBuilder outputText) { outputText.Append(" "); }

        /// <summary>
        /// Метод говорит время в данный момент
        /// </summary>
        private static void SayTime(ref StringBuilder outputText) { outputText.Append("Сейчас: " + DateTime.Now + "\n"); }

        /// <summary>
        /// Метод который говорит имя бота
        /// </summary>
        private static void SayName(ref StringBuilder outputText) { outputText.Append(botAnswers.GetRandomAnswer(AnswerCategory.Name) + "\n"); }

        /// <summary>
        /// Метод который выводит на экран рандомный анекдот
        /// </summary>
        private static void SayJokes(ref StringBuilder outputText) { outputText.Append(botAnswers.GetRandomAnswer(AnswerCategory.Joke) + "\n"); }

        /// <summary>
        /// Метод прощания с пользователем
        /// </summary>
        private static void SayGodbyes(ref StringBuilder outputText) { outputText.Append(botAnswers.GetRandomAnswer(AnswerCategory.Bye) + "\n"); }

        /// <summary>
        /// Метод который выводит на экран рандомный афоризм
        /// </summary>
        private static void SayAphorisms(ref StringBuilder outputText) { outputText.Append(botAnswers.GetRandomAnswer(AnswerCategory.Aphorism) + "\n"); }

        /// <summary>
        /// Метод приветствия пользователя
        /// </summary>
        private static void SayGreetings(ref StringBuilder outputText) { outputText.Append(botAnswers.GetRandomAnswer(AnswerCategory.Hello) + "\n"); }

        /// <summary>
        /// Метод проверки нужной команды в листе
        /// </summary>
        /// <param name="list">Проверяющийся список</param> 
        /// <param name="inputWord"> Фраза с консоли </param>
        /// <returns></returns>
        private static bool CheckWord(IList<string> list, string inputWord)
        {
            foreach (var wordFromList in list)
            {
                if (inputWord.ToLower().Contains(wordFromList.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
