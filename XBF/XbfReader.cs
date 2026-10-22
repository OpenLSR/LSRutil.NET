using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml;

#if !NETFRAMEWORK
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
#endif

namespace LSRutil.XBF
{
    public class XbfReader
    {
        private Stream stream;
        private BinaryReader reader;
        
        private XbfModel model;

        public void ReadModel(Stream stream, String name)
        {
            this.stream = stream;
            reader = new BinaryReader(stream);
            
            model = new XbfModel(name);
            
            int version = reader.ReadInt32();
            ColoredConsole.WriteLineDebug("{0:x8} Version? {1:x8}", reader.BaseStream.Position, version);
            
            int unknownInt = reader.ReadInt32();
            ColoredConsole.WriteLineDebug("{0:x8} UnknownInt {1:x8}", reader.BaseStream.Position, unknownInt);
            
            int num = reader.ReadInt32();
            ColoredConsole.WriteLineInfo("{0:x8} SizeOfTextureNames {1:x8}", reader.BaseStream.Position, num);
            
            for (int i = 0; i < num; i += 2)
            {
                ColoredConsole.WriteDebug("{0:x8} ", reader.BaseStream.Position);
                List<byte> list = new List<byte>();
                byte nchar = reader.ReadByte();
                while (nchar != 0x00) // Seperator
                {
                    list.Add(nchar);
                    nchar = reader.ReadByte();
                    i++;
                }
                ColoredConsole.WriteLineDebug("{0}", Encoding.ASCII.GetString(list.ToArray()));
                model.Materials.Add(Encoding.ASCII.GetString(list.ToArray()));
                reader.BaseStream.Seek(1L, SeekOrigin.Current);
            }
            
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                int verts = reader.ReadInt32();
                if(verts == -1) break;
                reader.BaseStream.Seek(-4L, SeekOrigin.Current);
                
                model.Objects.Add(ReadObject(true, " "));
            }
        }

