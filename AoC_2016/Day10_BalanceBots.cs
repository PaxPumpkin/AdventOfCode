using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day10_BalanceBots : AoCodeModule
    {
        public Day10_BalanceBots()
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
            //If Comma Delimited on a single input line
            //List<string> inputItems = inputFile[0].Split(new char[] { ',' }).ToList<string>();
            //string finalResult = "Not Set";
            ResetProcessTimer(true);// true also iterates the section marker
            List<int> botIDList = new List<int>();
            List<int> outputIDList = new List<int>();
            foreach (string processingLine in inputFile)
            {
                string line = processingLine;
                //
                //bot 192 gives low to bot 40 and high to bot 177
                //bot 195 gives low to output 4 and high to bot 130
                if (line.StartsWith("bot "))
                {
                    line = line.Substring(4);
                    botIDList.Add(int.Parse(line.Substring(0, line.IndexOf(' '))));
                }
                if (line.Contains("output"))
                {
                    line = line.Substring(line.IndexOf("output") + 7);
                    outputIDList.Add(int.Parse(line.Substring(0, line.IndexOf(' '))));
                    if (line.Contains("output"))
                    {
                        line = line.Substring(line.IndexOf("output") + 7);
                        outputIDList.Add(int.Parse(line.Trim().Substring(0)));
                    }
                }

            }
            outputIDList = outputIDList.Distinct().OrderBy(x=>x).ToList();
            botIDList = botIDList.Distinct().OrderBy(x => x).ToList();
            List<Reciever> targets= new List<Reciever>();
            foreach (int ID in outputIDList)
            {
                targets.Add(new OutputBin(ID));
            }
            foreach (int ID in botIDList)
            {
                targets.Add(new Bot(ID));
            }
            int parsedBotID;
            foreach (string processingLine in inputFile)
            {
                //bot 192 gives low to bot 40 and high to bot 177
                //bot 195 gives low to output 4 and high to bot 130
                //value 61 goes to bot 49
                string line = processingLine;
                if (line.StartsWith("bot "))
                {
                    line = line.Substring(4);
                    parsedBotID = int.Parse(line.Substring(0, line.IndexOf(' ')));
                    line = line.Substring(line.IndexOf(' ') + 1);
                    Bot giver = (Bot)targets.Where(x => x.Type() == "bot" && x.ID() == parsedBotID).First();
                    Reciever target1, target2;
                    line = line.Substring("gives low to ".Length);
                    if (line.StartsWith("bot"))
                    {
                        line = line.Substring(4);
                        parsedBotID = int.Parse(line.Substring(0, line.IndexOf(' ')));
                        line = line.Substring(line.IndexOf(' ') + 1);
                        target1 = targets.Where(x => x.Type() == "bot" && x.ID() == parsedBotID).First();
                        line = line.Substring("and high to ".Length);
                        if (line.StartsWith("bot"))
                        {
                            line = line.Substring(4);
                            parsedBotID = int.Parse(line.Substring(0).Trim());
                            target2 = targets.Where(x => x.Type() == "bot" && x.ID() == parsedBotID).First();
                        }
                        else
                        {
                            line = line.Substring(7);
                            parsedBotID = int.Parse(line.Substring(0).Trim());
                            target2 = targets.Where(x => x.Type() == "output" && x.ID() == parsedBotID).First();
                        }
                    }
                    else
                    {
                        line = line.Substring(7);
                        parsedBotID = int.Parse(line.Substring(0, line.IndexOf(' ')));
                        line = line.Substring(line.IndexOf(' ') + 1);
                        target1 = targets.Where(x => x.Type() == "output" && x.ID() == parsedBotID).First();
                        line = line.Substring("and high to ".Length);
                        if (line.StartsWith("bot"))
                        {
                            line = line.Substring(4);
                            parsedBotID = int.Parse(line.Substring(0).Trim());
                            target2 = targets.Where(x => x.Type() == "bot" && x.ID() == parsedBotID).First();
                        }
                        else
                        {
                            line = line.Substring(7);
                            parsedBotID = int.Parse(line.Substring(0).Trim());
                            target2 = targets.Where(x => x.Type() == "output" && x.ID() == parsedBotID).First();
                        }
                    }
                    giver.Low = target1;
                    giver.High = target2;
                }
                else
                {
                    //value 61 goes to bot 49
                    line = line.Substring(6);
                    int parsedValue = int.Parse(line.Substring(0, line.IndexOf(' ')));
                    line = line.Substring(line.IndexOf("goes to bot ") + 12);
                    parsedBotID = int.Parse(line.Substring(0).Trim());
                    Microchip newChip = new Microchip(parsedValue);
                    targets.Where(x => x.Type() == "bot" && x.ID() == parsedBotID).First().Accept(newChip);
                }
            }
            targets.Where(x => x.Type() == "bot").ToList().ForEach(x => ((Bot)x).Ready = true);
            targets.Where(x => x.Type() == "bot").ToList().ForEach(x => { if (((Bot)x).chipBin.Count > 1) { ((Bot)x).Give(); } });
            Reciever result = targets.Where(x => x.Type() == "bot" && ((Bot)x).responsibilities.Contains(61) && ((Bot)x).responsibilities.Contains(17)).ToList().FirstOrDefault();
            AddResult(result==null?"Got Me, Boos":result.ID().ToString()); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);
            int resultNumber = 1;
            targets.Where(x => x.Type() == "output" && (x.ID() == 0 || x.ID() == 1 || x.ID() == 2)).ToList().ForEach(x => resultNumber *= ((OutputBin)x).responsibility);
            AddResult(resultNumber.ToString());
        }
    }
    public interface Reciever
    {
        int ID();
        void Accept(Microchip microchip);
        string Type();
    }
    public class OutputBin:Reciever
    {
        private int id { get; set; }
        public int ID() { return id; }
        public string Type() { return "output"; }
        private List<Microchip> chipBin { get; set; }
        public int responsibility { get; set; }
        public OutputBin(int Id)
        {
            chipBin = new List<Microchip>();
            id = Id;
        }
        public void Accept(Microchip microchip)
        {
            responsibility = microchip.Value;
            chipBin.Add(microchip);

        }
    }
    public class Microchip
    {
        public int Value { get; set; }
        public Microchip(int value)
        {
            Value = value;
        }
    }
    public class Bot:Reciever
    {
        private int id { get; set; }
        public int ID() { return id; }
        public string Type() { return "bot"; }
        public Reciever Low { get; set; }
        public Reciever High { get; set; }
        public bool Ready { get; set; }
        public List<int> responsibilities { get; set; }
        public List<Microchip>chipBin {get;set;}
        public Bot(int Id)
        {
            chipBin = new List<Microchip>();
            responsibilities = new List<int>();
            Ready = false;
            id = Id;
        }
        public void Accept(Microchip microchip)
        {
            chipBin.Add(microchip);
            responsibilities.Add(microchip.Value);
            if (chipBin.Count > 1)
            {
                if (Ready)
                    Give();
            }
        }
        public void Give()
        {
            chipBin = chipBin.OrderBy(x => x.Value).ToList();
            Microchip low = chipBin[0];
            Microchip high = chipBin[1];
            chipBin.Clear();
            Low.Accept(low);
            High.Accept(high);

        }
    }
}
