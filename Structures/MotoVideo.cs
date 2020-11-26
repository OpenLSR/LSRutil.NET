using System;
using System.Collections.Generic;
using System.IO;

namespace LSRutil
{
    public class MotoVideo
    {
        public static readonly byte[] fileMagic = { 
            0x4D, 0x4F, 0x54, 0x4F, 0x5F, 0x56, 0x49, 0x44, 0x45, 0x4F, 0x20, 0x31, 0x00, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC 
        };

        /// <summary>
        /// Number of frames in the video.
        /// </summary>
        public int numFrames;

        /// <summary>
        /// The width of the video in pixels.
        /// </summary>
        public int width;
        /// <summary>
        /// The height of the video in pixels.
        /// </summary>
        public int height;

        /// <summary>
        /// The bit depth of the video.
        /// </summary>
        /// <remarks>
        /// This value is 16 when loading official videos.
        /// </remarks>
        public int bitDepth;

        /// <summary>
        /// A <see cref="System.Collections.Generic.List{T}"/> of <see cref="MotoVideoFrame"/> objects.
        /// </summary>
        public List<MotoVideoFrame> frames = new List<MotoVideoFrame>();

        /// <summary>
        /// Instanciates a new MotoVideo object with preset parameters.
        /// </summary>
        /// <param name="numFrames">The number of frames in the video.</param>
        /// <param name="width">The width of the video.</param>
        /// <param name="height">The height of the video.</param>
        public MotoVideo(int numFrames, int width, int height)
        {
            this.numFrames = numFrames;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Instanciates a new <see cref="MotoVideo"/>.
        /// </summary>
        public MotoVideo() { }

        /// <summary>
        /// Prints information about this video to the console. This should only be used for debugging.
        /// </summary>
        public void GetInfo()
        {
            Console.WriteLine("## Video information ##");
            Console.WriteLine("Video Frames: {0}", numFrames);
            Console.WriteLine("Video Resolution: {0}x{1}", width, height);
            Console.WriteLine("Bit Depth: {0}bpp", bitDepth);
            Console.WriteLine("---");
        }
    }

    public class MotoVideoFrame
    {
        //RGB565, 16-bit
        public byte[] bytes;

        public int width;
        public int height;

        public int bitDepth;

        public MotoVideoFrame(int width, int height, int bitDepth = 16)
        {
            this.width = width;
            this.height = height;
            this.bitDepth = bitDepth;
        }

        public MotoVideoFrame(int width, int height, int bitDepth, byte[] bytes)
        {
            this.bytes = bytes;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Prints information about this frame to the console. This should only be used for debugging.
        /// </summary>
        public void GetInfo()
        {
            Console.WriteLine("## Frame information ##");
            Console.WriteLine("Frame Bytes: {0}", bytes.Length);
            Console.WriteLine("Frame Resolution: {0}x{1}", width, height);
            Console.WriteLine("Bit Depth: {0}bpp", bitDepth);
            Console.WriteLine("---");
        }

        public void DumpFrame(string filename = "dumpedFrame.rgb565.raw")
        {
            using var writer = new BinaryWriter(File.Open(filename, FileMode.Create));
            writer.Write(bytes);
        }
    }
}
