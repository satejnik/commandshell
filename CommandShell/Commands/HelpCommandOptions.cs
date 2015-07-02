//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System.Collections.Generic;
using CommandShell.Metadata;

namespace CommandShell.Commands
{
    internal class HelpCommandOptions
    {
        [ValueList("Name of a command to get help for", MinimumElements = 0, MaximumElements = 1)]
        public List<string> CommandName { get; set; }
    }
}
