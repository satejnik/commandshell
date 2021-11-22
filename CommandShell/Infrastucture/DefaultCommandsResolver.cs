//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandShell.Infrastucture
{
    public class DefaultCommandsResolver : ICommandsResolver
    {
        #region Methods

        public virtual IEnumerable<CommandMetadata> Resolve()
        {
            return AttributedModelServices.GetMetadataFromTypeAssembly(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());
        }

        [Obsolete("This method may be removedin the future releses, please use AttributedModelServices.GetMetadataFromTypeAssembly instead.")]
        public static IEnumerable<CommandMetadata> ResolveFromAssembly(Assembly assembly)
        {
            return AttributedModelServices.GetMetadataFromTypeAssembly(assembly);
        }

        [Obsolete("This method may be removed in the future releses, please use AttributedModelServices.GetMetadataFromType instead.")]
        public static CommandMetadata GetCommandMetadata(Type type)
        {
            return AttributedModelServices.GetMetadataFromType(type);
        }

        #endregion
    }
}
