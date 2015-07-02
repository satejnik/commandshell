//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

namespace CommandShell.Metadata
{
    public abstract class RequiredOptionAttribute : NamedOptionAttribute
    {
        #region Constructor

        protected RequiredOptionAttribute(char shortName, string description)
            : base(shortName, description)
        {
        }

        protected RequiredOptionAttribute(string longName, string description)
            : base(longName, description)
        {
        }

        protected RequiredOptionAttribute(char shortName, string longName, string description)
            : base(shortName, longName, description)
        {
        }

        #endregion

        #region Properties

        public bool Required { get; set; }

        public object DefaultValue { get; set; }

        #endregion
    }
}
