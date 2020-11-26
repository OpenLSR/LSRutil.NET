using System;
using System.Diagnostics;
using LSRutil;
using LSRutil.RF;

namespace ReadRF
{
    class Program
    {
        static void Main(string[] args)
        {
            var reader = new RfReader();
            var resArchive = reader.ReadArchive(@"C:\LSR\res\TEXT0001.rfh"); // Can be loaded with or without extension

            var timer = new Stopwatch();
            timer.Start();
            foreach (var file in resArchive.resources)
            {
                file.Unpack("extract", true);
            }
            //resArchive.ExtractAllFiles("extract");
            timer.Stop();
            Console.WriteLine("Operation took {0}ms",timer.ElapsedMilliseconds);
        }
    }
}
