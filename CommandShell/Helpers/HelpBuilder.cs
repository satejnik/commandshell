//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommandShell.Infrastucture;
using CommandShell.Infrastucture.Parsing;

namespace CommandShell.Helpers
{
    public class HelpBuilder
    {
        #region Constructor

        private static readonly HelpBuilder Instance = new HelpBuilder();

        private HelpBuilder() { }

        public static HelpBuilder Current
        {
            get
            {
                return Instance;
            }
        }

        #endregion

        #region Methods

        public virtual void PrintHelp(TextWriter writer, IEnumerable<CommandMetadata> commands, AssemblyInfo info)
        {
            PrintDefaultHelpHeader(writer, info);
            writer.WriteLine();
            writer.WriteLine("Available commands are:");
            PrintDefaultHelpCommands(writer, commands);
            writer.WriteLine();
        }

        public virtual void PrintCommandHelp(TextWriter writer, CommandMetadata metadata, AssemblyInfo info, ParsingResult parsingResult)
        {
            //PrintDefaultHelpHeader(writer, info);
            //writer.WriteLine();
            writer.Write(metadata.Name);
            writer.Write(" - ");
            writer.WriteLine(metadata.Description);
            writer.WriteLine();
            PrintDefaultHelpCommandErrors(writer, parsingResult.Errors);
            if (!parsingResult) writer.WriteLine();
            PrintDefaultHelpCommandUsage(writer, metadata);
            writer.WriteLine();
            PrintDefaultHelpCommandOptions(writer, metadata);
        }

        public virtual void PrintError(TextWriter writer, Exception error)
        {
            if (error is ShellException)
                writer.WriteLine(error.Message);
            else
                writer.WriteLine(error.ToString());
        }

        #endregion

        #region Helpers

        protected void PrintDefaultHelpHeader(TextWriter writer, AssemblyInfo info)
        {
            writer.Write(info.FriendlyName);
            writer.Write(" ");
            writer.WriteLine(info.Version);
            writer.Write(info.Copyright);
            writer.Write(" ");
            writer.WriteLine(info.Company);
        }

        protected void PrintDefaultHelpCommandErrors(TextWriter writer, IEnumerable<ParsingError> errors)
        {
            errors = errors.ToArray();
            if (errors.Any()) writer.WriteLine("ERROR(S):");
            foreach (var error in errors)
            {
                var builder = new StringBuilder();
                var named = error.OptionMetadata as CommandNamedOptionMetadata;
                if (named != null)
                {
                    if (named.HasShortName) builder.Append("-").Append(named.ShortName);
                    if (named.HasShortName && named.HasLongName) builder.Append(", ");
                    if (named.HasLongName) builder.Append("--").Append(named.LongName);
                    builder.Append(" option");
                }
                else if (error.OptionMetadata.IsValueOption && !error.OptionMetadata.IsListOption)
                {
                    builder.Append("argument (" + ((CommandValueOptionMetadata)error.OptionMetadata).Index + ")");
                }
                var name = builder.ToString();
                switch (error.ParsingErrorBase)
                {
                    case ParsingErrorBase.OptionMissed:
                        writer.WriteLine("   - {0} {1}", name, " is missing");
                        break;
                    case ParsingErrorBase.ValueMissed:
                        writer.WriteLine("   - value for {0} {1}", name, " is missing");
                        break;
                    case ParsingErrorBase.InvalidValue:
                        writer.WriteLine("   - invalid value: {0}", error.Value);
                        break;
                    case ParsingErrorBase.UnboundValue:
                        writer.WriteLine("   - unbound value: {0}", error.Value);
                        break;
                    case ParsingErrorBase.RedundantOption:
                        writer.WriteLine("   - {0} is redundant", name);
                        break;
                    case ParsingErrorBase.IndexOutOfRange:
                        writer.WriteLine("   - wrong number of arguments");
                        break;
                }
            }
        }

