using System;
using System.IO;
using System.Numerics;
using System.Text;
using static LSRutil.Constants;

namespace LSRutil
{
    public class TRKReader
    {
        private Stream stream;
        private BinaryReader reader;

        /// <summary>
        /// Instanciates a new TRKReader to read a track file.
        /// </summary>
        public TRKReader() { }

        /// <summary>
        /// Reads a string from the stream.
        /// </summary>
        /// <param name="len">The length of the string</param>
        /// <returns>The string</returns>
        private string ReadString(int len)
        {
            byte[] data = reader.ReadBytes(len);
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Reads an integer from the stream.
        /// </summary>
        /// <returns>The integer</returns>
        private int ReadInt()
        {
            byte[] data = reader.ReadBytes(4);
            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        /// Reads a float from the stream.
        /// </summary>
        /// <returns>The float</returns>
        private float ReadFloat()
        {
            byte[] data = reader.ReadBytes(4);
            return BitConverter.ToSingle(data, 0);
        }

        /// <summary>
        /// Reads a track height from the stream.
        /// </summary>
        /// <returns>The track height</returns>
        private int ReadHeight()
        {
            int height = (int)ReadFloat();
            return height == -1 ? 0 : height/8;
        }

        /// <summary>
        /// Reads a track element theme from the stream.
        /// </summary>
        /// <returns>The track element theme</returns>
        private TrackTheme ReadElementTheme()
        {
            int data = reader.ReadByte();
            TrackTheme theme;

            switch (data)
            {
                case 0x3B: theme = TrackTheme.Jungle; break;
                case 0x3F: theme = TrackTheme.Ice;    break;
                case 0x47: theme = TrackTheme.Desert; break;
                case 0x43: theme = TrackTheme.City;   break;
                default:   theme = TrackTheme.City;   break;
            }

            return theme;
        }

        /// <summary>
        /// Reads a track from the provided stream.
        /// </summary>
        /// <param name="stream">The stream to read the track from</param>
        /// <returns>The track as an object</returns>
        /// <exception cref="InvalidDataException">Thrown when the track file is formatted incorrectly.</exception>
        public Track ReadTrack(Stream stream)
        {
            this.stream = stream;

            var track = new Track();

            using (reader = new BinaryReader(stream))
            {
                int RealFilesize = (int)stream.Length;
                string LEGOHeader = ReadString(12);
                int TRKVersion = ReadInt();
                int ClaimedFilesize = ReadInt();

                if (ClaimedFilesize != 65576) throw new InvalidDataException("TRK file reports incorrect filesize!");
                else if (ClaimedFilesize != RealFilesize) throw new InvalidDataException("Incorrect filesize, corrupted track!");

                track.size = (TrackSize)ReadInt();
                track.theme = (TrackTheme)ReadInt();
                track.time = (TrackTime)ReadInt();


                int iters = 8 * ((int)track.size + 1);
                int skip = (56 - ((int)track.size * 8)) * 16;

                for (int z = 0; z < iters; z++)
                {
                    for (int x = 0; x < iters; x++)
                    {
                        var element = new TrackElement();

                        stream.Seek(4, SeekOrigin.Current); // skip padding
                        var height = ReadHeight();
                        var id = reader.ReadByte();
                        element.theme = ReadElementTheme();
                        element.SetId(id);
                        stream.Seek(2, SeekOrigin.Current);
                        element.rotation = (TrackRotation)reader.ReadByte();
                        stream.Seek(3, SeekOrigin.Current);
                        element.pos = new Vector3(x, height, z);

                        track.Add(element);
                    }
                    stream.Seek(skip, SeekOrigin.Current);
                }
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
