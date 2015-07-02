//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using CommandShell.Metadata;

namespace DemoShell.Commands
{
    [ShellCommand("exception", Description = "Throws an exception")]
    public class ExceptionCommand
    {
        public void Run()
        {
            throw new ApplicationException("Desired exception was thrown.");
        }
    }
}
