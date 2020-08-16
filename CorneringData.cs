using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LSRutil
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

        public CorneringData() { }

        public void Load(string filename)
        {
            FileStream file = File.Open(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader reader = new BinaryReader(file))
            {
                this.Small = reader.ReadSingle();
                this.Big = reader.ReadSingle();
                this.SmallSmall_Same = reader.ReadSingle();
                this.SmallSmall_Opp = reader.ReadSingle();
                this.BigBig_Same = reader.ReadSingle();
                this.BigBig_Opp = reader.ReadSingle();
                this.BigSmall_Same = reader.ReadSingle();
                this.SmallBig_Same = reader.ReadSingle();
                this.BigSmall_Opp = reader.ReadSingle();
                this.SmallBig_Opp = reader.ReadSingle();
            }
        }

        public void Save(string filename)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
                writer.Write(this.Small);
                writer.Write(this.Big);
                writer.Write(this.SmallSmall_Same);
                writer.Write(this.SmallSmall_Opp);
                writer.Write(this.BigBig_Same);
                writer.Write(this.BigBig_Opp);
                writer.Write(this.BigSmall_Same);
                writer.Write(this.SmallBig_Same);
                writer.Write(this.BigSmall_Opp);
                writer.Write(this.SmallBig_Opp);
            }
        }
    }
}
