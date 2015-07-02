//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;

namespace CommandShell.Infrastucture
{
    public class ShellException : Exception
    {
        #region Constructor

        public ShellException(string message)
            : base(message)
        {
        }

        #endregion
    }
}
