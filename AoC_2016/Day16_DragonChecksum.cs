using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day16_DragonChecksum : AoCodeModule
    {
        public Day16_DragonChecksum()
        {
            // If you always save input file in the /InputFiles/ subfolder and name it the same as the class processing it, this will work.
            // if you put it elsewhere or name it differently, just change below. 
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); //base class method
            OutputFileReset(); // output file will be in the same location as input, with ".output.txt" appended to the name. This clears previous output from file.
            //Print("Did Something");// outputs to console immediately
            //Print("DidSomethingElse", FileOutputAlso); // both console and output file
            //Print("Another Thing", FileOutputOnly); // output file only.
        }
        public override void DoProcess()
        {
            ResetProcessTimer(true);// true also iterates the section marker
            string diskData = "10001001100000001";
            int diskSpace = 272;
            while (diskData.Length < diskSpace) { diskData = Dragonize(diskData); }
            diskData = diskData.Substring(0, diskSpace);
            diskData = GetChecksum(diskData);
            AddResult("Checksum is " + diskData);
            ResetProcessTimer(true);
            diskData = "10001001100000001";
            diskSpace = 35651584;
            while (diskData.Length < diskSpace) { diskData = Dragonize(diskData); }
            diskData = diskData.Substring(0, diskSpace);
            diskData = GetChecksum(diskData);
            AddResult("Checksum is " + diskData);
        }
        public string Dragonize(string a)
        {
            string b = a;
            b = new string(b.Reverse().ToArray());
            b = b.Replace('0', 'x').Replace('1','0').Replace('x','1');


            return a + "0" + b;
        }
        public string GetChecksum(string input)
        {
            StringBuilder result = new StringBuilder(input.Length / 2 + 1);
            char[] convert= input.ToCharArray();
            int point = 0;
            while (point * 2 < input.Length)
            {
                //string test = input.Substring(point * 2, 2);
                //result += (input[point*2] == input[(point*2)+1]) ? "1" : "0";
                result.Append((convert[point * 2] == convert[(point * 2) + 1]) ? "1" : "0");
                point++;
                if (point % 500000 == 0) Print(point.ToString());
            }
            string csResult = result.ToString();
            if (csResult.Length % 2 == 0) csResult = GetChecksum(csResult);
            return csResult;
        }
    }
}
