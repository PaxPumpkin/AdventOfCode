using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2020
{
    class Day05_BinaryBoarding : AoCodeModule
    {
        public Day05_BinaryBoarding()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        public override void DoProcess()
        {
            //** > Result for Day05_BinaryBoarding part 1:Highest ticket id is 801(Process: 0 ms)
            //** > Result for Day05_BinaryBoarding part 2:My Boarding Pass ID is 597(Process: 3 ms)
            
            List<string[]> boardingPasses = new List<string[]>();
            List<int[]> resolvedBoardingPasses = new List<int[]>();
            inputFile.ForEach(line => { boardingPasses.Add(new string[] {line.Substring(0,7).Replace('F','0').Replace('B','1'), line.Substring(7).Replace('R','1').Replace('L','0') }); });
            boardingPasses.ForEach(line => { resolvedBoardingPasses.Add(new int[] {Convert.ToInt32(line[0],2),Convert.ToInt32(line[1],2) }); });
            int max = 0;
            resolvedBoardingPasses.ForEach(line => { int test = (line[0]<<3) | line[1]; max = test > max ? test : max; });
            string finalResult = "Highest ticket id is " + max.ToString();
            ResetProcessTimer(true);
            AddResult(finalResult);
            ResetProcessTimer(true);
            List<int> allBoardingPasses = new List<int>();
            //10 bit int, duh. Just mash together. sheesh.
            boardingPasses.ForEach(line => { allBoardingPasses.Add(Convert.ToInt32(line[0]+ line[1], 2)); });
            // find the one where the next ID is not in our list, but that the one after THAT one is. Add 1 to get the actual missing boarding pass ID.
            finalResult = "My Boarding Pass ID is " + (allBoardingPasses.Where(x => !allBoardingPasses.Contains(x + 1) && allBoardingPasses.Contains(x + 2)).ToList()[0]+1).ToString();
            AddResult(finalResult);

        }
    }
}
