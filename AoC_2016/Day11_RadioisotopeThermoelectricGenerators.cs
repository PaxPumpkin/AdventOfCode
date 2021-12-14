using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day11_RadioisotopeThermoelectricGenerators : AoCodeModule
    {
        public Day11_RadioisotopeThermoelectricGenerators()
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
            foreach (string processingLine in inputFile)
            {

            }
            // includes elapsed time from last ResetProcessTimer
            int promethium = 1, cobalt = 2, curium = 4, ruthenium = 8, plutonium = 16;
            List<floor> floors = new List<floor>();
            floors.Add(new floor(1)); floors.Add(new floor(2)); floors.Add(new floor(3)); floors.Add(new floor(4));
            floors.Where(x => x.number == 1).First().items.AddRange(new int[] {promethium, -promethium });
            floors.Where(x => x.number == 2).First().items.AddRange(new int[] { cobalt,curium,ruthenium,plutonium });
            floors.Where(x => x.number == 3).First().items.AddRange(new int[] { -cobalt,-curium,-ruthenium,-plutonium });
            //int currentFloor = 1;


        }
    }
    public class floor
    {
        public List<int> items = new List<int>();
        public int number;
        public floor(int floorNumber)
        {
            number = floorNumber;
        }
    }
}
