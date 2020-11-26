using System;
using System.Diagnostics;
using System.IO;
using LSRutil;
using LSRutil.MVD;

namespace ReadMotoVideo
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = new Stopwatch();
            timer.Start();
            var reader = new MvdReader();
            MotoVideo video = reader.ReadVideo(@"C:\LSR\art\taunts\opponents\videos\mrx.mvd");
            timer.Stop();
            Console.WriteLine("Loaded and parsed, took {0}ms", timer.ElapsedMilliseconds, video.numFrames);
            video.GetInfo();

            Directory.CreateDirectory("dump");
            timer.Restart();
            var i = 0;
            foreach (var frame in video.frames)
            {
                frame.DumpFrame("dump/" + i.ToString().PadLeft(4,'0') + ".raw");
                i++;
            }
            timer.Stop();
            Console.WriteLine("Dumping {1} frames took {0}ms", timer.ElapsedMilliseconds, video.numFrames);

        }
    }
}
