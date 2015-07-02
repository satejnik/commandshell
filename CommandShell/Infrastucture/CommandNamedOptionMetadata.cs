//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System.Reflection;
using CommandShell.Extensions;

namespace CommandShell.Infrastucture
{
    public class CommandNamedOptionMetadata : CommandOptionMetadataBase
    {
        #region Constructor

        internal CommandNamedOptionMetadata(PropertyInfo info)
            : base(info)
        {
        }

        #endregion

        #region Properties

        public char? ShortName { get; internal set; }

        public string LongName { get; internal set; }

        #endregion
        
        #region Helpers

        internal bool HasShortName { get { return ShortName != null; } }

        internal bool HasLongName { get { return !LongName.IsNullOrEmptyOrWhiteSpace(); } }

        #endregion
    }
}
