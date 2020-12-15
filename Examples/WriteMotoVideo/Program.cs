using System;
using System.IO;
using LSRutil;
using LSRutil.MVD;

namespace WriteMotoVideo
{
    class Program
    {
        static void Main(string[] args)
        {
            var reader = new MvdReader();
            var writer = new MvdWriter();
            MotoVideo video = reader.ReadVideo(@"C:\LSR\art\frontend\help\help.mvd");
            writer.WriteVideo(video, @"C:\LSR\art\frontend\help\help.mvd.out");
        }
    }
}
