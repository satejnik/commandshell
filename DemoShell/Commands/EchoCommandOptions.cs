﻿//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using CommandShell.Metadata;

namespace DemoShell.Commands
{
    public class EchoCommandOptions
    {
        [ValueList("Values to echo to the console")]
        public string[] Values { get; set; }
    }
}
