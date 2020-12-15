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
            reader.ReadModel(@"D:\_CODE\C#\LSRutil.NET\Examples\ReadRF\bin\Debug\netcoreapp3.1\extract-old\worlds\city\ctrack\ctchican\ctchican.xbf");
        }
    }
}
