using System;
using System.IO;
using System.Text;
using static LSRutil.Constants;
// ReSharper disable MemberCanBePrivate.Global

namespace LSRutil.TRK
{
    public class TrkReader
    {
        private Stream _stream;
        private BinaryReader _reader;

        /// <summary>
        /// Enables extended features which are not supported by the official standard.
        /// </summary>
        /// <remarks>
        /// <para>Allows you to use the <see cref="TrackSize.Mega"/> track size.</para>
        /// </remarks>
        // ReSharper disable once RedundantDefaultMemberInitializer
        public readonly bool extensions = false;

        /// <summary>
        /// Instanciates a new TrkReader to read a track file.
        /// </summary>
        /// <param name="extensions">Enables extended features which are not supported by the official standard.</param>
        public TrkReader(bool extensions = false) {
            this.extensions = extensions;
        }

        /// <summary>
        /// Reads a string from the stream.
        /// </summary>
        /// <param name="len">The length of the string</param>
        /// <returns>The string</returns>
        private string ReadString(int len)
        {
            var data = _reader.ReadBytes(len);
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Reads an integer from the stream.
        /// </summary>
        /// <returns>The integer</returns>
        private int ReadInt()
        {
            var data = _reader.ReadBytes(4);
            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        /// Reads a float from the stream.
        /// </summary>
        /// <returns>The float</returns>
        private float ReadFloat()
        {
            var data = _reader.ReadBytes(4);
            return BitConverter.ToSingle(data, 0);
        }

        /// <summary>
        /// Reads a track height from the stream.
        /// </summary>
        /// <returns>The track height</returns>
        private int ReadHeight()
        {
            var height = (int)ReadFloat();
            return height == -1 ? 0 : (extensions ? height/8 : Math.Min(height/8, 3));
        }

        /// <summary>
        /// Reads a track element theme from the stream.
        /// </summary>
        /// <returns>The track element theme</returns>
        private TrackTheme ReadElementTheme()
        {
            int data = _reader.ReadByte();
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
            _stream = stream;

            var track = new Track();

            using (_reader = new BinaryReader(_stream))
            {
                var realFilesize = (int)stream.Length;
                var legoHeader = ReadString(12);
                var trkVersion = ReadInt();
                var claimedFilesize = ReadInt();

                if (claimedFilesize != 65576) throw new InvalidDataException("TRK file reports incorrect filesize!");
                if (claimedFilesize != realFilesize) throw new InvalidDataException("Incorrect filesize, corrupted track!");

                track.size = (TrackSize)ReadInt();
                if(!extensions && track.size > TrackSize.Singleplayer) throw new NotSupportedException($"Please turn extensions on to read {track.size} size maps.");
                track.theme = (TrackTheme)ReadInt();
                track.time = (TrackTime)ReadInt();


                var iters = 8 * ((int)track.size + 1);
                var skip = (56 - (int)track.size * 8) * 16;

                for (var z = 0; z < iters; z++)
                {
                    for (var x = 0; x < iters; x++)
                    {
                        var element = new TrackElement();

                        element.mystery = ReadInt();
                        var height = ReadHeight();
                        var id = _reader.ReadByte();
                        element.theme = ReadElementTheme();
                        element.SetId(id);
                        stream.Seek(2, SeekOrigin.Current);
                        element.rotation = (TrackRotation)_reader.ReadByte();
                        stream.Seek(3, SeekOrigin.Current);
                        element.pos = new GridPosition(x, height, z);

                        track.Add(element);
                    }
                    stream.Seek(skip, SeekOrigin.Current);
                }

                stream.Seek(65572, SeekOrigin.Begin);
                track.playable = ReadInt() == 1;
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
            _stream = File.Open(filename, FileMode.Open, FileAccess.Read);
            return ReadTrack(_stream);
        }
    }
}
