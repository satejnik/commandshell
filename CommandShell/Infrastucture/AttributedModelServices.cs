//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using CommandShell.Extensions;
using CommandShell.Helpers;
using CommandShell.Infrastucture.Parsing;
using CommandShell.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CommandShell.Infrastucture
{
    public static class AttributedModelServices
    {
        #region Methods

        public static IEnumerable<CommandMetadata> GetMetadataFromTypeAssembly(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            return assembly.GetTypes().Where(type => Attribute.IsDefined(type, typeof(ShellCommandAttribute), false) && !Attribute.IsDefined(type, typeof(IgnoreCommandAttribute), false) && !type.IsAbstract).Select(GetMetadataFromType);
        }

        public static CommandMetadata GetMetadata(object command)
        {
            if(command == null) throw new ArgumentNullException("command");
            return GetMetadataFromType(command.GetType());
        }

        public static CommandMetadata GetMetadataFromType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            AssertCommandType(type);
            var attribute = (ShellCommandAttribute)type.GetCustomAttributes(typeof(ShellCommandAttribute), false).SingleOrDefault();
            if (attribute.Options != null) AssertOptionsType(attribute.Options);
            var helpmethod = GetHelpMethod(type);
            var runmethod = GetRunnableMethod(type, attribute.Options);
            return new CommandMetadata(type, attribute.Name.ToLower())
            {
                Description = attribute.Description,
                Options = GetOptionsMetadata(attribute.Options),
                HelpMethod = helpmethod,
                RunnableMethod = runmethod
            };
        }

        #endregion

        #region Helpers

        internal static void AssertCommandType(Type type)
        {
            if (!Attribute.IsDefined(type, typeof(ShellCommandAttribute), false)) throw new TypeLoadException(string.Format("{0} does not provide ShellCommandAttribute attribute.", type));
            if (type.GetConstructor(Type.EmptyTypes) == null) throw new TypeLoadException(string.Format("{0} does not provide parameterless constructor.", type));
        }

        internal static void AssertOptionsType(Type type)
        {
            if (type.GetConstructor(Type.EmptyTypes) == null) throw new TypeLoadException(string.Format("{0} does not provide parameterless constructor.", type));
        }

        private static CommandOptionsMetadata GetOptionsMetadata(Type optionsType)
        {
            if (optionsType == null) return null;
            var optionsMetadata = new CommandOptionsMetadata(optionsType);
            foreach (var property in optionsType.GetProperties())
            {
                var attributes = property.GetCustomAttributes(typeof(OptionBaseAttribute), true).Cast<OptionBaseAttribute>().ToArray();
                Asserts.OperationNotAllowed(attributes.Length > 1, "More than one option attribute was declared on property {0}.".FormatFor(property.Name));
                var attribute = attributes.SingleOrDefault();
                if (attribute == null) continue;
                optionsMetadata.Add(GetOptionMetadata(property, attribute));
            }
            return optionsMetadata;
        }

        private static CommandOptionMetadataBase GetOptionMetadata(PropertyInfo propertyInfo, OptionBaseAttribute attribute)
        {
            var setter = propertyInfo.GetSetMethod();
            if (setter == null) throw new MissingMemberException("{0} property doesn't have public setter.".FormatFor(propertyInfo.Name));
            if (!setter.IsPublic || setter.IsAbstract) throw new MemberAccessException("{0} property should be public and not abstract.".FormatFor(propertyInfo.Name));
            CommandOptionMetadataBase optionMetadata = null;
            if (attribute is ValueAttribute)
            {
                Asserts.OperationNotAllowed(!propertyInfo.PropertyType.CanConvertFromString(), "{0} property decorated with {1} attribute must be of primitive type, decimal, DateTime or enum. Otherwise make sure custom type is decorated with TypeConverterAttribute attribute with a provided conversion from string.".FormatFor(propertyInfo.Name, attribute.GetType().Name));
                optionMetadata = new CommandValueOptionMetadata(propertyInfo) { Index = ((ValueAttribute)attribute).Index };
            }
            if (attribute is ValueListAttribute)
            {
                if (!propertyInfo.PropertyType.IsArray)
                {
                    var arguments = propertyInfo.PropertyType.GetGenericArguments();
                    var genericType = typeof(List<>);
                    Asserts.OperationNotAllowed(arguments.Length != 1, "{0} property decorated with {1} attribute must be an array or a colection assignable from {2}.".FormatFor(propertyInfo.Name, attribute.GetType().Name, genericType.Name));
                    var argumentType = arguments.Single();
                    Asserts.OperationNotAllowed(!argumentType.CanConvertFromString(), "{0} property decorated with {1} must be an array or a collection of primitive types, decimal, DateTime or enum. Otherwise make sure custom type is decorated with TypeConverterAttribute attribute with a provided conversion from string.".FormatFor(propertyInfo.Name, attribute.GetType().Name));
                    Asserts.OperationNotAllowed(!propertyInfo.PropertyType.IsAssignableFrom(genericType.MakeGenericType(argumentType)), "{0} property decorated with {1} attribute must be an array or a colection assignable from {2}.".FormatFor(propertyInfo.Name, attribute.GetType().Name, genericType.Name));
                }
                else
                    Asserts.OperationNotAllowed(!propertyInfo.PropertyType.GetElementType().CanConvertFromString(), "{0} property decorated with {1} must be an array or a collection of primitive types, decimal, DateTime or enum. Otherwise make sure custom type is decorated with TypeConverterAttribute attribute with a provided conversion from string.".FormatFor(propertyInfo.Name, attribute.GetType().Name));
                optionMetadata = new CommandValueListOptionMetadata(propertyInfo)
                {
                    MinElements = ((ValueListAttribute)attribute).MinimumElements,
                    MaxElements = ((ValueListAttribute)attribute).MaximumElements
                };
            }
            if (attribute is SwitchAttribute)
            {
                Asserts.OperationNotAllowed(propertyInfo.PropertyType != typeof(bool), "{0} property decorated with {1} attribute muste be of boolean type.".FormatFor(propertyInfo.Name, attribute.GetType().Name));
                optionMetadata = new CommandNamedOptionMetadata(propertyInfo)
                {
                    ShortName = ((SwitchAttribute)attribute).ShortName,
                    LongName = ((SwitchAttribute)attribute).LongName
                };
            }
            if (attribute is OptionListAttribute)
            {
                if (!propertyInfo.PropertyType.IsArray)
                {
                    var arguments = propertyInfo.PropertyType.GetGenericArguments();
                    var genericType = typeof(List<>);
                    Asserts.OperationNotAllowed(arguments.Length != 1, "{0} property decorated with {1} attribute must be an array or a colection assignable from {2}.".FormatFor(propertyInfo.Name, attribute.GetType().Name, genericType.Name));
                    var argumentType = arguments.Single();
                    Asserts.OperationNotAllowed(!argumentType.CanConvertFromString(), "{0} property decorated with {1} must be an array or a collection of primitive types, decimal, DateTime or enum. Otherwise make sure custom type is decorated with TypeConverterAttribute attribute with a provided conversion from string.".FormatFor(propertyInfo.Name, attribute.GetType().Name));
                    Asserts.OperationNotAllowed(!propertyInfo.PropertyType.IsAssignableFrom(genericType.MakeGenericType(argumentType)), "{0} property decorated with {1} attribute must be an array or a colection assignable from {2}.".FormatFor(propertyInfo.Name, attribute.GetType().Name, genericType.Name));
                }
                else
                    Asserts.OperationNotAllowed(!propertyInfo.PropertyType.GetElementType().CanConvertFromString(), "{0} property decorated with {1} must be an array or a collection of primitive types, decimal, DateTime or enum. Otherwise make sure custom type is decorated with TypeConverterAttribute attribute with a provided conversion from string.".FormatFor(propertyInfo.Name, attribute.GetType().Name));
                optionMetadata = new CommandOptionListOptionMetadata(propertyInfo)
                {
                    ShortName = ((OptionListAttribute)attribute).ShortName,
                    LongName = ((OptionListAttribute)attribute).LongName,
                    Required = ((OptionListAttribute)attribute).Required,
                    Separator = ((OptionListAttribute)attribute).Separator
                };
            }
            if (attribute is OptionAttribute)
            {
                Asserts.OperationNotAllowed(!propertyInfo.PropertyType.CanConvertFromString(), "{0} property decorated with {1} attribute must be of primitive type, decimal, DateTime or enum. Otherwise make sure custom type is decorated with TypeConverterAttribute attribute with a provided conversion from string.".FormatFor(propertyInfo.Name, attribute.GetType().Name));
                var defaultValue = ((OptionAttribute)attribute).DefaultValue;
                Asserts.ArgumentNotAllowed(defaultValue != null && !(defaultValue.GetType() == propertyInfo.PropertyType || TypeDescriptor.GetConverter(defaultValue.GetType()).CanConvertTo(propertyInfo.PropertyType)), "defaultValue", "Default value of a property {0} decorated with {1} attribute must be of the same type.".FormatFor(propertyInfo.Name, attribute.GetType().Name));
                optionMetadata = new CommandRequiredOptionMetadata(propertyInfo)
                {
                    ShortName = ((OptionAttribute)attribute).ShortName,
                    LongName = ((OptionAttribute)attribute).LongName,
                    Required = ((OptionAttribute)attribute).Required,
                    DefaultValue = ((OptionAttribute)attribute).DefaultValue
                };
            }
            optionMetadata.Description = attribute.Description;
            optionMetadata.MetaValue = attribute.MetaValue;
            return optionMetadata;
        }

        internal static MethodInfo GetRunnableMethod(Type type, Type optionsType)
        {
            var methods = type.GetMethods().Where(info => Attribute.IsDefined(info, typeof(RunCommandAttribute), false)).ToArray();
            Asserts.OperationNotAllowed(methods.Length > 1, "More than one method are decorated with RunCommandAttribute.");
            if (methods.Length == 0)
                methods = type.GetMethods().Where(info => info.Name == "Run").ToArray();
            methods = methods.Where(info => (info.ReturnType == typeof(int) || info.ReturnType == typeof(void))).ToArray();
            if (optionsType != null)
                methods = type == optionsType ? methods.Where(info => info.GetParameters().Length == 0).ToArray() : methods.Where(info => info.GetParameters().Length == 1 && info.GetParameters().First().ParameterType == optionsType).ToArray();
            Asserts.OperationNotAllowed(methods.Length > 1, "More than one method to invoke found. Command method must be decorated with RunCommandAttribute or named Run. The method must have int or void as return type and have either void or only one appropriate options parameter type.");
            var method = methods.FirstOrDefault();
            if (method == null) throw new MissingMethodException("Target object does not contain any methods to invoke. Command method must be decorated with RunCommandAttribute or named Run. The method must have int or void as return type and have either void or only one appropriate options parameter type.");
            if (!method.IsPublic || method.IsConstructor || method.IsStatic) throw new MethodAccessException("Target object does not contain appropriate method to invoke. Command method must be decorated with RunCommandAttribute or named Run. The method must have int or void as return type and have either void or only one appropriate options parameter type.");
            return method;
        }

        internal static MethodInfo GetHelpMethod(Type type)
        {
            var methods = type.GetMethods().Where(info => Attribute.IsDefined(info, typeof(GetHelpAttribute), false)).ToArray();
            Asserts.OperationNotAllowed(methods.Length > 1, "More than one method are decorated with GetHelpAttribute.");
            if (methods.Length != 1) return null;
            var method = methods.First();
            if (!method.IsPublic || method.IsConstructor || method.IsStatic) throw new MethodAccessException("Method decorated with GetHelpAttribute must be a public instance method.");
            Asserts.OperationNotAllowed(method.ReturnType != typeof(string), "Method decorated with GetHelpAttribute must have a string as return type.");
            Asserts.OperationNotAllowed(method.GetParameters().Any(param => param.ParameterType != typeof(CommandMetadata) && param.ParameterType != typeof(ParsingResult)), "Method decorated with GetHelpAttribute can have up to two parameters of type CommandMetadata or ParsingResult.");
            Asserts.OperationNotAllowed(method.GetParameters().Count(param => param.ParameterType == typeof(CommandMetadata)) > 1, "Method decorated with GetHelpAttribute can have up to two parameters of type CommandMetadata or ParsingResult.");
            Asserts.OperationNotAllowed(method.GetParameters().Count(param => param.ParameterType == typeof(ParsingResult)) > 1, "Method decorated with GetHelpAttribute can have up to two parameters of type CommandMetadata or ParsingResult.");
            return method;
        }

        internal static bool CheckMethodSignature(MethodInfo value, object options)
        {
            return (value.ReturnType == typeof(int) || value.ReturnType == typeof(void)) && options != null ? value.GetParameters().Length == 1 && value.GetParameters().First().ParameterType == options.GetType() : value.GetParameters().Length == 0;
        }

        #endregion
    }
}
