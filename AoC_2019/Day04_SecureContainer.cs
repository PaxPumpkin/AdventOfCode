using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2019
{
    class Day04_SecureContainer : AoCodeModule
    {
        public Day04_SecureContainer()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day04_SecureContainer part 1:Meets rules: 1864(Process: 271 ms)
            //** > Result for Day04_SecureContainer part 2:Meets Part 2 rules: 1258(Process: 276 ms)
 
            ResetProcessTimer(true);
            int bottomValue = 137683, topValue = 596253;
            int validCounter = 0;
            for (int testedValue = bottomValue; testedValue<=topValue; testedValue++)
            {
                if (TestValue(testedValue)) validCounter++;
            }
            AddResult("Meets rules: " + validCounter.ToString());
            ResetProcessTimer(true);
            validCounter = 0;
            for (int testedValue = bottomValue; testedValue <= topValue; testedValue++)
            {
                if (TestValuePart2(testedValue)) validCounter++;
            }
            //1599 is too high.
            //1045 is too low.
            AddResult("Meets Part 2 rules: " + validCounter.ToString());
        }
        // It is a six - digit number.
        // The value is within the range given in your puzzle input.
        // Two adjacent digits are the same(like 22 in 122345).
        // Going from left to right, the digits never decrease; they only ever increase or stay the same(like 111123 or 135679).
        bool TestValue(int value)
        {
            bool result = true;
            string sValue = value.ToString();
            result = result && sValue.Length == 6;
            bool hasDouble = false;
            for (int x=0; x<sValue.Length-1; x++) // testing chars 0-4
            {
                hasDouble = hasDouble || (sValue[x] == sValue[x + 1]);
            }
            result = result && hasDouble;
            bool onlyIncreases = true;
            for (int x = 0; x < sValue.Length - 1; x++) // testing chars 0-4
            {
                onlyIncreases = onlyIncreases && (Int32.Parse(sValue[x].ToString()) <= Int32.Parse(sValue[x + 1].ToString()));
            }
            result = result && onlyIncreases;
            return result;
        }
        // It is a six - digit number.
        // The value is within the range given in your puzzle input.
        // Two adjacent digits are the same(like 22 in 122345).
        // More than 2 in a row being the same invalidates the condition for that digit. But if it contains at least ONE grouping that satisfies the condition, it's OK.
        // Going from left to right, the digits never decrease; they only ever increase or stay the same(like 111123 or 135679).
        bool TestValuePart2(int value)
        {
            bool result = true;
            string sValue = value.ToString();
            result = result && sValue.Length == 6;
            bool onlyIncreases = true;
            for (int x = 0; x < sValue.Length - 1; x++) // testing chars 0-4
            {
                onlyIncreases = onlyIncreases && (Int32.Parse(sValue[x].ToString()) <= Int32.Parse(sValue[x + 1].ToString()));
            }
            result = result && onlyIncreases;

            // it's OK to have a group of 3 or more in a row, but there must be at least ONE grouping of just 2.
            bool hasDouble = false;
            // length =6
            // 0-5
            for (int x = 0; x < sValue.Length - 1; x++) // testing chars 0-4
            {
                if (x == 0) {
                    hasDouble = hasDouble || (sValue[x] == sValue[x + 1] && sValue[x] != sValue[x + 2] );
                }
                else if (x!=4)
                {
                    hasDouble = hasDouble || (sValue[x] == sValue[x + 1] && sValue[x] != sValue[x + 2] && sValue[x]!=sValue[x-1]);
                }
                else
                {
                    hasDouble = hasDouble || (sValue[x] == sValue[x + 1] && sValue[x] != sValue[x - 1]);
                }
            }
            result = result && hasDouble;

            return result;
        }
    }
}
