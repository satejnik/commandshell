//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using CommandShell.Helpers;

namespace CommandShell.Metadata
{
    public abstract class NamedOptionAttribute : OptionBaseAttribute
    {
        #region Constructor

        protected NamedOptionAttribute(char shortName, string description)
        {
            Asserts.ArgumentNotEmptyOrWhitespace(shortName, "shortName");
            Asserts.ArgumentNotNullOrEmptyOrWhitespace(description, "description");
            ShortName = shortName;
            Description = description;
        }

        protected NamedOptionAttribute(string longName, string description)
        {
            Asserts.ArgumentNotNullOrEmptyOrWhitespace(longName, "longName");
            Asserts.ArgumentNotNullOrEmptyOrWhitespace(description, "description");
            LongName = longName;
            Description = description;
        }

        protected NamedOptionAttribute(char shortName, string longName, string description)
        {
            Asserts.ArgumentNotEmptyOrWhitespace(shortName, "shortName");
            Asserts.ArgumentNotNullOrEmptyOrWhitespace(longName, "longName");
            Asserts.ArgumentNotNullOrEmptyOrWhitespace(description, "description");
            ShortName = shortName;
            LongName = longName;
            Description = description;
        }

        #endregion

        #region Properties

        public char? ShortName { get; private set; }

        public string LongName { get; private set; }

        #endregion
    }
}
