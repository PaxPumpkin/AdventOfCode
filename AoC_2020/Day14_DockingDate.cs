using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day14_DockingDate : AoCodeModule
    {
        public Day14_DockingDate()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 

        }
        public override void DoProcess()
        {
            //** > Result for Day14_DockingDate part 1:Summation of memory values: 10035335144067(Process: 7.2813 ms)
            //** > Result for Day14_DockingDate part 2:Summation of floating memory values: 3817372618036(Process: 111638.5418 ms)
            // golfed method strategy for char replacement and redoing unecessary work.
            //** > Result for Day14_DockingDate part 1:Summation of memory values: 10035335144067(Process: 4.1683 ms)
            //** > Result for Day14_DockingDate part 2:Summation of floating memory values: 3817372618036(Process: 46947.6735 ms)
            // golfed method for straight bit logic. No significant improvement.
            //** > Result for Day14_DockingDate part 1:Summation of memory values: 10035335144067(Process: 2.2201 ms)
            //** > Result for Day14_DockingDate part 2:Summation of floating memory values: 3817372618036(Process: 43179.371 ms)
            ResetProcessTimer(true);

            List<MemoryAddress> memory = new List<MemoryAddress>();
            string bitMask = "";
            List<string> parsedLine;
            long result = 0;
            char[] splitters = new char[] { ' ', '=', '[', ']' };
            MemoryAddress workingAddress;
            foreach (string processingLine in inputFile)
            {
                parsedLine = processingLine.Split(splitters, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (parsedLine[0] == "mask") bitMask = parsedLine[1];
                else
                {
                    long address = long.Parse(parsedLine[1]);
                    if (memory.Count(x => x.address == address) == 0)
                    { 
                        memory.Add( new MemoryAddress() { address = address, value = MemoryAddress.ComputeValueBS(bitMask, parsedLine[2]) }); 
                    }
                    else
                    {
                        memory.Where(x => x.address == address).First().value = MemoryAddress.ComputeValueBS(bitMask, parsedLine[2]);
                    }
                }
            }
            result = 0;
            memory.ForEach(x => result += x.value);
            AddResult("Summation of memory values: " + result.ToString());
            ResetProcessTimer(true);
            memory.Clear();
            long assignedValue = 0;
            List<long> allAddresses;
            foreach (string processingLine in inputFile)
            {
                parsedLine = processingLine.Split(splitters, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (parsedLine[0] == "mask")
                {
                    bitMask = parsedLine[1];
                }
                else
                {
                    //allAddresses = MemoryAddress.ComputeFloatingAddresses(bitMask, parsedLine[1]);
                    allAddresses = MemoryAddress.ComputeFloatingAddressesGolfing(bitMask, parsedLine[1]);
                    assignedValue = long.Parse(parsedLine[2]);
                    allAddresses.ForEach(address =>
                    {
                        if (memory.Count(x => x.address == address) == 0)
                        {
                            memory.Add(new MemoryAddress() { address = address, value = assignedValue });
                        }
                        else
                        {
                            memory.Where(x => x.address == address).First().value = assignedValue;
                        }
                    });

                }
            }
            result = 0;
            memory.ForEach(x => result += x.value);
            AddResult("Summation of floating memory values: " + result.ToString());
        }
    }
    class MemoryAddress
    {
        public long address { get; set; }
        public long value { get; set; }
        public static long ComputeValueBS(string bitmask, string input)
        {
            long result = long.Parse(input);
            for (int x = 0; x < 36; x++)
            {
                if (bitmask[x] == '0' || bitmask[x] == '1')
                {
                    result = bitmask[x] == '0' ? result & ~(long)((long)1 << (35 - x)) : result | (long)((long)1 << (35 - x));
                }
            }
            return result;
        }

        public static List<long> ComputeFloatingAddresses(string bitmask, string input)
        {
            List<long> possibilities = new List<long>();
            long converted = long.Parse(input);
            string binaryInput = Convert.ToString(converted, 2).PadLeft(36, '0');
            char[] templateBuilder = binaryInput.ToArray();

            for (int x = 0; x < 36; x++)
            { // replace all input bits with floating indicators or setting explicit 1 values. bitmask values of 0 mean leave that value alone. 
                if (bitmask[x] == 'X') templateBuilder[x] = 'X';
                if (bitmask[x] == '1') templateBuilder[x] = '1';
            }
            string template = new string(templateBuilder);
            if (template.Contains("X")) // pretty sure they ALL will, but just in case....
            {
                // quick explanation....
                // any "X" has two options. Multiple Xs have powers-of-two permutations: 2^(x count)
                // each permutation is really just all zeroes through to all ones.
                // So, get the binary representation of each permutation, padded out for all significant bits
                //  and iterate through all permuations, replacing each X with the appropriate bit symbol
                //  going left-to-right. 
                // eg. template "X0X0X0X0" has 4 Xs, so 2^4 permutations (16)
                // count from 0 to 15, convert to padded binary
                // "0000" (example "0101") until "1111"
                // replace first X with first bit
                // replace second X with second bit
                // etc...
                // "X0X0X0X0"
                // "00000000" for first permuation (0)
                // "00100010" for the example from above
                // "10101010" for the last (15)
                // convert to a long int(more than 32 bits);

                int baseCount = template.Count(q => q == 'X');
                int templateFiddleCount = 2;
                //math.pow did REALLY funky shit. Don't wanna fuck with it, so do it manually. 
                for (int x=1; x<baseCount; x++) { templateFiddleCount *= 2; }
                // templateFiddleCount is now the count of all permutations. 
                string templateFiddler;
                char[] aResult = new char[templateBuilder.Length];
                int XPointer=0;
                int[] allIndices = new int[baseCount];
                bool didZeroOnce = false; // flag to overcome the silliness of a floating bit at the first position.
                for (int x = 0; x < baseCount; x++)
                {
                    XPointer = template.IndexOf('X', (XPointer == 0 && !didZeroOnce ? 0 : XPointer + 1));
                    didZeroOnce = true; // always, it can only be flipped first time through anyway. 
                    allIndices[x] =XPointer; 
                }
                templateBuilder.CopyTo(aResult, 0); // put our template into our result calculator. Only have to do this once, since the bits that change always get overwritten.
                for (int x = 0; x< templateFiddleCount; x++) // for each permutation
                {
                    templateFiddler = Convert.ToString(x, 2).PadLeft(baseCount, '0'); // pad out our permutation to binary
                    for (int y=0; y<allIndices.Length; y++)
                    {
                        aResult[allIndices[y]] = templateFiddler[y];
                    }
                    possibilities.Add(Convert.ToInt64(new string(aResult),2)); 
                }
            }
            else
            {
                possibilities.Add(Convert.ToInt64(template,2));
            }
            return possibilities;
        }
        public static List<long> ComputeFloatingAddressesGolfing(string bitmask, string input)
        {
            List<long> possibilities = new List<long>();
            long converted = long.Parse(input);
            string binaryInput = Convert.ToString(converted, 2).PadLeft(36, '0');
            char[] templateBuilder = binaryInput.ToArray();

            for (int x = 0; x < 36; x++)
            { // replace all input bits with floating indicators or setting explicit 1 values. bitmask values of 0 mean leave that value alone. 
                if (bitmask[x] == 'X') templateBuilder[x] = 'X';
                if (bitmask[x] == '1') templateBuilder[x] = '1';
            }
            string template = new string(templateBuilder);
            // quick explanation....
            // any "X" has two options. Multiple Xs have powers-of-two permutations: 2^(x count)
            // each permutation is really just all zeroes through to all ones.
            // So, get the binary representation of each permutation, padded out for all significant bits
            //  and iterate through all permuations, replacing each X with the appropriate bit symbol
            //  going left-to-right. 
            // eg. template "X0X0X0X0" has 4 Xs, so 2^4 permutations (16)
            // count from 0 to 15, convert to padded binary
            // "0000" (example "0101") until "1111"
            // replace first X with first bit
            // replace second X with second bit
            // etc...
            // "X0X0X0X0"
            // "00000000" for first permuation (0)
            // "00100010" for the example from above
            // "10101010" for the last (15)
            // convert to a long int(more than 32 bits);

            int baseCount = template.Count(q => q == 'X');
            int templateFiddleCount = 2;
            //math.pow did REALLY funky shit. Don't wanna fuck with it, so do it manually. 
            for (int x = 1; x < baseCount; x++) { templateFiddleCount *= 2; }
            // templateFiddleCount is now the count of all permutations. 
            string templateFiddler;
            char[] aResult = new char[templateBuilder.Length];
            int XPointer = 0;
            long[] allIndices = new long[baseCount];
            bool didZeroOnce = false; // flag to overcome the silliness of a floating bit at the first position.
            for (int x = 0; x < baseCount; x++)
            {
                XPointer = template.IndexOf('X', (XPointer == 0 && !didZeroOnce ? 0 : XPointer + 1));
                didZeroOnce = true; // always, it can only be flipped first time through anyway. 
                allIndices[x] = (long)1<<(35-XPointer);
            }
            template = template.Replace('X', '0');
            long baseValue = Convert.ToInt64(template, 2);
            long resetValue = baseValue;
            for (int x = 0; x < templateFiddleCount; x++) // for each permutation
            {
                templateFiddler = Convert.ToString(x, 2).PadLeft(baseCount, '0'); // pad out our permutation to binary
                for (int y = 0; y < allIndices.Length; y++)
                {
                    // this will SET 0 or 1 as necessary
                    //baseValue = templateFiddler[y] == '0' ? baseValue &~allIndices[y] : baseValue | allIndices[y];
                    // but if we reset to the "all Zeroes" version each iteration, we should only have to set the "1" bits.
                    if (templateFiddler[y] == '1') baseValue |= allIndices[y];
                }
                possibilities.Add(baseValue);
                baseValue = resetValue; 
            }

            return possibilities;
        }

    }

}
