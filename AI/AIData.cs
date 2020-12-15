using System.IO;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace LSRutil.AI
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Class for AI data info.
    /// </summary>
    public class AIData
    {
        public ushort Reflex;
        public ushort RacingLine;
        public ushort Overtaking;
        public byte Blocking;
        public byte CutsCorners;
        public ushort Braking;
        public ushort Speed;
        public byte Intelligence;
        public byte Craziness;
        
        public void Load(string filename)
        {
            using var reader = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read));
            reader.ReadBytes(4);
            RacingLine = reader.ReadUInt16();
            Braking = reader.ReadUInt16();
            Overtaking = reader.ReadUInt16();
            Speed = reader.ReadUInt16();
            Reflex = reader.ReadUInt16();
            reader.ReadBytes(2);
            Blocking = reader.ReadByte();
            CutsCorners = reader.ReadByte();
            Intelligence = reader.ReadByte();
            Craziness = reader.ReadByte();
        }

        public void Save(string filename)
        {
            using var writer = new BinaryWriter(File.Open(filename, FileMode.Create));
            writer.Write(new byte[] { 0, 0, 0, 0 });
            writer.Write(RacingLine);
            writer.Write(Braking);
            writer.Write(Overtaking);
            writer.Write(Speed);
            writer.Write(Reflex);
            writer.Write(new byte[] { 0, 0 });
            writer.Write(Blocking);
            writer.Write(CutsCorners);
            writer.Write(Intelligence);
            writer.Write(Craziness);
        }
    }
}
