namespace LSRutil
{
    public interface IDataDescription
    {
        int GetTypeIdx();
    }
    
    public class DataDescription<T> : IDataDescription
    {
        public uint ID;
        
        public T Default;
        public T Minimum;
        public T Maximum;

        public string Name;
        public string Description;
        
        public int GetTypeIdx()
        {
            switch (typeof(T).Name)
            {
                case "int":
                    return 0;
                case "float":
                    return 1;
                case "Vector3":
                    return 2;
                case "string":
                    return 3;
                default:
                    return -1;
            }
        }

        public DataDescription(uint id, string name, string desc, T def, T min, T max)
        {
            ID = id;
            Default = def;
            Minimum = min;
            Maximum = max;

            Name = name; 
            Description = desc;
        }
    }
}