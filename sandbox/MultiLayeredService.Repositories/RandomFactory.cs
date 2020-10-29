using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLayeredService.Repositories
{
    public static class RandomFactory
    {
        public static readonly Random rnd = new Random();
        private const string lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur maximus congue nulla in elementum. Nunc finibus vehicula arcu, eget iaculis ligula vestibulum vel. Aliquam interdum ligula libero. Morbi placerat tellus lorem, sit amet iaculis felis tincidunt quis. Nam vel nibh et risus semper laoreet. Maecenas ligula lorem, dictum vitae convallis nec, venenatis sed urna. Etiam at porta diam, sed interdum arcu. Nulla rutrum arcu eget quam pellentesque, at luctus sem posuere. In lectus erat, lacinia sit amet leo vitae, accumsan pharetra diam. Nulla vitae ultricies sapien. Sed malesuada malesuada augue a efficitur.";
        public static string GetString(int length)
        {
            return lorem.Substring(rnd.Next(lorem.Length - length - 1), length);
        }
        public static string GetString(int minLength, int maxLength)
        {
            var length = GetInt32(min: minLength, max: maxLength);
            return lorem.Substring(rnd.Next(lorem.Length - length - 1), length);
        }
        public static int GetInt32(int max = 100, int min = 0)
        {
            return rnd.Next(min, max);
        }

        public static double GetDouble(double min = 0, double max = 1, int decimalCharacterCount = 2)
        {
            var multiplier = Math.Pow(10, decimalCharacterCount);

            var randomed = GetInt32((int)(max * multiplier), (int)(min * multiplier));
            return randomed / multiplier;
        }

        public static string GetEmail()
        {
            return $"{GetString(rnd.Next(5, 15)).Replace(" ", string.Empty)}@{GetChar()}mail.com";
        }
        public static char GetChar()
        {
            return Convert.ToChar(rnd.Next(97, 122));
        }
        public static T GetEnum<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            var target = rnd.Next(values.Length - 1);
            int current = 0;
            foreach (var item in values)
            {
                if (target == current)
                    return (T)item;
                current++;
            }
            return default;
        }

        public static bool GetBool(int successRate = 50)
        {
            return rnd.Next(100) < successRate;
        }

        public static DateTime GetDateTime(DateTime min, DateTime max)
        {
            var diff = max - min;

            var generatedValue = GetInt32(min: 0, max: (int)diff.TotalSeconds);

            return DateTime.Now.AddSeconds(-generatedValue);
        }

        public static DateTime GetDateTime(int pastYearsOffset = 10)
            => GetDateTime(DateTime.UtcNow.AddYears(-GetInt32(min: 0, max: pastYearsOffset)), DateTime.UtcNow);

        public static string GetImageUrl()
        {
            return "https://source.unsplash.com/random";
        }

        /// <summary>
        /// Repeats given data by lambda.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="predicate">Data generation factory.</param>
        /// <param name="count">Times to repeat. -1 means random between 0 - 10.</param>
        /// <returns></returns>
        public static IEnumerable<T> Repeat<T>(Func<T> predicate, int count = -1)
        {
            if (count == -1)
                count = rnd.Next(0, 10);

            for (int i = 0; i < count; i++)
            {
                yield return predicate();
            }
        }
    }
}
