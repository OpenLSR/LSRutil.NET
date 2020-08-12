using System;
using LSRutil;

namespace ReadTrack
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = string.Empty;
            try
            {
                filename = args[0];
            }
            catch (IndexOutOfRangeException)
            {
                Console.Beep();
                Console.WriteLine("ERROR: Please provide a track file as an argument.");
                System.Environment.Exit(1);
            }

            var reader = new TRKReader();
            var track = reader.ReadTrack(filename);

            track.GetInfo();
            track.GetElements()[0].GetInfo();
        }
    }
}
