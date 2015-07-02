//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.ComponentModel;
using CommandShell.Extensions;

namespace CommandShell.Helpers
{
    internal static class Converter
    {
        internal static object ConvertFromString(Type type, string value)
        {
            if (type.IsSimple()) return type.IsEnum ? Enum.Parse(type, value, true) : Convert.ChangeType(value, type);
            return TypeDescriptor.GetConverter(type).ConvertFrom(value);
        }

        internal static bool ConvertFromString(Type type, string value, out object obj)
        {
            try
            {
                obj = ConvertFromString(type, value);
                return true;
            }
            catch (Exception)
            {
                obj = null;
                return false;
            }
        }
    }
}
