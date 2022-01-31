using System;
using System.Collections.Generic;
using System.Text;

namespace LSRutil
{
    public class ResourceTable
    {
        public Dictionary<uint,string> contents;
        public List<string> directories;

        public ResourceTable()
        {
            this.contents = new Dictionary<uint,string>();
            this.directories = new List<string>();
        }

        public ResourceTable(Dictionary<uint,string> contents)
        {
            this.contents = contents;
        }

        public void Add(uint id, string path)
        {
            contents.Add(id, path);
        }

        public void AddDirectory(string directory)
        {
            if (directories.Count >= 4) throw new IndexOutOfRangeException("Yeah");
            else directories.Add(directory);
        }

        /// <summary>
        /// Finds the first empty slot in the RTB file, starting from the specified index, or 1 if none is specified.
        /// </summary>
        /// <param name="start">The index to start searching from.</param>
        /// <returns>The ID of the slot found</returns>
        ///
        public uint GetFirstAvailableSlot(uint start = 1)
        {
            for (uint i = start; i < uint.MaxValue; i++)
            {
                if (!contents.ContainsKey(i)) return i;
            }
            return 0;
        }
    }
}
