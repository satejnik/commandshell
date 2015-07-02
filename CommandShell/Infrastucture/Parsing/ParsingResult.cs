//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CommandShell.Infrastucture.Parsing
{
    public sealed class ParsingResult
    {
        #region Constructor

        internal ParsingResult(IList<ParsingError> errors)
        {
            Errors = new ReadOnlyCollection<ParsingError>(errors);
        }

        #endregion

        #region Properties

        public ReadOnlyCollection<ParsingError> Errors { get; private set; }

        public ParsingState State
        {
            get { return Errors.Any() ? ParsingState.Failed : ParsingState.Success; }
        }

        #endregion

        #region operators

        public static implicit operator bool(ParsingResult result)
        {
            return result.State == ParsingState.Success;
        }

        public static implicit operator ParsingState(ParsingResult result)
        {
            return result.State;
        }

        #endregion
    }
}
