//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using CommandShell.Infrastucture.Parsing;

namespace CommandShell.Infrastucture
{
    public sealed class ShellCommandHelpException : Exception
    {
        #region Constructors

        public ShellCommandHelpException(object command)
        {
            Command = command;
        }

        #endregion

        #region Properties

        public object Command { get; private set; }

        internal ParsingResult ParsingResult { get; set; }

        #endregion
    }
}
