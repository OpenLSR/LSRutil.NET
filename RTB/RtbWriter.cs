using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LSRutil.RTB
{
    /// <summary>
    /// Class for writing RTB files.
    /// </summary>
    public class RtbWriter
    {
        private Stream stream;
        private BinaryWriter writer;

        public void WriteTable(ResourceTable table, Stream stream)
        {
            this.stream = stream;

            using (writer = new BinaryWriter(stream))
            {
                writer.Write(Encoding.ASCII.GetBytes("WigRTB 1.0\0\0"));
                writer.Write(table.contents.Count);
                if (table.directories.Count != 4) throw new ArgumentOutOfRangeException("table","RTB file must have exactly 4 directories!");
                foreach (var directory in table.directories)
                {
                    writer.Write(Encoding.ASCII.GetBytes(directory+"\0"));
                    writer.Write(Enumerable.Repeat((byte)0xCD, (260 - (directory.Length + 1))).ToArray());
                }
                foreach (var file in table.contents)
                {
                    writer.Write(file.Key); // id
                    writer.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF});
                    writer.Write(Encoding.ASCII.GetBytes(file.Value+"\0\0\0\0")); // filename
                }
            }
            
            
        }
        
        public void WriteTable(ResourceTable table, string file)
        {
            var fileExt = Path.GetExtension(file).ToLower();
            if(fileExt==".rtb") file = Path.ChangeExtension(file, null);
            stream = new FileStream(file+".rtb", FileMode.CreateNew);

            WriteTable(table, stream);
        }
    }
}