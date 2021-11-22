//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using CommandShell.Commands;
using CommandShell.Extensions;
using CommandShell.Helpers;
using CommandShell.Infrastucture;
using CommandShell.Infrastucture.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CommandShell
{
    public static class Shell
    {
        #region Constructor

        static Shell()
        {
            Input = Console.In;
            Output = Console.Out;
            Error = new ErrorConsoleWriter(Console.Error);
            defaultCommandsResolver = new DefaultCommandsResolver();
            defaultCommandActivator = new DefaultCommandActivator();
            HelpBuilder = HelpBuilder.Default;
            AssemblyInfo = new AssemblyInfo(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());
        }

        #endregion

        #region Properties

        internal static bool InteractiveMode = false;
        internal static AssemblyInfo AssemblyInfo;

        private static Dictionary<CommandMetadata, object> commands;
        internal static Dictionary<CommandMetadata, object> Commands
        {
            get { return commands ?? (commands = ResolveCommands()); }
        }

        public static TextReader Input { get; set; }

        public static TextWriter Output { get; set; }

        public static TextWriter Error { get; set; }

        private static readonly ICommandsResolver defaultCommandsResolver;
        private static ICommandsResolver commandsResolver;
        public static ICommandsResolver CommandsResolver
        {
            get { return commandsResolver ?? defaultCommandsResolver; }
            set { commandsResolver = value; }
        }

        private static readonly ICommandActivator defaultCommandActivator;
        private static ICommandActivator sommandActivator;
        public static ICommandActivator CommandActivator
        {
            get { return sommandActivator ?? defaultCommandActivator; }
            set { sommandActivator = value; }
        }

        public static HelpBuilder HelpBuilder { get; set; }

        #endregion

        #region Methods

        public static int Run(string[] args)
        {
            try
            {
                return CommandDispatcher.DispatchCommand(Commands, args);
            }
            catch (ShellHelpException)
            {
                ShowShellHelp();
            }
            catch (ShellCommandHelpException commandHelp)
            {
                var metadata = Commands.Select(pair => pair.Key).SingleOrDefault(meta => meta.Type == commandHelp.Command.GetType());
                Asserts.ReferenceNotNull(metadata, "Metadata for the provided command object couldn't be found. Ensure to provide command object to ShellCommandHelpException.");
                ShowCommandHelp(commandHelp.Command, metadata, commandHelp.ParsingResult);
            }
            catch (ExitInteractiveModeException)
            {
                throw;
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
            return -1;
        }

        public static int RunInteractive(string[] args)
        {
            var returnValue = -1;
            try
            {
                InteractiveMode = true;
                while (true)
                {
                    returnValue = Run(args);
                    args = ParseNextLine();
                }
            }
            catch (ExitInteractiveModeException)
            {
            }
            finally
            {
                InteractiveMode = false;
            }
            return returnValue;
        }

        public static int RunCommand(object command, string[] args)
        {
            var metadata = AttributedModelServices.GetMetadata(command);
            try
            {
                return CommandDispatcher.RunCommand(command, metadata, args);
            }
            catch (ShellCommandHelpException commandHelp)
            {
                ShowCommandHelp(commandHelp.Command, metadata, commandHelp.ParsingResult);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
            return -1;
        }

        #endregion

        #region Helpers

        private static Dictionary<CommandMetadata, object> ResolveCommands()
        {
            var metadata = CommandsResolver.Resolve().ToArray();
            if (metadata.Any(meta => meta.Name.IsNullOrEmptyOrWhiteSpace())) throw new InvalidOperationException("Empty command name is not allowed.");
            if (metadata.GroupBy(meta => meta.Name).Any(gr => gr.Count() > 1)) throw new InvalidOperationException("Commands with the same name are not allowed.");
            commands = new Dictionary<CommandMetadata, object>();
            foreach (var commandMetadata in metadata)
                commands.Add(commandMetadata, CommandActivator.Create(commandMetadata.Type));
            if (commands.All(command => command.Key.Type != typeof(HelpCommand))) Commands.Add(AttributedModelServices.GetMetadataFromType(typeof(HelpCommand)), new HelpCommand());
            if (InteractiveMode && commands.All(command => command.Key.Type != typeof(ExitCommand))) Commands.Add(AttributedModelServices.GetMetadataFromType(typeof(ExitCommand)), new ExitCommand());
            return commands;
        }

        private static void ShowShellHelp()
        {
            HelpBuilder.PrintHelp(Output, Commands.Keys, AssemblyInfo);
        }

        private static void ShowCommandHelp(object command, CommandMetadata metadata, ParsingResult parsingResult)
        {
            // ReSharper disable PossibleNullReferenceException
            if (metadata.HelpMethod == null) HelpBuilder.PrintCommandHelp(Output, metadata, AssemblyInfo, parsingResult);
            else
            {
                var parameters = new List<object>();
                foreach (var type in metadata.HelpMethod.GetParameters().Select(param => param.ParameterType))
                    if (type == typeof(CommandMetadata)) parameters.Add(metadata);
                    else parameters.Add(parsingResult);
                Output.WriteLine(metadata.HelpMethod.Invoke(command, parameters.ToArray()));
            }
            // ReSharper restore PossibleNullReferenceException
        }

        private static void ShowError(Exception exception)
        {
            HelpBuilder.PrintError(Error, exception);
        }

        private static string[] ParseNextLine()
        {
            string input;
            while ((input = Input.ReadLine()).IsNullOrEmptyOrWhiteSpace()) { }
            return CommandLineParser.Parse(input);
        }

        #endregion
    }
}
