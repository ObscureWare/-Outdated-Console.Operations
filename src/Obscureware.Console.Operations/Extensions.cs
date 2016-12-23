namespace Obscureware.Console.Operations
{
    using System;
    using System.Globalization;

    public static class Extensions
    {

        // http://stackoverflow.com/questions/837155/fastest-function-to-generate-excel-column-letters-in-c-sharp

        public static string ToAlphaEnum(this uint @value)
        {
            string columnString = string.Empty;
            decimal columnNumber = @value;
            while (columnNumber > 0)
            {
                decimal currentLetterNumber = (columnNumber - 1) % 26;
                char currentLetter = (char)(currentLetterNumber + 65);
                columnString = currentLetter + columnString;
                columnNumber = (columnNumber - (currentLetterNumber + 1)) / 26;
            }

            return columnString;
        }

        public static int FromAlphaEnum(this string @value)
        {
            int retVal = 0;
            string col = @value.ToUpper();
            for (int iChar = col.Length - 1; iChar >= 0; iChar--)
            {
                char colPiece = col[iChar];
                int colNum = colPiece - 64;
                retVal = retVal + colNum * (int)Math.Pow(26, col.Length - (iChar + 1));
            }
            return retVal;
        }

        private static readonly string[] Sufixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        /// <summary>
        /// Converts number of bytes into user friendly format of KB, MB, GB etc.
        /// </summary>
        /// <param name="byteCount">Count of bytes</param>
        /// <param name="culture">Optional culture for number formatting</param>
        /// <returns></returns>
        public static string ToFriendlyXBytesText(this long byteCount, CultureInfo culture = null)
        {
            // http://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net

            if (byteCount == 0)
            {
                return $"{0} {Sufixes[0]}";
            }

            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));

            if (place >= Sufixes.Length)
            {
                throw new ArgumentException($"Unexpectedly large number, objects larger than 1023{Sufixes[Sufixes.Length - 1]} are not expected to exists in the entire universe!",
                    nameof(byteCount));
            }
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);

            return $"{(Math.Sign(byteCount) * num).ToString(culture ?? CultureInfo.InvariantCulture),1} {Sufixes[place]}";
        }
    }
}