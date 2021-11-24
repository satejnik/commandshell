//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using CommandShell;
using DemoShell.Commands;

namespace DemoShell
{
    class Program
    {
        static void Main(string[] args)
        {
            //Shell.Run(args);
            Shell.RunInteractive(args);
            //Shell.RunCommand(new TestCommand(), args);
        }
    }
}
