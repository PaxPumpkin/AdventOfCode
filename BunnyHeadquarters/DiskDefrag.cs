using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class DiskDefrag : AoCodeModule
    {
        public DiskDefrag()
        {
            // no file input, just a code. 
            oneLineData = "nbysizxe";
            //oneLineData = "flqrgnkx"; // this is the sample input for testing. test result answers for part 1 is 8108, part 2 is 1242
        }
        public override void DoProcess()
        {
            List<int[]> binaryRows = new List<int[]>();
            int sumOfUsedSpots = 0;
            for (int x = 0; x < 128; x++)
            {
                string thisKnotHash = KnotHash.GetKnotHash(oneLineData + "-" + x.ToString());
                string binaryString = "";
                foreach (char ch in thisKnotHash) { binaryString += Convert.ToString(Convert.ToInt32(ch.ToString(), 16), 2).PadLeft(4, '0'); }
                int[] intArray = new int[128];
                for (int ptr = 0; ptr < binaryString.Length; ptr++)
                {
                    intArray[ptr] = Convert.ToInt32(binaryString[ptr])==49?1:0; // char code for 1 is 49,
                }
                binaryRows.Add(intArray);
                sumOfUsedSpots += binaryString.Count(q => q == '1');
            }
            FinalOutput.Add("Sum of used spots: " + sumOfUsedSpots.ToString());
            //Part 2.... find contiguous regions.
            // convert List to int array.
            int[,] grid = new int[128,128];
            int bRow = 0;
            foreach (int[] ch in binaryRows)
            {
                int y = 0;
                foreach (int c in ch) { grid[bRow, y] = c; y++; }
                bRow++;
            }
            int regionCount = 0;
            // iterate through all spots looking for a lit-up spot ( ==1 )
            for (int row = 0; row < 128; row++)
            {
                for (int col = 0; col < 128; col++)
                {
                    if (grid[row, col] == 1)// we got one
                    {
                        regionCount++; // new region ( any previous region would have already cleared the associate array bits to 0 )
                        FloodFill(grid, row, col); // recursive function to find all contiguous ( but not diagonal, as per spec ) that are also turned on to 1
                    }
                }

            }
            FinalOutput.Add("Total Regions: " + regionCount.ToString());
        }
        public void FloodFill(int[,] grid,int x, int y){
            grid[x, y] = 0; // clear this spot so that we don't iterate over it again and get a stack overflow. 

            // these will call each direction recursively. When that path is exhausted, it will return and then we can look in the next direction. 

            // look up. if lit up, go "that direction" in a recursive call.
            if (x > grid.GetLowerBound(0))
            {
                if (grid[x-1,y]==1)
                    FloodFill(grid, x - 1, y);
            }
            // look down
            if (x < grid.GetUpperBound(0))
            {
                if (grid[x+1, y] == 1)
                    FloodFill(grid, x+1, y);
            }
            // look left
            if (y > grid.GetLowerBound(1))
            {
                if (grid[x , y-1] == 1)
                    FloodFill(grid, x , y-1);
            }
            // look right
            if (y < grid.GetUpperBound(1))
            {
                if (grid[x, y + 1] == 1)
                    FloodFill(grid, x, y + 1);
            }
        }
    }
}
