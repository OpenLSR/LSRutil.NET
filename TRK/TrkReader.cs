using System;
using System.IO;
using System.Text;
// ReSharper disable MemberCanBePrivate.Global

namespace LSRutil.TRK
{
    public class TrkReader
    {
        private Stream stream;
        private BinaryReader reader;

        /// <summary>
        /// Reads a string from the stream.
        /// </summary>
        /// <param name="len">The length of the string</param>
        /// <returns>The string</returns>
        private string ReadString(int len)
        {
            var data = reader.ReadBytes(len);
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Reads a track height from the stream.
        /// </summary>
        /// <returns>The track height</returns>
        private int ReadHeight()
        {
            var height = (int)reader.ReadSingle();
            return height == -1 ? 0 : Math.Min(height/8, 3);
        }

        /// <summary>
        /// Reads a track from the provided stream.
        /// </summary>
        /// <param name="stream">The stream to read the track from</param>
        /// <returns>The track as an object</returns>
        /// <exception cref="InvalidDataException">Thrown when the track file is formatted incorrectly or corrupted.</exception>
        public Track ReadTrack(Stream stream)
        {
            this.stream = stream;

            var track = new Track();

            using (reader = new BinaryReader(this.stream))
            {
                var realFilesize = (int)stream.Length;
                if (realFilesize < 20) throw new InvalidDataException("Incorrect filesize, corrupted track!");

                var legoHeader = ReadString(12);
                var trkVersion = reader.ReadUInt32();
                var claimedFilesize = reader.ReadUInt32();

                if (claimedFilesize != 65576) throw new InvalidDataException("TRK file reports incorrect filesize!");
                if (claimedFilesize != realFilesize) throw new InvalidDataException("Incorrect filesize, corrupted track!");

                track.size = (Track.TrackSize)reader.ReadUInt32();
                track.theme = (Track.TrackTheme)reader.ReadUInt32();
                track.time = (Track.TrackTime)reader.ReadUInt32();


                var iters = 8 * ((int)track.size + 1);
                var skip = (56 - (int)track.size * 8) * 16;

                for (var z = 0; z < iters; z++)
                {
                    for (var x = 0; x < iters; x++)
                    {
                        var element = new TrackElement();

                        element.mystery = reader.ReadBytes(4);
                        element.pos = new GridPosition(x, ReadHeight(), z);
                        element.SetId(reader.ReadUInt32());
                        element.rotation = (TrackElement.TrackRotation)reader.ReadUInt32();
                        

                        track.Add(element);
                    }
                    stream.Seek(skip, SeekOrigin.Current);
                }

                stream.Seek(65572, SeekOrigin.Begin);
                track.playable = reader.ReadUInt32() == 1;
            }

            return track;
        }

        /// <summary>
        /// Reads a track from the provided TRK file.
        /// </summary>
        /// <param name="filename">The filename of the TRK file</param>
        /// <returns>The track as an object</returns>
        public Track ReadTrack(string filename)
        {
            stream = File.Open(filename, FileMode.Open, FileAccess.Read);
            return ReadTrack(stream);
        }
    }
}
