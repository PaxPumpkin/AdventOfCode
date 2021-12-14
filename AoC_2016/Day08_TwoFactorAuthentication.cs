using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day08_TwoFactorAuthentication : AoCodeModule
    {
        public Day08_TwoFactorAuthentication()
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
            Screen screen = new Screen();
            foreach (string processingLine in inputFile)
            {
                screen.Command(processingLine);
            }
            screen.printConsole(); // oddly enough, this was part two and I'd already done it. 
            AddResult(screen.ListCount().ToString()); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);

        }
    }
    public class Screen
    {
        private int[,] screen = new int[50, 6];
        private const string RECT = "rect";
        private const string ROW = "rotate row";
        private const string COLUMN = "rotate column";
        public Screen()
        {
        }
        public int ListCount()
        {
            int litCount = 0;
            for (int x = 0; x <= screen.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= screen.GetUpperBound(1); y++)
                {
                    litCount += screen[x, y];
                }
            }
            return litCount;
        }
        public void printConsole()
        {
            string line = "";
            for (int x = 0; x <= screen.GetUpperBound(0); x++)
            {
                line+=(x%10)==0?"@":"-";
            }
            Console.WriteLine(line);
            for (int y = 0; y <= screen.GetUpperBound(1); y++)
            {
                line = "";
                for (int x = 0; x <= screen.GetUpperBound(0); x++)
                {
                    line += screen[x, y]==1?"#":".";
                }
                Console.WriteLine(line);
            }
        }
        public void Command(string command)
        {
            if (command.StartsWith(RECT)) { DrawRect(command.Substring(5)); }
            else if (command.StartsWith(ROW)){ ShiftRow(command.Split(new char[] { '=' })[1]); }
            else if (command.StartsWith(COLUMN)){ ShiftColumn(command.Split(new char[] { '=' })[1]); }

        }
        public void DrawRect(string dimensions)
        {
            string[] dims = dimensions.Split(new char[] { 'x' });
            int wide = int.Parse(dims[0]);
            int tall = int.Parse(dims[1]);
            for (int y = 0; y < tall; y++)
            {
                for (int x = 0; x < wide; x++)
                {
                    screen[x, y] = 1;
                }
            }
        }
        public void ShiftRow(string row)
        {
            // 0 by 4   -- should result in rowpointer=0, shiftAmount=4
            string[] cmds = row.Split(new char[] { ' ', 'b', 'y' }, StringSplitOptions.RemoveEmptyEntries);
            int rowPointer = int.Parse(cmds[0]);
            int shiftAmount = int.Parse(cmds[1]);
            // since the screen width is fixed, any shifting by anything greater than the total width is superfluous.
            // so remove all whole divisors of the width
            shiftAmount = shiftAmount % (screen.GetUpperBound(0)+1); // 50 wide gives upper bound of 49, not quite useful. Add 1 for dividing.
            // now we know exactly how much on the screen to shift, and it is less than one whole screen width worth. 
            // how much of the current row is going off screen? The shift amount, from the end.
            int[] newrow = new int[screen.GetUpperBound(0)+1];
            int columnPointer = 0;
            // take the last "shift amount" chars from the end and put at the beginning of the new row.
            for (int x = screen.GetUpperBound(0) - shiftAmount + 1; x <= screen.GetUpperBound(0); x++)
            {
                newrow[columnPointer] = screen[x,rowPointer];
                columnPointer++;
            }
            // take the first part of the original array up until the shifted amount and put at the end of the new row. 
            for (int x = 0; x <= screen.GetUpperBound(0) - shiftAmount; x++)
            {
                newrow[columnPointer] = screen[x, rowPointer];
                columnPointer++;
            }
            // copy the new row over top of the original row.
            for (int x = 0; x <= screen.GetUpperBound(0); x++)
            {
                screen[x, rowPointer] = newrow[x];
            }

        }
        public void ShiftColumn(string column)
        {
            // 0 by 4   -- should result in rowpointer=0, shiftAmount=4
            string[] cmds = column.Split(new char[] { ' ', 'b', 'y' }, StringSplitOptions.RemoveEmptyEntries);
            int colPointer = int.Parse(cmds[0]);
            int shiftAmount = int.Parse(cmds[1]);
            // since the screen height is fixed, any shifting by anything greater than the total height is superfluous.
            // so remove all whole divisors of the height
            shiftAmount = shiftAmount % (screen.GetUpperBound(1) + 1); // 6 tall gives upper bound of 5, not quite useful. Add 1 for dividing.
            // now we know exactly how much on the screen to shift, and it is less than one whole screen height worth. 
            // how much of the current column is going off screen? The shift amount, from the end.
            int[] newcol = new int[screen.GetUpperBound(1) + 1];
            int rowPointer = 0;
            // take the last "shift amount" chars from the end and put at the beginning of the new column
            for (int x = screen.GetUpperBound(1) - shiftAmount + 1; x <= screen.GetUpperBound(1); x++)
            {
                newcol[rowPointer] = screen[colPointer, x];
                rowPointer++;
            }
            // take the first part of the original array up until the shifted amount and put at the end of the new row. 
            for (int x = 0; x <= screen.GetUpperBound(1) - shiftAmount; x++)
            {
                newcol[rowPointer] = screen[colPointer, x];
                rowPointer++;
            }
            // copy the new row over top of the original row.
            for (int x = 0; x <= screen.GetUpperBound(1); x++)
            {
                screen[colPointer, x] = newcol[x];
            }
        }
    }
}
