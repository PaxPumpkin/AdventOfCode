using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day09_EncodingError : AoCodeModule
    {
        List<long> XMAS;
        int pointer;
        public Day09_EncodingError()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput();
            OutputFileReset();

        }
        public override void DoProcess()
        {
            //** > Result for Day09_EncodingError part 1:The first number in the stream that does not validate is 90433990(Process: 8 ms)
            //** > Result for Day09_EncodingError part 2:At a range of length 17(max: 559), we found contiguous block of numbers starting at 441(Process: 0 ms)
            //** > Result for Day09_EncodingError part 2:The encryption weakness is 11691646(Process: 0 ms)
             
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            XMAS = new List<long>();
            foreach (string processingLine in inputFile)
            {
                XMAS.Add(Convert.ToInt64(processingLine));
            }
            AddResult("Data input processing completed");
            ResetProcessTimer(true);
            
            pointer = 25;
            while (ValidInput()) { pointer++; }
            finalResult = "The first number in the stream that does not validate is " + XMAS[pointer] + " at index " + pointer.ToString();
            AddResult(finalResult);

            ResetProcessTimer(true);
            long result = SolveFor(XMAS[pointer]);
            finalResult = "The encryption weakness is " + result.ToString();
            AddResult(finalResult);
        }

        private bool ValidInput()
        {
            bool isValid = false;
            long target = XMAS[pointer];
            List<long> availableHeader = XMAS.GetRange(pointer-25, 25);
            isValid = availableHeader.Count(first => ( availableHeader.Count(second=> second!=first && second+first==target)>0)) > 0;

            return isValid;
        }
        private long SolveFor(long target)
        {
            long result = 0;
            // range technically can't go farther than 999 as that is the entire file
            //also, have to remember that "pointer" is still artificially bounding our range, as only contiguous blocks are allowed
            // so it had to be no longer than [0,pointer-1] or [pointer+1,999]
            int lowerBoundForRanging = 0, upperBoundForRanging = pointer - 1; // pointer of 25(26th value) has a range length of 25, from 0-24
            int maxRangeSize = upperBoundForRanging - lowerBoundForRanging + 1;
            int currentRangeSize = 2; //start with blocks of 2.
            while (currentRangeSize <= maxRangeSize)
            {
                int rangeMarker = lowerBoundForRanging;
                while (rangeMarker + currentRangeSize <= upperBoundForRanging+1) { // eg if pointer==25, then lower=0, upper=24, max=25. If we reach max, a range of 25 from 0[values 0-24], then even the case where rangemarker=0 fails the first time even though it's legit.
                    List<long> testRange = XMAS.GetRange(rangeMarker, currentRangeSize);
                    if (testRange.Sum() == target)
                    {
                        AddResult("At a range of length " + currentRangeSize.ToString() + "(max:" + maxRangeSize.ToString() + "), we found contiguous block of numbers starting at " + rangeMarker.ToString());
                        result = testRange.Min() + testRange.Max();
                        break;
                    }
                    rangeMarker++;
                }
                if (result > 0) { break; }
                currentRangeSize++;
            }
            // If result is still zero, the answer was not in the lower segment of numbers. 
            // Reset to search above the index of our pointer 
            // (although the puzzle doesn't actually say, I suspect the answer is supposed 
            //      to be found in the numbers BELOW the target, not above. Setting up for
            //      the possibility, though, just in case.)
            if (result == 0) 
            {
                lowerBoundForRanging = pointer + 1;
                upperBoundForRanging = XMAS.Count() - 1;
                maxRangeSize = upperBoundForRanging - lowerBoundForRanging + 1;
                currentRangeSize = 2;
                while (currentRangeSize <= maxRangeSize)
                {
                    int rangeMarker = lowerBoundForRanging;
                    while (rangeMarker + currentRangeSize <= upperBoundForRanging+1)
                    {
                        List<long> testRange = XMAS.GetRange(rangeMarker, currentRangeSize);
                        if (testRange.Sum() == target)
                        {
                            result = testRange.Min() + testRange.Max();
                            break;
                        }
                        rangeMarker++;
                    }
                    if (result > 0) { break; }
                    currentRangeSize++;
                }
            }
            // if result is still zero, we didn't find a range that met the requirements, which would be a coding problem. It is implied that the answer IS there. We just have to find it. 
            return result;
        }
    }
}
