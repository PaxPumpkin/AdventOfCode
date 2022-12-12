using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day10_CathodeRayTube : AoCodeModule
    {
        public Day10_CathodeRayTube()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput();
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day10_CathodeRayTube part 1: Sum of these signal strengths is 13860 (Process: 0.0452 ms)
            //** > Result for Day10_CathodeRayTube part 1: What is on the screen? RZHFGJCB (Process: 2.496 ms)
            // requires monospaced font to see correctly. 
            /*
             * ###..####.#..#.####..##....##..##..###..
                #..#....#.#..#.#....#..#....#.#..#.#..#.
                #..#...#..####.###..#.......#.#....###..
                ###...#...#..#.#....#.##....#.#....#..##
                #.#..#....#..#.#....#..#.#..#.#..#.#..##
                #..#.####.#..#.#.....###..##...##..###..
            */
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            List<long> registerValueAfterCycle = new List<long>() { 1 };
            long accumulator = 1;
            bool lastInstructionWasAdd = false;
            foreach (string processingLine in inputFile)
            {
                if (processingLine.Equals("noop"))
                {
                    registerValueAfterCycle.Add(accumulator);
                    //Print("Noop");
                    lastInstructionWasAdd = false;
                }
                else
                {
                    long instruction = long.Parse(processingLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
                    //Print("ADDX " + instruction.ToString());
                    registerValueAfterCycle.Add(accumulator);
                    registerValueAfterCycle.Add(accumulator);
                    accumulator += instruction;
                    lastInstructionWasAdd = true;
                }
            }
            if (lastInstructionWasAdd) registerValueAfterCycle.Add(accumulator);
            long summation = 0;
            //20th, 60th, 100th, 140th, 180th, and 220th
            foreach (int cycle in new int[] {20,60,100,140,180,220 })
            {
                summation += cycle * registerValueAfterCycle[cycle];
            }
            AddResult("Sum of these signal strengths is " + summation.ToString());
            Print("Starting CRT cycle");
            string crtLine = "";
            for(int x=1; x<=240; x++)
            {
                if ((x-1) % 40 == 0)
                {
                    Print(crtLine);
                    crtLine = "";
                }
                if (x%40 >= registerValueAfterCycle[x]  && x%40 <= registerValueAfterCycle[x] +2 )
                    crtLine += "#";
                else
                    crtLine += ".";
            }
            Print(crtLine);
            AddResult("What is on the screen? RZHFGJCB");
        }
    }
}
