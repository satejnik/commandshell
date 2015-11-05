//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandShell.Commands;
using CommandShell.Extensions;
using CommandShell.Helpers;
using CommandShell.Infrastucture;
using CommandShell.Infrastucture.Parsing;

namespace CommandShell
{
    public static class Shell
    {
        #region Fields

        internal static bool InteractiveMode = false;
        internal static Dictionary<CommandMetadata, object> Commands;
        internal static AssemblyInfo AssemblyInfo;

        #endregion

        #region Constructor

        static Shell()
        {
            Input = Console.In;
            Output = Console.Out;
            Error = new ErrorConsoleWriter(Console.Error);
            CommandsResolver = Infrastucture.CommandsResolver.Default;
            HelpBuilder = HelpBuilder.Default;
            AssemblyInfo = new AssemblyInfo(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());
        }

        #endregion

        #region Properties

        public static TextReader Input { get; set; }

        public static TextWriter Output { get; set; }

        public static TextWriter Error { get; set; }

        public static ICommandsResolver CommandsResolver { get; set; }

        public static HelpBuilder HelpBuilder { get; set; }

        #endregion

        #region Methods

        public static int Run(string[] args)
        {
            LoadCommands(CommandsResolver);
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
                Asserts.ReferenceNotNull(metadata, "Metadata forthe provided command object couldn't be found. Ensure to provide command object to ShellCommandHelpException.");
                ShowCommandHelp(commandHelp.Command, metadata, commandHelp.ParsingResult);
            }
            catch (ExitInteractiveModeException)
            {
                throw;
            }
            catch (Exception error)
            {
                ShowError(error);
            }
            return -1;
        }

        public static int RunInteractive(string[] args)
        {
            InteractiveMode = true;
            var returnValue = -1;
            LoadCommands(CommandsResolver);
            try
            {
                while (true)
                {
                    returnValue = Run(args);
                    args = ParseNextLine();
                }
            }
            catch (ExitInteractiveModeException)
            {
            }
            return returnValue;
        }

        public static int RunCommand(object command, string[] args)
        {
            var metadata = Infrastucture.CommandsResolver.GetCommandMetadata(command.GetType());
            try
            {
                return CommandDispatcher.RunCommand(command, metadata, args);
            }
            catch (ShellCommandHelpException commandHelp)
            {
                ShowCommandHelp(commandHelp.Command, metadata, commandHelp.ParsingResult);
            }
            catch (Exception error)
            {
                ShowError(error);
            }
            return -1;
        }

        #endregion

        #region Helpers

        private static void LoadCommands(ICommandsResolver resolver)
        {
            if (Commands != null) return;
            var metadata = resolver.Resolve().ToArray();
            if (metadata.Any(meta => meta.Name.IsNullOrEmptyOrWhiteSpace())) throw new InvalidOperationException("Empty command name is not allowed.");
            if (metadata.GroupBy(meta => meta.Name).Any(gr => gr.Count() > 1)) throw new InvalidOperationException("Commands with the same name are not allowed.");
            Commands = new Dictionary<CommandMetadata, object>();
            foreach (var commandMetadata in metadata)
                Commands.Add(commandMetadata, Activator.CreateInstance(commandMetadata.Type));
            if (Commands.All(command => command.Key.Type != typeof(HelpCommand))) Commands.Add(Infrastucture.CommandsResolver.GetCommandMetadata(typeof(HelpCommand)), new HelpCommand());
            if (InteractiveMode && Commands.All(command => command.Key.Type != typeof(ExitCommand))) Commands.Add(Infrastucture.CommandsResolver.GetCommandMetadata(typeof(ExitCommand)), new ExitCommand());
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

        private static void ShowError(Exception error)
        {
            HelpBuilder.PrintError(Error, error);
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