        public void ReadModel(string filename)
        {
            Console.WriteLine("Reading file {0}", filename);
            stream = File.Open(filename, FileMode.Open, FileAccess.Read);
            ReadModel(stream, Path.GetFileNameWithoutExtension(filename));
            //WriteObj(model.Objects[0]);
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

        private XbfObject ReadObject(bool isMain, string prefix)
        {
            XbfObject obj = new XbfObject();
            
            int numVerts = reader.ReadInt32();
            ColoredConsole.WriteLineInfo("{0:x8} {1} NumberOfVertices {2:x8}", reader.BaseStream.Position, prefix, numVerts);
            
            int unknownInt = reader.ReadInt32();
            ColoredConsole.WriteLineDebug("{0:x8} {1} UnknownInt {2:x8}", reader.BaseStream.Position, prefix, unknownInt);
            
            int numFaces = reader.ReadInt32();
            ColoredConsole.WriteLineInfo("{0:x8} {1} NumberOfFaces {2:x8}", reader.BaseStream.Position, prefix, numFaces);
            
            int numSubMesh = reader.ReadInt32();
            ColoredConsole.WriteLineInfo("{0:x8} {1} NumberOfSubMeshes {2:x8}", reader.BaseStream.Position, prefix, numSubMesh);
            
            // the matrix is stored as double-precision,
            // but System.Numerics only supports single-precision,
            // so for .NET Framework we have to cast to floats.
            
            #if !NETFRAMEWORK
            obj.Matrix = new Matrix4x4Double(
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble(), 
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble()
            );
            #else
            obj.Matrix = new Matrix4x4(
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble(),
                (float)reader.ReadDouble()
            );
            #endif
            
            ColoredConsole.WriteLineDebug("{0:x8} {1} ObjectMatrix {2}", reader.BaseStream.Position, prefix, (Matrix4x4)obj.Matrix);
            
            int objNameLen = reader.ReadInt32();
            ColoredConsole.WriteLineDebug("{0:x8} {1} ObjectNameLength {2:x8}", reader.BaseStream.Position, prefix, objNameLen);
            
            List<byte> list = new List<byte>();
            ColoredConsole.WriteDebug("{0:x8} {1} ", reader.BaseStream.Position, prefix);
            for (int i = 0; i < objNameLen; i++)
            {
                byte nchar = reader.ReadByte();
                if (nchar != 0x00) // Seperator
                {
                    list.Add(nchar);
                }
            }
            ColoredConsole.WriteLineWarn("ObjectName: {0}", Encoding.ASCII.GetString(list.ToArray()));
            obj.Name = Encoding.ASCII.GetString(list.ToArray());

            for (int i = 0; i < numSubMesh; i++)
            {
                obj.Children.Add(ReadObject(false, prefix + "  "));
            }
            
            // mesh things
            
            obj.Mesh = new Mesh();
            
            // further research required
            /*
            for (int i = 0; i < numSubMesh; i++)
            {
                obj.Mesh.SubMeshes.Add(ReadObject(false, prefix + "  "));
            }
            */
            
            ColoredConsole.WriteLineInfo("{0:x8} {1} ReadVertices", reader.BaseStream.Position, prefix);
            for (int i = 0; i < numVerts; i++)
            {
                obj.Mesh.Vectors.Add(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                obj.Mesh.Normals.Add(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
            }
            
            ColoredConsole.WriteLineInfo("{0:x8} {1} ReadFaces", reader.BaseStream.Position, prefix);
            for (int i = 0; i < numFaces; i++)
            {
                
                int a = reader.ReadInt32();
                int b = reader.ReadInt32();
                int c = reader.ReadInt32();
                
                int textureId = reader.ReadInt32();
                
                int e = reader.ReadInt32();
                
                Vector2 uv_a = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                Vector2 uv_b = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                Vector2 uv_c = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                
                obj.Mesh.Triangles.Add(new Triangle(a, b, c, textureId, e, uv_a, uv_b, uv_c));
            }
            ColoredConsole.WriteLineInfo("{0:x8} {1} ReadSmoothingGroups", reader.BaseStream.Position, prefix);
            for (int i = 0; i < numFaces; i++)
            {
                obj.Mesh.Triangles[i].SmoothingGroup = reader.ReadInt32();
            }

            //if (isMain) return obj;

            if ((unknownInt & 8) != 0)
            {
                int frameCount = reader.ReadInt32();
                ColoredConsole.WriteLineError("{0:x8} {1} FrameCount {2:x8}", reader.BaseStream.Position - 4,
                    prefix, frameCount);

                // this logic needs to change
                if (frameCount != 0)
                {
                    int numKeys = reader.ReadInt32();
                    ColoredConsole.WriteLineError("{0:x8} {1} NumberOfKeys {2:x8}", reader.BaseStream.Position - 4,
                        prefix, numKeys);

                    for (int i = 0; i < numKeys; i++)
                    {
                        short frameIdx = reader.ReadInt16();
                        short frameType = reader.ReadInt16();
                        ColoredConsole.WriteLineWarn("{0:x8} {1} Frame {2:x4} Type {3:x4}", reader.BaseStream.Position,
                            prefix, frameIdx, frameType);

                        if ((frameType & 4096) == 4096)
                        {
                            Quaternion rot = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                            ColoredConsole.WriteLineDebug("{0:x8} {1}   Rotation: {2} {3} {4} {5}", reader.BaseStream.Position,
                                prefix, rot.X, rot.Y, rot.Z, rot.W);
                        }

                        if ((frameType & 8192) == 8192)
                        {
                            Vector3 scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            ColoredConsole.WriteLineDebug("{0:x8} {1}   Scale: {2} {3} {4}", reader.BaseStream.Position,
                                prefix, scale.X, scale.Y, scale.Z);
                        }

                        if ((frameType & 16384) == 16384)
                        {
                            Vector3 pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            ColoredConsole.WriteLineDebug("{0:x8} {1}   Position: {2} {3} {4}", reader.BaseStream.Position,
                                prefix, pos.X, pos.Y, pos.Z);
                        }

                        if (frameType > 28672)
                        {
                            throw new NotSupportedException("FrameType: " + frameType);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Seek(-4L, SeekOrigin.Current);
                }
            }

            return obj;
        }

        public void WriteObj(XbfObject guy)
        {
            FileStream fileStream = File.Create(guy.Name + ".obj");
            StreamWriter streamWriter = new StreamWriter(fileStream);
            foreach (Vector3 arg in guy.Mesh.Vectors)
            {
                streamWriter.WriteLine("v {0:F6} {1:F6} {2:F6}", arg.X, arg.Y, arg.Z);
            }
            streamWriter.WriteLine();
            foreach (Vector3 arg2 in guy.Mesh.Normals)
            {
                streamWriter.WriteLine("vn {0:F3} {1:F3} {2:F3}", arg2.X, arg2.Y, arg2.Z);
            }
            streamWriter.WriteLine();
            foreach (Triangle tri in guy.Mesh.Triangles)
            {
                streamWriter.WriteLine("vt {0:F3} {1:F3}", tri.UV_A.X, 1 - tri.UV_A.Y);
                streamWriter.WriteLine("vt {0:F3} {1:F3}", tri.UV_B.X, 1 - tri.UV_B.Y);
                streamWriter.WriteLine("vt {0:F3} {1:F3}", tri.UV_C.X, 1 - tri.UV_C.Y);
            }
            string text = string.Empty;
            streamWriter.WriteLine();
            streamWriter.WriteLine("g " + guy.Name);
            for (int i = 0; i < guy.Mesh.Triangles.Count; i++)
            {
                var triangle = guy.Mesh.Triangles[i];
                
                if (triangle.TextureId < model.Materials.Count && model.Materials[triangle.TextureId] != text)
                {
                    text = model.Materials[triangle.TextureId];
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("usemtl " + text);
                }
                if (triangle.TextureId < model.Materials.Count)
                {
                    streamWriter.WriteLine("f {0}/{3}/{0} {1}/{4}/{1} {2}/{5}/{2}", new object[]
                    {
                        triangle.A + 1,
                        triangle.B + 1,
                        triangle.C + 1,
                        (i * 3) + 1,
                        (i * 3) + 2,
                        (i * 3) + 3,
                    });
                }
                else
                {
                    streamWriter.WriteLine("f {0}//{0} {1}//{1} {2}//{2}", triangle.A + 1, triangle.B + 1, triangle.C + 1);
                }
            }
            streamWriter.Close();
            fileStream.Close();
            foreach (XbfObject guy2 in guy.Children)
            {
                WriteObj(guy2);
            }
        }

        #if !NETFRAMEWORK
        // TODO: textures, UVs
        public NodeBuilder BuildGltf(SceneBuilder scene, NodeBuilder parentNode, XbfObject obj)
        {
            var m = new MeshBuilder<VertexPosition>(obj.Name);
                
            var pList = model.Materials.Select(matName =>
            {
                var mat = new MaterialBuilder(matName).WithDoubleSide(true);
                var ch = mat.GetChannel(KnownChannel.BaseColor);
                
                return m.UsePrimitive(mat);
            }).ToList();
                
            var msh = obj.Mesh;
                
            foreach (Triangle tri in msh.Triangles)
            {
                var prim = pList[tri.TextureId];
                var tv = new[] {tri.A, tri.B, tri.C}.Select(p =>
                {
                    var v = msh.Vectors[p];
                    var n = msh.Normals[p];
 
                    // TODO: normals are broken on certain models, glTF angry
                    return new VertexPosition(v);
                }).ToArray();
                
                prim.AddTriangle(tv[0], tv[1], tv[2]);
            }
            
            // TODO: this is insane
            var mtx = (Matrix4x4)obj.Matrix;
            mtx.M14 = 0;
            mtx.M24 = 0;
            mtx.M34 = 0;
            mtx.M44 = 1;
            
            //var ins = scene.AddRigidMesh(m, mtx);
            NodeBuilder node = parentNode != null ? parentNode.CreateNode(obj.Name) : new NodeBuilder(obj.Name);
            node.LocalMatrix = mtx;

            foreach (var child in obj.Children)
            {
                BuildGltf(scene, node, child);
            }

            return node;
        }
        
        // TODO: UV maps?
        public void WriteGltf()
        {
            var scene = new SceneBuilder(model.Name);

            foreach (XbfObject obj in model.Objects)
            {
                scene.AddNode(BuildGltf(scene, null, obj));
            }
            
            var mr = scene.ToGltf2();
            mr.SaveGLB($"{model.Name}.glb");
            mr.SaveGLTF($"{model.Name}.gltf");
        }
        #endif
    }
}
