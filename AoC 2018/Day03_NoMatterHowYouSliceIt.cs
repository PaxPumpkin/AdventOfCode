using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day03_NoMatterHowYouSliceIt : AoCodeModule
    {
        public Day03_NoMatterHowYouSliceIt()
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
            List<SantaCloth> clothClaims = new List<SantaCloth>();
            foreach (string processingLine in inputFile)
            {
                SantaCloth newOne = new SantaCloth();
                List<int> overlappedWith = newOne.Claim(processingLine);
                if (overlappedWith.Count != 0) {
                    overlappedWith.ForEach(x => { clothClaims.Where(y => y.ClaimNumber == x).FirstOrDefault().Overlapped = true; });
                }
                clothClaims.Add(newOne);
            }
            AddResult(SantaCloth.OverlappedInches.ToString()); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);
            AddResult(clothClaims.Where(x => x.Overlapped == false).FirstOrDefault().ClaimNumber.ToString());
        }
    }
    public class SantaCloth{
        private static int[,,] cloth = new int[1000, 1000, 2]; // 0-999,0-999 sq inch markers,  0-1 0=claimed by X, 1=overlapped 
        public int ClaimNumber { get; set; }
        private int fromLeft;
        private int fromTop;
        private int width;
        private int height;
        public bool Overlapped { get; set; }
        public static int OverlappedInches { get {
                int overlapped = 0;
                for (int x = 0; x <= 999; x++)
                {
                    for (int y = 0; y <= 999; y++)
                    {
                        if (cloth[x, y, 1] != 0)
                        {
                            overlapped++;
                        }
                    }
                }
                return overlapped;
            } }
        public SantaCloth()
        {

        }
        public List<int> Claim(string claimDefintion)
        {
            //#1336 @ 916,193: 22x27
            string[] pieces = claimDefintion.Split(new char[] { '#', '@', ',', ':', 'x' }, StringSplitOptions.RemoveEmptyEntries);
            ClaimNumber = int.Parse(pieces[0].Trim());
            fromLeft = int.Parse(pieces[1].Trim());
            fromTop= int.Parse(pieces[2].Trim());
            width= int.Parse(pieces[3].Trim());
            height= int.Parse(pieces[4].Trim());
            Overlapped = false;
            List<int> overlappedWith = new List<int>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (cloth[fromLeft + x, fromTop + y,0] != 0) //overlap
                    {
                        overlappedWith.Add(cloth[fromLeft + x, fromTop + y, 0]);
                        cloth[fromLeft + x, fromTop + y, 1] = ClaimNumber; // at the moment, we're only accounting for caring about one overlap, this can be overwritten though
                        Overlapped = true;
                    }
                    else
                    {
                        cloth[fromLeft + x, fromTop + y, 0] = ClaimNumber;
                    }
                }
            }
            return overlappedWith;
        }
    }
}