        protected void PrintDefaultHelpCommandUsage(TextWriter writer, CommandMetadata command)
        {
            const int staticWidth = 60;
            var linesCount = 1;
            writer.Write("Usage: ");
            var usageBuilder = new StringBuilder(command.Name).Append(" ");
            if (command.Options != null)
            {
                foreach (var option in command.Options.RequiredOptions.Where(required => required.Required))
                {
                    usageBuilder.Append("<");
                    if (option.HasShortName)
                        usageBuilder.Append("-").Append(option.ShortName);
                    if (option.HasShortName && option.HasLongName)
                        usageBuilder.Append(", ");
                    if (option.HasLongName)
                        usageBuilder.Append("--").Append(option.LongName);
                    if (option.HasMetaValue) usageBuilder.Append(" ").Append(option.MetaValue);
                    else usageBuilder.Append(" <value(s)>");
                    usageBuilder.Append("> ");
                    if (usageBuilder.Length <= staticWidth * linesCount) continue;
                    usageBuilder.AppendLine().Append("       ");
                    linesCount++;
                }
                foreach (var option in command.Options.RequiredOptions.Where(required => !required.Required))
                {
                    usageBuilder.Append("[");
                    if (option.HasShortName)
                        usageBuilder.Append("-").Append(option.ShortName);
                    if (option.HasShortName && option.HasLongName)
                        usageBuilder.Append(", ");
                    if (option.HasLongName)
                        usageBuilder.Append("--").Append(option.LongName);
                    usageBuilder.Append(" <value(s)>");
                    usageBuilder.Append("] ");
                    if (usageBuilder.Length <= staticWidth*linesCount) continue;
                    usageBuilder.AppendLine().Append("       ");
                    linesCount++;
                }
                foreach (var option in command.Options.Switches)
                {
                    usageBuilder.Append("[");
                    if (option.HasShortName)
                        usageBuilder.Append("-").Append(option.ShortName);
                    if (option.HasShortName && option.HasLongName)
                        usageBuilder.Append(", ");
                    if (option.HasLongName)
                        usageBuilder.Append("--").Append(option.LongName);
                    usageBuilder.Append("] ");
                    if (usageBuilder.Length <= staticWidth * linesCount) continue;
                    usageBuilder.AppendLine().Append("       ");
                    linesCount++;
                }
                if (command.Options.ValueOptions.Any() || command.Options.ValueListOption != null)
                    usageBuilder.Append("<args>");
            }
            writer.WriteLine(usageBuilder.ToString());
        }

        protected void PrintDefaultHelpCommandOptions(TextWriter writer, CommandMetadata metadata)
        {
            if (metadata.Options == null || !metadata.Options.Any()) return;
            const int length = 20;
            var commandFormatString = "   {0,-" + length + "}- {1}";
            foreach (var option in metadata.Options.NamedOptions)
            {
                var optionBuilder = new StringBuilder();
                if (option.HasShortName) optionBuilder.Append("-").Append(option.ShortName);
                if (option.HasShortName && option.HasLongName) optionBuilder.Append(", ");
                if (option.HasLongName) optionBuilder.Append("--").Append(option.LongName);
                optionBuilder.Append(" ");
                if (option.HasMetaValue) optionBuilder.Append(option.MetaValue);
                var descriptionBuilder = new StringBuilder();
                if (option.IsRequiredOption && ((CommandRequiredOptionMetadata)option).HasDefaultValue)
                    descriptionBuilder.Append("(Default: ")
                        .Append(((CommandRequiredOptionMetadata)option).DefaultValue)
                        .Append(") ");
                descriptionBuilder.Append(option.Description);
                writer.WriteLine(commandFormatString, optionBuilder, descriptionBuilder);
            }
            foreach (var option in metadata.Options.ValueOptions.OrderBy(valued => valued.Index))
            {
                var optionBuilder = new StringBuilder();
                optionBuilder.Append("arg[").Append(option.Index).Append("]");
                var descriptionBuilder = new StringBuilder();
                if (option.HasMetaValue) descriptionBuilder.Append("(").Append(option.MetaValue).Append(") ");
                descriptionBuilder.Append(option.Description);
                writer.WriteLine(commandFormatString, optionBuilder, descriptionBuilder);
            }
            if (metadata.Options.ValueListOption != null)
            {
                var descriptionBuilder = new StringBuilder();
                if (metadata.Options.ValueListOption.HasMetaValue) descriptionBuilder.Append("(").Append(metadata.Options.ValueListOption.MetaValue).Append(") ");
                descriptionBuilder.Append(metadata.Options.ValueListOption.Description);
                writer.WriteLine(commandFormatString, "args", descriptionBuilder);
            }
        }

        protected void PrintDefaultHelpCommands(TextWriter writer, IEnumerable<CommandMetadata> commands)
        {
            var commandsMetadata = commands.Where(meta => meta.Name != "help" && meta.Name != "exit").OrderBy(meta => meta.Name);
            var length = commandsMetadata.Max(meta => meta.Name.Length) + 1;
            if (length < 20) length = 20;
            var commandFormatString = "   {0,-" + length + "}- {1}";
            foreach (var command in commandsMetadata)
                writer.WriteLine(commandFormatString, command.Name, command.Description);
            writer.WriteLine(commandFormatString, "help <command>", "For help with one of the above commands");
            if (Shell.InteractiveMode) writer.WriteLine(commandFormatString, "exit", "Leaves interactive shell mode");
        }

        #endregion
    }
}
