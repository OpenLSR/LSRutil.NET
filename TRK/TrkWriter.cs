using System;
using System.IO;
using System.Text;
using static LSRutil.Constants;
// ReSharper disable MemberCanBePrivate.Global

namespace LSRutil.TRK
{
    public class TrkWriter
    {
        private Stream _stream;
        private BinaryWriter _writer;

        /// <summary>
        /// Enables extended features which are not supported by the official standard.
        /// </summary>
        /// <remarks>
        /// <para>Allows you to use the <see cref="TrackSize.Mega"/> track size.</para>
        /// </remarks>
        // ReSharper disable once RedundantDefaultMemberInitializer
        public readonly bool extensions = false;

        /// <summary>
        /// The <a href="https://en.wikipedia.org/wiki/File_format#Magic_number">file magic</a> for TRK files.
        /// </summary>
        private const string legoHeader = "LEGO MOTO\0\0\0";

        /// <summary>
        /// The TRK file format version number.
        /// </summary>
        /// <remarks>
        /// <para>Anything other than 5 causes a standardized implementation to crash.</para>
        /// </remarks>
        private const int trkVersion = 5;

        /// <summary>
        /// The total filesize of the TRK file in bytes.
        /// </summary>
        /// <remarks>
        /// <para>If the header has an incorrect filesize, standardized implementations will crash.</para>
        /// <para>If the file is not exactly 65,576 bytes long, standardized implementations will ignore it.</para>
        /// </remarks>
        private const int filesize = 65576;

        /// <summary>
        /// The fixed padding bytes used in the TRK format between regions of elements.
        /// </summary>
        private static readonly byte[] trkPad = { 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 255, 255, 0, 0, 0, 0 };

        /// <summary>
        /// Creates a new TrkTrkWriter to write a track file.
        /// </summary>
        /// <param name="extensions">Enables extended features which are not supported by the official standard.</param>
        public TrkWriter(bool extensions = false) {
            this.extensions = extensions;
        }

        /// <summary>
        /// Writes a string to the stream.
        /// </summary>
        /// <param name="str">The string to write</param>
        private void WriteString(string str)
        {
            _writer.Write(Encoding.ASCII.GetBytes(str));
        }

        /// <summary>
        /// Writes an int to the stream.
        /// </summary>
        /// <param name="data">The int to write</param>
        private void WriteData(int data)
        {
            _writer.Write(BitConverter.GetBytes(data));
        }

        /// <summary>
        /// Writes a float to the stream.
        /// </summary>
        /// <param name="data">The float to write</param>
        private void WriteData(float data)
        {
            _writer.Write(BitConverter.GetBytes(data));
        }

        /// <summary>
        /// Writes a byte to the stream.
        /// </summary>
        /// <param name="data">The byte to write</param>
        private void WriteByte(byte data)
        {
            _writer.Write(data);
        }

        /// <summary>
        /// Serializes a track element and writes it to the stream.
        /// </summary>
        /// <param name="element">The track element to serialize</param>
        private void WriteElement(TrackElement element)
        {
            WriteData(element.mystery);
            WriteData(GetHeight(element.Y));
            WriteByte(element.id);
            WriteByte(GetElementTheme(element.theme));
            _writer.Write(new byte[] {0,0}); // more padding
            WriteData((int)element.rotation);
        }

        /// <summary>
        /// Writes an empty serialized element to the stream.
        /// </summary>
        private void WriteEmptyElement()
        {
            _writer.Write(new byte[] { 0, 0, 0, 0, 0, 0, 128, 191, 255, 255, 255, 255, 0, 0, 0, 0 });
        }

        /// <summary>
        /// Calculates the float height from the Y position of the track element.
        /// </summary>
        /// <param name="height">The Y position of the track element</param>
        /// <returns>The float height, for serialization</returns>
        private float GetHeight(int height)
        {
            return height == 0 ? -1 : height * 8;
        }

        /// <summary>
        /// Calculates the serialized theme byte from the track element's theme.
        /// </summary>
        /// <param name="theme">The track element's theme</param>
        /// <returns>The serialized theme byte</returns>
        private byte GetElementTheme(TrackTheme theme)
        {
            switch (theme)
            {
                case TrackTheme.Jungle: return 0x3B;
                case TrackTheme.Ice:    return 0x3F;
                case TrackTheme.Desert: return 0x47;
                case TrackTheme.City:   return 0x43;
                default:                return 0x43;
            }
        }

        /// <summary>
        /// Writes the correct amount of padding to the stream depending on the track size.
        /// </summary>
        /// <param name="size">The track size</param>
        private void WriteTrackPadding(TrackSize size)
        {
            var amt = 56 - (int)size * 8;

            for (var i = 0; i < amt; i++)
            {
                _writer.Write(trkPad);
            }
        }

        /// <summary>
        /// Writes all of the track elements in a track to the stream.
        /// </summary>
        /// <param name="track">The track to write to the stream</param>
        /// <returns>Amount of track elements written</returns>
        private int WriteTrackElements(Track track)
        {
            var written = 0;
            var loop = (int)Math.Sqrt(track.GetMaxElements());

            var elements = track.GetElements();

            for (var z = 0; z < loop; z++)
            {
                for (var x = 0; x < loop; x++)
                {
                    var element = elements.Find(l => l.X.Equals(x) && l.Z.Equals(z));
                    // There is an element at these coordinates
                    if (element != null)
                    {
                        if (element.xid != -1) WriteElement(element);
                        // Height block
                        else if (element.xid == -1 && element.Y > 0)
                        {
                            element.Y -= 1;
                            element.xid = 69;
                            WriteElement(element);
                        }
                    }
                    else WriteEmptyElement();
                    
                    written++;
                }
                WriteTrackPadding(track.size);
            }

            return written;
        }

        /// <summary>
        /// Writes a track to the provided stream.
        /// </summary>
        /// <param name="track">The track to write</param>
        /// <param name="stream">The stream to write the track to</param>
        public void WriteTrack(Track track, Stream stream)
        {
            if (!extensions && track.size > TrackSize.Singleplayer) throw new NotSupportedException($"Please turn extensions on to write {track.size} size maps.");
            var correctLength = track.GetMaxElements();

            _stream = stream;

            using (_writer = new BinaryWriter(_stream))
            {
                WriteString(legoHeader);
                WriteData(trkVersion);
                WriteData(filesize);
                WriteData((int)track.size);
                WriteData((int)track.theme);
                WriteData((int)track.time);
                var elementsWritten = WriteTrackElements(track);
                if (elementsWritten != correctLength) {
                    _writer.Dispose();
                    throw new NotSupportedException(
                        $"Not enough track elements were written to the file. Got {elementsWritten}, expected {correctLength}."); 
                }

                for (var pad = 0; pad < 64; pad++)
                {
                    WriteTrackPadding(track.size);
                }

                _writer.Seek(65572,SeekOrigin.Begin);
                WriteData(track.playable?1:0);
                //Console.WriteLine("Wrote {0} bytes.", writer.BaseStream.Position);
            }
        }

        /// <summary>
        /// Writes a track to a file with the provided filename.
        /// </summary>
        /// <param name="track">The track to write</param>
        /// <param name="filename">The filename to write the track to</param>
        public void WriteTrack(Track track, string filename)
        {
            var fileStream = File.Open(filename, FileMode.Create);
            WriteTrack(track, fileStream);
        }

    }
}
