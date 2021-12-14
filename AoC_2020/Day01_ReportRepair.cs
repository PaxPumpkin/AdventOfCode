using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2020
{
    class Day01_ReportRepair : AoCodeModule
    {

        public Day01_ReportRepair()
        {

            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput();
            OutputFileReset();

        }
        public override void DoProcess()
        {
            /* Using the LINQ List<T>.ForEach (unable to break) gets:
             * ** > Result for Day01_ReportRepair part 1:Answer to part 1: 898299(Process: 7 ms)
             * ** > Result for Day01_ReportRepair part 2:Answer to part 2: 143933922(Process: 1306 ms)
             * 
             * Using LINQ to generate a new list, then using standard foreach to iterate, with "break" to stop iteration gets:
             * ** > Result for Day01_ReportRepair part 1:Answer to part 1: 898299(Process: 3 ms)
             * ** > Result for Day01_ReportRepair part 2:Answer to part 2: 143933922(Process: 615 ms)
             */
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            bool found = false;
            foreach (string line in inputFile)
            {
                int thisItem = Convert.ToInt32(line);
                List<string> allOtherLines = inputFile.Where(item => item != line).ToList();
                foreach(string otherLine in allOtherLines)
                {
                    int otherItem = Convert.ToInt32(otherLine);
                    if (thisItem + otherItem == 2020)
                    {
                        finalResult = "Answer to part 1: " + (thisItem*otherItem).ToString();
                        found = true;
                        break;  
                    }
                }
            if (found) break;
            }
            AddResult(finalResult);
            ResetProcessTimer(true);
            found = false;
            foreach (string firstValue in inputFile)
            {
                int firstInt = Convert.ToInt32(firstValue);
                List<string> secondLines = inputFile.Where(item => item != firstValue).ToList();
                foreach(string secondValue in secondLines)
                {
                    int secondInt = Convert.ToInt32(secondValue);
                    List<string> thirdLines = inputFile.Where(item => item != firstValue && item != secondValue).ToList();
                    foreach(string thirdValue in thirdLines)
                    {
                        int thirdInt = Convert.ToInt32(thirdValue);
                        if (firstInt + secondInt + thirdInt == 2020)
                        {
                            finalResult = "Answer to part 2: " + (firstInt * secondInt * thirdInt).ToString();
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
                if (found) break;
            }
            AddResult(finalResult);
        }
    }
}
