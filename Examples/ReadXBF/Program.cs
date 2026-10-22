using System;
using LSRutil;
using LSRutil.XBF;

namespace ReadXBF
{
    class Program
    {
        static void Main(string[] args)
        {
            var reader = new XbfReader();
            //reader.ReadModel("../../../../ReadRF/bin/Debug/net6.0/extract-art/worlds/city/ctrack/ctchican/ctchican.xbf");
            //reader.ReadModel("O:\\LEGOSR\\art\\worlds\\city\\cskirts\\ck16pc\\ck16pc.XBF");
            reader.ReadModel("C:\\bruh\\CITY0001\\ctrack\\cccrusher\\cccrusher.xbf");
            //reader.ReadModel("O:\\LEGOSR\\art\\worlds\\ice\\iskirts\\ik16pc\\ik16pc.XBF");
            //reader.ReadModel("O:\\LEGOSR\\art\\worlds\\ice\\iscene\\isradar\\isradar.XBF"); 
            reader.WriteGltf();
        }
    }
}
