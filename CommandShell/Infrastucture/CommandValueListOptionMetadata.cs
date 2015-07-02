//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System.Reflection;

namespace CommandShell.Infrastucture
{
    public sealed class CommandValueListOptionMetadata : CommandOptionMetadataBase
    {
        #region Constructor

        internal CommandValueListOptionMetadata(PropertyInfo info)
            : base(info)
        {
        }

        #endregion

        #region Properties

        public int MinElements { get; internal set; }

        public int MaxElements { get; internal set; }

        #endregion

        #region Helpers

        internal int ElementsCount { get; set; }

        internal override bool IsProvided
        {
            get { return ElementsCount <= MaxElements && ElementsCount >= MinElements; }
            set { }
        }

        #endregion
    }
}
