//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using CommandShell.Helpers;
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
                var metadata = SearchMetadata(commands, ref args);
                if (metadata == null) throw new ShellHelpException();
                object command = null;
                if (!metadata.RunnableMethod.IsStatic)
                {
                    command = Shell.CommandActivator.Create(metadata.Type);
                    Asserts.ReferenceNotNull(command, string.Format("Instance of command '{0}' could not be created.", metadata.Name));
                }
                try
                {
                    return RunCommand(command, metadata, args);
                }
                finally
                {
                    if (!metadata.RunnableMethod.IsStatic)
                    {
                        Shell.CommandActivator.Release(command);
                    }
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
                if (metadata.Type == metadata.Options.Type && command != null)
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
                object command = null;
                if (!metadata.HelpMethod.IsStatic)
                {
                    command = Shell.CommandActivator.Create(metadata.Type);
                    Asserts.ReferenceNotNull(command, string.Format("Instance of the command {0} could not be created.", metadata.Name));
                }
                try
                {
                    Shell.Output.WriteLine(metadata.HelpMethod.Invoke(command, parameters.ToArray()));
                }
                finally
                {
                    if (!metadata.HelpMethod.IsStatic)
                    {
                        Shell.CommandActivator.Release(command);
                    }
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

        internal static CommandMetadata SearchMetadata(CommandMetadata[] commands, ref string[] args)
        {
            if (args == null || !args.Any()) throw new ShellHelpException();
            var @namespace = args.First().ToLower();
            args = args.Skip(1).ToArray();
            var candidates = commands.Where(meta => meta.Namespace == @namespace).ToArray();
            if (candidates.Any())
            {
                var commandName = args.FirstOrDefault();
                if (!string.IsNullOrEmpty(commandName))
                {
                    commandName = commandName.ToLower();
                    var metadata = candidates.SingleOrDefault(meta => meta.Name == commandName);
                    if (metadata != null)
                    {
                        args = args.Skip(1).ToArray();
                        return metadata;
                    }
                }
            }
            return commands.SingleOrDefault(meta => string.IsNullOrEmpty(meta.Namespace) && meta.Name == @namespace);
        }

        #endregion
    }
}
