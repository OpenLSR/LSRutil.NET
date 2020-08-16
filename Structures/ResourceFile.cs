using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LSRutil
{
    public class ResourceFile
    {
        public int pathLength;
        public DateTime timestamp;
        public int compressionType;
        public int compressedSize;
        public int offset;
        public string filepath;
        public byte[] data;

        public void Extract(string directory, bool preserveStructure)
        {
            var dir = Directory.CreateDirectory(directory + (preserveStructure ? ("\\" + Path.GetDirectoryName(filepath) + "\\") : string.Empty));
            var location = directory + "\\" + (preserveStructure ? ("\\" + Path.GetDirectoryName(filepath) + "\\") : string.Empty) + Path.GetFileName(filepath);
            FileStream stream = new FileStream(location, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            byte[] fileBytes = data;
            if (data.Length > 4)
            {
                if (compressionType != 0)
                {
                    byte[] array = new byte[data.Length - 4];
                    Buffer.BlockCopy(data, 4, array, 0, data.Length - 4);
                    fileBytes = ZlibStream.UncompressBuffer(array);
                }
            }
            writer.Write(fileBytes);
            writer.Close();
            stream.Close();
            File.SetLastWriteTime(directory + "\\" + (preserveStructure ? ("\\" + Path.GetDirectoryName(filepath) + "\\") : string.Empty) + Path.GetFileName(filepath), timestamp);
        }
    }
}
