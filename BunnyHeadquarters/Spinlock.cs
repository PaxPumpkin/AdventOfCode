using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class Spinlock : AoCodeModule
    {
        public Spinlock()
        {
            // no datafile input.
        }
        
        public override void DoProcess()
        {
            int nextValue = 0;
            int MagicNumber = 367;
            int currentPosition = 0;
            int nextPosition;
            bool stillWorkingOnPart1 = true;
            int lastInsertionAtPosition1 = 0;
            List<int> circularBuffer = new List<int>();
            circularBuffer.Insert(currentPosition, nextValue);
            //while (nextValue != 2017)
            while (nextValue != 50000000)
            {
                nextValue++;
                
                //nextPosition = ((currentPosition + MagicNumber)%circularBuffer.Count)+1;
                nextPosition = ((currentPosition + MagicNumber) % nextValue) + 1;
                // to help with part 2, always output what number we'd put in position 1
                if (nextPosition == 1) { lastInsertionAtPosition1 = nextValue; }// Console.WriteLine(nextValue.ToString() + " is being inserted at position 1");

                if (stillWorkingOnPart1) circularBuffer.Insert(nextPosition, nextValue); // only need the actual buffer for part one, after that, it's just counting. 
                currentPosition = nextPosition;
                if (nextValue == 2017) { FinalOutput.Add("The value after I inserted 2017 is " + circularBuffer[currentPosition + 1].ToString()); stillWorkingOnPart1 = false; } //1487
            }
            FinalOutput.Add("The value after 0 after 50M iterations is " + lastInsertionAtPosition1.ToString());//25674054
        }
    }
}
