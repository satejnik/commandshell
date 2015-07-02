//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using CommandShell.Infrastucture;
using CommandShell.Metadata;

namespace DemoShell.Commands
{
    [ShellCommand("error", Description = "Hints a custom error message happened (using ShellException class)")]
    public class ErrorCommand
    {
        public void Run()
        {
            throw new ShellException("Omitting error message causes printing command help.");
        }
    }
}
