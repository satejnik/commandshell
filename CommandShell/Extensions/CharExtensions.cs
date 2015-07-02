//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

namespace CommandShell.Extensions
{
    internal static class CharExtensions
    {
        public static bool IsWhiteSpace(this char c)
        {
            switch (c)
            {
                case '\f':
                case '\v':
                case ' ':
                case '\t':
                    return true;
                default:
                    return c > 127 && char.IsWhiteSpace(c);
            }
        }

        public static bool IsLineTerminator(this char c)
        {
            switch (c)
            {
                case '\xD':
                case '\xA':
                case '\x2028':
                case '\x2029':
                    return true;
                default:
                    return false;
            }
        }
    }
}
