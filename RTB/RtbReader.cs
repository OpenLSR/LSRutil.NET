using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LSRutil.RTB
{
    /// <summary>
    /// Class for reading RTB files.
    /// </summary>
    public class RtbReader
    {
        private Stream stream;
        private BinaryReader reader;

        public ResourceTable ReadTable(Stream stream)
        {
            this.stream = stream;

            var table = new ResourceTable();

            using (reader = new BinaryReader(stream))
            {
                if(Encoding.ASCII.GetString(reader.ReadBytes(10)) != "WigRTB 1.0")
                {
                    throw new InvalidDataException("Incorrect RTB header!");
                }

                reader.BaseStream.Seek(12L, SeekOrigin.Begin);
                uint assetCount = reader.ReadUInt32();

                for (uint i = 0; i < 4; i++)
                {
                    var dirBytes = reader.ReadBytes(260);
                    var dir = Encoding.ASCII.GetString(dirBytes);
                    dir = dir.Substring(0, dir.IndexOf("\0"));
                    table.directories.Add(dir);
                }

                for (uint i = 0; i < assetCount; i++)
                {
                    uint assetId = reader.ReadUInt32();

                    reader.BaseStream.Seek(4L, SeekOrigin.Current); 

                    int offset = 1;
                    byte nchar = reader.ReadByte();
                    while (nchar != 0x00) // Seperator
                    {
                        offset++;
                        nchar = reader.ReadByte();
                    }

                    reader.BaseStream.Seek(-(offset), SeekOrigin.Current);
                    string path = Encoding.ASCII.GetString(reader.ReadBytes(offset-1));

                    table.Add(assetId, path);

                    reader.BaseStream.Seek(4L, SeekOrigin.Current);
                }
            }

            return table;
        }

        public ResourceTable ReadTable(string filename)
        {
            stream = File.Open(filename, FileMode.Open, FileAccess.Read);
            return ReadTable(stream);
        }
    }
}
