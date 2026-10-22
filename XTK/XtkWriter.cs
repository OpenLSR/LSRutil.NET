using System;
using System.IO;
using System.Text;

namespace LSRutil.XTK
{
    public class XtkWriter
    {
        private Stream stream;
        private BinaryWriter writer;

        private const string xtkHeader = "XTK";
        private const byte xtkVersion = 2;

        /// <summary>
        /// Writes a string to the stream.
        /// </summary>
        /// <param name="str">The string to write</param>
        private void WriteString(string str)
        {
            if (str.Length > byte.MaxValue) throw new Exception("String was too long");
            writer.Write((byte)str.Length);
            writer.Write(Encoding.ASCII.GetBytes(str));
        }

        /// <summary>
        /// Serializes a track element and writes it to the stream.
        /// </summary>
        /// <param name="element">The track element to serialize</param>
        private void WriteElement(ExtendedTrackElement piece)
        {
            if (piece.metadata.Count > 127) throw new Exception("Metadata contains too many elements");
            writer.Write((byte)piece.pos.X);
            writer.Write((byte)piece.pos.Y);
            writer.Write((byte)piece.pos.Z);
            writer.Write((byte)piece.index);
            writer.Write((byte)piece.theme);
            writer.Write((byte)piece.rotation);
            writer.Write(piece.variant);

            writer.Write((byte)piece.metadata.Count);
            foreach (var meta in piece.metadata)
            {
                if (meta.Key > 127) throw new Exception("Attempted to save invalid metadata key");
                writer.Write(meta.Key);
                writer.Write(meta.Value);
            }
        }

        public void WriteTrack(ExtendedTrack track, Stream stream)
        {
            this.stream = stream;

            using (writer = new BinaryWriter(this.stream))
            {
                // header
                writer.Write(Encoding.ASCII.GetBytes(xtkHeader));
                writer.Write(xtkVersion);
                writer.Write(track.size_x);
                writer.Write(track.size_y);
                writer.Write((byte)track.theme);
                writer.Write(track.x_time);
                writer.Write(track.playable);
                
                // track meta
                WriteString(track.name);
                WriteString(track.author);
                WriteString(track.description);
                
                // pieces
                foreach (var piece in track.GetElements())
                {
                    WriteElement(piece as ExtendedTrackElement);
                }
            }
        }
        
        /// <summary>
        /// Writes a track to a file with the provided filename.
        /// </summary>
        /// <param name="track">The track to write</param>
        /// <param name="filename">The filename to write the track to</param>
        public void WriteTrack(ExtendedTrack track, string filename)
        {
            var fileStream = File.Open(filename, FileMode.Create);
            WriteTrack(track, fileStream);
        }
    }
}