//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

namespace CommandShell.Metadata
{
    public sealed class ValueListAttribute : OptionBaseAttribute
    {
        #region Constants

        public static readonly int DefaultMinElements = 0;
        public static readonly int DefaultMaxElements = int.MaxValue;

        #endregion

        public ValueListAttribute(string description)
        {
            Description = description;
            MinimumElements = DefaultMinElements;
            MaximumElements = DefaultMaxElements;
        }

        #region Properties

        public int MinimumElements { get; set; }

        public int MaximumElements { get; set; }

        #endregion
    }
}
