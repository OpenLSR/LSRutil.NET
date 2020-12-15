using System;
using System.Collections.Generic;
using System.Text;

namespace LSRutil
{
    public class ResourceTable
    {
        public Dictionary<int,string> contents;
        public List<string> directories;

        public ResourceTable()
        {
            this.contents = new Dictionary<int,string>();
            this.directories = new List<string>();
        }

        public ResourceTable(Dictionary<int,string> contents)
        {
            this.contents = contents;
        }

        public void Add(int id, string path)
        {
            contents.Add(id, path);
        }

        public void AddDirectory(string directory)
        {

        }
    }
}
