//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;

namespace CommandShell.Metadata
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ShellCommandAttribute : Attribute
    {
        #region Constructor

        public ShellCommandAttribute(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        public string Namespace { get; set; }

        public string Name { get; private set; }

        public string Description { get; set; }

        public Type Options { get; set; }

        #endregion
    }
}
