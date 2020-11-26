using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;

namespace LSRutil
{
    public class ResourceArchive
    {
        public List<ResourceFile> resources;
        public int nextOffset;

        /// <summary>
        /// Instanciates a new ResourceArchive based on a List of ResourceFiles.
        /// </summary>
        /// <param name="resList">a List of ResourceFiles.</param>
        /// <remarks>NOTE: This assumes resources are in order from lowest to highest offset.</remarks>
        public ResourceArchive(List<ResourceFile> resList)
        {
            resources = resList;
            var lastRes = resList[resList.Count-1]; // NOTE: This assumes resources are in order from lowest to highest offset.
            nextOffset = lastRes.offset + lastRes.compressedSize;
        }

        /// <summary>
        /// Instanciates a new ResourceArchive.
        /// </summary>
        public ResourceArchive() {
            resources = new List<ResourceFile>();
            nextOffset = 0;
        }

        /// <summary>
        /// Adds a ResourceFile to the archive.
        /// </summary>
        /// <param name="resFile">The ResourceFile to add.</param>
        public void Add(ResourceFile resFile)
        {
            resFile.offset = nextOffset;
            resources.Add(resFile);
            nextOffset += resFile.compressedSize;
        }

        /// <summary>
        /// Adds a file to the archive.
        /// </summary>
        /// <param name="file">The file to add.</param>
        /// <param name="compress">If true, compresses file with zlib.</param>
        public void Add(string file, bool compress=false)
        {
            var resFile = new ResourceFile();
            resFile.Pack(file, compress);
            Add(resFile);
        }

        /// <summary>
        /// Extracts all files in the archive to selected directory.
        /// </summary>
        /// <param name="directory">Directory to extract files to.</param>
        /// <returns>Amount of files extracted.</returns>
        public int ExtractAllFiles(string directory)
        {
            if (resources.Count <= 0) return 0;
            var counter = 0;
            foreach (var file in resources)
            {
                file.Unpack(directory, true);
                counter++;
            }
            return counter;
        }
    }
    
    public class ResourceFile
    {
        public DateTime timestamp;
        public int compressionType;
        public int compressedSize;
        public int offset;
        public string filepath;
        public byte[] data;

        public void Unpack(string directory, bool preserveStructure)
        {
            var dir = Directory.CreateDirectory(directory + (preserveStructure ? ("\\" + Path.GetDirectoryName(filepath) + "\\") : string.Empty));
            var location = dir.FullName + Path.GetFileName(filepath);
            var stream = new FileStream(location, FileMode.Create);
            var writer = new BinaryWriter(stream);
            var fileBytes = data;
            //Console.WriteLine("[RF] Extracting {0}...", filepath);
            if (data.Length > 4)
            {
                if (compressionType != 0)
                {
                    //Console.WriteLine("[RF] Decompressing {0}...", filepath);
                    var array = new byte[data.Length - 4];
                    Buffer.BlockCopy(data, 4, array, 0, data.Length - 4);
                    fileBytes = ZlibStream.UncompressBuffer(array);
                }
            }
            writer.Write(fileBytes);
            writer.Close();
            stream.Close();
            File.SetLastWriteTime(location, timestamp);
        }

        public void Pack(string file, bool compress = false)
        {
            using (var fileStream = new FileStream(file, FileMode.Open))
            using (var fileReader = new BinaryReader(fileStream))
            {
                if(fileStream.Length > int.MaxValue) throw new NotSupportedException("File is too big to fit inside of RF archive.");

                filepath = file;
                data = fileReader.ReadBytes((int)fileStream.Length);

                fileReader.Close();
                fileStream.Close();

                if(compress)
                {
                    throw new NotImplementedException("Compression not implemented yet.");
                    compressionType = 2;
                    data = ZlibStream.CompressBuffer(data);
                } else compressionType = 0;

                compressedSize = data.Length;
                timestamp = File.GetLastWriteTime(file);
            }
        }
    }
}
