//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.IO;
using System.Text;

namespace CommandShell
{
    public class ColourfulConsoleWriter : TextWriter
    {
        #region Fields

        private readonly TextWriter originalWriter;

        #endregion

        #region Constructors

        public ColourfulConsoleWriter(TextWriter consoleTextWriter)
        {
            originalWriter = consoleTextWriter;
            ForegroundColor = Console.ForegroundColor;
            BackgroundColor = Console.BackgroundColor;
        }

        #endregion

        #region Properties

        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        #endregion

        #region Methods

        public override void WriteLine(string value)
        {
            var originalForeground = Console.ForegroundColor;
            var originalBackground = Console.BackgroundColor;
            Console.ForegroundColor = ForegroundColor;
            Console.BackgroundColor = BackgroundColor;
            originalWriter.WriteLine(value);
            Console.ForegroundColor = originalForeground;
            Console.BackgroundColor = originalBackground;
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }

        #endregion
    }
}
