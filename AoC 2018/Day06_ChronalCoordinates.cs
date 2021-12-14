using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day06_ChronalCoordinates : AoCodeModule
    {
        public Day06_ChronalCoordinates()
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
            List<CCPoint> coordinates = new List<CCPoint>();
            int coordCounter = 1;
            foreach (string processingLine in inputFile)
            {
                string[] bits = processingLine.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                coordinates.Add(new CCPoint(int.Parse(bits[0]), int.Parse(bits[1]), coordCounter));
                coordCounter++;
            }
            int maxx = coordinates.Max(x => x.x);
            int maxy = coordinates.Max(x => x.y);
            int[,,] coordField = new int[maxx + 1, maxy + 1, 3];
            coordinates.ForEach(x => coordField[x.x, x.y, 0] = x.ID);
            int testdist, distance, closestID;
            for (int i = 0; i <= coordField.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= coordField.GetUpperBound(1); j++)
                {
                    distance = 8675309; closestID = 0;
                    coordinates.ForEach(x =>
                    {
                        testdist = Math.Abs(i - x.x) + Math.Abs(j - x.y);
                        coordField[i, j, 2] += testdist;
                        if (testdist == distance) { closestID = 0; }
                        else
                        if (testdist < distance) { distance = testdist; closestID = x.ID; }
                    });
                    coordField[i, j, 1] = closestID;
                }
            }
            List<int> infiniteAreaIDs = new List<int>();
            Dictionary<int, int> finiteAreaIDs = new Dictionary<int, int>();
            for (int x = 0; x <= coordField.GetUpperBound(0); x++) { if (coordField[x, 0, 1] != 0) { infiniteAreaIDs.Add(coordField[x, 0, 1]); } }
            for (int x = 0; x <= coordField.GetUpperBound(0); x++) { if (coordField[x, maxy, 1] != 0) { infiniteAreaIDs.Add(coordField[x, maxy, 1]); } }
            for (int y = 0; y <= coordField.GetUpperBound(1); y++) { if (coordField[0, y, 1] != 0) { infiniteAreaIDs.Add(coordField[0, y, 1]); } }
            for (int y = 0; y <= coordField.GetUpperBound(1); y++) { if (coordField[maxx, y, 1] != 0) { infiniteAreaIDs.Add(coordField[maxx, y, 1]); } }
            infiniteAreaIDs = infiniteAreaIDs.Distinct().ToList();
            for (int i = 0; i <= coordField.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= coordField.GetUpperBound(1); j++)
                {
                    if (coordField[i, j, 1] != 0)
                    {
                        if (!infiniteAreaIDs.Contains(coordField[i, j, 1]))
                        {
                            if (finiteAreaIDs.ContainsKey(coordField[i, j, 1]))
                            {
                                finiteAreaIDs[coordField[i, j, 1]] += 1;
                            }
                            else
                            {
                                finiteAreaIDs.Add(coordField[i, j, 1], 1);
                            }
                        }
                    }
                }
            }
            int largestArea = finiteAreaIDs.Max(x => x.Value);
            AddResult(largestArea.ToString()); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);
            int inside10000 = 0;
            for (int i = 0; i <= coordField.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= coordField.GetUpperBound(1); j++)
                {
                    if (coordField[i, j, 2] < 10000) inside10000++;
                }
            }
            AddResult(inside10000.ToString());
        }
    }
    public class CCPoint
    {
        public int x, y;
        public int ID;
        public CCPoint(int xcoord, int ycoord, int pointID)
        {
            x = xcoord; y = ycoord; ID=pointID;
        }
    }
}
