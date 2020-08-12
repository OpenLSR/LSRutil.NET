namespace LSRutil
{
    public struct GridPosition
    {
        public int X;
        public int Y;
        public int Z;

        public GridPosition(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType() != GetType()) return false;
            var arg = (GridPosition)obj;
            return X.Equals(arg.X) && Y.Equals(arg.Y) && Z.Equals(arg.Z);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})",X,Y,Z);
        }
    }
}
