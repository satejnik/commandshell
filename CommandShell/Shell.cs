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
using System.IO;
using System.Linq;

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
            defaultHelpBuilder = new HelpBuilder();
        }

        #endregion

        #region Properties

        internal static bool InteractiveMode = false;

        private static CommandMetadata[] commands;
        internal static CommandMetadata[] Commands
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

        private static readonly HelpBuilder defaultHelpBuilder;
        private static HelpBuilder helpBuilder;
        public static HelpBuilder HelpBuilder
        {
            get { return helpBuilder ?? defaultHelpBuilder; }
            set { helpBuilder = value; }
        }

        #endregion

        #region Methods

        public static int Run(string[] args)
        {
            try
            {
                return CommandDispatcher.DispatchCommand(Commands, args);
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
            try
            {
                return CommandDispatcher.RunCommand(command, AttributedModelServices.GetMetadata(command), args);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
            return -1;
        }

        #endregion

        #region Helpers

        private static CommandMetadata[] ResolveCommands()
        {
            var metadata = CommandsResolver.Resolve().ToList();
            Asserts.OperationNotAllowed(metadata.GroupBy(meta => new { meta.Namespace, meta.Name }).Any(group => group.Count() > 1), "Commands with the same name are not allowed.");
            if (metadata.All(command => command.Type != typeof(HelpCommand))) metadata.Add(AttributedModelServices.GetMetadataFromType(typeof(HelpCommand)));
            if (InteractiveMode && metadata.All(command => command.Type != typeof(ExitCommand))) metadata.Add(AttributedModelServices.GetMetadataFromType(typeof(ExitCommand)));
            return metadata.ToArray();
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
