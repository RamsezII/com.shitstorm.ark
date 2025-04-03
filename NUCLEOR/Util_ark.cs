using UnityEngine;

namespace _ARK_
{
    public static class Util_ark
    {
        public const char
            CHAR_PIPE = '|',
            CHAR_BACKPIPE = '!',
            CHAR_TAB = '\t',
            CHAR_RETURN = '\r',
            CHAR_NEWLINE = '\n',
            CHAR_SPACE = ' ',
            CHAR_BACKSLASH = '\\';

        //--------------------------------------------------------------------------------------------------------------

        public static char GetRotator(in float speed = 10) => ((int)(Time.unscaledTime * speed) % 4) switch
        {
            0 => '|',
            1 => '/',
            2 => '-',
            3 => '\\',
            _ => '?',
        };

        public static void SkipCharactersUntil(this string text, ref int read_i, in bool positive)
        {
            while (read_i < text.Length)
            {
                char c = text[read_i];
                switch (c)
                {
                    case '|':
                        return;

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

        public static bool TryReadPipe(this string text, ref int read_i)
        {
            SkipCharactersUntil(text, ref read_i, true);
            if (read_i < text.Length && text[read_i] == CHAR_PIPE)
            {
                ++read_i;
                return true;
            }
            else
                return false;
        }
    }
}