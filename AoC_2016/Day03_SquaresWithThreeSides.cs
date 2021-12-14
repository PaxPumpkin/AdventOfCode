using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day03_SquaresWithThreeSides : AoCodeModule
    {
        public Day03_SquaresWithThreeSides()
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
            List<BadTriangle> allTriangles = new List<BadTriangle>();
            foreach (string processingLine in inputFile)
            {
                allTriangles.Add(new BadTriangle(processingLine));
            }
            AddResult(allTriangles.Count(x=>x.IsGood==true).ToString()); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);
            int rowIndex = 0;
            string t1, t2, t3, r1,r2,r3 = "";
            t1 = t2 = t3 = "";
            string[] sides;
            allTriangles.Clear();
            while (rowIndex + 2 <= inputFile.Count)
            {
                r1 = inputFile[rowIndex];
                r2 = inputFile[rowIndex+1];
                r3 = inputFile[rowIndex+2];
                sides=r1.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                t1 += sides[0] + " ";
                t2 += sides[1] + " ";
                t3 += sides[2] + " ";
                sides = r2.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                t1 += sides[0] + " ";
                t2 += sides[1] + " ";
                t3 += sides[2] + " ";
                sides = r3.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                t1 += sides[0] + " ";
                t2 += sides[1] + " ";
                t3 += sides[2] + " ";
                allTriangles.Add(new BadTriangle(t1));
                allTriangles.Add(new BadTriangle(t2));
                allTriangles.Add(new BadTriangle(t3));
                t1 = t2= t3 = "";
                rowIndex += 3;
            }
            AddResult(allTriangles.Count(x => x.IsGood == true).ToString());
        }
    }
    public class BadTriangle
    {
        int a, b, c = 0;
        public bool IsGood{get{
                return (a + b > c && a + c > b && b + c > a);
            } }
        public BadTriangle(string definition)
        {
            string[] sides = definition.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            a = int.Parse(sides[0]);
            b = int.Parse(sides[1]);
            c = int.Parse(sides[2]);
        }
    }
}
