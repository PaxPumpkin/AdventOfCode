using System;
using System.Collections.Generic;

namespace AoC_2020
{
    class Day03_TobagganTrajectory : AoCodeModule
    {
        private char[,] mountainSlope;
        public Day03_TobagganTrajectory()
        {
             
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();

            mountainSlope = new char[inputFile.Count, inputFile[0].Length];
            int positionX=0, positionY;
            inputFile.ForEach(line =>
            {
                positionY = 0;
                foreach (char slopeGeography in line)
                {
                    mountainSlope[positionX, positionY] = slopeGeography;
                    positionY++;
                }
                positionX++;
            });
        }
        public override void DoProcess()
        {
            // ** > Result for Day03_TobagganTrajectory part 1:Encountered 178 trees!(Process: 0 ms)
            // ** > Result for Day03_TobagganTrajectory part 2:Product of all paths is 3492520200 trees.(Process: 0 ms)
            ResetProcessTimer(true);
            AddResult("Encountered " + TraverseTheSlope(3,1).ToString() + " trees!");
            ResetProcessTimer(true);
            List<int[]> Paths = new List<int[]>
            {
                new int[] { 1, 1 },
                new int[] { 3, 1 },
                new int[] { 5, 1 },
                new int[] { 7, 1 },
                new int[] { 1, 2 }
            };
            long finalProduct = 1;
            Paths.ForEach(traversal => finalProduct *= TraverseTheSlope(traversal[0], traversal[1]));
            AddResult("Product of all paths is " + finalProduct.ToString() + " trees.");
            
        }

        private int TraverseTheSlope(int MoveRight, int MoveDown)
        {
            int slopeRow = 0, columnPointer = 0, treeCounter = 0;
            int totalRows = mountainSlope.GetUpperBound(0) + 1, totalColumns = mountainSlope.GetUpperBound(1) + 1;
            while (slopeRow < totalRows)
            {
                slopeRow += MoveDown; columnPointer += MoveRight; columnPointer %= totalColumns;
                treeCounter += (slopeRow < totalRows)?((mountainSlope[slopeRow, columnPointer] == '#') ? 1 : 0):0;
            }
            return treeCounter;
        }
    }
}
