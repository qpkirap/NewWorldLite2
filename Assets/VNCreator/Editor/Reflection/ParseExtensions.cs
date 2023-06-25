using System.Globalization;

namespace VNCreator
{
    public static class ParseExtensions
    {
        private static readonly NumberStyles style = NumberStyles.Float;
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

        public static bool TryIntParse(this string str, out int result)
        {
            return int.TryParse(str, out result);
        }

        public static int IntParse(this string str)
        {
            return !string.IsNullOrEmpty(str) && str.TryIntParse(out var result)
                ? result
                : 0;
        }

        /// <summary>
        /// Парсинг с учетом . или , в написании числа
        /// </summary>
        public static bool TryFloatParse(this string str, out float result)
        {
            return float.TryParse(str, out result) 
                   || float.TryParse(str, style, culture, out result)
                   || float.TryParse(str, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("fr-FR"), out result);
        }

        /// <summary>
        /// Парсинг с учетом . или , в написании числа
        /// </summary>
        public static float FloatParse(this string str)
        {
            return !string.IsNullOrEmpty(str) && str.TryFloatParse(out var result)
                ? result
                : 0;
        }
    }
}