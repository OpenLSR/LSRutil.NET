using System.IO;
using System.Text;

namespace LSRutil.XTK
{
    public class XtkReader
    {
        private Stream stream;
        private BinaryReader reader;
        
        /// <summary>
        /// Reads a string from the stream.
        /// </summary>
        /// <param name="len">The length of the string</param>
        /// <returns>The string</returns>
        private string ReadString(int len = -1)
        {
            if (len == -1) len = reader.ReadByte();
            var data = reader.ReadBytes(len);
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Reads a track from the provided stream.
        /// </summary>
        /// <param name="stream">The stream to read the track from</param>
        /// <returns>The track as an object</returns>
        /// <exception cref="InvalidDataException">Thrown when the track file is formatted incorrectly or corrupted.</exception>
        public ExtendedTrack ReadTrack(Stream stream)
        {
            this.stream = stream;

            var track = new ExtendedTrack();

            using (reader = new BinaryReader(this.stream))
            {
                // header
                var xtkMagic = ReadString(3);
                var version = reader.ReadByte();
                track.size_x = reader.ReadByte();
                track.size_y = reader.ReadByte();
                track.theme = (Track.TrackTheme)reader.ReadByte();
                track.x_time = reader.ReadByte();
                track.playable = reader.ReadBoolean();
                
                // track meta
                track.name = ReadString();
                track.author = ReadString();
                track.description = ReadString();
                
                while (stream.Position != stream.Length)
                {
                    var piece = new ExtendedTrackElement();
                    piece.pos = new GridPosition(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                    piece.index = reader.ReadByte();
                    piece.theme = (Track.TrackTheme)reader.ReadByte();
                    piece.rotation = (TrackElement.TrackRotation)reader.ReadByte();
                    piece.variant = reader.ReadByte();
                    var metadata_length = reader.ReadByte();
                    for (int i = 0; i < metadata_length; i++)
                    {
                        piece.metadata.Add(reader.ReadByte(), reader.ReadByte());
                    }
                }
            }

            return track;
        }

        /// <summary>
        /// Reads a track from the provided XTK file.
        /// </summary>
        /// <param name="filename">The filename of the XTK file</param>
        /// <returns>The track as an object</returns>
        public ExtendedTrack ReadTrack(string filename)
        {
            stream = File.Open(filename, FileMode.Open, FileAccess.Read);
            return ReadTrack(stream);
        }
    }
}