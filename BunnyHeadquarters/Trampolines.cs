using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace BunnyHeadquarters
{
    class Trampolines
    {
        static bool problemTwo = false; // toggle behavior for problem 1 v 2
        static void Main(string[] args)
        {
            List<int> jumpfile = new List<int>(); 
            StreamReader sr = new StreamReader("C:\\Sandbox\\2016AoC\\BunnyHeadquarters\\BunnyHeadquarters\\trampolines.txt");
            while (!sr.EndOfStream)
            {
                jumpfile.Add(Int32.Parse(sr.ReadLine())); // read all the jump instructions
            }
            sr.Close();
            int[] jumpstack = jumpfile.ToArray(); // turn it into a proper stack.
            int InstructionPointer = 0; // start at the beginning.
            int stepsToLeave = 0; //counter of how many jumps we've made. 
            int thisJump = 0; // the jump number forward or back for this read/execute in the stack. Holder variable.
            // if we leave the array, I guess we "got out". The example only shows moving out the top, but I'd say if I drop out the bottom, that would count, too. 
            while (InstructionPointer >= jumpstack.GetLowerBound(0) && InstructionPointer <= jumpstack.GetUpperBound(0))
            {
                thisJump = jumpstack[InstructionPointer]; // read jump command at our stack pointer. 
                // decrement the jump instruction if we're doing problem two AND the jump instruction is positive 3 or higher. Otherwise always increment.
                jumpstack[InstructionPointer] += (problemTwo && thisJump >= 3) ? -1 : 1;
                InstructionPointer += thisJump; // move pointer to new location as per instruction
                stepsToLeave++; //increment counter.
            }
            Console.WriteLine("total steps: " + stepsToLeave.ToString());
            Console.ReadLine();
        }
    }
}
