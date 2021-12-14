using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day15_RambunctiousRecitation : AoCodeModule
    {
        Dictionary<int, int[]> memoryGame = new Dictionary<int, int[]>();
        public Day15_RambunctiousRecitation()
        {

            OutputFileReset(); // output file will be in the same location as input, with ".output.txt" appended to the name. This clears previous output from file.
        }
        public override void DoProcess()
        {
            //** > Result for Day15_RambunctiousRecitation part 1:The 2020th number is 447(Process: 0.1688 ms)
            //** > Result for Day15_RambunctiousRecitation part 2:The 30000000th number is 11721679(Process: 5071.9555 ms)

            // went through many different ways of doing this. Lookup times on dictionary collection is the best so far. List<object> SUCKED.

            ResetProcessTimer(true);
            List<int> startingNumbers = "8,11,0,19,1,2".Split(new char[] { ',' }).ToList<string>().Select(x => int.Parse(x)).ToList();
            memoryGame.Clear();
            int idx = 0;
            startingNumbers.ForEach(x => memoryGame.Add(x, new int[2] { ++idx, -1 }) );
            AddResult("The 2020th number is " + PlayGameToTurn(2020,startingNumbers.Last()).ToString());

            ResetProcessTimer(true);
            memoryGame.Clear();
            idx = 0;
            startingNumbers.ForEach(x => memoryGame.Add(x, new int[2] { ++idx, -1 }));
            AddResult("The 30000000th number is " + PlayGameToTurn(30000000, startingNumbers.Last()).ToString());
        }
        public int PlayGameToTurn(int NumberOfTurns, int lastNumberSpoken )
        {
            int[] flipFlopTheValues = memoryGame[lastNumberSpoken];
            int idx = memoryGame.Count + 1;
            while (idx <= NumberOfTurns)
            {
                // we always start each iteration with the last number spoken and the array of turns involved already set to avoid the second lookup.
                lastNumberSpoken = (flipFlopTheValues[1] == -1) ? 0 : flipFlopTheValues[0] - flipFlopTheValues[1];

                if (!memoryGame.ContainsKey(lastNumberSpoken))
                {
                    flipFlopTheValues = new int[2] { idx, -1 };
                    memoryGame.Add(lastNumberSpoken, flipFlopTheValues);
                }
                else
                {
                    flipFlopTheValues = memoryGame[lastNumberSpoken];
                    flipFlopTheValues[1] = flipFlopTheValues[0];
                    flipFlopTheValues[0] = idx;
                    memoryGame[lastNumberSpoken] = flipFlopTheValues;
                }
                idx++;
            }
            return lastNumberSpoken;
        }
    }
}
