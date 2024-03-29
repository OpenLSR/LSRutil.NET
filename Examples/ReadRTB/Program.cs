﻿using System;
using LSRutil;
using LSRutil.RTB;

namespace ReadRTB
{
    class Program
    {
        static void Main(string[] args)
        {
            var reader = new RtbReader();
            var table = reader.ReadTable(@"C:\LSR\MOTO.rtb");

            foreach (var item in table.directories)
            {
                Console.WriteLine("DIR: {0}", item);
            }
            foreach (var item in table.contents)
            {
                Console.WriteLine("[0x{0:X8}] {1}", item.Key, item.Value);
            }
            Console.WriteLine("{0} entries", table.contents.Count);
            Console.WriteLine("First available slot: [0x{0:X8}]", table.GetFirstAvailableSlot());

            var writer = new RtbWriter();
            writer.WriteTable(table, "deeznuts.rtb");
        }
    }
}
