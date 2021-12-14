using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day18_LikeARogue : AoCodeModule
    {
        public Day18_LikeARogue()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        public override void DoProcess()
        {
            //string finalResult = "Not Set";
            ResetProcessTimer(true);
            string startingRow = ".^^^^^.^^.^^^.^...^..^^.^.^..^^^^^^^^^^..^...^^.^..^^^^..^^^^...^.^.^^^^^^^^....^..^^^^^^.^^^.^^^.^^";
            //startingRow = "..^^.";

            int maxRows = 40;// 400000;// part 1 is 40// 6;
            string[] rows = new string[maxRows];
            rows[0] = startingRow;
            ///Its left and center tiles are traps, but its right tile is not.
            ///Its center and right tiles are traps, but its left tile is not.
            ///Only its left tile is a trap.
            ///Only its right tile is a trap.
            char trap = '^', safe = '.';
            long safeTiles = rows[0].Count(c => c == safe);
            Print(startingRow);
            for (int x = 1; x < maxRows; x++)
            {
                string nextRow = "";
                for (int y = 0; y < startingRow.Length; y++)
                {
                    string test = (y == 0) ?safe.ToString()+ rows[x - 1].Substring(y, 2) : ((y==startingRow.Length-1)?rows[x-1].Substring(y-1,2)+safe.ToString(): rows[x - 1].Substring(y - 1, 3));
                    string result = ((test[1] == trap && ((test[0] == trap && test[2] == safe) || (test[0] == safe && test[2] == trap))) || (test[1] == safe && ((test[0] == safe && test[2] == trap) || (test[0] == trap && test[2] == safe)))) ? trap.ToString() : safe.ToString();
                    nextRow += result;
                }
                if (maxRows < 50) { Print(nextRow); }
                else
                {
                    if (x % 50000 == 0) Print(x.ToString());
                }
                rows[x] = nextRow;
                safeTiles += rows[x].Count(c => c == safe);
            }
            AddResult("All safe: " + safeTiles.ToString());

        }
    }
}
