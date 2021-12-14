using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class KnotHash : AoCodeModule
    {
        public KnotHash()
        {
            oneLineData = "129,154,49,198,200,133,97,254,41,6,2,1,255,0,191,108";
        }
        public override void DoProcess()
        {
            
            FinalOutput.Add("Static call: " + GetKnotHash(oneLineData));
        }

        // refactored in a static function for use by another day's problem ( day 14, DiskDefrag.cs )
        public static string GetKnotHash(string input)
        {
            string hashHex = "";
            Int64 result = 0; // used for part 1, but the algorithm changed, and this can't really be used this way anymore.
            List<int> thingy = new List<int>();
            // round one was parsing out the ints...
            //(new List<string>(oneLineData.Split(new char[] { ',' }))).ForEach(x => thingy.Add(Convert.ToInt32(x)));
            // round two is to treat EACH CHAR as a byte (int) value, including the commas
            // and then add these values to the end....   17,31,73,47,23 (NOT including the commas, just add the numbers (as char)
            List<char> ASCIIconversion = input.ToCharArray().ToList(); // this includes commas!!
            byte[] additions = new byte[] { 17, 31, 73, 47, 23 }; // this will add discrete values, 5 of them (no commas).
            additions.ToList().ForEach(x => ASCIIconversion.Add(Convert.ToChar(x)));
            // all of the input has been converted to ascii and then the individual numbers have been added.
            ASCIIconversion.ForEach(x => thingy.Add(Convert.ToInt32(x))); // now put them back into the int array to be processed. 




            int[] numbers = new int[256];
            for (int x = 0; x < numbers.Length; x++) { numbers[x] = x; }

            int currentPosition = 0;
            int skipSize = 0;
            for (int q = 1; q <= 64; q++) // round two, doing the whole thing 64 times. Preserve the current position and skip size(so those init stay up there, out of this loop)
            {
                foreach (int length in thingy)
                {
                    int additionalLength = 0;
                    if (currentPosition + length >= numbers.Length)
                    {
                        additionalLength = Math.Abs(numbers.Length - (currentPosition + length));
                    }
                    int[] subArrary = (numbers.Skip(currentPosition).Take(length).ToArray().Concat(numbers.Skip(0).Take(additionalLength).ToArray())).ToArray();
                    int[] reversed = subArrary.Reverse().ToArray();
                    if (additionalLength > 0)
                    {
                        reversed.Skip(0).Take(length - additionalLength).ToArray().CopyTo(numbers, currentPosition);
                        reversed.Skip(length - additionalLength).Take(additionalLength).ToArray().CopyTo(numbers, 0);
                    }
                    else
                    {
                        reversed.CopyTo(numbers, currentPosition);
                    }
                    currentPosition += length + skipSize;
                    while (currentPosition >= numbers.Length) { currentPosition = currentPosition - numbers.Length; }
                    skipSize++;
                }
                // round one was to multiply the first two numbers after one interation. This is no longer useful for round two, and not even accurate after doing the char-to-int conversion.
                //if (q==1){result = numbers[0] * numbers[1];} // first loop only.  !!!!  ooops! the input param isnt' treated the same way. This value is useless now. 
            }

            //XOR each block of 16 numbers 
            int[] hashBlocks = new int[16];
            for (int q = 0; q < 16; q++) // 16 blocks of 16 in 256 chars.
            {
                int blockHash = 0;
                numbers.Skip(q * 16).Take(16).ToList().ForEach(x => blockHash = blockHash ^ x);
                hashBlocks[q] = blockHash;
            }
            foreach (int x in hashBlocks)
            {
                hashHex += x.ToString("x").PadLeft(2,'0'); // number to HEX (C# built-in), padded because a value of 15 or less will give only a 1-digit hex code, and we want 2 digits ( to make 32 bytes )
            }

            return hashHex;
        }
    }
}
