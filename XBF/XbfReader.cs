using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// I fucking hate this code, this code is garbage, why doesn't this work. -Y 12/05/2020

namespace LSRutil.XBF
{
    public class XbfReader
    {
        private Stream stream;
        private BinaryReader reader;

        private byte[] fileData;

        private List<string> textures = new List<string>();

        private int unknownInt;

        public void ReadModel(Stream stream)
        {
            this.stream = stream;
            this.reader = new BinaryReader(stream);
            
            this.unknownInt = reader.ReadInt32();
            ColoredConsole.WriteLineDebug("{0:x8} Version? {1:x8}", new object[]
            {
            reader.BaseStream.Position,
            this.unknownInt
            });
            this.unknownInt = reader.ReadInt32();
            ColoredConsole.WriteLineDebug("{0:x8} UnknownInt {1:x8}", new object[]
            {
            reader.BaseStream.Position,
            this.unknownInt
            });
            int num = reader.ReadInt32();
            ColoredConsole.WriteLineInfo("{0:x8} SizeOfTextureNames {1:x8}", new object[]
            {
            reader.BaseStream.Position,
            num
            });
            for (int i = 0; i < num; i += 2)
            {
                ColoredConsole.WriteDebug("{0:x8} ", new object[]
                {
                    reader.BaseStream.Position
                });
                List<byte> list = new List<byte>();
                byte nchar = reader.ReadByte();
                while (nchar != 0x00) // Seperator
                {
                    list.Add(nchar);
                    nchar = reader.ReadByte();
                    i++;
                }
                ColoredConsole.WriteLineDebug("{0}", new object[]
                {
                Encoding.ASCII.GetString(list.ToArray())
                });
                this.textures.Add(Encoding.ASCII.GetString(list.ToArray()));
                reader.BaseStream.Seek(2L, SeekOrigin.Current);
            }
            
            Mesh mesh = this.ReadMesh(true, " ");
        }

        public void ReadModel(string filename)
        {
            stream = File.Open(filename, FileMode.Open, FileAccess.Read);
            ReadModel(stream);
        }

        /*
        public void Extract(string directory)
        {
            
            fileStream.Close();
            this.unknownInt = BitConverter.ToInt32(this.fileData, this.pos);
            ColoredConsole.WriteLineDebug("{0:x8} UnknownInt {1:x8}", new object[]
            {
                this.pos,
                this.unknownInt
            });
            this.pos += 4;
            this.unknownInt = BitConverter.ToInt32(this.fileData, this.pos);
            ColoredConsole.WriteLineDebug("{0:x8} UnknownInt {1:x8}", new object[]
            {
                this.pos,
                this.unknownInt
            });
            this.pos += 4;
            int num = BitConverter.ToInt32(this.fileData, this.pos);
            ColoredConsole.WriteLineInfo("{0:x8} SizeOfTextureNames {1:x8}", new object[]
            {
                this.pos,
                num
            });
            this.pos += 4;
            for (int i = 0; i < num; i += 2)
            {
                ColoredConsole.WriteDebug("{0:x8} ", new object[]
                {
                    this.pos
                });
                List<byte> list = new List<byte>();
                while (this.fileData[this.pos] != 0)
                {
                    list.Add(this.fileData[this.pos]);
                    this.pos++;
                    i++;
                }
                ColoredConsole.WriteLineDebug("{0}", new object[]
                {
                    Encoding.ASCII.GetString(list.ToArray())
                });
                this.textures.Add(Encoding.ASCII.GetString(list.ToArray()));
                this.pos += 2;
            }
            Mesh mesh = this.ReadMesh(true, " ");
            this.WriteObj(mesh);
        }*/

