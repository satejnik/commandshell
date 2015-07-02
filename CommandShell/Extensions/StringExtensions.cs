//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

namespace CommandShell.Extensions
{
    internal static class StringExtensions
    {
        public static bool IsNullOrEmptyOrWhiteSpace(this string source)
        {
            return string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(source);
        }

        public static string FormatFor(this string source, params object[] args)
        {
            return string.Format(source, args);
        }
    }
}
