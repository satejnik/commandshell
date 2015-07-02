//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System.Reflection;

namespace CommandShell.Infrastucture
{
    public sealed class CommandValueOptionMetadata : CommandOptionMetadataBase
    {
        #region Constructor

        internal CommandValueOptionMetadata(PropertyInfo info)
            : base(info)
        {
        }

        #endregion

        #region Properties

        public int Index { get; internal set; }

        #endregion
    }
}
