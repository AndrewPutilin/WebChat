using System;

namespace WebChat.SharpBot.Untils
{
    public static class Randomizer
    {
        public static int GetRandomInt(int maxValue)
        {
            Random rnd = new Random();
            return rnd.Next(maxValue);
        }
    }
}
