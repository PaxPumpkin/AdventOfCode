using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day02_InventoryManagementSystem : AoCodeModule
    {
        public Day02_InventoryManagementSystem()
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
            string finalResult = "Not Set";
            ResetProcessTimer(true);// true also iterates the section marker
            List<SantaWareHouseBox> boxes = new List<SantaWareHouseBox>();
            foreach (string processingLine in inputFile)
            {
                boxes.Add(new SantaWareHouseBox(processingLine));
            }
            int twos = boxes.Count(x => x.isTwo == true);
            int threes = boxes.Count(x => x.isThree == true);
            finalResult = (twos * threes).ToString();
            AddResult(finalResult); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);
            string matchedString = "Not Found";
            int currentPointer = 0;
            while (matchedString.Equals("Not Found") && currentPointer<=inputFile[0].Length)
            {
                List<string> modded = new List<string>();
                foreach (string line in inputFile)
                {
                    modded.Add(line.Remove(currentPointer, 1));
                }
                var possible = modded.GroupBy(line => line).Select(line => new { k = line.Key, cnt = line.Count() }).Where(val => val.cnt > 1).ToList();
                if (possible.Count>0)
                {
                    matchedString = possible[0].k;
                }
                currentPointer++;
            }
            AddResult(matchedString);
            Print(matchedString, FileOutputOnly);
        }
    }
    public class SantaWareHouseBox
    {
        private string origLabel = "";
        public bool isTwo { get; set; }
        public bool isThree { get; set; }
        public SantaWareHouseBox(string label)
        {
            origLabel = label;
            var x=label.GroupBy(character=>character).Select(character=>new {c=character.Key, co=character.Count() });
            isTwo = x.Count(z => z.co == 2) > 0;
            isThree = x.Count(z => z.co == 3) > 0;

        }
    }
}
