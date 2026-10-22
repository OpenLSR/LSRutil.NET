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
            var resArchive = reader.ReadArchive("/home/yellow/Downloads/iw/Program_Data/res/ART0001.RFH"); // Can be loaded with or without extension

            var timer = new Stopwatch();
            timer.Start();
            /*foreach (var file in resArchive.resources)
            {
                file.Unpack("extract", true);
            }
            */
            resArchive.ExtractAllFiles("extract-art-hebrew");
            timer.Stop();
            Console.WriteLine("Operation took {0}ms",timer.ElapsedMilliseconds);
        }
    }
}
