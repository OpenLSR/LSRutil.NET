using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LSRutil.MVD
{
    public class MvdWriter
    {
        private Stream stream;
        private BinaryWriter writer;

        /// <summary>
        /// Writes a video to the provided stream.
        /// </summary>
        /// <param name="video">The video to write</param>
        /// <param name="stream">The stream to write the track to</param>
        public void WriteVideo(MotoVideo video, Stream stream)
        {
            this.stream = stream;

            using (writer = new BinaryWriter(stream))
            {
                writer.Write(MotoVideo.fileMagic);
                writer.Write((int)video.frames.Count);
                writer.Write(video.width);
                writer.Write(video.height);
                writer.Write(video.bitDepth);
                foreach (var frame in video.frames)
                {
                    writer.Write(frame.bytes);
                }
            }
        }

        /// <summary>
        /// Writes a video to a file with the provided filename.
        /// </summary>
        /// <param name="video">The video to write</param>
        /// <param name="filename">The filename to write the track to</param>
        public void WriteVideo(MotoVideo video, string filename)
        {
            var fileStream = File.Open(filename, FileMode.Create);
            WriteVideo(video, fileStream);
        }
    }
}
