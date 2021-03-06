﻿//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System.Collections.Generic;

namespace CommandShell.Infrastucture
{
    public interface ICommandsResolver
    {
        IEnumerable<CommandMetadata> Resolve();
    }
}
