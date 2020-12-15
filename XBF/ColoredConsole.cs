using System;
using System.Collections.Generic;
using System.Text;

namespace LSRutil.XBF
{
    class ColoredConsole
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public static void SetWriteInfo(bool writeInfo)
        {
            ColoredConsole._writeInfo = writeInfo;
        }

        // Token: 0x06000002 RID: 2 RVA: 0x0000205C File Offset: 0x0000025C
        public static void WriteLine()
        {
            if (ColoredConsole._writePlain)
            {
                Console.WriteLine();
            }
        }

        // Token: 0x06000003 RID: 3 RVA: 0x00002080 File Offset: 0x00000280
        public static void Write(string format, params object[] values)
        {
            if (ColoredConsole._writePlain)
            {
                Console.Write(format, values);
            }
        }

        // Token: 0x06000004 RID: 4 RVA: 0x000020A4 File Offset: 0x000002A4
        public static void WriteLine(string format, params object[] values)
        {
            if (ColoredConsole._writePlain)
            {
                Console.WriteLine(format, values);
            }
        }

        // Token: 0x06000005 RID: 5 RVA: 0x000020C8 File Offset: 0x000002C8
        public static void WriteDebug(string format, params object[] values)
        {
            if (ColoredConsole._writeDebug)
            {
                Console.Write(format, values);
            }
        }

        // Token: 0x06000006 RID: 6 RVA: 0x000020EC File Offset: 0x000002EC
        public static void WriteLineDebug(string format, params object[] values)
        {
            if (ColoredConsole._writeDebug)
            {
                Console.WriteLine(format, values);
            }
        }

        // Token: 0x06000007 RID: 7 RVA: 0x00002110 File Offset: 0x00000310
        public static void WriteInfo(string format, params object[] values)
        {
            if (ColoredConsole._writeInfo)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(format, values);
                Console.ResetColor();
            }
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002144 File Offset: 0x00000344
        public static void WriteLineInfo(string format, params object[] values)
        {
            if (ColoredConsole._writeInfo)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(format, values);
                Console.ResetColor();
            }
        }

        // Token: 0x06000009 RID: 9 RVA: 0x00002178 File Offset: 0x00000378
        public static void WriteWarn(string format, params object[] values)
        {
            if (ColoredConsole._writeWarn)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(format, values);
                Console.ResetColor();
            }
        }

        // Token: 0x0600000A RID: 10 RVA: 0x000021AC File Offset: 0x000003AC
        public static void WriteLineWarn(string format, params object[] values)
        {
            if (ColoredConsole._writeWarn)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(format, values);
                Console.ResetColor();
            }
        }

        // Token: 0x0600000B RID: 11 RVA: 0x000021E0 File Offset: 0x000003E0
        public static void WriteError(string format, params object[] values)
        {
            if (ColoredConsole._writeError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(format, values);
                Console.ResetColor();
            }
        }

        // Token: 0x0600000C RID: 12 RVA: 0x00002214 File Offset: 0x00000414
        public static void WriteLineError(string format, params object[] values)
        {
            if (ColoredConsole._writeError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(format, values);
                Console.ResetColor();
            }
        }

        // Token: 0x04000001 RID: 1
        private static bool _writeDebug = true;

        // Token: 0x04000002 RID: 2
        private static bool _writeInfo = true;

        // Token: 0x04000003 RID: 3
        private static bool _writeWarn = true;

        // Token: 0x04000004 RID: 4
        private static bool _writeError = true;

        // Token: 0x04000005 RID: 5
        private static bool _writePlain = true;
    }
}
