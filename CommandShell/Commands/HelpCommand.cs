//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using CommandShell.Infrastucture;
using CommandShell.Metadata;
using System.Linq;

namespace CommandShell.Commands
{
    [IgnoreCommand]
    [ShellCommand("help", Description = "Get help with one of the above commands", Options = typeof(HelpCommandOptions))]
    internal class HelpCommand
    {
        public int Run(HelpCommandOptions options)
        {
            if (options.CommandName == null || !options.CommandName.Any())
                throw new ShellHelpException();
            var name = options.CommandName.First();
            var metadata = Shell.Commands.SingleOrDefault(command => command.Name == name);
            if (metadata == null) throw new ShellHelpException();
            throw new ShellCommandHelpException(metadata);
        }
    }
}
