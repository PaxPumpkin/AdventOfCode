using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day05_SupplyStacks : AoCodeModule
    {
        public Day05_SupplyStacks()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput();
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day05_SupplyStacks part 1: Last crate on each stack is PTWLTDSJV (Process: 0.6281 ms)
            //** > Result for Day05_SupplyStacks part 2: When doing all at once, the last crate on each stack is WZMFVGGZP (Process: 0.265 ms)
            ResetProcessTimer(true);
            List<List<char>> stacks = new List<List<char>>(), originalStacks = new List<List<char>>();
            for (int x = 0; x < 9; x++) stacks.Add(new List<char>());
            List<MovementInstruction> craneOperations = new List<MovementInstruction>();
            string[] movementParse = new string[] { "move", "from", "to", " " };
            bool stackLoad = true; // beginning of input file is the starting stack arrangment, and is parsed differently than the movement instructions
            foreach (string processingLine in inputFile)
            {
                if (processingLine.Trim().Equals(string.Empty) && stackLoad) stackLoad = false; // marker between parsing styles. 
                else if (processingLine.Trim().Equals(string.Empty)) { } // !stackLoad implied, and no operation. Should now terminate input parsing loop
                else
                {
                    if (stackLoad)
                    {
                        for (int x = 0; x < 9 && processingLine.Contains("["); x++)// until stack loading is completed, the final line in that data set is just stack numbers and has no brackets. Ignore it. 
                            if (processingLine.Substring(x + x * 3, 3)[1] != ' ') stacks[x].Insert(0, processingLine.Substring(x + x * 3, 3)[1]);
                    }
                    else
                        craneOperations.Add(new MovementInstruction(processingLine.Split(movementParse, StringSplitOptions.RemoveEmptyEntries)));
                }
            }
            stacks.ForEach(stack => originalStacks.Add(stack.ToList())); // store originals for reuse on part 2
            AddResult("Parsing Complete"); ResetProcessTimer(true);
            ExecuteOperations(craneOperations, stacks, false);// CrateMover 9000 moves one crate at a time
            AddResult("Last crate on each stack is " + TopCrates(stacks));
            ResetProcessTimer(true);
            ExecuteOperations(craneOperations, originalStacks, true);// CrateMover 9001 can move multiples at once. 
            AddResult("When doing all at once, the last crate on each stack is " + TopCrates(originalStacks));
            ResetProcessTimer(true);
        }
        public void ExecuteOperations(List<MovementInstruction> operations, List<List<char>> stacks, bool atOnce)
        {
            // stack numbering is 1-based, but list indices are, of course, 0-based. 
            operations.ForEach(movement => OperateCrane(movement.Quantity, stacks[movement.From - 1], stacks[movement.To - 1], atOnce));
        }
        public string TopCrates(List<List<char>> stackSet)
        {
            StringBuilder builder = new StringBuilder();
            stackSet.ForEach(stack => builder.Append(stack.Last()));
            return builder.ToString();
        }
        public void OperateCrane(int quantity, List<char> from, List<char> to, bool atOnce = false)
        {
            List<char> tempStack = from.ToList(); // to do potential reverse operations, create separate worklist.
            from.RemoveRange(from.Count - quantity, quantity);
            if (!atOnce) tempStack.Reverse();// if taken one at a time, the order will be changed 
            to.AddRange((atOnce ? tempStack.Skip(tempStack.Count - quantity) : tempStack).Take(quantity));
        }
        public struct MovementInstruction
        {
            public int Quantity; // number of crates to move
            public int From; // stack number of source
            public int To; // stack number of destination
            public MovementInstruction(string[] movementInstruction)
            {
                Quantity = int.Parse(movementInstruction[0]); From = int.Parse(movementInstruction[1]); To = int.Parse(movementInstruction[2]);
            }
        }
    }
}
