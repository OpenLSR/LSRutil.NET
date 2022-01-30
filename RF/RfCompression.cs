using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;

namespace LSRutil.RF
{
    /// <summary>
    /// Compress and Uncompress functions used by <see cref="ResourceFile"/> data within <c>.rfd</c> archives.
    /// </summary>
    public static class RfCompression
    {
        // In the future, these functions can be rewritten/overloaded to directly take a Stream as output.

        /// <summary>
        /// Decompresses <see cref="ResourceFile"/> data using <see cref="ResourceCompressionType.Best"/>.
        /// </summary>
        /// <param name="data">Compressed file data, including the original size header field.</param>
        /// <returns>Original file data.</returns>
        public static byte[] UncompressZlib(byte[] data)
        {
            // Rewrite of the following, while also handling the original data size field:
            // * ZlibStream.UncompressBuffer(byte[] compressed)
            // * ZlibBaseStream.UncompressBuffer(byte[] compressed, Stream decompressor)
            using (var input = new MemoryStream(data))
            using (var output = new MemoryStream())
            {
                // Skip original data size field.
                //  (Zlib already has this information in its encoding)
                input.Seek(4, SeekOrigin.Begin);

                using (Stream decompressor = new ZlibStream(input, CompressionMode.Decompress))
                {
                    byte[] working = new byte[1024];
                    int n;
                    while ((n = decompressor.Read(working, 0, working.Length)) != 0)
                    {
                        output.Write(working, 0, n);
                    }
                }
                return output.ToArray();
            }
        }

        /// <summary>
        /// Compresses <see cref="ResourceFile"/> data using <see cref="ResourceCompressionType.Best"/>.
        /// </summary>
        /// <param name="data">Original file data.</param>
        /// <returns>Compressed file data, including the original size header field.</returns>
        public static byte[] CompressZlib(byte[] data)
        {
            // Rewrite of the following, while also handling the original data size field:
            // * ZlibStream.CompressBuffer(byte[] b)
            // * ZlibBaseStream.CompressBuffer(byte[] b, Stream compressor)
            using (var output = new MemoryStream())
            using (var writer = new BinaryWriter(output))
            {
                // Write original data size field.
                writer.Write((Int32)data.Length);

                using (Stream compressor = new ZlibStream(output, CompressionMode.Compress, CompressionLevel.BestCompression))
                {
                    compressor.Write(data, 0, data.Length);
                }
                return output.ToArray();
            }
        }

        /// <summary>
        /// Decompresses <see cref="ResourceFile"/> data using <see cref="ResourceCompressionType.Fast"/>.
        /// </summary>
        /// <param name="data">Compressed file data, including the original size header field.</param>
        /// <returns>Original file data.</returns>
        public static byte[] UncompressHuffman(byte[] data)
        {
            // Read original data size field.
            int origSize = BitConverter.ToInt32(data, 0);
            byte[] orig = new byte[origSize];

            UncompressHuffmanBase(data, 4, orig);

            return orig;
        }

        /// <summary>
        /// Compresses <see cref="ResourceFile"/> data using <see cref="ResourceCompressionType.Fast"/>.
        /// </summary>
        /// <param name="data">Original file data.</param>
        /// <returns>Compressed file data, including the original size header field.</returns>
        public static byte[] CompressHuffman(byte[] data)
        {
            using (var output = new MemoryStream())
            using (var writer = new BinaryWriter(output))
            {
                // Write original data size field.
                writer.Write((Int32)data.Length);

                CompressHuffmanBase(data, writer);

                return output.ToArray();
            }
        }

        // Based on <msr.exe EN @004d9c00>
        // DOES NOT include reading the original data size field. Skip this field by passing 4 to index.
        internal static void UncompressHuffmanBase(byte[] data, int index, byte[] orig)
        {
            //int origSize = BitConverter.ToInt32(data, 0x0);
            //byte[] orig = new byte[origSize];

            // Root value/code used in Huffman tree lookup.
            ushort initialCode = (ushort)BitConverter.ToUInt32(data, index + 0x0);

            // Huffman tree. Each lookup follows a branch down the tree until value < 0x100.
            int treeStart = index + 0x4;
            ushort[] treeCodes = new ushort[0x400];
            Buffer.BlockCopy(data, treeStart, treeCodes, 0, 0x800);

            // Bit buffer is read as little-endian in 32-bit integer blocks.
            int nextData = index + 0x4 + 0x800;
            int bitsLeft = 32;
            uint bitBuff = BitConverter.ToUInt32(data, nextData);
            nextData += 4;

            for (int i = 0; i < orig.Length; i++)
            {
                // Transform value through huffman tree lookup until we have a single byte.
                ushort value = initialCode;
                // Values < 0x100 are final byte values.
                //  (if initialCode < 0x100, then the entire file contains only one unqiue byte value)
                while (value >= 0x100)
                {
                    value = treeCodes[(ushort)(value<<1) | (bitBuff & 0x1)];

                    // Move bit buffer after consuming bit used in value.
                    bitBuff >>= 1;
                    if (--bitsLeft == 0)
                    {
                        bitsLeft = 32;
                        // Some file formats poorly handle flushing of the final bit buffer block.
                        if (nextData < data.Length)
                        {
                            bitBuff = BitConverter.ToUInt32(data, nextData);
                            nextData += 4;
                        }
                    }
                }

                orig[i] = (byte)value;
            }

            //return orig;
        }


