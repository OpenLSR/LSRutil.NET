using System;
using System.IO;

namespace LSRutil.MVD
{
    /// <summary>
    /// Class for reading MVD files.
    /// </summary>
    public class MvdReader
    {
        private Stream stream;
        private BinaryReader reader;

        /// <summary>
        /// Reads a video from the provided stream.
        /// </summary>
        /// <param name="stream">The stream to read the video from</param>
        /// <returns>The video as an object</returns>
        /// <exception cref="InvalidDataException">Thrown when the video file is formatted incorrectly.</exception>
        public MotoVideo ReadVideo(Stream stream)
        {
            this.stream = stream;

            var video = new MotoVideo();

            using (reader = new BinaryReader(stream))
            {
                var videoHeader = reader.ReadBytes(20);

                // TODO: equality check for file magic.
                //if (!videoHeader.Equals(MotoVideo.fileMagic)) throw new InvalidDataException("MVD file has incorrct header!");

                var numFrames = ReadInt();
                video.numFrames = numFrames;

                var width = ReadInt();
                var height = ReadInt();
                video.width = width;
                video.height = height;

                var bitDepth = ReadInt();
                video.bitDepth = bitDepth;

                var frameSize = width * height * (bitDepth / 8);
                for (var fr = 0; fr < numFrames; fr++)
                {
                    var frame = new MotoVideoFrame(width, height, bitDepth);

                    frame.bytes = reader.ReadBytes(frameSize);

                    if (frame.bytes.Length != frameSize) throw new InvalidDataException($"Invalid size on frame {fr}!");

                    video.frames.Add(frame);
                }
            }

            return video;
        }

        /// <summary>
        /// Reads a video from the provided MVD file.
        /// </summary>
        /// <param name="filename">The filename of the MVD file</param>
        /// <returns>The video as an object</returns>
        public MotoVideo ReadVideo(string filename)
        {
            stream = File.Open(filename, FileMode.Open, FileAccess.Read);
            return ReadVideo(stream);
        }

        /// <summary>
        /// Reads an integer from the stream.
        /// </summary>
        /// <returns>The integer</returns>
        private int ReadInt()
        {
            var data = reader.ReadBytes(4);
            return BitConverter.ToInt32(data, 0);
        }

        public void CloseHandles()
        {
            reader.Close();
            stream.Close();
        }
    }
}
