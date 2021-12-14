using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day15_TimingIsEverything : AoCodeModule
    {
        public Day15_TimingIsEverything()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); //base class method
            OutputFileReset(); // output file will be in the same location as input, with ".output.txt" appended to the name. This clears previous output from file.

        }
        public override void DoProcess()
        {
            string finalResult = "Not Set";
            ResetProcessTimer(true);// true also iterates the section marker
            List<TimingDisc> discs = new List<TimingDisc>();
            foreach (string processingLine in inputFile)
            {
                // Disc #1 has 13 positions; at time=0, it is at position 11.
                string[] pieces = processingLine.Split(new string[] { "Disc #", " has ", " positions; at time=0, it is at position ", "." }, StringSplitOptions.RemoveEmptyEntries);
                discs.Add(new TimingDisc(int.Parse(pieces[0]), int.Parse(pieces[1]), int.Parse(pieces[2])));
            }
            int timeIndex = 0;
            while (discs.Count(disc => disc.perfectTimeIndex(timeIndex) == true) != discs.Count) { timeIndex++; }
            finalResult ="Release at Time Index " + timeIndex;
            AddResult(finalResult); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);
            discs.Add(new TimingDisc(7, 11, 0));
            timeIndex = 0;
            while (discs.Count(disc => disc.perfectTimeIndex(timeIndex) == true) != discs.Count) { timeIndex++; }
            finalResult = "Release at Time Index " + timeIndex;
            AddResult(finalResult);
        }
    }
    public class TimingDisc
    {
        public int discNumber;
        public int positionCount;
        public int positionAtZero;
        public TimingDisc(int number, int positions, int posatzero)
        {
            discNumber = number;
            positionCount = positions;
            positionAtZero = posatzero;
        }
        public bool perfectTimeIndex(int timeIndex)
        {
            return (timeIndex + discNumber + positionAtZero) % positionCount == 0;
        }
    }
}