        private Mesh ReadMesh(bool isMain, string prefix)
        {
            Mesh mesh = new Mesh();
            int num = reader.ReadInt32();
            ColoredConsole.WriteLineInfo("{0:x8} {1} NumberOfVertices {2:x8}", new object[]
            {
                reader.BaseStream.Position,
                prefix,
                num
            });
            this.unknownInt = reader.ReadInt32();
            ColoredConsole.WriteLineDebug("{0:x8} {1} UnknownInt {2:x8}", new object[]
            {
                reader.BaseStream.Position,
                prefix,
                this.unknownInt
            });
            int num2 = reader.ReadInt32();
            ColoredConsole.WriteLineInfo("{0:x8} {1} NumberOfIndices {2:x8}", new object[]
            {
                reader.BaseStream.Position,
                prefix,
                num2
            });
            int num3 = reader.ReadInt32();
            ColoredConsole.WriteLineInfo("{0:x8} {1} NumberOfSubMeshes {2:x8}", new object[]
            {
                reader.BaseStream.Position,
                prefix,
                num3
            });
            reader.BaseStream.Seek(128L, SeekOrigin.Current);
            int num4 = reader.ReadInt32();
            ColoredConsole.WriteLineDebug("{0:x8} {1} MeshNameLength {2:x8}", new object[]
            {
                reader.BaseStream.Position,
                prefix,
                num4
            });
            
            List<byte> list = new List<byte>();
            ColoredConsole.WriteDebug("{0:x8} {1} ", new object[]
            {
                reader.BaseStream.Position,
                prefix
            });
            for (int i = 0; i < num4; i++)
            {
                byte nchar = reader.ReadByte();
                if (nchar != 0x00) // Seperator
                {
                    list.Add(nchar);
                    nchar = reader.ReadByte();
                }
            }
            ColoredConsole.WriteLineWarn("MeshName: {0}", new object[]
            {
                Encoding.ASCII.GetString(list.ToArray())
            });
            mesh.Name = Encoding.ASCII.GetString(list.ToArray());
            /*
            for (int i = 0; i < num3; i++)
            {
                mesh.SubMeshes.Add(this.ReadMesh(false, prefix + "  "));
            }
            ColoredConsole.WriteLineInfo("{0:x8} {1} ReadVertices", new object[]
            {
                this.pos,
                prefix
            });
            for (int i = 0; i < num; i++)
            {
                float num5 = BitConverter.ToSingle(this.fileData, this.pos);
                this.pos += 4;
                float num6 = BitConverter.ToSingle(this.fileData, this.pos);
                this.pos += 4;
                float num7 = BitConverter.ToSingle(this.fileData, this.pos);
                this.pos += 4;
                float num8 = BitConverter.ToSingle(this.fileData, this.pos);
                this.pos += 4;
                float num9 = BitConverter.ToSingle(this.fileData, this.pos);
                this.pos += 4;
                float num10 = BitConverter.ToSingle(this.fileData, this.pos);
                this.pos += 4;
                mesh.Vectors.Add(new Vector3D((double)num5, (double)num6, (double)num7));
                mesh.Normals.Add(new Vector3D((double)num8, (double)num9, (double)num10));
            }
            ColoredConsole.WriteLineInfo("{0:x8} {1} ReadIndices", new object[]
            {
                this.pos,
                prefix
            });
            for (int i = 0; i < num2; i++)
            {
                int a = BitConverter.ToInt32(this.fileData, this.pos);
                this.pos += 4;
                int b = BitConverter.ToInt32(this.fileData, this.pos);
                this.pos += 4;
                int c = BitConverter.ToInt32(this.fileData, this.pos);
                this.pos += 4;
                int textureId = BitConverter.ToInt32(this.fileData, this.pos);
                this.pos += 4;
                int e = BitConverter.ToInt32(this.fileData, this.pos);
                this.pos += 4;
                float x = BitConverter.ToSingle(this.fileData, this.pos);
                this.pos += 4;
                float y = BitConverter.ToSingle(this.fileData, this.pos);
                this.pos += 4;
                mesh.UVs.Add(new Vector2D(x, y));
                float x2 = BitConverter.ToSingle(this.fileData, this.pos);
                this.pos += 4;
                float y2 = BitConverter.ToSingle(this.fileData, this.pos);
                this.pos += 4;
                mesh.UVs.Add(new Vector2D(x2, y2));
                float x3 = BitConverter.ToSingle(this.fileData, this.pos);
                this.pos += 4;
                float y3 = BitConverter.ToSingle(this.fileData, this.pos);
                this.pos += 4;
                mesh.UVs.Add(new Vector2D(x3, y3));
                mesh.Triangles.Add(new Triangle(a, b, c, textureId, e, mesh.UVs.Count - 3, mesh.UVs.Count - 2, mesh.UVs.Count - 1));
            }
            for (int i = 0; i < num2; i++)
            {
                this.pos += 4;
            }
            if (!isMain && num > 0)
            {
                int num11 = BitConverter.ToInt32(this.fileData, this.pos);
                ColoredConsole.WriteLineError("{0:x8} {1} NumberOfFrames {2:x8}", new object[]
                {
                    this.pos,
                    prefix,
                    num11
                });
                this.pos += 4;
                if (num11 != 0)
                {
                    int num12 = BitConverter.ToInt32(this.fileData, this.pos);
                    ColoredConsole.WriteLineError("{0:x8} {1} NumberOfPoints {2:x8}", new object[]
                    {
                        this.pos,
                        prefix,
                        num12
                    });
                    this.pos += 4;
                    for (int i = 0; i < num12; i++)
                    {
                        short num13 = BitConverter.ToInt16(this.fileData, this.pos);
                        this.pos += 2;
                        short num14 = BitConverter.ToInt16(this.fileData, this.pos);
                        this.pos += 2;
                        ColoredConsole.WriteLineWarn("{0:x8} {1} Frame {2:x4} Type {3:x4}", new object[]
                        {
                            this.pos,
                            prefix,
                            num13,
                            num14
                        });
                        if ((num14 & 4096) == 4096)
                        {
                            float num15 = BitConverter.ToSingle(this.fileData, this.pos);
                            this.pos += 4;
                            float num16 = BitConverter.ToSingle(this.fileData, this.pos);
                            this.pos += 4;
                            float num17 = BitConverter.ToSingle(this.fileData, this.pos);
                            this.pos += 4;
                            float num18 = BitConverter.ToSingle(this.fileData, this.pos);
                            this.pos += 4;
                            ColoredConsole.WriteLineDebug("{0:x8} {1}   Rotation: {2} {3} {4} {5}", new object[]
                            {
                                this.pos,
                                prefix,
                                num15,
                                num16,
                                num17,
                                num18
                            });
                        }
                        if ((num14 & 8192) == 8192)
                        {
                            float num15 = BitConverter.ToSingle(this.fileData, this.pos);
                            this.pos += 4;
                            float num16 = BitConverter.ToSingle(this.fileData, this.pos);
                            this.pos += 4;
                            float num17 = BitConverter.ToSingle(this.fileData, this.pos);
                            this.pos += 4;
                            ColoredConsole.WriteLineDebug("{0:x8} {1}   Scale: {2} {3} {4}", new object[]
                            {
                                this.pos,
                                prefix,
                                num15,
                                num16,
                                num17
                            });
                        }
                        if ((num14 & 16384) == 16384)
                        {
                            float num15 = BitConverter.ToSingle(this.fileData, this.pos);
                            this.pos += 4;
                            float num16 = BitConverter.ToSingle(this.fileData, this.pos);
                            this.pos += 4;
                            float num17 = BitConverter.ToSingle(this.fileData, this.pos);
                            this.pos += 4;
                            ColoredConsole.WriteLineDebug("{0:x8} {1}   Position: {2} {3} {4}", new object[]
                            {
                                this.pos,
                                prefix,
                                num15,
                                num16,
                                num17
                            });
                        }
                        if (num14 > 28672)
                        {
                            throw new NotSupportedException("FrameType: " + num14);
                        }
                    }
                }
            }
            */
            return mesh;
        }

