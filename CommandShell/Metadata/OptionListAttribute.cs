//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

namespace CommandShell.Metadata
{
    public sealed class OptionListAttribute : RequiredOptionAttribute
    {
        #region Constants

        public static readonly char DefaultSeparator = ';';

        #endregion

        #region Constructors

        public OptionListAttribute(char shortName, string description)
            : base(shortName, description)
        {
            Separator = DefaultSeparator;
        }

        public OptionListAttribute(string longName, string description)
            : base(longName, description)
        {
            Separator = DefaultSeparator;
        }

        public OptionListAttribute(char shortName, string longName, string description)
            : base(shortName, longName, description)
        {
            Separator = DefaultSeparator;
        }

        #endregion

        #region Properties

        public char Separator { get; set; }

        #endregion
    }
}
