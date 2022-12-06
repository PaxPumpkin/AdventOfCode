using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day01_CalorieCounting : AoCodeModule
    {
        public Day01_CalorieCounting()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day01_CalorieCounting part 1: Elf with the most calories has 71502 (Process: 0.5902 ms)
            //** > Result for Day01_CalorieCounting part 2: The total of the top three elves' calorie count is 208191 (Process: 0.0888 ms)

            string finalResult = "Not Set";
            ResetProcessTimer(true);
            List<CalorieElf> elves = new List<CalorieElf>();
            CalorieElf elf = new CalorieElf();
            foreach (string processingLine in inputFile)
            {
                if (processingLine.Trim().Equals(string.Empty))
                {
                    elves.Add(elf);
                    elf = new CalorieElf();
                }
                else
                {
                    elf.AddItem(processingLine);
                }
            }
            finalResult = "Elf with the most calories has " + elves.Max(x => x.CalorieTotal).ToString();
            AddResult(finalResult); 
            ResetProcessTimer(true);
            finalResult = "The total of the top three elves' calorie count is " + elves.OrderByDescending(x => x.CalorieTotal).Take(3).Sum(x => x.CalorieTotal).ToString();
            AddResult(finalResult);
            ResetProcessTimer(true);
        }
    }
    public class CalorieElf
    {
        List<long> items = new List<long>();
        public long CalorieTotal
        {
            get
            {
                return items.Sum();
            }
        }
        public int ItemCount
        {
            get
            {
                return items.Count();
            }
        }
        public void AddItem(string item)
        {
            if (long.TryParse(item, out long calorieCount))
            {
                items.Add(calorieCount);
            }
        }
    }
}
