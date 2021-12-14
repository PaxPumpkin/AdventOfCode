using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day07_InternetProtocolVersion7 : AoCodeModule
    {
        public Day07_InternetProtocolVersion7()
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
            List<IP> addresses = new List<IP>();
            foreach (string processingLine in inputFile)
            {
                addresses.Add(new IP(processingLine));
            }
            AddResult(addresses.Count(x => x.SupportTLS == true).ToString()); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);
            AddResult(addresses.Count(x => x.SupportSSL == true).ToString());

        }
    }
    public class IP
    {
        public bool SupportTLS { get; set; }
        public bool SupportSSL { get; set; }

        public IP(string addy)
        {
            SupportTLS = false;
            SupportSSL = false;
            List<string> insideParts = new List<string>();
            List<string> outsideParts = new List<string>();
            string[] pieces = addy.Split(new char[] { ']' }, StringSplitOptions.RemoveEmptyEntries);
            // pieces should now contain entries that are split with opening brackets or have none at all...
            pieces.ToList().ForEach(x => {
                if (x.Contains("["))
                { // last part is "inside brackets" or "a hypernet sequence", first is outside
                    string[] nextPieces = x.Split(new char[] { '[' }, StringSplitOptions.RemoveEmptyEntries);
                    outsideParts.Add(nextPieces[0]);
                    insideParts.Add(nextPieces[1]);
                }
                else
                { // outside only. 
                    outsideParts.Add(x);
                }
            });
            // now we have a list of inside and outside parts. 
            // For TLS - since the inside parts disqualify at all times, check those first
            bool foundDisqualification = false;
            insideParts.ForEach(x => { if (ContainsABBA(x) == true) { foundDisqualification = true; } });
            if (!foundDisqualification)
            {
                outsideParts.ForEach(x => { if (ContainsABBA(x) == true) { SupportTLS = true; } });
            }

            //For SSL - a bit more discerning.
            //We need a list of "ABA" patterns in all outside blocks
            //Then we need to check each inside block to see if the "BAB" pattern is found anywhere. 
            List<string> ABAs = new List<string>();
            outsideParts.ForEach(x => ABAs.AddRange(GetABAs(x)));
            // the list may be empty....
            if (ABAs.Count > 0)
            {
                ABAs.ForEach(x => { insideParts.ForEach(y => { SupportSSL = SupportSSL || ContainsBAB(y,x); }); });
            }

        }
        private static bool ContainsABBA(string candidate)
        {
            bool result = false;
            int index = 0;
            string test = "";
            string testPiece = "";
            char[] testPieceArray;
            while (index + 4 <= candidate.Length)
            {
                test = candidate.Substring(index, 4);
                testPieceArray = test.Substring(0, 2).ToCharArray();
                Array.Reverse(testPieceArray);
                testPiece = new string(testPieceArray);
                if (testPiece[0]!=testPiece[1] && test.Substring(2, 2).Equals(testPiece))
                {
                    result = true;
                    break;
                }
                index++;
            }
            return result;
        }
        private List<string> GetABAs(string candidate)
        {
            List<string> results = new List<string>();
            int index = 0;
            string test = "";
            string comparison = "";
            while (index + 3 <= candidate.Length) // so long as we have 3 chars to compare.
            {
                test = candidate.Substring(index, 2); // take the next two chars.
                if (test[0] != test[1]) // so long as these two aren't the same...
                {
                    test += test[0].ToString(); // add the first char to the end to make an "ABA" pattern.
                    // just realized there was a more direct way: candidateSubstring(index+2,1)[0]==test[0], oh well...
                    comparison = candidate.Substring(index, 3);// get the three character string at the same index.
                    if (test.Equals(comparison))// if they match, we have an ABA 
                    {
                        results.Add(test);
                    }
                }
                index++;
            }

            return results;
        }
        private bool ContainsBAB(string candidate, string test)
        {
            // test should be a three character string: ABA
            test = (test[1].ToString() + test).Substring(0,3); // take the center, add to the beginning, and now ABA becomes BABA, take the new string's first 3: BAB
            return candidate.Contains(test); // is it in the string anywhere? 
        }
    }
}
