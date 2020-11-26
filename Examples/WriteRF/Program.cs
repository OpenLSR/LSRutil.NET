using System;
using System.IO;
using LSRutil;
using LSRutil.RF;

namespace WriteRF
{
    class Program
    {
        static void Main(string[] args)
        {
            var resArchive = new ResourceArchive();
            resArchive.Add("test.txt");
            resArchive.Add("smile.png");

            try
            {
                var writer = new RfWriter();
                writer.WriteArchive(resArchive,"tomata");
            }
            catch (IOException)
            {
                Console.WriteLine("there was a problem saving the file. it probably exists.");
            }
            
        }
    }
}
