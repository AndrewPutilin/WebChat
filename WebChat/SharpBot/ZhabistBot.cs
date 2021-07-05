using System;
using System.Collections.Generic;

namespace WebChat.SharpBot
{
    public static class ZhabistBot
    {
        private static List<string> KeyWords = new List<string>() {"привет",  "добрый день", "добрый вечер", "здравствуйте", "hello", "здравствуй" };

        public static string ZhabistStart( string inputText)
        {
            if (CheckKeyWords(inputText))
            {
                return CheckTimeForGreting();
            }

            else return null;
        }

        private static bool CheckKeyWords(string inputText)
        {
            foreach (var wordFromList in KeyWords)
            {
                if (inputText.ToLower().Contains(wordFromList.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        private static string CheckTimeForGreting()
        {
            var nowTime = DateTime.Now;

            if (nowTime.Hour >= 5 && nowTime.Hour < 12) return "Доброе утро, удачно вам провести этот день";

            else if (nowTime.Hour >= 12 && nowTime.Hour < 16) return "Добрый день, надеюсь вечером вы отдохнете";

            else if (nowTime.Hour >= 16 && nowTime.Hour <= 24) return "Добрый вечер, вы хорошо сегодня потрудились";

            else if (nowTime.Hour >= 0 && nowTime.Hour < 5) return "Доброй ночи, удачого вам завтрашенего дня";

            else return "";
        }
    }
}
