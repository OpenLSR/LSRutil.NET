using System.IO;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace LSRutil.AI
{
    public class CorneringData
    {
        public float Small;
        public float Big;
        public float SmallSmall_Same;
        public float SmallSmall_Opp;
        public float BigBig_Same;
        public float BigBig_Opp;
        public float BigSmall_Same;
        public float SmallBig_Same;
        public float BigSmall_Opp;
        public float SmallBig_Opp;
        
        public void Load(string filename)
        {
            using var reader = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read));
            Small = reader.ReadSingle();
            Big = reader.ReadSingle();
            SmallSmall_Same = reader.ReadSingle();
            SmallSmall_Opp = reader.ReadSingle();
            BigBig_Same = reader.ReadSingle();
            BigBig_Opp = reader.ReadSingle();
            BigSmall_Same = reader.ReadSingle();
            SmallBig_Same = reader.ReadSingle();
            BigSmall_Opp = reader.ReadSingle();
            SmallBig_Opp = reader.ReadSingle();
        }

        public void Save(string filename)
        {
            using var writer = new BinaryWriter(File.Open(filename, FileMode.Create));
            writer.Write(Small);
            writer.Write(Big);
            writer.Write(SmallSmall_Same);
            writer.Write(SmallSmall_Opp);
            writer.Write(BigBig_Same);
            writer.Write(BigBig_Opp);
            writer.Write(BigSmall_Same);
            writer.Write(SmallBig_Same);
            writer.Write(BigSmall_Opp);
            writer.Write(SmallBig_Opp);
        }
    }
}
