//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using CommandShell.Infrastucture;
using CommandShell.Metadata;

namespace CommandShell.Commands
{
    [IgnoreCommand]
    [ShellCommand("exit", Description = "Leaves interactive shell mode")]
    internal class ExitCommand
    {
        public int Run()
        {
            throw new ExitInteractiveModeException();
        }
    }
}
