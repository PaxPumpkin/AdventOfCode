using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace AoC_2016
{
    class Day05_HowAboutANiceGameOfChess : AoCodeModule
    {
        public Day05_HowAboutANiceGameOfChess()
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
            string inputLine = "ffykfhsq";
            finalResult = "";
            long index = 0;
            string hashResult = "";
            ///
            /// TO SKIP THE FIRST PART, UNCOMMENT THIS LINE
            /// COMMENT IT OUT TO DO ALL OF THE CALCULATIONS
            /// 
            //finalResult = "                 ";
            ///
            ///
            ///
            HackerScannerPrint hsp = new HackerScannerPrint("HACKING: >--------<", '-');
            hsp.Print("HACKING: >--------<", true);
            while (finalResult.Length<8)
            {
                hashResult = CalculateMD5Hash(inputLine + index.ToString());
                if (hashResult.Substring(0, 5) == "00000")
                {
                    finalResult += hashResult.Substring(5, 1);
                }
                if (index % 5000 == 0) { hsp.Print("HACKING: >" + finalResult.PadRight(8, '-') + "<"); }
                index++;
            }
            hsp.Print("HACKING: >" + finalResult.PadRight(8, '-') + "<");
            Console.WriteLine();
            AddResult(finalResult); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);

            finalResult = "--------";
            index = 0;
            //int bsLength = ("HACKING: >" + finalResult + "<").Length;
            //string bs = new string('\b', bsLength);

            Console.WriteLine("Starting Part 2");
            hsp.Print("HACKING: >" + finalResult + "<",true);
            while (finalResult.IndexOf('-') >= 0)
            {
                hashResult = CalculateMD5Hash(inputLine + index.ToString());
                if (hashResult.Substring(0, 5) == "00000")
                {
                    string position = hashResult.Substring(5, 1);
                    int charIndex = -1;
                    bool parsedOK = int.TryParse(position, out charIndex);
                    if (parsedOK)
                    {
                        if (charIndex >= 0 && charIndex <= 7 && finalResult[charIndex] == '-')
                        {
                            if (charIndex == 0) { finalResult = hashResult.Substring(6, 1) + finalResult.Substring(1); }
                            else 
                            {
                                finalResult = finalResult.Substring(0, charIndex) + hashResult.Substring(6, 1) + ((charIndex==7)?"":finalResult.Substring(charIndex+1));
                            }
                            //Print("part 2 - Found so far: >" + finalResult + "<");
                        }
                    }
                }
                if (index % 5000 == 0) hsp.Print("HACKING: >" + finalResult + "<");
                //Console.Write(bs);
                //Console.Write("HACKING: >" + fillRandom(finalResult) + "<");
                index++;
            }
            hsp.Print("HACKING: >" + finalResult + "<");
            Console.WriteLine();
            AddResult(finalResult);
        }
        public string CalculateMD5Hash(string input)

        {

            // step 1, calculate MD5 hash from input

            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);


            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {

                sb.Append(hash[i].ToString("x2"));

            }

            return sb.ToString();

        }
    }
    public class HackerScannerPrint
    {
        private static Random rnd = new Random();
        private string stringToPrint = "";
        private string backspaces = "";
        private char markerCharacter = '-';
        public HackerScannerPrint(string scannerString, char replaceChar)
        {
            stringToPrint = scannerString;
            markerCharacter = replaceChar;
            backspaces = new string('\b', stringToPrint.Length);

        }
        public void Print(string toPrint) { Print(toPrint,false); }
        public void Print(string toPrint, bool initial)
        {
            if (!initial)
            {
                Console.Write(backspaces);
            }
            Console.Write(fillRandom(toPrint, markerCharacter));
        }

        public static string fillRandom(string input, char marker)
        {
            string hackChars = @"!@#$%^&*()`~\|_?;:ABCDEF0123456789";
            while (input.IndexOf(marker) >= 0)
            {
                int next = input.IndexOf(marker);
                input = input.Remove(next, 1);
                input = input.Insert(next, hackChars[rnd.Next(hackChars.Length)].ToString());
            }

            return input;

        }
    }
}
