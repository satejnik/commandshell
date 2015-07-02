//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

namespace CommandShell.Metadata
{
    public sealed class OptionAttribute : RequiredOptionAttribute
    {
        #region Constructor

        public OptionAttribute(char shortName, string description)
            : base(shortName, description)
        {
        }

        public OptionAttribute(string longName, string description)
            : base(longName, description)
        {
        }

        public OptionAttribute(char shortName, string longName, string description)
            : base(shortName, longName, description)
        {
        }

        #endregion
    }
}