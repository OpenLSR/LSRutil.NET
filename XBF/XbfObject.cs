using System;
using System.Collections.Generic;
using System.Numerics;

#if !NETFRAMEWORK
using SharpGLTF.Transforms;
#endif

namespace LSRutil.XBF
{
    public class XbfModel
    {
        public List<string> Materials = new List<string>();
        
        public List<XbfObject> Objects = new List<XbfObject>();

        public string Name;

        public XbfModel(string name)
        {
            Name = name;
        }
    }
    
    public class XbfObject
    {
        public string Name;
        
        #if !NETFRAMEWORK
        public Matrix4x4Double Matrix;
        #else
        public Matrix4x4 Matrix;
        #endif

        public Mesh Mesh;
        
        public List<XbfObject> Children = new List<XbfObject>();
    }
    
    public class Mesh
    {
        public List<Vector3> Vectors = new List<Vector3>();
        
        public List<Triangle> Triangles = new List<Triangle>();
        
        public List<Vector3> Normals = new List<Vector3>();
    }

    public class Triangle
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        
        public int TextureId { get; set; }
        
        public Vector2 UV_A { get; set; }
        public Vector2 UV_B { get; set; }
        public Vector2 UV_C { get; set; }
        
        public int SmoothingGroup { get; set; }

        public Triangle(int a, int b, int c, int textureId, int e, Vector2 uvA, Vector2 uvB, Vector2 uvC)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.TextureId = textureId;
            this.UV_A = uvA;
            this.UV_B = uvB;
            this.UV_C = uvC;
        }
    }
}