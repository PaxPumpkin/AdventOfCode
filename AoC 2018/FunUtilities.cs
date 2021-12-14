using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018.FunUtilities
{
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
        public void Print(string toPrint) { Print(toPrint, false); }
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
