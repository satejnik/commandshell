//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using CommandShell;
using CommandShell.Infrastucture;
using CommandShell.Infrastucture.Parsing;
using CommandShell.Metadata;

namespace DemoShell.Commands
{
    [ShellCommand("test", Description = "Test all the available options", Options = typeof(TestCommand))]
    internal class TestCommand
    {
        #region Options

        [EmailAddress]
        [Option("email", "Email")]
        public string Email { get; set; }

        [Url]
        [Option("url", "Url")]
        public string Url { get; set; }

        [Range(1, 3)]
        [Option("range", "Int from 1-3")]
        public int Value { get; set; }

        [Option('r', "read", "Input file ", MetaValue = "FILE", Required = true)]
        public string InputFile { get; set; }

        [Option('w', "write", "Output file", MetaValue = "FILE")]
        public string OutputFile { get; set; }

        [Switch("calculate", "Add results in bottom of tabular data")]
        public bool Calculate { get; set; }

        [Option('v', "Verbose level. Range: from 0 to 2", MetaValue = "INT")]
        public int? VerboseLevel { get; set; }

        [Switch('i', "If file has errors don't stop processing")]
        public bool IgnoreErrors { get; set; }

        [Option('j', "jump", "start offset", MetaValue = "INT", DefaultValue = 3)]
        public double StartOffset { get; set; }

        [Option("enum", "Testing enumeration")]
        public TestEnum Enumeration { get; set; }

        [Option('d', "DateTime")]
        public DateTime Date { get; set; }

        [Option('p', "person", "Person")]
        public Person Person { get; set; }

        [OptionList('o', "operators", "Operators included in processing (+;-;...). Separate each operator with a semicolon. Do not include spaces between operators and separator", Separator = ';')]
        public IList<int> AllowedOperators { get; set; }

        [Value(0, "First value")]
        public double Int { get; set; }

        [Value(1, "Second Value")]
        public double Double { get; set; }

        [ValueList("Remained Values")]
        public IList<string> RemainedArgs { get; set; }

        #endregion

        #region Methods

        [GetHelp]
        public string GetUsage(CommandMetadata metadata, ParsingResult parsingResult)
        {
            return new StringBuilder(metadata.Name).AppendLine().AppendLine(metadata.Description)
                .AppendLine(string.Join(Environment.NewLine,
                    parsingResult.Errors.Select(
                        err =>
                            (err.OptionMetadata.IsNamedOption
                                ? ((CommandNamedOptionMetadata)err.OptionMetadata).ShortName +
                                  ((CommandNamedOptionMetadata)err.OptionMetadata).LongName
                                : string.Empty) + " " + err.OptionMetadata.ToString() + " - " + err.ParsingErrorBase.ToString()))).ToString();
        }

        [RunCommand]
        public int Execute()
        {
            Shell.Output.WriteLine(Value);
            if (!string.IsNullOrEmpty(Email)) Shell.Output.WriteLine(Email);
            if (!string.IsNullOrEmpty(Url)) Shell.Output.WriteLine(Url);
            Shell.Output.WriteLine(Date == default(DateTime) ? DateTime.Now : Date);

            if (VerboseLevel == null)
            {
                Shell.Output.WriteLine("verbose [off]");
            }
            else
            {
                Shell.Output.WriteLine(
                    "verbose [on]: {0}",
                    VerboseLevel < 0 || VerboseLevel > 2 ? "#invalid value#" : VerboseLevel.ToString());
            }

            Shell.Output.WriteLine();
            Shell.Output.WriteLine("input file: {0} ...", InputFile);

            Shell.Output.WriteLine("  start offset: {0}", StartOffset);
            Shell.Output.WriteLine("  tabular data computation: {0}", Calculate.ToString().ToLowerInvariant());
            Shell.Output.WriteLine("  on errors: {0}", IgnoreErrors ? "continue" : "stop processing");
            Shell.Output.WriteLine("  enum for: {0}", Enumeration.ToString().ToLowerInvariant());

            if (AllowedOperators != null)
            {
                var builder = new StringBuilder();
                builder.Append("  allowed operators: ");

                foreach (var op in AllowedOperators)
                {
                    builder.Append(op);
                    builder.Append(", ");
                }

                Shell.Output.WriteLine(builder.Remove(builder.Length - 2, 2).ToString());
            }

            Shell.Output.WriteLine("Indexed:");
            Shell.Output.WriteLine(Int);
            Shell.Output.WriteLine(Double);

            if (RemainedArgs != null)
            {
                Shell.Output.WriteLine("remained:");
                foreach (var remainedArg in RemainedArgs)
                {
                    Shell.Output.WriteLine(remainedArg);
                }
            }

            if (Person != null)
                Shell.Output.WriteLine("Person: {0}", Person.Name);

            Shell.Output.WriteLine();

            if (!string.IsNullOrEmpty(OutputFile))
            {
                Shell.Output.WriteLine("input file: {0} ...", OutputFile);
            }
            else
            {
                Shell.Output.WriteLine("[...]");
            }

            return 0;
        }

        #endregion
    }
}
