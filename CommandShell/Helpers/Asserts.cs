//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using CommandShell.Extensions;

namespace CommandShell.Helpers
{
    internal static class Asserts
    {
        public static void ReferenceNotNull<T>(T value, string message)
            where T : class
        {
            if (value == null) throw new NullReferenceException(message);
        }

        public static void ArgumentNotNull<T>(T value, string paramName, string message)
            where T : class
        {
            if (value == null) throw new ArgumentNullException(paramName, message.FormatFor(paramName));
        }

        public static void ArgumentNotEmptyOrWhitespace(char value, string paramName, string message)
        {
            if (value.IsWhiteSpace() || value.IsLineTerminator()) throw new ArgumentException(message.FormatFor(paramName), paramName);
        }

        public static void ArgumentNotNullOrEmptyOrWhitespace(string value, string paramName, string message)
        {
            if (value.IsNullOrEmptyOrWhiteSpace()) throw new ArgumentException(message.FormatFor(paramName), paramName);
        }

        public static void ArgumentNotAllowed(bool flag, string paramName, string message)
        {
            if (flag) throw new ArgumentException(message.FormatFor(paramName), paramName);
        }

        public static void OperationNotAllowed(bool flag, string message)
        {
            if (flag) throw new InvalidOperationException(message);
        }

        #region Defaults

        public static void ArgumentNotNull<T>(T value, string paramName)
            where T : class
        {
            ArgumentNotNull(value, paramName, "{0} cannot be null.".FormatFor(paramName));
        }

        public static void ArgumentNotEmptyOrWhitespace(char value, string paramName)
        {
            ArgumentNotEmptyOrWhitespace(value, paramName, "{0}  cannot be empty or whitespace.".FormatFor(paramName));
        }

        public static void ArgumentNotNullOrEmptyOrWhitespace(string value, string paramName)
        {
            ArgumentNotNullOrEmptyOrWhitespace(value, paramName, "{0}  cannot be empty or whitespace.".FormatFor(paramName));
        }

        #endregion
    }
}
