//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;

namespace CommandShell.Metadata
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class OptionBaseAttribute : Attribute
    {
        #region Properties

        public string Description { get; protected set; }

        public string MetaValue { get; set; }

        #endregion
    }
}
