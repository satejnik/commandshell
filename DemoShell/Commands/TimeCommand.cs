//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using CommandShell;
using CommandShell.Metadata;

namespace DemoShell.Commands
{
    [ShellCommand("time", Namespace = "now", Description = "Provides current system time", Options = typeof(TimeCommand))]
    public class TimeCommand
    {
        [Option('f', "format", "time format string")]
        public string Format { get; set; }

        public void Run()
        {
            Shell.Output.WriteLine(DateTime.Now.ToString(Format));
        }
    }
}
