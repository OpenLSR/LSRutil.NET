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
        
        private Encoding encoding;

        /// <summary>
        /// Loads the archive from a header and data stream with specified filename.
        /// </summary>
        /// <param name="headerStream">The stream from the RFH file.</param>
        /// <param name="dataStream">The stream from the RFD file.</param>
        /// <returns>A <see cref="ResourceArchive"/> with the loaded streams' data.</returns>
        public ResourceArchive ReadArchive(Stream headerStream, Stream dataStream, Encoding encoding = null)
        {
            this.headerStream = headerStream;
            this.dataStream = dataStream;
            this.encoding = encoding ?? Encoding.GetEncoding(1252);

            var archive = new ResourceArchive();
            
            using (headerReader = new BinaryReader(headerStream))
            using (dataReader = new BinaryReader(dataStream))
            {
                do
                {
                    var pathLen = headerReader.ReadUInt32();
                    var timestamp = DateTimeOffset.FromUnixTimeSeconds(headerReader.ReadUInt32()).DateTime;
                    var cmpType = (ResourceCompressionType)headerReader.ReadUInt32();
                    var cmpSize = headerReader.ReadUInt32();
                    var offset = headerReader.ReadUInt32();
                    var relPath = headerReader.ReadBytes((int)pathLen);
                    dataReader.BaseStream.Seek(offset, SeekOrigin.Begin);
                    var data = dataReader.ReadBytes((int)cmpSize);
                    
                    archive.resources.Add(new ResourceFile
                    {
                        filepath = this.encoding.GetString(relPath).Trim('\0'), // Null terminated strings aren't a thing in C#.
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
            var fileExt = Path.GetExtension(file).ToUpper();
            if(fileExt==".RFH"||fileExt==".RFD") file = Path.ChangeExtension(file, null);
            headerStream = new FileStream(file+".RFH", FileMode.Open);
            dataStream = new FileStream(file+".RFD", FileMode.Open);

            return ReadArchive(headerStream, dataStream);
        }
    }
}