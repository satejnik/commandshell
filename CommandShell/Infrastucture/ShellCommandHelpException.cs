//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.Linq;
using CommandShell.Helpers;
using CommandShell.Infrastucture.Parsing;

namespace CommandShell.Infrastucture
{
    public sealed class ShellCommandHelpException : Exception
    {
        #region Constructors

        public ShellCommandHelpException(string name)
            : this(null, name)
        {
        }

        public ShellCommandHelpException(string @namespace, string command)
            : this(Shell.Commands.SingleOrDefault(c => c.Namespace == @namespace && c.Name == command))
        {
        }

        public ShellCommandHelpException(object command)
            : this(AttributedModelServices.GetMetadata(command))
        {
        }

        public ShellCommandHelpException(CommandMetadata metadata)
        {
            Asserts.ArgumentNotNull(metadata, "metadata");
            Metadata = metadata;
        }

        #endregion

        #region Properties

        public CommandMetadata Metadata { get; private set; }

        internal ParsingResult ParsingResult { get; set; }

        #endregion
    }
}
