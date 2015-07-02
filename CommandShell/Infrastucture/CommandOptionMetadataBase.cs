//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using CommandShell.Extensions;

namespace CommandShell.Infrastucture
{
    public abstract class CommandOptionMetadataBase
    {
        #region Constructor

        protected CommandOptionMetadataBase(PropertyInfo info)
        {
            Property = info;
        }

        #endregion

        #region Properties

        public Type Type
        {
            get { return Property.PropertyType; }
        }

        internal PropertyInfo Property { get; private set; }

        internal IEnumerable<ValidationAttribute> Validators
        {
            get { return Property.GetValidators(); }
        }

        public string Description { get; internal set; }

        public string MetaValue { get; internal set; }

        public bool IsValueOption
        {
            get { return this is CommandValueOptionMetadata || this is CommandValueListOptionMetadata; }
        }

        public bool IsListOption
        {
            get { return this is CommandValueListOptionMetadata || this is CommandOptionListOptionMetadata; }
        }

        public bool IsNamedOption
        {
            get { return this is CommandNamedOptionMetadata; }
        }

        public bool IsSwitchOption
        {
            get { return GetType() == typeof(CommandNamedOptionMetadata); }
        }

        public bool IsRequiredOption
        {
            get { return this is CommandRequiredOptionMetadata; }
        }

        public bool IsOption
        {
            get { return IsRequiredOption || !IsListOption; }
        }

        #endregion

        #region Helpers

        internal bool HasDescription { get { return !Description.IsNullOrEmptyOrWhiteSpace(); } }

        internal bool HasMetaValue { get { return !MetaValue.IsNullOrEmptyOrWhiteSpace(); } }

        internal virtual bool IsProvided { get; set; }

        #endregion
    }
}
