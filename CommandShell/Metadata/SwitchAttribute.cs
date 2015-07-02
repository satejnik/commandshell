//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

namespace CommandShell.Metadata
{
    public sealed class SwitchAttribute : NamedOptionAttribute
    {
        #region Constructor

        public SwitchAttribute(char shortName, string description)
            : base(shortName, description)
        {
        }

        public SwitchAttribute(string longName, string description)
            : base(longName, description)
        {
        }

        public SwitchAttribute(char shortName, string longName, string description)
            : base(shortName, longName, description)
        {
        }

        #endregion
    }
}
