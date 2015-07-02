//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandShell.Infrastucture.Parsing;

namespace CommandShell.Infrastucture
{
    internal static class CommandDispatcher
    {
        #region Methods

        internal static int DispatchCommand(Dictionary<CommandMetadata, object> commands, string[] args)
        {
            if (args == null || !args.Any()) throw new ShellHelpException();
            var commandName = args.First().ToLower();
            if (commands.All(meta => meta.Key.Name != commandName)) throw new ShellHelpException();
            var command = commands.Single(meta => meta.Key.Name == commandName).Value;
            var metadata = commands.Single(meta => meta.Key.Name == commandName).Key;
            return RunCommand(command, metadata, args.Skip(1).ToArray());
        }

        internal static int RunCommand(object command, CommandMetadata metadata, string[] args)
        {
            if (metadata.Options == null)
            {
                try
                {
                    return Run(command, metadata, null);
                }
                catch (ShellCommandHelpException helpException)
                {
                    helpException.ParsingResult = new ParsingResult(new List<ParsingError>());
                    throw;
                }
            }
            ParsingResult parsingResult;
            object options = null;
            if (metadata.Type == metadata.Options.Type)
                parsingResult = CommandOptionsParser.Parse(metadata.Options, command, args);
            else
            {
                options = Activator.CreateInstance(metadata.Options.Type);
                parsingResult = CommandOptionsParser.Parse(metadata.Options, options, args);
            }
            if (!parsingResult) throw new ShellCommandHelpException(command) { ParsingResult = parsingResult };
            try
            {
                return Run(command, metadata, options);
            }
            catch (ShellCommandHelpException helpException)
            {
                helpException.ParsingResult = parsingResult;
                throw;
            }
        }

        #endregion

        #region Helpers

        private static int Run(object command, CommandMetadata metadata, object options)
        {
            object returnValue;
            try
            {
                returnValue = options == null ? metadata.RunnableMethod.Invoke(command, new object[] { }) : metadata.RunnableMethod.Invoke(command, new[] { options });
            }
            catch (TargetInvocationException invocationError)
            {
                throw invocationError.InnerException;
            }
            return returnValue != null ? (int)returnValue : 0;
        }

        #endregion
    }
}