        /*public void WriteObj(Mesh mesh)
        {
            FileStream fileStream = File.Create(this.directoryname + "\\" + mesh.Name + ".obj");
            StreamWriter streamWriter = new StreamWriter(fileStream);
            foreach (Vector3D arg in mesh.Vectors)
            {
                streamWriter.WriteLine(string.Format("v {0}", arg).Replace(',', '.'));
            }
            streamWriter.WriteLine();
            foreach (Vector3D arg2 in mesh.Normals)
            {
                streamWriter.WriteLine(string.Format("vn {0}", arg2).Replace(',', '.'));
            }
            streamWriter.WriteLine();
            foreach (Vector2D arg3 in mesh.UVs)
            {
                streamWriter.WriteLine(string.Format("vt {0}", arg3).Replace(',', '.'));
            }
            string text = string.Empty;
            streamWriter.WriteLine();
            streamWriter.WriteLine("g " + mesh.Name);
            int count = mesh.Vectors.Count;
            int count2 = mesh.UVs.Count;
            foreach (Triangle triangle in mesh.Triangles)
            {
                if (triangle.TextureId < this.textures.Count && this.textures[triangle.TextureId] != text)
                {
                    text = this.textures[triangle.TextureId];
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("usemtl " + text);
                }
                if (triangle.TextureId < this.textures.Count)
                {
                    streamWriter.WriteLine("f {0}/{3}/{0} {1}/{4}/{1} {2}/{5}/{2}", new object[]
                    {
                        triangle.A - count,
                        triangle.B - count,
                        triangle.C - count,
                        triangle.UV1 - count2,
                        triangle.UV2 - count2,
                        triangle.UV3 - count2
                    });
                }
                else
                {
                    streamWriter.WriteLine("f {0}//{0} {1}//{1} {2}//{2}", triangle.A - count, triangle.B - count, triangle.C - count);
                }
            }
            streamWriter.Close();
            fileStream.Close();
            foreach (Mesh mesh2 in mesh.SubMeshes)
            {
                this.WriteObj(mesh2);
            }
        }*/
    }
}
