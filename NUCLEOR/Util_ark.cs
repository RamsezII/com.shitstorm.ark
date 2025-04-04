using System.Collections.Generic;
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

        [System.Obsolete]
        public static void SkipCharactersUntil(this string text, ref int read_i, in bool positive, in int increment = 1)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            while (read_i < text.Length && read_i >= 0)
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
                        read_i += increment;
                        break;

                    default:
                        if (positive && c != ' ')
                            return;
                        if (!positive && c == ' ')
                            return;
                        break;
                }
                read_i += increment;
            }
            read_i = Mathf.Clamp(read_i, 0, text.Length);
        }

        public static void SkipCharactersUntil(this string text, ref int read_i, in bool positive, in string chars, in bool left_to_right)
        {
            HashSet<char> charSet = new(chars);
            if (!left_to_right)
                --read_i;
            while (read_i < text.Length && read_i >= 0)
            {
                char c = text[read_i];
                switch (c)
                {
                    case '\t':
                    case '\r':
                    case '\n':
                        break;

                    case '\\':
                        if (left_to_right)
                            ++read_i;
                        else
                            --read_i;
                        break;

                    default:
                        if (positive == charSet.Contains(c))
                            return;
                        break;
                }
                if (left_to_right)
                    ++read_i;
                else
                    --read_i;
            }
            read_i = Mathf.Clamp(read_i, 0, text.Length);
        }

        public static bool TryReadPipe(this string text, ref int read_i)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

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