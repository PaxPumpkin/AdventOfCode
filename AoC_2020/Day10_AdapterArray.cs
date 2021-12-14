using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day10_AdapterArray : AoCodeModule
    {
        public Day10_AdapterArray()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput();
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day10_AdapterArray part 1:There are 69 one jolt differences and 24 three jolt differences(Process: 0 ms)
            //** > Result for Day10_AdapterArray part 1:Answer: 1656(Process: 0 ms)
            //** > Result for Day10_AdapterArray part 2:Sheesh: 56693912375296(Process: 0 ms)
             
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            List<int> inputValues = new List<int>();
            foreach (string processingLine in inputFile)
            {
                inputValues.Add(Convert.ToInt32(processingLine));
            }
            inputValues = inputValues.OrderBy(x => x).ToList();
            int oneJolt = 0;
            int threeJolt = 0;

            int lastComparitor = 0; // the socket always starts at zero
            inputValues.ForEach(x =>
            {
                if (x-lastComparitor==1) { oneJolt++;  }
                if (x-lastComparitor==3) { threeJolt++; }
                lastComparitor = x;
            });
            threeJolt++; // for the phone, since the difference is always 3 jolts from the highest value.
            finalResult = "Answer: " + (oneJolt*threeJolt).ToString();
            AddResult(finalResult); 

            ResetProcessTimer(true);

            // to count path permutations, remember that to get the path count to value "x", 
            // there are only a certain number of values in our list below that value that 
            // can reach it. To reach X, the value must be <=3 different than the value of X.
            // Starting from the socket (which is 0 "jolts" from anywhere as per puzzle info)
            // ...and adding the final destination (which is always +3 jolts from the last adapter in the list)
            // And so, for any given X, just look at the values that are <=3 "jolts" from it below
            // and add the paths that can get to THAT value.
            // So, we iterate forward from the beginning and count up all paths that can get to each one
            // then by looking backwards, and just add them all together. 
            // The puzzle says "trillions", so... using a Long int for accumulator/counting. 
            inputValues.Insert(0, 0); //add the socket to start
            inputValues.Add(inputValues.Last()+3);//phone as our ending point in the iteration. 
            List<long> paths = new List<long>(inputValues.Count); // if we set the capacity ahead of time, we will save time in having it grow as we add.
            paths.Add(1);// the number of paths to the socket is 1, to start. 
            
            // now go through all the rest and add up anything before it that can reach it.
            inputValues.GetRange(1,inputValues.Count-1).ForEach(adapter =>
            {
                paths.Add(0);
                inputValues.Where(v => v < adapter && adapter - v <= 3).ToList().ForEach(v => paths[paths.Count - 1] += paths[inputValues.IndexOf(v)]);
            });

            finalResult = "Sheesh: " + paths.Last().ToString();
            AddResult(finalResult);
        }
    }
}
