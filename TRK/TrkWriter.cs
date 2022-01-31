using System;
using System.IO;
using System.Text;
// ReSharper disable MemberCanBePrivate.Global

namespace LSRutil.TRK
{
    public class TrkWriter
    {
        private Stream stream;
        private BinaryWriter writer;

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
        private const uint trkVersion = 5;

        /// <summary>
        /// The total filesize of the TRK file in bytes.
        /// </summary>
        /// <remarks>
        /// <para>If the header has an incorrect filesize, standardized implementations will crash.</para>
        /// <para>If the file is not exactly 65,576 bytes long, standardized implementations will ignore it.</para>
        /// </remarks>
        private const uint filesize = 65576;

        /// <summary>
        /// The fixed padding bytes used in the TRK format between regions of elements.
        /// </summary>
        private static readonly byte[] trkPad = { 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 255, 255, 0, 0, 0, 0 };

        /// <summary>
        /// Creates a new TrkWriter to write a track file.
        /// </summary>
        public TrkWriter() { }

        /// <summary>
        /// Writes a string to the stream.
        /// </summary>
        /// <param name="str">The string to write</param>
        private void WriteString(string str)
        {
            writer.Write(Encoding.ASCII.GetBytes(str));
        }

        /// <summary>
        /// Serializes a track element and writes it to the stream.
        /// </summary>
        /// <param name="element">The track element to serialize</param>
        private void WriteElement(TrackElement element)
        {
            writer.Write(element.mystery);
            writer.Write(GetHeight(element.Y));
            writer.Write(element.id);
            writer.Write((uint)element.rotation);
        }

        /// <summary>
        /// Writes an empty serialized element to the stream.
        /// </summary>
        private void WriteEmptyElement()
        {
            writer.Write(new byte[] { 0, 0, 0, 0, 0, 0, 128, 191, 255, 255, 255, 255, 0, 0, 0, 0 });
        }

        /// <summary>
        /// Calculates the float height from the Y position of the track element.
        /// </summary>
        /// <param name="height">The Y position of the track element</param>
        /// <returns>The float height, for serialization</returns>
        private float GetHeight(int height)
        {
            if(height>3) height=3;
            return height == 0 ? -1 : height * 8;
        }

        /// <summary>
        /// Calculates the serialized theme byte from the track element's theme.
        /// </summary>
        /// <param name="theme">The track element's theme</param>
        /// <returns>The serialized theme byte</returns>
        private byte GetElementTheme(Track.TrackTheme theme)
        {
            switch (theme)
            {
                case Track.TrackTheme.Jungle: return 0x3B;
                case Track.TrackTheme.Ice:    return 0x3F;
                case Track.TrackTheme.Desert: return 0x47;
                case Track.TrackTheme.City:   return 0x43;
                default:                return 0x43;
            }
        }

        /// <summary>
        /// Writes the correct amount of padding to the stream depending on the track size.
        /// </summary>
        /// <param name="size">The track size</param>
        private void WriteTrackPadding(Track.TrackSize size)
        {
            var amt = 56 - (int)size * 8;

            for (var i = 0; i < amt; i++)
            {
                writer.Write(trkPad);
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
                        if (element.index != -1) WriteElement(element);
                        // Height block
                        else if (element.index == -1 && element.Y > 0)
                        {
                            element.Y -= 1;
                            element.index = 69;
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
            var correctLength = track.GetMaxElements();

            this.stream = stream;

            using (writer = new BinaryWriter(this.stream))
            {
                WriteString(legoHeader);
                writer.Write(trkVersion);
                writer.Write(filesize);
                writer.Write((uint)track.size);
                writer.Write((uint)track.theme);
                writer.Write((uint)track.time);
                var elementsWritten = WriteTrackElements(track);
                if (elementsWritten != correctLength) {
                    writer.Dispose();
                    throw new NotSupportedException(
                        $"Not enough track elements were written to the file. Got {elementsWritten}, expected {correctLength}."); 
                }

                for (var pad = 0; pad < 64; pad++)
                {
                    WriteTrackPadding(track.size);
                }

                writer.Seek(65572, SeekOrigin.Begin);
                writer.Write(track.playable?1:0);
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
