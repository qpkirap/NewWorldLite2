using System;
using System.Linq;
using UnityEngine;

namespace VNCreator
{
    public static class StringExtensions
    {
        public static string ToUpperFirst(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var a = value.ToLower().ToCharArray();
            a[0] = char.ToUpper(a[0]);

            return new(a);
        }

        public static Color ToColor(this string color)
        {
            if (color.StartsWith("#", StringComparison.InvariantCulture))
            {
                color = color.Substring(1);
            }

            if (color.Length == 6)
            {
                color += "FF";
            }

            var hex = Convert.ToUInt32(color, 16);
            var r = ((hex & 0xff000000) >> 0x18) / 255f;
            var g = ((hex & 0xff0000) >> 0x10) / 255f;
            var b = ((hex & 0xff00) >> 8) / 255f;
            var a = (hex & 0xff) / 255f;

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Разделение текста на несколько строк по словам.
        /// </summary>
        /// <param name="text">Исходный текст</param>
        /// <param name="max">Максимум символов в одной строке</param>
        /// <returns></returns>
        public static string Wrap(this string text, int max)
        {
            var charCount = 0;

            var lines = text
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(word => CharCount(word, max, ref charCount))
                .Select(group => string.Join(" ", group.ToArray()))
                .ToArray();

            return string.Join("\n", lines);
        }

        private static int CharCount(string word, int max, ref int charCount)
        {
            return (charCount += (((charCount % max) + word.Length + 1 >= max) ? max - (charCount % max) : 0) + word.Length + 1) / max;
        }
    }
}