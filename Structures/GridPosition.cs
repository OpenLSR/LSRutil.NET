namespace LSRutil
{
    /// <summary>
    /// A simple class to define positions on the game grid.
    /// </summary>
    public struct GridPosition
    {
        public int X;
        public int Y;
        public int Z;

        [System.Obsolete("Please use the Y property instead.")]
        public int Height => Y;

        public GridPosition(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public override bool Equals(object obj)
        {
            if(obj == null) return false;
            if(obj.GetType() != GetType()) return false;
            var arg = (GridPosition)obj;
            return X.Equals(arg.X) && Y.Equals(arg.Y) && Z.Equals(arg.Z);
        }

        public static bool operator ==(GridPosition lhs, GridPosition rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(GridPosition lhs, GridPosition rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override int GetHashCode()
        {
            var hash = 47;
            hash = hash * 5 + X.GetHashCode();
            hash = hash * 5 + Y.GetHashCode();
            hash = hash * 5 + Z.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }
    }
}
