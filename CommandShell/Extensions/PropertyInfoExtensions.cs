//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using CommandShell.Helpers;
using Validator = CommandShell.Helpers.Validator;

namespace CommandShell.Extensions
{
    internal static class PropertyInfoExtensions
    {
        internal static bool WriteValue(this PropertyInfo property, object target, object value)
        {
            try
            {
                if (value is string)
                    value = Converter.ConvertFromString(property.PropertyType, (string)value);
                Validator.Validate(property.GetValidators(), value);
                property.SetValue(target, value, null);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static IEnumerable<ValidationAttribute> GetValidators(this PropertyInfo property)
        {
            return property == null ? new ValidationAttribute[] { } : property.GetCustomAttributes(typeof(ValidationAttribute), true).Cast<ValidationAttribute>();
        }
    }
}
