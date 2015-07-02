//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using CommandShell.Helpers;

namespace CommandShell.Metadata
{
    public sealed class ValueAttribute : OptionBaseAttribute
    {
        #region Constructor
        
        public ValueAttribute(int index, string description)
        {
            Asserts.ArgumentNotNullOrEmptyOrWhitespace(description, "description");
            Asserts.ArgumentNotAllowed(index < 0, "index", "Position index is a zero-based index. Negative values are not allowed.");
            Description = description;
            Index = index;
        }

        #endregion

        #region Properties

        public int Index { get; private set; }

        #endregion
    }
}
