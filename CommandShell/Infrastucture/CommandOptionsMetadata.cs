//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommandShell.Extensions;
using CommandShell.Helpers;

namespace CommandShell.Infrastucture
{
    public sealed class CommandOptionsMetadata : IEnumerable<CommandOptionMetadataBase>
    {
        #region Fields

        private readonly HashSet<CommandOptionMetadataBase> metadata;

        #endregion

        #region Constructors

        internal CommandOptionsMetadata(Type type)
        {
            Asserts.ArgumentNotNull(type, "type");
            Type = type;
            metadata = new HashSet<CommandOptionMetadataBase>();
        }

        #endregion

        #region Properties

        public Type Type { get; private set; }

        public ReadOnlyCollection<CommandValueOptionMetadata> ValueOptions
        {
            get { return new ReadOnlyCollection<CommandValueOptionMetadata>(metadata.Where(meta => meta.IsValueOption && !meta.IsListOption).Cast<CommandValueOptionMetadata>().ToList()); }
        }

        public CommandValueListOptionMetadata ValueListOption
        {
            get { return (CommandValueListOptionMetadata)metadata.SingleOrDefault(meta => meta.IsListOption && meta.IsValueOption); }
        }

        public ReadOnlyCollection<CommandNamedOptionMetadata> NamedOptions
        {
            get { return new ReadOnlyCollection<CommandNamedOptionMetadata>(metadata.Where(meta => meta.IsNamedOption).Cast<CommandNamedOptionMetadata>().ToList()); }
        }

        public ReadOnlyCollection<CommandNamedOptionMetadata> Switches
        {
            get { return new ReadOnlyCollection<CommandNamedOptionMetadata>(metadata.Where(meta => meta.IsSwitchOption).Cast<CommandNamedOptionMetadata>().ToList()); }
        }

        public ReadOnlyCollection<CommandRequiredOptionMetadata> RequiredOptions
        {
            get { return new ReadOnlyCollection<CommandRequiredOptionMetadata>(metadata.Where(meta => meta.IsRequiredOption).Cast<CommandRequiredOptionMetadata>().ToList()); }
        }

        public ReadOnlyCollection<CommandOptionListOptionMetadata> OptionListOptions
        {
            get { return new ReadOnlyCollection<CommandOptionListOptionMetadata>(metadata.Where(meta => meta.IsListOption && meta.IsOption).Cast<CommandOptionListOptionMetadata>().ToList()); }
        }

        public ReadOnlyCollection<CommandRequiredOptionMetadata> Options
        {
            get { return new ReadOnlyCollection<CommandRequiredOptionMetadata>(metadata.Where(meta => meta.IsOption && !meta.IsListOption).Cast<CommandRequiredOptionMetadata>().ToList()); }
        }

        public int Count
        {
            get { return metadata.Count; }
        }

        #endregion

        #region Methods

        internal void Add(CommandOptionMetadataBase item)
        {
            if (item is CommandNamedOptionMetadata)
            {
                var namedItem = item as CommandNamedOptionMetadata;
                Asserts.OperationNotAllowed(namedItem.ShortName.HasValue && NamedOptions.Any(meta => meta.ShortName.HasValue && meta.ShortName == namedItem.ShortName), "Command {0} property has been already declared.".FormatFor("ShortName"));
                Asserts.OperationNotAllowed(!namedItem.LongName.IsNullOrEmptyOrWhiteSpace() && NamedOptions.Any(meta => meta.LongName.IsNullOrEmptyOrWhiteSpace() && meta.LongName == namedItem.LongName), "Command {0} property has been already declared.".FormatFor("LongName"));
            }
            if (item is CommandValueListOptionMetadata) Asserts.OperationNotAllowed(ValueListOption != null, "Only one property can be declared with ValueListAttribute.");
            if (item is CommandValueOptionMetadata)
            {
                var indexedItem = item as CommandValueOptionMetadata;
                Asserts.OperationNotAllowed(ValueOptions.Any(meta => meta.Index == indexedItem.Index), "Property decorated with ValueAttribute with the same index  {0} has been already declared.".FormatFor(indexedItem));
            }
            metadata.Add(item);
        }

        internal bool TryAdd(CommandOptionMetadataBase item)
        {
            try
            {
                Add(item);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        internal void Remove(CommandOptionMetadataBase item)
        {
            metadata.Remove(item);
        }

        internal void RemoveAt(char shortName)
        {
            Remove(this[shortName]);
        }

        internal void RemoveAt(string longName)
        {
            Remove(this[longName]);
        }

        internal void Clear()
        {
            metadata.Clear();
        }

        internal void Reset()
        {
            foreach (var option in this.Where(option => !(option.IsListOption && option.IsValueOption)))
                option.IsProvided = false;
            if (ValueListOption != null) ValueListOption.ElementsCount = 0;
        }

        public IEnumerator<CommandOptionMetadataBase> GetEnumerator()
        {
            return metadata.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Indexes

        public CommandNamedOptionMetadata this[char shortName]
        {
            get { return NamedOptions.SingleOrDefault(meta => meta.ShortName.HasValue && meta.ShortName == shortName); }
        }

        public CommandNamedOptionMetadata this[string longName]
        {
            get { return NamedOptions.SingleOrDefault(meta => meta.LongName == longName); }
        }

        public CommandValueOptionMetadata this[int index]
        {
            get { return ValueOptions.SingleOrDefault(meta => meta.Index == index); }
        }

        #endregion
    }
}
