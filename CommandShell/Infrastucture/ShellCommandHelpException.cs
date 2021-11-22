//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.Linq;
using CommandShell.Infrastucture.Parsing;

namespace CommandShell.Infrastucture
{
    public sealed class ShellCommandHelpException : Exception
    {
        #region Constructors

        public ShellCommandHelpException(string command)
            : this(Shell.Commands.SingleOrDefault(c => c.Name == command))
        {
        }

        public ShellCommandHelpException(object command)
            : this(AttributedModelServices.GetMetadata(command))
        {
        }

        public ShellCommandHelpException(CommandMetadata metadata)
        {
            if (metadata == null) throw new ArgumentNullException("metadata");
            Metadata = metadata;
        }

        #endregion

        #region Properties

        public CommandMetadata Metadata { get; private set; }

        internal ParsingResult ParsingResult { get; set; }

        #endregion
    }
}
