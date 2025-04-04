using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public static class Util_ark
    {
        public const char
            CHAR_SPACE = ' ',
            CHAR_CHAIN = '&',
            CHAR_PIPE = '|',
            CHAR_BACKPIPE = '!';

        //--------------------------------------------------------------------------------------------------------------

        public static char GetRotator(in float speed = 10) => ((int)(Time.unscaledTime * speed) % 4) switch
        {
            0 => '|',
            1 => '/',
            2 => '-',
            3 => '\\',
            _ => '?',
        };

        public static void SkipCharactersUntil(this string text, ref int read_i, in bool positive, params char[] key_chars)
        {
            HashSet<char> charSet = new(key_chars);

            while (read_i < text.Length && read_i >= 0)
            {
                char c = text[read_i];

                if (positive == charSet.Contains(c))
                    return;

                switch (c)
                {
                    case '\t':
                    case '\r':
                    case '\n':
                        break;

                    case '"':
                    case '\'':
                        ++read_i;
                        SkipCharactersUntil(text, ref read_i, true, c);
                        break;

                    case '\\':
                        ++read_i;
                        break;
                }
                ++read_i;
            }
            read_i = Mathf.Clamp(read_i, 0, text.Length);
        }

        public static bool TryReadPipe(this string text, ref int read_i)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            SkipCharactersUntil(text, ref read_i, true, CHAR_CHAIN, CHAR_PIPE);
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