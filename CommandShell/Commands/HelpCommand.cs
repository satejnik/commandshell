//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System.Linq;
using CommandShell.Infrastucture;
using CommandShell.Metadata;

namespace CommandShell.Commands
{
    [ShellCommand("help", Description = "Get help with one of the above commands", Options = typeof(HelpCommandOptions))]
    internal class HelpCommand
    {
        public int Run(HelpCommandOptions options)
        {
            if (options.CommandName == null || !options.CommandName.Any())
                throw new ShellHelpException();
            var name = options.CommandName.First();
            if (Shell.Commands.All(command => command.Key.Name != name)) throw new ShellHelpException();
            throw new ShellCommandHelpException(Shell.Commands.Single(command => command.Key.Name == name).Value);
        }
    }
}