        internal class HuffmanTreeNode
        {
            /// <summary>
            /// Value of this node in the tree. This is a leaf node when <c><see cref="code"/> &lt; 0x100</c>.
            /// </summary>
            public ushort code;
            /// <summary>
            /// Number of times this leaf or branch node appears in the file.
            /// </summary>
            public uint frequency;
            /// <summary>
            /// Index of this node in the tree. Determines which side this node branches out from using <see cref="Bit"/>.
            /// </summary>
            public ushort treeIndex;

            public HuffmanTreeNode parent;

            /// <summary>
            /// Branching bit: <c>0x0</c> when left node, <c>0x1</c> when right node.
            /// </summary>
            public byte Bit => (byte) (treeIndex & 0x1);
        }

        // based on <msr.exe EN @004d9880>
        // DOES NOT include writing the original data size field.
        internal static void CompressHuffmanBase(byte[] data, BinaryWriter writer)
        {
            //long startPosition = writer.BaseStream.Position;

            HuffmanTreeNode[] leafNodes = new HuffmanTreeNode[256];

            // Initial tree setup, by counting appearances of byte values.
            List<HuffmanTreeNode> workingNodes = new List<HuffmanTreeNode>();
            foreach (byte b in data)
            {
                if (leafNodes[b] == null)
                {
                    var newNode = new HuffmanTreeNode
                    {
                        code = (ushort) b
                    };
                    leafNodes[b] = newNode;
                    workingNodes.Add(newNode);
                }
                leafNodes[b].frequency++;
            }


            // Build a huffman tree starting with least-frequent to most-frequent nodes.
            ushort[] treeCodes = new ushort[0x400];

            ushort nextCode = 0x100; // first code after non-byte values
            HuffmanTreeNode dummyNode = new HuffmanTreeNode
            {
                frequency = uint.MaxValue // dummy frequency that we can't go higher than.
            };
            while (workingNodes.Count > 1)
            {
                HuffmanTreeNode leftNode = dummyNode, rightNode = dummyNode; // dummy inits
                int leftIndex = -1, rightIndex = -1;

                // Find next two lowest nodes.
                for (int j = 0; j < workingNodes.Count; j++)
                {
                    var node = workingNodes[j];

                    if (node.frequency < leftNode.frequency)
                    {
                        // First-lowest node, old left node is moved to right node.
                        rightNode = leftNode;
                        rightIndex = leftIndex;
                        leftNode = node;
                        leftIndex = j;
                    }
                    else if (node.frequency < rightNode.frequency)
                    {
                        rightNode = node;
                        rightIndex = j;
                    }
                }

                HuffmanTreeNode newNode = new HuffmanTreeNode
                {
                    code = nextCode,
                    frequency = (leftNode.frequency + rightNode.frequency)
                };
                leftNode.parent  = newNode;
                rightNode.parent = newNode;

                leftNode.treeIndex  = (ushort)((nextCode<<1) | 0x0);
                rightNode.treeIndex = (ushort)((nextCode<<1) | 0x1);
                treeCodes[leftNode.treeIndex]  = leftNode.code;
                treeCodes[rightNode.treeIndex] = rightNode.code;

                workingNodes[leftIndex] = newNode; // No need for list expansion, just reuse left node index.
                workingNodes.RemoveAt(rightIndex);

                nextCode++;
            }


            ushort initialCode = 0; // No value if the file has zero bytes.
            if (workingNodes.Count > 0)
                initialCode = workingNodes[0].code; // Otherwise use the code of the root tree node.

            // Write compression header.
            //writer.Write((Int32)data.Length);
            writer.Write((Int32)initialCode);

            // Write array of ushort[1024], which points to individual values in the tree by a code-based lookup.
            // Note that values [0,255] are ALWAYS 0 (unused). Why they're even included in the data at all is
            //  likely to speed up lookup by removing the the subtraction(?)
            for (int i = 0; i < treeCodes.Length; i++)
            {
                writer.Write((UInt16)treeCodes[i]);
            }


            // Now encode the actual file payload, byte-by-byte into a bit buffer.
            byte bitsUsed = 0;
            uint bitBuff = 0;
            byte[] workingBitsList = new byte[256];

            foreach (byte b in data)
            {
                var node = leafNodes[b];

                int digits = 0; // numLookups
                while (node.parent != null)
                {
                    workingBitsList[digits++] = node.Bit;
                    node = node.parent;
                }

                while (--digits >= 0)
                {
                    bitBuff |= ((uint) workingBitsList[digits] << bitsUsed);
                    if (++bitsUsed == 32)
                    {
                        writer.Write((UInt32)bitBuff);
                        bitsUsed = 0;
                        bitBuff = 0;
                    }
                }
            }

            // Flush current bit buffer, if there are any bits.
            // The first block in the bit buffer is ALWAYS loaded during decompression, so we also
            //  need to make sure to have flushed at least one block during the compression process.

            // These 3 other scenarios when we'll need to forcefully flush the bit buffer.
            // 1) The usual scenario, where some bits have been added, but not written to the payload.
            // 2) When the file is zero bytes in length.
            // 3) When there is only one byte value appearing in the entire file.
            if (bitsUsed != 0 || workingNodes.Count == 0 || workingNodes[0].code < 0x100)
            {
                writer.Write((UInt32)bitBuff);
            }

            //return writer.BaseStream.Position - startPosition;
        }
    }
}
