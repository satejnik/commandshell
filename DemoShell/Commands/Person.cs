//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;

namespace DemoShell.Commands
{
    [TypeConverter(typeof(PersonConverter))]
    public class Person
    {
        public string Name { get; set; }

        public static implicit operator Person(string name)
        {
            return new Person { Name = name };
        }

        public static implicit operator string(Person person)
        {
            return person.Name;
        }
    }

    public class PersonConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (Person)(string)value;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string)) throw new NotSupportedException(string.Format("Converting Person type to {0} is not supported.", destinationType));
            return (string)(Person)value;
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return value is string;
        }
    }
}
