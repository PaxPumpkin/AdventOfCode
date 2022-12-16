using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day13_DistressSignal : AoCodeModule
    {
        public Day13_DistressSignal()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + "Sample.txt";
            GetInput();
            OutputFileReset();

        }
        public override void DoProcess()
        {
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            List<PacketPairs> packetPairs = new List<PacketPairs>();
            for (int x=0; x<inputFile.Count; x+=3)
            {
                DistressPacket left = new DistressPacket(inputFile[x]);
                DistressPacket right = new DistressPacket(inputFile[x+1]);
                packetPairs.Add(new PacketPairs(left, right));
            }

            AddResult(finalResult); 
        }
    }
    public class DistressPacket
    {
        public string Packet;
        private string currentDecoding;

        public DistressPacket(string packetData)
        {
            Packet = currentDecoding = packetData;
        }
        public string DecodeAStep()
        {
            currentDecoding = currentDecoding.Substring(1, currentDecoding.Length - 2);
            return currentDecoding;
        }
    }
    public class PacketPairs
    {
        public DistressPacket Left;
        public DistressPacket Right;
        public PacketPairs(DistressPacket left, DistressPacket right)
        {
            Left = left; Right = right;
        }
    }
}
