using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LSRutil
{
    class AIData
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

        public AIData() { }

        public void Load(string filename)
        {
            FileStream file = File.Open(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader reader = new BinaryReader(file))
            {
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
        }

        public void Save(string filename)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
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
}
