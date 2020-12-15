using System;
using System.Collections.Generic;
using System.Text;

namespace LSRutil.XBF
{
    public class Mesh
    {
        // Token: 0x0400000A RID: 10
        public List<Vector3D> Vectors = new List<Vector3D>();

        // Token: 0x0400000B RID: 11
        public List<Triangle> Triangles = new List<Triangle>();

        // Token: 0x0400000C RID: 12
        public List<Vector3D> Normals = new List<Vector3D>();

        // Token: 0x0400000D RID: 13
        public List<Vector2D> UVs = new List<Vector2D>();

        // Token: 0x0400000E RID: 14
        public List<Mesh> SubMeshes = new List<Mesh>();

        // Token: 0x0400000F RID: 15
        public string Name;
    }

    public class Triangle
    {
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x0600000A RID: 10 RVA: 0x000032E8 File Offset: 0x000014E8
        // (set) Token: 0x0600000B RID: 11 RVA: 0x000032FF File Offset: 0x000014FF
        public int A { get; set; }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x0600000C RID: 12 RVA: 0x00003308 File Offset: 0x00001508
        // (set) Token: 0x0600000D RID: 13 RVA: 0x0000331F File Offset: 0x0000151F
        public int B { get; set; }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x0600000E RID: 14 RVA: 0x00003328 File Offset: 0x00001528
        // (set) Token: 0x0600000F RID: 15 RVA: 0x0000333F File Offset: 0x0000153F
        public int C { get; set; }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000010 RID: 16 RVA: 0x00003348 File Offset: 0x00001548
        // (set) Token: 0x06000011 RID: 17 RVA: 0x0000335F File Offset: 0x0000155F
        public int TextureId { get; set; }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000012 RID: 18 RVA: 0x00003368 File Offset: 0x00001568
        // (set) Token: 0x06000013 RID: 19 RVA: 0x0000337F File Offset: 0x0000157F
        public int UV1 { get; set; }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000014 RID: 20 RVA: 0x00003388 File Offset: 0x00001588
        // (set) Token: 0x06000015 RID: 21 RVA: 0x0000339F File Offset: 0x0000159F
        public int UV2 { get; set; }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000016 RID: 22 RVA: 0x000033A8 File Offset: 0x000015A8
        // (set) Token: 0x06000017 RID: 23 RVA: 0x000033BF File Offset: 0x000015BF
        public int UV3 { get; set; }

        // Token: 0x06000018 RID: 24 RVA: 0x000033C8 File Offset: 0x000015C8
        public Triangle(int a, int b, int c, int textureId, int e, int uv1, int uv2, int uv3)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.TextureId = textureId;
            this.UV1 = uv1;
            this.UV2 = uv2;
            this.UV3 = uv3;
        }
    }

    public class Vector2D
    {
        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000019 RID: 25 RVA: 0x0000341C File Offset: 0x0000161C
        // (set) Token: 0x0600001A RID: 26 RVA: 0x00003433 File Offset: 0x00001633
        public float X { get; set; }

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x0600001B RID: 27 RVA: 0x0000343C File Offset: 0x0000163C
        // (set) Token: 0x0600001C RID: 28 RVA: 0x00003453 File Offset: 0x00001653
        public float Y { get; set; }

        // Token: 0x0600001D RID: 29 RVA: 0x0000345C File Offset: 0x0000165C
        public Vector2D(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        // Token: 0x0600001E RID: 30 RVA: 0x00003478 File Offset: 0x00001678
        public override string ToString()
        {
            return string.Format("{0:0.#####} {1:0.#####} ", this.X, this.Y).Replace(',', '.');
        }
    }

    public class Vector3D
    {
        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600001F RID: 31 RVA: 0x000034B4 File Offset: 0x000016B4
        // (set) Token: 0x06000020 RID: 32 RVA: 0x000034CB File Offset: 0x000016CB
        public double X { get; set; }

        // Token: 0x1700000B RID: 11
        // (get) Token: 0x06000021 RID: 33 RVA: 0x000034D4 File Offset: 0x000016D4
        // (set) Token: 0x06000022 RID: 34 RVA: 0x000034EB File Offset: 0x000016EB
        public double Y { get; set; }

        // Token: 0x1700000C RID: 12
        // (get) Token: 0x06000023 RID: 35 RVA: 0x000034F4 File Offset: 0x000016F4
        // (set) Token: 0x06000024 RID: 36 RVA: 0x0000350B File Offset: 0x0000170B
        public double Z { get; set; }

        // Token: 0x06000025 RID: 37 RVA: 0x00003514 File Offset: 0x00001714
        public Vector3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        // Token: 0x06000026 RID: 38 RVA: 0x00003538 File Offset: 0x00001738
        public override string ToString()
        {
            return string.Format("{0:0.#####} {1:0.#####} {2:0.#####} ", this.X, this.Y, this.Z).Replace(',', '.');
        }
    }
}
