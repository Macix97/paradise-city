namespace HierarchyExplorer
{

    public static class ExtensionMethods
    {
        public static string RemoveSpaces(this string s)
        {
            return s.Replace(" ", string.Empty);
        }
    }


    public static class SimpleRegex
    {
        private const char wildcard = '?';

        public static bool IsMatch(string s, string expression)
        {
            if (expression == null || s == null)
            {
                return false;
            }

            int it = 0;
            for (; it < expression.Length && it < s.Length; it++)
            {
                if (s[it] != expression[it] && expression[it] != wildcard)
                {
                    return false;
                }
            }
            // if we iterated through all the string characters, condition is met
            if (it == s.Length)
            {
                return true;
            }

            // if the expression is ended with wildcard characters, return true
            if (it != expression.Length)
            {
                //try to pass all wildcard characters
                for (int i = it; it < expression.Length; i++)
                {
                    if (expression[i] != wildcard)
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }
    }
}
