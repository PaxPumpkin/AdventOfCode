using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day03_RucksackReorganization : AoCodeModule
    {
        public Day03_RucksackReorganization()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day03_RucksackReorganization part 1: The sum of the priorities is 8176 (Process: 0.7746 ms)
            //** > Result for Day03_RucksackReorganization part 2: The sum of the badge priorities is 2689 (Process: 1.3977 ms)
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            List<ReorgRucksack> rucksacks = new List<ReorgRucksack>();
            int lineCounter = 0, group =0;
            foreach (string processingLine in inputFile)
            {
                if (!processingLine.Trim().Equals(string.Empty))
                    rucksacks.Add(new ReorgRucksack(processingLine, group));
                if ((++lineCounter % 3) == 0) group++;
            }
            finalResult = "The sum of the priorities is " + rucksacks.Sum(x => x.Priority);
            AddResult(finalResult);
            ResetProcessTimer(true);
            finalResult = "The sum of the badge priorities is " + rucksacks.GroupBy(sack => sack.ElfGroup).Sum(grouping => ReorgRucksack.ItemPriority(ReorgRucksack.CommonBadge(grouping.ToList())));
            AddResult(finalResult);
            ResetProcessTimer(true);
        }
    }
    class ReorgRucksack
    {
        List<char> FirstCompartment = new List<char>();
        List<char> SecondCompartment = new List<char>();
        private List<char> WholeRucksack
        {
            get
            {
                return FirstCompartment.Concat(SecondCompartment).ToList();
            }
        }
        public int ElfGroup;
        public char Overlap
        {
            get
            {
                return FirstCompartment.Intersect(SecondCompartment).First();
            }
        }
        public int Priority
        {
            get
            {
                return ItemPriority(Overlap);
            }
        }
        public static int ItemPriority (char item)
        {
            string symbol = item.ToString();
            return (symbol.ToLower()[0]) + (symbol.ToLower() != symbol ? 26 : 0) - 96;
        }
        public static char CommonBadge(List<ReorgRucksack> groupRucksacks)
        {
            List<char> commonItems = groupRucksacks.First().WholeRucksack;
            groupRucksacks.ForEach(sack => commonItems = commonItems.Intersect(sack.WholeRucksack).ToList());
            return commonItems.First(); // should be only one.
        }
        public ReorgRucksack(string contents, int group)
        {
            string firstCompart = contents.Substring(0, contents.Length / 2);
            string secondCompart = contents.Substring(contents.Length / 2);
            foreach (char c in firstCompart) FirstCompartment.Add(c);
            foreach (char c in secondCompart) SecondCompartment.Add(c);
            ElfGroup = group;
        }

    }
}
