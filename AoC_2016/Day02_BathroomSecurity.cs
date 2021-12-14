using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day02_BathroomSecurity : AoCodeModule
    {
        public Day02_BathroomSecurity()
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
            string finalResult = "";
            ResetProcessTimer(true);// true also iterates the section marker
            KeyPad keys = new KeyPad();
            foreach (string processingLine in inputFile)
            {
                foreach (char x in processingLine)
                {
                    keys.Move(x);
                }
                finalResult += keys.Button;
            }
            AddResult(finalResult); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);
            KeyPadSpecial keyss = new KeyPadSpecial();
            finalResult = "";
            foreach (string processingLine in inputFile)
            {
                foreach (char x in processingLine)
                {
                    keyss.Move(x);
                }
                finalResult += keyss.Button;
            }
            AddResult(finalResult);
        }
    }
    public class KeyPad
    {
        readonly int[,] keys = new int[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
        int x,y = 1; // on number 5;
        public string Button { get { return keys[x, y].ToString(); } }
        public KeyPad() { }
        public void Move(char direction)
        {
            x += (direction == 'U') ? (x == 0 ? 0 : -1) : (direction == 'D' ? (x == 2 ? 0 : 1) : 0);
            y += (direction == 'L') ? (y == 0 ? 0 : -1) : (direction == 'R' ? (y == 2 ? 0 : 1) : 0);
        }
    }
    public class KeyPadSpecial
    {
        readonly char[,] keys = new char[,] { 
            { ' ', ' ', '1', ' ', ' ' },
            { ' ', '2', '3', '4', ' ' }, 
            { '5', '6', '7', '8', '9' }, 
            { ' ', 'A', 'B', 'C', ' ' }, 
            { ' ', ' ', 'D', ' ', ' ' } };
        int x = 2;
        int y = 0; // on number 5;
        public string Button { get { return keys[x, y].ToString(); } }
        public KeyPadSpecial() { }
        public void Move(char direction)
        {
            x += (direction == 'U') ? ((x == 0 || keys[x-1,y]==' ') ? 0 : -1) : (direction == 'D' ? ((x == 4 || keys[x+1,y]==' ')? 0 : 1) : 0);
            y += (direction == 'L') ? ((y == 0 || keys[x , y-1] == ' ') ? 0 : -1) : (direction == 'R' ? ((y == 4 || keys[x , y+1] == ' ') ? 0 : 1) : 0);

        }
    }
}
