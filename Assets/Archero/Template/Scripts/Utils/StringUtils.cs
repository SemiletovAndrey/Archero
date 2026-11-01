namespace Template.Scripts.Utils
{
    public static class StringUtils
    {
        public static string TrimWithDots(this string text, int length)
        {
            if (text.Length <= length)
            {
                return text;
            }
            
            return text[..length] + "...";
        }
    }
}