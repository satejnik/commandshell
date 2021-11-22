//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using CommandShell.Infrastucture.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandShell.Infrastucture
{
    internal static class CommandDispatcher
    {
        #region Methods

        internal static int DispatchCommand(CommandMetadata[] commands, string[] args)
        {
            try
            {
                if (args == null || !args.Any()) throw new ShellHelpException();
                var commandName = args.First().ToLower();
                var metadata = commands.SingleOrDefault(meta => meta.Name == commandName);
                if (metadata == null) throw new ShellHelpException();
                var command = Shell.CommandActivator.Create(metadata.Type);
                try
                {
                    return RunCommand(command, metadata, args.Skip(1).ToArray());
                }
                finally
                {
                    Shell.CommandActivator.Release(command);
                }
            }
            catch (ShellHelpException)
            {
                ShowHelp();
            }
            return -1;
        }

        internal static int RunCommand(object command, CommandMetadata metadata, string[] args)
        {
            try
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
                if (!parsingResult) throw new ShellCommandHelpException(metadata) { ParsingResult = parsingResult };
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
            catch (ShellCommandHelpException commandHelp)
            {
                ShowCommandHelp(commandHelp.Metadata, commandHelp.ParsingResult);
            }
            return -1;
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

        private static void ShowCommandHelp(CommandMetadata metadata, ParsingResult parsingResult)
        {
            if (metadata.HelpMethod == null) Shell.HelpBuilder.PrintCommandHelp(Shell.Output, metadata, GetAssemblyInfo(), parsingResult);
            else
            {
                var parameters = new List<object>();
                foreach (var type in metadata.HelpMethod.GetParameters().Select(param => param.ParameterType))
                    if (type == typeof(CommandMetadata)) parameters.Add(metadata);
                    else parameters.Add(parsingResult);
                var command = Shell.CommandActivator.Create(metadata.Type);
                try
                {
                    Shell.Output.WriteLine(metadata.HelpMethod.Invoke(command, parameters.ToArray()));
                }
                finally
                {
                    Shell.CommandActivator.Release(command);
                }
            }
        }

        private static void ShowHelp()
        {
            Shell.HelpBuilder.PrintHelp(Shell.Output, Shell.Commands, GetAssemblyInfo());
        }

        private static AssemblyInfo GetAssemblyInfo()
        {
            return new AssemblyInfo(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());
        }

        #endregion
    }
}
