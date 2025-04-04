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

        public static void SkipCharactersUntil(this string text, ref int read_i, in bool left_to_right, in bool positive, params char[] key_chars)
        {
            static void Increment(ref int read_i, in bool left_to_right)
            {
                if (left_to_right)
                    ++read_i;
                else
                    --read_i;
            }

            if (!left_to_right)
                --read_i;

            HashSet<char> charSet = new(key_chars);

            while (read_i >= 0 && read_i < text.Length)
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
                        Increment(ref read_i, left_to_right);
                        SkipCharactersUntil(text, ref read_i, true, true, c);
                        break;

                    case '\\':
                        Increment(ref read_i, left_to_right);
                        break;
                }
                Increment(ref read_i, left_to_right);
            }
            read_i = Mathf.Clamp(read_i, 0, text.Length);
        }

        public static bool TryReadPipe(this string text, ref int read_i)
        {
            SkipCharactersUntil(text, ref read_i, true, true, CHAR_CHAIN, CHAR_PIPE);
            if (read_i < text.Length && text[read_i] == CHAR_PIPE)
            {
                ++read_i;
                return true;
            }
            else
                return false;
        }

        public static bool TryReadChain(this string text, ref int read_i)
        {
            SkipCharactersUntil(text, ref read_i, true, true, CHAR_CHAIN, CHAR_PIPE);
            if (read_i + 1 < text.Length && text[read_i] == CHAR_CHAIN && text[read_i + 1] == CHAR_CHAIN)
            {
                ++read_i;
                ++read_i;
                return true;
            }
            else
                return false;
        }
    }
}