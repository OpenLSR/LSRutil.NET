using System;
using System.IO;
using System.Text;

namespace LSRutil.RF
{
    /// <summary>
    /// Class for reading RFH/RFD files.
    /// </summary>
    public class RfReader
    {
        private Stream headerStream;
        private BinaryReader headerReader;
        private Stream dataStream;
        private BinaryReader dataReader;

        /// <summary>
        /// Loads the archive from a header and data stream with specified filename.
        /// </summary>
        /// <param name="headerStream">The stream from the RFH file.</param>
        /// <param name="dataStream">The stream from the RFD file.</param>
        /// <returns>A <see cref="ResourceArchive"/> with the loaded streams' data.</returns>
        public ResourceArchive ReadArchive(Stream headerStream, Stream dataStream)
        {
            this.headerStream = headerStream;
            this.dataStream = dataStream;

            var archive = new ResourceArchive();
            
            using (headerReader = new BinaryReader(headerStream))
            using (dataReader = new BinaryReader(dataStream))
            {
                do
                {
                    var pathLen = headerReader.ReadInt32();
                    var timestamp = DateTimeOffset.FromUnixTimeSeconds(headerReader.ReadUInt32()).DateTime;
                    var cmpType = (ResourceCompressionType)headerReader.ReadInt32();
                    var cmpSize = headerReader.ReadInt32();
                    var offset = headerReader.ReadInt32();
                    var relPath = headerReader.ReadBytes(pathLen);
                    dataReader.BaseStream.Seek(offset, SeekOrigin.Begin);
                    var data = dataReader.ReadBytes(cmpSize);
                    archive.resources.Add(new ResourceFile
                    {
                        filepath = Encoding.ASCII.GetString(relPath).Trim('\0'), // Null terminated strings aren't a thing in C#.
                        timestamp = timestamp,
                        compressionType = cmpType,
                        compressedSize = cmpSize,
                        offset = offset,
                        data = data
                    });
                    archive.nextOffset = offset + cmpSize;
                }
                while (headerReader.BaseStream.Position < headerReader.BaseStream.Length); 
            }

            return archive;
        }

        /// <summary>
        /// Loads the archive from a header and data file with specified filename.
        /// </summary>
        /// <param name="file">The file to load from. Extensions are optional.</param>
        /// <exception cref="FileNotFoundException">One of the files was not found.</exception>
        /// <returns>A <see cref="ResourceArchive"/> with the loaded files' data.</returns>
        public ResourceArchive ReadArchive(string file)
        {
            var fileExt = Path.GetExtension(file).ToLower();
            if(fileExt==".rfh"||fileExt==".rfd") file = Path.ChangeExtension(file, null);
            headerStream = new FileStream(file+".rfh", FileMode.Open);
            dataStream = new FileStream(file+".rfd", FileMode.Open);

            return ReadArchive(headerStream, dataStream);
        }
    }
}