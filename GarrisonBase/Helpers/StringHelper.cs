using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herbfunk.GarrisonBase.Helpers
{
    public static class StringHelper
    {
        public static bool TextIsAllNumerical(string text)
        {
            return text.ToCharArray().All(Char.IsNumber);
        }

        public static string ExtractNumbers(string text)
        {
            return text.ToCharArray().Where(Char.IsNumber).Aggregate(string.Empty, (current, c) => current + c);
        }

        private static readonly Random Rand = new Random();
        public static string RandomString
        {
            get
            {
                int size = Rand.Next(6, 15);
                var sb = new StringBuilder(size);
                for (int i = 0; i < size; i++)
                {
                    // random upper/lowercase character using ascii code
                    sb.Append((char)(Rand.Next(2) == 1 ? Rand.Next(65, 91) + 32 : Rand.Next(65, 91)));
                }
                return sb.ToString();
            }
        }
    }
}
