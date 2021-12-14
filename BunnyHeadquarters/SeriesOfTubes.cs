using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BunnyHeadquarters
{
    class SeriesOfTubes : AoCodeModule
    {
        int x = 0; int y = 0;//current position indices
        int totalSteps = 1; // counter for how many moves until we're done for part 2. Pre-init to "1 move" as per the puzzle spec ( finding/getting onto the first spot counts as a move )
        char currentDirection = 'd'; // init to "down" as per puzzle specification (start at a top line, find the '|', which means we have to go "down" )
        char[,] grid; // the char map to load from the puzzle input file.
        char[] endings = new char[] { '\0', ' ' }; // if we get to a space or null character, we're done moving in the grid. 
        List<char> collectedLetters = new List<char>(); // the letters we find along the way, in the order we find them.
        public SeriesOfTubes()
        { // the setup. 
            inputFileName = "seriesoftubes.txt"; base.GetInput(); // load puzzle input file 
            grid = new char[inputFile[0].Length, inputFile.Count]; // init grid size. each line is fixed length for the x-axis. y-axis is total lines.
            inputFile.ForEach(line =>{ x = 0; line.ToArray().ToList().ForEach(c =>{ grid[x++,y] = c; });y++;}); // load up the chars. Probably a better way to do this, but it works for now. 
            x = -1; y = 0; // reset coordinates since we'll be using these again for "current position". I'm setting x to -1 because we'll be pre-incrementing on the "find starting point" loop.
            while (grid[++x, y] != '|') { } // find incoming spot from top of grid. -- x was reset to -1, so preincrement to start. Do nothing but increment X until we find that entry spot. If this blows up, the input file is bad.
        }
        public override void DoProcess()
        {
            while (!endings.Contains(Move())) { } // so long as we don't find an ending character, keep moving. 
            FinalOutput.Add("We're out. Collected letters: " + (new string(collectedLetters.ToArray()))); // turn the list of characters into a string for output.
            FinalOutput.Add("TotalSteps until the end: " + (--totalSteps).ToString());// last move counted was out/off. So don't count it in the total, predecrement during output.
        }
        public char Move()
        {
            totalSteps++; // count ALL THE STEPS! This is here instead of in the "while" loop because recursion is used below. Recursion probably doesn't help/matter, so I could move it if I cared to remove the recursion...
            // "whatever" stores nothing useful, this line just compacts the "if" statements for incrementing/decrementing our coordinate pointers. 
            int whatever = (currentDirection == 'd' || currentDirection == 'u') ? ((currentDirection == 'd') ? y++ : y--) : ((currentDirection == 'r') ? x++ : x--);
            if (char.IsLetter(grid[x, y])) collectedLetters.Add(grid[x, y]); // if we find a letter, add it to our list!
            return (currentDirection == 'd' || currentDirection == 'u')// probably a sneakier way to do this. It looks WAY too symmetrical. 
                ? ((grid[x, y] == '|') ? grid[x, y] : ((grid[x, y] == '-') ? Move() : ((grid[x, y] == '+') ? ChangeDirection(grid[x, y], '-') : grid[x, y])))
                : ((grid[x, y] == '-') ? grid[x, y] : ((grid[x, y] == '|') ? Move() : ((grid[x, y] == '+') ? ChangeDirection(grid[x, y], '|') : grid[x, y])));
        }
        public char ChangeDirection(char nextChar, char marker) // cheap way to embed multifunction in a ternary. 
        { //if we're looking for a dash, we'll return either left or right as a result. Otherwise, we're looking for the pipe and it'll be either up or down.
            currentDirection = (marker == '-') ? ((!(x - 1 < 0) && grid[x - 1, y] == '-') ? 'l' : 'r') : ((!(y - 1 < 0) && grid[x, y - 1] == '|') ? 'u' : 'd');
            return nextChar; // spit back our input to match final ternary operator and Move() method signatures. Yup, cheap-o move on my part. 
        }
    }
}
