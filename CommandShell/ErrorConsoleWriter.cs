//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.IO;

namespace CommandShell
{
    internal class ErrorConsoleWriter : ColourfulConsoleWriter
    {
        public ErrorConsoleWriter(TextWriter consoleTextWriter) : base(consoleTextWriter)
        {
            ForegroundColor = ConsoleColor.Red;
        }
    }
}
