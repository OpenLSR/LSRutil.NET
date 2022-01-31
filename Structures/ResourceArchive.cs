using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LSRutil
{
    /// <summary>
    /// Compression types supported by <see cref="ResourceFile"/> data within <c>.rfd</c> archives.
    /// </summary>
    public enum ResourceCompressionType
    {
        /// <summary>
        /// Data is stored as-is.
        /// </summary>
        Store = 0,
        /// <summary>
        /// Data is compressed using Huffman coding.
        /// </summary>
        Fast  = 1,
        /// <summary>
        /// Data is compressed using Zlib.
        /// </summary>
        Best  = 2,
    }

    public class ResourceArchive
    {
        public List<ResourceFile> resources;
        public uint nextOffset;

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
        public void Add(string file, bool compress=false, string path="")
        {
            var resFile = new ResourceFile();
            resFile.Pack(file, compress, path);
            Add(resFile);
        }

        /// <summary>
        /// Adds a file to the archive.
        /// </summary>
        /// <param name="file">The file to add.</param>
        /// <param name="compressionType">Compression method to use for storing the file.</param>
        public void Add(string file, ResourceCompressionType compressionType, string path="")
        {
            var resFile = new ResourceFile();
            resFile.Pack(file, compressionType, path);
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

        /// <summary>
        /// A helper function which lists all files in a directory relative to the directory.
        /// </summary>
        /// <remarks>
        /// Unfortunately relies on Linq, performance might not be fantastic.
        /// </remarks>
        /// <param name="directory">The directory to list files from.</param>
        /// <param name="addToResource">Adds the list of files to the archive.</param>
        /// <param name="compress">If added to resource, compress file.</param>
        /// <returns>A list of relative files to the directory.</returns>
        public List<string> GetRelativeFiles(string directory, bool addToResource = true, bool compress = false)
        {
            List<string> fileList = new List<string>();
            foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories).ToList())
            {
                string fileRel = file;
                while (fileRel.StartsWith(directory + Path.DirectorySeparatorChar))
                {
                    fileRel = fileRel.Substring(directory.Length + 1);
                }
                if (addToResource)
                {
                    Add(file, compress, fileRel);
                }
                fileList.Add(fileRel);
            }

            return fileList;
        }

        public void GenerateOffsets()
        {
            nextOffset = 0;
            foreach (var resFile in this.resources)
            {
                resFile.offset = nextOffset;
                nextOffset += resFile.compressedSize;
            }
        }
    }
    
    public class ResourceFile
    {
        public DateTime timestamp;
        public ResourceCompressionType compressionType;
        public uint compressedSize;
        public uint offset;
        public string filepath;
        public byte[] data;

        public void Unpack(string directory, bool preserveStructure)
        {
            var dir = Directory.CreateDirectory(directory + (preserveStructure ? ("\\" + Path.GetDirectoryName(filepath) + "\\") : string.Empty));
            var location = dir.FullName + Path.GetFileName(filepath);
            using (var fileStream = new FileStream(location, FileMode.Create))
            using (var fileWriter = new BinaryWriter(fileStream))
            {
                var fileBytes = data;
                //Console.WriteLine("[RF] Extracting {0}...", filepath);
                switch (compressionType)
                {
                case ResourceCompressionType.Store:
                    // data is ready to use already.
                    break;
                case ResourceCompressionType.Fast:
                    if (data.Length > 4)
                    {
                        //Console.WriteLine("[RF] Decompressing {0}...", filepath);
                        fileBytes = RF.RfCompression.UncompressHuffman(data);
                    }
                    break;
                case ResourceCompressionType.Best:
                    if (data.Length > 4)
                    {
                        //Console.WriteLine("[RF] Decompressing {0}...", filepath);
                        fileBytes = RF.RfCompression.UncompressZlib(data);
                    }
                    break;
                default:
                    throw new InvalidDataException("Unsupported compression type");
                }

                fileWriter.Write(fileBytes);
                fileWriter.Close();
                fileStream.Close();
                File.SetLastWriteTime(location, timestamp);
            }
        }

        public void Pack(string file, bool compress = false, string path = "")
        {
            ResourceCompressionType cmpType = (compress ? ResourceCompressionType.Best : ResourceCompressionType.Store);
            Pack(file, cmpType, path);
        }

        public void Pack(string file, ResourceCompressionType cmpType, string path = "")
        {
            if (path == string.Empty) path = file;
            using (var fileStream = new FileStream(file, FileMode.Open))
            using (var fileReader = new BinaryReader(fileStream))
            {
                if(fileStream.Length > int.MaxValue) throw new NotSupportedException("File is too big to fit inside of RF archive.");

                filepath = path;
                data = fileReader.ReadBytes((int)fileStream.Length);

                fileReader.Close();
                fileStream.Close();

                compressionType = cmpType;

                switch (compressionType)
                {
                case ResourceCompressionType.Store:
                    // data is already in original format.
                    break;
                case ResourceCompressionType.Fast:
                    //Console.WriteLine("[RF] Compressing {0}...", filepath);
                    data = RF.RfCompression.CompressHuffman(data);
                    break;
                case ResourceCompressionType.Best:
                    //Console.WriteLine("[RF] Compressing {0}...", filepath);
                    data = RF.RfCompression.CompressZlib(data);
                    break;
                default:
                    throw new InvalidDataException("Unsupported compression type");
                }

                compressedSize = (uint)data.Length;
                timestamp = File.GetLastWriteTime(file);
            }
        }
    }
}
