//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

namespace CommandShell.Infrastucture.Parsing
{
    public sealed class ParsingError
    {
        #region Constructor

        internal ParsingError(CommandOptionMetadataBase optionMetadata, ParsingErrorBase parsingErrorBase)
        {
            OptionMetadata = optionMetadata;
            ParsingErrorBase = parsingErrorBase;
        }

        #endregion

        #region Properties

        public CommandOptionMetadataBase OptionMetadata { get; private set; }

        public ParsingErrorBase ParsingErrorBase { get; private set; }

        public string Value { get; internal set; }

        #endregion
    }
}
