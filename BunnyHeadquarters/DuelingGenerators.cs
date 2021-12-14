using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class DuelingGenerators : AoCodeModule
    {
        public DuelingGenerators()
        {
            // No input file for this day

        }

        public override void DoProcess()
        {
            //Generator A starts with 634  ( or 516)
            //Generator B starts with 301   (or 190)
            int startGenA = 634; // these are reset after the "part 1" is done, so saving them separately.
            int startGenB = 301;


            long genAValue = startGenA;// init for part 1
            long genBValue = startGenB;
            long genAFactor = 16807; // these two stay the same for parts 1 and 2
            long genBFactor = 48271;
            int divisor = Int32.MaxValue; // the two billion number.
            int matchesOnLowest16Bits = 0; // counter
            for (int x=0; x<40000000; x++) // 40 million iterations.
            {
                // generate and compare.
                genAValue= (genAValue*genAFactor)%divisor;
                genBValue= (genBValue*genBFactor)%divisor;
                if (((short)genAValue) == ((short)genBValue))// auto-drops all the bits we don't want any more. Only need to match the last 16 bits, so a short int.
                {
                    matchesOnLowest16Bits++;
                }
            }
            FinalOutput.Add("Part 1 matches: " + matchesOnLowest16Bits.ToString());
            // init for part 2. 
            genAValue = startGenA;
            genBValue = startGenB;
            matchesOnLowest16Bits = 0;
            for (int x = 0; x < 5000000; x++) // 5 million pairs to be matched.
            {
                genAValue= (genAValue*genAFactor)%divisor; // generate numbers
                genBValue= (genBValue*genBFactor)%divisor;
                while (genAValue % 4 != 0)// while the generated number from A is not evenly divisible by 4, keep generating a new A.
                {
                    genAValue = (genAValue * genAFactor) % divisor;
                }
                // ok, so GenA is evenly divisible by 4, so now do the same thing for B, but evenly divisible by 8 instead of 4.
                while (genBValue % 8 != 0)
                {
                    genBValue = (genBValue * genBFactor) % divisor;
                }
                // we now have a pair that matches the pre-conditions. Compare them.
                if (((short)genAValue) == ((short)genBValue)) // auto-drops all the bits we don't want any more.
                {
                    matchesOnLowest16Bits++;
                }
            }// keep looping for 5 million iterations.
            FinalOutput.Add("Part 2 matches: " + matchesOnLowest16Bits.ToString());
        }
    }
}
