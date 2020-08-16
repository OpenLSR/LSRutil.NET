using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LSRutil
{
    public class ResourceArchive
    {
        public List<ResourceFile> resources;

        public ResourceArchive(List<ResourceFile> resList)
        {
            resources = resList;
        }

        public ResourceArchive() {
            resources = new List<ResourceFile>();
        }

        public void Load(string file)
        {
            FileStream headerStream = new FileStream(file+".rfh", FileMode.Open);
            BinaryReader headerReader = new BinaryReader(headerStream);
            FileStream dataStream = new FileStream(file+".rfd", FileMode.Open);
            BinaryReader dataReader = new BinaryReader(dataStream);
            do
            {
                int pathLen = headerReader.ReadInt32();
                DateTime timestamp = DateTimeOffset.FromUnixTimeSeconds(headerReader.ReadUInt32()).DateTime;
                int cmpType = headerReader.ReadInt32();
                int cmpSize = headerReader.ReadInt32();
                int offset = headerReader.ReadInt32();
                byte[] relPath = headerReader.ReadBytes(pathLen);
                dataReader.BaseStream.Seek((long)offset, SeekOrigin.Begin);
                byte[] data = dataReader.ReadBytes(cmpSize);
                resources.Add(new ResourceFile
                {
                    filepath = Encoding.UTF8.GetString(relPath).Trim('\0'), // Null terminated strings aren't a thing in C#.
                    pathLength = pathLen,
                    timestamp = timestamp,
                    compressionType = cmpType,
                    compressedSize = cmpSize,
                    offset = offset,
                    data = data
                }); ;
            }
            while (headerReader.BaseStream.Position < headerReader.BaseStream.Length);
            dataReader.Close();
            dataStream.Close();
            headerReader.Close();
            headerStream.Close();
        }

        public int ExtractAllFiles(string directory)
        {
            int counter = 0;
            if (resources.Count > 0)
            {
                foreach (ResourceFile file in this.resources)
                {
                    file.Extract(directory, true);
                    counter++;
                }
            }
            return counter;
        }
    }
}
