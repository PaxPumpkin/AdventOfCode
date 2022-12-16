using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day09_RopeBridge : AoCodeModule
    {
        public Day09_RopeBridge()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput();
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day09_RopeBridge part 1: 2 Knots Number of Places Tail Visited Once: 6011 (Process: 1.9518 ms)
            //** > Result for Day09_RopeBridge part 2: 10 Knots Number of Places Tail Visited Once: 2419 (Process: 4.4202 ms)
            ResetProcessTimer(true);
            foreach(int KnotCount in new int[]{ 2, 10 }) { AddResult(RunProblemForKnotCount(KnotCount, inputFile)); ResetProcessTimer(true); }
        }
        public string RunProblemForKnotCount(int numberOfKnots, List<string> inputFile)
        {
            List<(int X, int Y)> Knots = new List<(int X, int Y)>();
            for (int x = 1; x <= numberOfKnots; x++) Knots.Add((0, 0));
            HashSet<(int X, int Y)> TailVisits = new HashSet<(int X, int Y)>() { Knots.Last() };
            foreach (string processingLine in inputFile) ProcessMovementCommand(processingLine, Knots, TailVisits);
            return Knots.Count.ToString() + " Knots Number of Places Tail Visited Once: " + TailVisits.Count.ToString();
        }
        public void ProcessMovementCommand(string command, List<(int X, int Y)> knots, HashSet<(int X, int Y)> tailVisits)
        {
            // command is a single line from the input 
            string[] parsed = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string direction = parsed[0];
            int numberOfSteps = int.Parse(parsed[1]);
            int xMovement, yMovement;
            (int X, int Y) head; //placeholder for tuple in list, since direct modification of the object in the list is iffy.
            for (int step = 0; step < numberOfSteps; step++)
            {
                switch (direction)
                {
                    case "U":
                        xMovement = 1; yMovement = 0;
                        break;
                    case "D":
                        xMovement = -1; yMovement = 0;
                        break;
                    case "L":
                        xMovement = 0; yMovement = -1;
                        break;
                    case "R":
                        xMovement = 0; yMovement = 1;
                        break;
                    default:
                        xMovement = 0; yMovement = 0;
                        break;
                }
                head = knots.First();
                head.X += xMovement; head.Y += yMovement;
                knots[0] = head; // tuple is by value, not by ref, so gotta put it back to retain new values.
                for (int x = 1; x < knots.Count; x++)
                {
                    (int X, int Y) testKnot = knots[x];
                    int diffX = head.X - testKnot.X, diffY = head.Y - testKnot.Y;
                    if (Math.Abs(diffX) > 1 || Math.Abs(diffY) > 1)
                    {
                        testKnot.X += diffX < 0 ? -1 : diffX > 0 ? 1 : 0;
                        testKnot.Y += diffY < 0 ? -1 : diffY > 0 ? 1 : 0;
                    }
                    head = testKnot; // new "head" to compare against next knot.
                    knots[x] = testKnot; 
                }
                tailVisits.Add(head); // after looping, head should be last knot - the tail. Add its current location to the list of visited spots. Hashset prevents duplication/overcounting inherently.
            }
        }
    }
}
