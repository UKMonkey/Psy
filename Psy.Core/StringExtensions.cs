namespace Psy.Core
{
    public static class StringExtensions
    {
        /// <summary>
        /// http://stackoverflow.com/q/298830/
        /// </summary>
        /// <returns></returns>
        public static string[] ParseArguments(this string commandLine)
        {
            var parmChars = commandLine.ToCharArray(); 
            var inQuote = false; 
            for (var index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"')                 
                    inQuote = !inQuote; 
                if (!inQuote && parmChars[index] == ' ')                 
                    parmChars[index] = '\n';
            } 
            return (new string(parmChars)).Replace("\"", "").Split('\n');
        } 
    }
}