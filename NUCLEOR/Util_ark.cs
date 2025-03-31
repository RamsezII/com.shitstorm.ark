namespace _ARK_
{
    public static class Util_ark
    {
        public static bool TryReadArgument(this string text, ref int start_i, ref int read_i, out string argument)
        {
            SkipCharactersUntil(text, ref read_i, true);
            start_i = read_i;
            SkipCharactersUntil(text, ref read_i, false);

            if (start_i < read_i)
            {
                argument = text[start_i..read_i];
                return true;
            }

            argument = string.Empty;
            return false;
        }

        public static void SkipCharactersUntil(this string text, ref int read_i, in bool positive)
        {
            while (read_i < text.Length)
            {
                char c = text[read_i];
                switch (c)
                {
                    case '\t':
                    case '\r':
                    case '\n':
                        break;

                    case '\\':
                        ++read_i;
                        break;

                    default:
                        if (positive && c != ' ')
                            return;
                        if (!positive && c == ' ')
                            return;
                        break;
                }
                ++read_i;
            }
            if (read_i > text.Length)
                read_i = text.Length;
        }
    }
}