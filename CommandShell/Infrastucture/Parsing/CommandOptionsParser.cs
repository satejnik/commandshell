//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommandShell.Extensions;
using CommandShell.Helpers;

namespace CommandShell.Infrastucture.Parsing
{
    internal static class CommandOptionsParser
    {
        #region Methods

        internal static ParsingResult Parse(CommandOptionsMetadata metadata, object options, string[] args)
        {
            metadata.Reset();
            var errors = new List<ParsingError>();
            if (args != null && args.Any())
            {
                var argsEnumerator = new TwoWayEnumerator<string>(args);
                var valueIndex = 0;
                var valueList = new List<string>();
                while (argsEnumerator.MoveNext())
                {
                    var currentArg = argsEnumerator.Current;
                    if (IsShortOption(currentArg))
                    {
                        #region short option
                        var value = currentArg.Substring(1);
                        if (value.IsNullOrEmptyOrWhiteSpace())
                        {
                            errors.Add(new ParsingError(null, ParsingErrorBase.InvalidValue) { Value = currentArg });
                            continue;
                        }
                        var groupEnumerator = new TwoWayEnumerator<char>(value);
                        while (groupEnumerator.MoveNext())
                        {
                            var currentChar = groupEnumerator.Current;
                            var optionMetadata = metadata[currentChar];
                            if (optionMetadata == null)
                            {
                                errors.Add(new ParsingError(null, ParsingErrorBase.UnboundValue) { Value = currentChar.ToString(CultureInfo.InvariantCulture) });
                                continue;
                            }
                            if (optionMetadata.IsProvided)
                            {
                                errors.Add(new ParsingError(optionMetadata, ParsingErrorBase.RedundantOption));
                                continue;
                            }
                            optionMetadata.IsProvided = true;
                            if (optionMetadata.IsSwitchOption)
                            {
                                optionMetadata.Property.WriteValue(options, true);
                                continue;
                            }
                            if (!groupEnumerator.IsLast || argsEnumerator.IsLast || !argsEnumerator.MoveNext() || !IsValue(argsEnumerator.Current))
                            {
                                errors.Add(new ParsingError(optionMetadata, ParsingErrorBase.ValueMissed));
                                continue;
                            }
                            if (optionMetadata.IsOption && optionMetadata.IsListOption)
                            {
                                var argumentType = !optionMetadata.Type.IsArray ? optionMetadata.Type.GetGenericArguments().Single() : optionMetadata.Type.GetElementType();
                                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(argumentType));
                                var values = argsEnumerator.Current.Split(new[] { ((CommandOptionListOptionMetadata)optionMetadata).Separator },
                                                StringSplitOptions.RemoveEmptyEntries);
                                var proceed = true;
                                foreach (var splittedValue in values)
                                {
                                    object convertedValue;
                                    if (!Converter.ConvertFromString(argumentType, splittedValue, out convertedValue) && !Validator.IsValid(optionMetadata.Validators, convertedValue))
                                    {
                                        errors.Add(new ParsingError(optionMetadata, ParsingErrorBase.InvalidValue) { Value = argsEnumerator.Current });
                                        proceed = false;
                                        break;
                                    }
                                    list.Add(convertedValue);
                                }
                                if (!proceed) continue;
                                if (optionMetadata.Type.IsArray)
                                {
                                    var array = Array.CreateInstance(argumentType, list.Count);
                                    list.CopyTo(array, 0);
                                    optionMetadata.Property.WriteValue(options, array);
                                }
                                else optionMetadata.Property.WriteValue(options, list);
                                continue;
                            }
                            // simple option left
                            if (!optionMetadata.Property.WriteValue(options, argsEnumerator.Current))
                                errors.Add(new ParsingError(optionMetadata, ParsingErrorBase.InvalidValue) { Value = argsEnumerator.Current });
                        }
                        #endregion
                    }
                    else if (IsLongOption(currentArg)) // long option
                    {
                        #region long option
                        var value = currentArg.Substring(2);
                        if (value.IsNullOrEmptyOrWhiteSpace())
                        {
                            errors.Add(new ParsingError(null, ParsingErrorBase.InvalidValue) { Value = currentArg });
                            continue;
                        }
                        var optionMetadata = metadata[value];
                        if (optionMetadata == null)
                        {
                            errors.Add(new ParsingError(null, ParsingErrorBase.UnboundValue) { Value = value });
                            continue;
                        }
                        if (optionMetadata.IsProvided)
                        {
                            errors.Add(new ParsingError(optionMetadata, ParsingErrorBase.RedundantOption));
                            continue;
                        }
                        optionMetadata.IsProvided = true;
                        if (optionMetadata.IsSwitchOption)
                        {
                            optionMetadata.Property.WriteValue(options, true);
                            continue;
                        }
                        if (!argsEnumerator.MoveNext() || !IsValue(argsEnumerator.Current))
                        {
                            errors.Add(new ParsingError(optionMetadata, ParsingErrorBase.ValueMissed));
                            continue;
                        }
                        if (optionMetadata.IsOption && optionMetadata.IsListOption)
                        {
                            var argumentType = !optionMetadata.Type.IsArray ? optionMetadata.Type.GetGenericArguments().Single() : optionMetadata.Type.GetElementType();
                            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(argumentType));
                            var values = argsEnumerator.Current.Split(new[] { ((CommandOptionListOptionMetadata)optionMetadata).Separator },
                                            StringSplitOptions.RemoveEmptyEntries);
                            var proceed = true;
                            foreach (var splittedValue in values)
                            {
                                object convertedValue;
                                if (!Converter.ConvertFromString(argumentType, splittedValue, out convertedValue) && !Validator.IsValid(optionMetadata.Validators, convertedValue))
                                {
                                    errors.Add(new ParsingError(optionMetadata, ParsingErrorBase.InvalidValue) { Value = argsEnumerator.Current });
                                    proceed = false;
                                    break;
                                }
                                list.Add(convertedValue);
                            }
                            if (!proceed) continue;
                            if (optionMetadata.Type.IsArray)
                            {
                                var array = Array.CreateInstance(argumentType, list.Count);
                                list.CopyTo(array, 0);
                                optionMetadata.Property.WriteValue(options, array);
                            }
                            else optionMetadata.Property.WriteValue(options, list);
                            continue;
                        }
                        // simple option left
                        if (!optionMetadata.Property.WriteValue(options, argsEnumerator.Current))
                            errors.Add(new ParsingError(optionMetadata, ParsingErrorBase.InvalidValue) { Value = argsEnumerator.Current });
                        #endregion
                    }
                    else
                    {
                        #region values
                        var optionMetadata = metadata[valueIndex++];
                        if (optionMetadata != null)
                        {
                            optionMetadata.IsProvided = true;
                            if (!optionMetadata.Property.WriteValue(options, argsEnumerator.Current))
                                errors.Add(new ParsingError(optionMetadata, ParsingErrorBase.InvalidValue) { Value = argsEnumerator.Current });
                            continue;
                        }
                        valueList.Add(argsEnumerator.Current);
                        #endregion
                    }
                }
                if (valueList.Any() && metadata.ValueListOption == null)
                    errors.Add(new ParsingError(null, ParsingErrorBase.UnboundValue) { Value = string.Join(",", valueList) });
                else if (valueList.Any())
                {
                    var argumentType = !metadata.ValueListOption.Type.IsArray ? metadata.ValueListOption.Type.GetGenericArguments().Single() : metadata.ValueListOption.Type.GetElementType();
                    var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(argumentType));
                    var proceed = true;
                    foreach (var item in valueList)
                    {
                        object convertedValue;
                        if (!Converter.ConvertFromString(argumentType, item, out convertedValue) && !Validator.IsValid(metadata.ValueListOption.Validators, convertedValue))
                        {
                            errors.Add(new ParsingError(metadata.ValueListOption, ParsingErrorBase.InvalidValue) { Value = item });
                            proceed = false;
                            break;
                        }
                        list.Add(convertedValue);
                        metadata.ValueListOption.ElementsCount++;
                    }
                    if (proceed)
                    {
                        if (metadata.ValueListOption.Type.IsArray)
                        {
                            var array = Array.CreateInstance(argumentType, list.Count);
                            list.CopyTo(array, 0);
                            metadata.ValueListOption.Property.WriteValue(options, array);
                        }
                        else metadata.ValueListOption.Property.WriteValue(options, list);
                    }
                }
            }
            CheckRemainedOptions(metadata, options, errors);
            return new ParsingResult(errors);
        }

        internal static void CheckRemainedOptions(CommandOptionsMetadata metadata, object options, IList<ParsingError> errors)
        {
            if (metadata.ValueListOption != null && !metadata.ValueListOption.IsProvided) errors.Add(new ParsingError(metadata.ValueListOption, ParsingErrorBase.IndexOutOfRange));
            foreach (var option in metadata.ValueOptions.Where(option => !option.IsProvided))
                errors.Add(new ParsingError(option, ParsingErrorBase.ValueMissed));
            foreach (var option in metadata.RequiredOptions.Where(option => !option.IsListOption && !option.IsProvided && option.HasDefaultValue))
            {
                option.Property.WriteValue(options, option.DefaultValue);
                option.IsProvided = true;
            }
            foreach (var option in metadata.RequiredOptions.Where(option => !option.IsProvided && !option.Required))
                option.IsProvided = true;
            foreach (var option in metadata.RequiredOptions.Where(option => !option.IsProvided))
                errors.Add(new ParsingError(option, ParsingErrorBase.OptionMissed));
        }

        #endregion

        #region Helpers

        internal static bool IsShortOption(string value)
        {
            if (value.Length < 1) return false;
            if (value.Length < 2) return value[0] == '-';
            return value[0] == '-' && value[1] != '-';
        }

        internal static bool IsLongOption(string value)
        {
            if (value.Length < 2) return false;
            return value[0] == '-' && value[1] == '-';
        }

        internal static bool IsOption(string value)
        {
            return IsShortOption(value);
        }

        internal static bool IsValue(string value)
        {
            return !IsOption(value);
        }

        #endregion
    }
}
