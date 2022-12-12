using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day11_MonkeyInTheMiddle : AoCodeModule
    {
        public Day11_MonkeyInTheMiddle()
        {
            // If you always save input file in the /InputFiles/ subfolder and name it the same as the class processing it, this will work.
            // if you put it elsewhere or name it differently, just change below. 
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); //base class method
            OutputFileReset(); // output file will be in the same location as input, with ".output.txt" appended to the name. This clears previous output from file.
            //Print("Did Something");// outputs to console immediately
            //Print("DidSomethingElse", FileOutputAlso); // both console and output file
            //Print("Another Thing", FileOutputOnly); // output file only.
        }
        public override void DoProcess()
        {
            ResetProcessTimer(true);
            //foreach (string processingLine in inputFile)
            List<KeepAwayMonkey> Monkies = new List<KeepAwayMonkey>();
            List<KeepAwayMonkey> StartingMonkies = new List<KeepAwayMonkey>();
            KeepAwayMonkey monkey, monkeyCopy;
            string[] parseHelper;
            UInt64 stupidSpecialThing=1;
            for (int x=0; x<inputFile.Count; x+= 7)
            {
                monkey = new KeepAwayMonkey();
                monkeyCopy = new KeepAwayMonkey();
                parseHelper = inputFile[x].Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
                monkeyCopy.MonkeyNumber = monkey.MonkeyNumber = int.Parse(parseHelper[1]);
                parseHelper = inputFile[x + 1].Split(new char[] {':', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int y = 2; y < parseHelper.Length; y++)
                {
                    monkey.ItemWorries.Enqueue(UInt64.Parse(parseHelper[y]));
                    monkeyCopy.ItemWorries.Enqueue(UInt64.Parse(parseHelper[y]));
                }

                parseHelper = inputFile[x + 2].Split(new char[] {' ',':','=' }, StringSplitOptions.RemoveEmptyEntries);
                monkeyCopy.Operation =  monkey.Operation = parseHelper[3] == "+" ? WorryOperation.Add : (parseHelper[3] == "-" ? WorryOperation.Subtract : (parseHelper[3] == "*" ? WorryOperation.Multiply : WorryOperation.Divide));
                monkeyCopy.OperationFactor = monkey.OperationFactor = parseHelper[4] == "old" ? (UInt64)0 : UInt64.Parse(parseHelper[4]);
                monkeyCopy.TestFactor = monkey.TestFactor = UInt64.Parse(inputFile[x + 3].Split(new char[] {' ',':' }, StringSplitOptions.RemoveEmptyEntries)[3]);
                stupidSpecialThing *= monkey.TestFactor;
                monkeyCopy.TrueMonkeyNumber = monkey.TrueMonkeyNumber = int.Parse(inputFile[x + 4].Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries)[5]);
                monkeyCopy.FalseMonkeyNumber = monkey.FalseMonkeyNumber = int.Parse(inputFile[x + 5].Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries)[5]);
                Monkies.Add(monkey);
                StartingMonkies.Add(monkeyCopy);
            }
            Monkies.ForEach(monk => monk.StupidSpecialThing = stupidSpecialThing);
            StartingMonkies.ForEach(monk => monk.StupidSpecialThing = stupidSpecialThing);
            for (int round = 1; round<=20; round++)
            {
                foreach (KeepAwayMonkey keepAwayMonkey in Monkies)
                {
                    //Print("Monkey " + keepAwayMonkey.MonkeyNumber.ToString() + ":");
                    while (keepAwayMonkey.ItemWorries.Count > 0)
                    {
                        (UInt64 item, int monkeyNumber) result = keepAwayMonkey.Decide(keepAwayMonkey.Inspect());
                        Monkies[result.monkeyNumber].ItemWorries.Enqueue(result.item);
                    }
                }
            }
            List<KeepAwayMonkey> monkeyBusiness = Monkies.OrderByDescending(monk => monk.InspectionCount).ToList();
            AddResult("The level of monkey business after 20 rounds of stuff-slinging simian shenanigans is " + (monkeyBusiness[0].InspectionCount * monkeyBusiness[1].InspectionCount).ToString());
            
            for (int round = 1; round <= 10000; round++)
            {
                foreach (KeepAwayMonkey keepAwayMonkey in StartingMonkies)
                {
                    //Print("Monkey " + keepAwayMonkey.MonkeyNumber.ToString() + ":");
                    while (keepAwayMonkey.ItemWorries.Count > 0)
                    {
                        (UInt64 item, int monkeyNumber) result = keepAwayMonkey.Decide(keepAwayMonkey.Inspect(WorryConcern.Aggravated));
                        StartingMonkies[result.monkeyNumber].ItemWorries.Enqueue(result.item);
                    }
                }
            }
            monkeyBusiness = StartingMonkies.OrderByDescending(monk => monk.InspectionCount).ToList();
            AddResult("The level of monkey business after 10000 rounds of stuff-slinging simian shenanigans is " + (monkeyBusiness[0].InspectionCount * monkeyBusiness[1].InspectionCount).ToString());
        }
    }
    public enum WorryOperation { Add, Subtract, Multiply, Divide }
    public enum WorryConcern { Relieved, Aggravated}
    public class KeepAwayMonkey
    {
        public int MonkeyNumber;
        public Queue<UInt64> ItemWorries { get; set; } = new Queue<UInt64>();
        public WorryOperation Operation;
        public UInt64 OperationFactor;
        public UInt64 TestFactor;
        public int TrueMonkeyNumber;
        public int FalseMonkeyNumber;
        public UInt64 InspectionCount = 0;
        public UInt64 StupidSpecialThing = 0;
    
        public UInt64 Inspect(WorryConcern concern = WorryConcern.Relieved)
        {
            InspectionCount++;
            UInt64 item = ItemWorries.Dequeue();
            //long origItem = item;
            //Console.WriteLine("\tMonkey inspects an item with a worry level of " + item.ToString());
            switch (Operation)
            {
                case WorryOperation.Add:
                    item += OperationFactor != 0 ? OperationFactor : item;
                    //Console.WriteLine("\t\tWorry level increases by " + (OperationFactor != -1 ? OperationFactor : origItem) + " to " + item.ToString());
                    break;
                case WorryOperation.Multiply:
                    item *= OperationFactor != 0 ? OperationFactor : item;
                    //Console.WriteLine("\t\tWorry level is multiplied by " + (OperationFactor != -1 ? OperationFactor : origItem) + " to " + item.ToString());
                    break;
            }
            if (concern == WorryConcern.Relieved)
            {
                item = (UInt64) Convert.ToInt64(Math.Floor(item / 3m));
                //Console.WriteLine("\t\tMonkey gets bored with item. Worry level is divided by 3 to " + item.ToString());
            }
            else
            {
               item = item % StupidSpecialThing;
            }
            
            return item;
        }
        public (UInt64 itemvalue, int monkeyNumber) Decide(UInt64 item)
        {
            (UInt64 itemvalue, int monkeyNumber) result = (item, item % TestFactor == 0 ? TrueMonkeyNumber : FalseMonkeyNumber);
            //Console.WriteLine("\t\tCurrent worry level " + (item % TestFactor == 0 ? "is" : "is not") + " divisible by " + TestFactor.ToString());
            //Console.WriteLine("\t\tItem with worry level " + result.itemvalue.ToString() + " is thrown to monkey " + result.monkeyNumber.ToString());
            return result;
        }
    }
}
