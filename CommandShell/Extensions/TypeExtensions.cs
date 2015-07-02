//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace CommandShell.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool IsBoolean(this Type type)
        {
            return type == typeof(bool);
        }

        internal static bool IsCollection(this Type type)
        {
            return type.GetInterface("IEnumerable") != null;
        }

        internal static bool IsSimple(this Type type)
        {
            return type.IsPrimitive || type == typeof(DateTime) || type.IsEnum || type == typeof(decimal);
        }

        internal static bool CanConvertFromString(this Type type)
        {
            if (IsSimple(type)) return true;
            //if (IsNullable(type) && IsSimple(type.GetGenericArguments().First())) return true;
            var converter = TypeDescriptor.GetConverter(type);
            return converter.CanConvertFrom(typeof(string));
        }

        internal static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
