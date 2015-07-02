//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System.Linq;
using CommandShell;
using CommandShell.Metadata;

namespace DemoShell.Commands
{
    [ShellCommand("echo", Description = "Echos writen values delimited by line", Options = typeof(EchoCommandOptions))]
    public class EchoCommand
    {
        public void Run(EchoCommandOptions options)
        {
            if (options.Values == null || !options.Values.Any())
            {
                Shell.Output.WriteLine("No values provided");
                return;
            }
            foreach (var value in options.Values)
            {
                Shell.Output.WriteLine(value);
            }
        }
    }
}
