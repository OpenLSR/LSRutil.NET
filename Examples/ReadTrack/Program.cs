using System;
using LSRutil;

namespace ReadTrack
{
    class Program
    {
        readonly static char[][] visTable = new char[16][]
        {
            new char[16],
            new char[16],
            new char[16],
            new char[16],
            new char[16],
            new char[16],
            new char[16],
            new char[16],
            new char[16],
            new char[16],
            new char[16],
            new char[16],
            new char[16],
            new char[16],
            new char[16],
            new char[16],
        }; // this is bad.

        static void SetVis(int x, int y, char vis)
        {
            int tableX = 15 - x;
            int tableY = 15 - y;
            visTable[tableY][tableX] = vis;
        }

        static void PrintVis()
        {
            foreach (var row in visTable)
            {
                foreach (var chr in row)
                {
                    //char pr = chr != '\0' ? chr : '.';
                    Console.Write(chr);
                }
                Console.Write("\n");
            }
        }

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
            //track.GetElements()[0].GetInfo();
            var elements = track.GetElements();

            foreach (var element in elements)
            {
                if(element.id != 255)
                    if(element.xid < 38)
                        SetVis(element.X, element.Z, '#'); // track element (probably)
                    else
                        SetVis(element.X, element.Z, '*'); // scenery element (probably)
            }

            PrintVis();
        }
    }
}
