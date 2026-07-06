using System;

namespace FlashcardsApp.Utilities
{
    public static class StringExtensions
    {
        /// <summary>
        /// Pads both side equally to align center
        /// </summary>
        public static string PadCenter(this string str, int totalLength, char paddingChar = ' ')
        {
            if(string.IsNullOrEmpty(str) || str.Length >= totalLength)
            {
                return str;
            }
            
            int sideLength = (totalLength - str.Length) / 2 + str.Length;
            return str.PadLeft(sideLength, paddingChar).PadRight(totalLength, paddingChar);
        }
    }
}
