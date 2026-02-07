using System.ComponentModel;

namespace Flashcards.m1chael888.Enums
{
    public static class EnumExtension
    {
        public static string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            return attributes[0].Description;
        }
    }
}
