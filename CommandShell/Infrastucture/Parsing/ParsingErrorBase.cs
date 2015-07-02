//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

namespace CommandShell.Infrastucture.Parsing
{
    public enum ParsingErrorBase
    {
        OptionMissed,
        ValueMissed,
        InvalidValue,
        IndexOutOfRange,
        RedundantOption,
        UnboundValue
    }
}
