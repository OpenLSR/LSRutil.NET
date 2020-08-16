using System;
using System.Diagnostics;
using LSRutil;

namespace ReadRF
{
    class Program
    {
        static void Main(string[] args)
        {
            var resourceArchive = new ResourceArchive();
            resourceArchive.Load(@"C:\LSR\res\ART0001");

            var timer = new Stopwatch();
            timer.Start();
            foreach (var file in resourceArchive.resources)
            {
                file.Extract("extract", true);
            }
            timer.Stop();
            Console.WriteLine("Operation took {0}ms",timer.ElapsedMilliseconds);
        }
    }
}
