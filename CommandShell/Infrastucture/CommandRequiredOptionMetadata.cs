//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System.Reflection;

namespace CommandShell.Infrastucture
{
    public class CommandRequiredOptionMetadata : CommandNamedOptionMetadata
    {
        #region Constructor

        internal CommandRequiredOptionMetadata(PropertyInfo info)
            : base(info)
        {
        }

        #endregion

        #region Properties
        
        public bool Required { get; internal set; }

        public object DefaultValue { get; internal set; }

        #endregion

        #region Helpers

        internal bool HasDefaultValue { get { return DefaultValue != null; } }

        #endregion
    }
}
