//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.Reflection;

namespace CommandShell.Infrastucture
{
    public sealed class CommandMetadata
    {
        #region Constructor

        internal CommandMetadata(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        #endregion

        #region Properties

        internal Type Type { get; private set; }

        internal MethodInfo RunnableMethod { get; set; }

        internal MethodInfo HelpMethod { get; set; }

        public string Namespace { get; internal set; }

        public string Name { get; private set; }

        public string Description { get; internal set; }

        public CommandOptionsMetadata Options { get; internal set; }

        #endregion
    }
}
