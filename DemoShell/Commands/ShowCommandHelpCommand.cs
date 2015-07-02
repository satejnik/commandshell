//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using CommandShell.Infrastucture;
using CommandShell.Metadata;

namespace DemoShell.Commands
{
    [ShellCommand("commandhelp", Description = "Causes showing command help")]
    public class ShowCommandHelpCommand
    {
        public void Run()
        {
            throw new ShellCommandHelpException(this);
        }
    }
}
