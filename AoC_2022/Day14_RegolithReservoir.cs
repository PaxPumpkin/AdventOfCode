using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day14_RegolithReservoir : AoCodeModule
    {
        public Day14_RegolithReservoir()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        public override void DoProcess()
        {
            //** > Result for Day14_RegolithReservoir part 2: 745 units of sand fell before falling into the abyss (Process: 15059.2877 ms)
            //** > Result for Day14_RegolithReservoir part 3: 27551 units of sand fell before blocking the sandhole (Process: 36357698.7861 ms)
            //606.212 minutes
            //10.10 hours
            // wow, that was a naive and crap solution
            ResetProcessTimer(true);
            HashSet<BlockedCoord> blocked= new HashSet<BlockedCoord>();
            foreach (string processingLine in inputFile)
            {
                ParseRegolith(processingLine, blocked);
            }
            HashSet<BlockedCoord> savedBase = new HashSet<BlockedCoord>(blocked);
            AddResult("Parsed Rock"); ResetProcessTimer(true);
            int sandAdded = 0;
            while(AddSand(blocked))
            {
                sandAdded++;
            }
            AddResult(sandAdded.ToString() + " units of sand fell before falling into the abyss");
            ResetProcessTimer(true);
            int minX = savedBase.Min(bc => bc.X) -400; //474 sample
            int maxX = savedBase.Max(bc => bc.X) + 400; // 523 sample
            int floorLevel = savedBase.Max(bc => bc.Y) + 2; // 11 sample
            for (int screwIt = minX; screwIt<=maxX; screwIt++)
            {
                savedBase.Add(new BlockedCoord {X=screwIt, Y=floorLevel });
            }
            sandAdded = 0;
            while (AddSand(savedBase, true))
            {
                sandAdded++;
            }
            AddResult((sandAdded + 1).ToString() + " units of sand fell before blocking the sandhole");
            ResetProcessTimer(true);
        }
        public void ParseRegolith(string structure, HashSet<BlockedCoord> set)
        {
            List<string> points = structure.Split(new string[] {" -> " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int x, y, tox, toy;
            string[] coords;
            // process all except last point, which is the end. 
            for (int pointCounter = 0; pointCounter< points.Count - 1; pointCounter++)
            {
                coords = points[pointCounter].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                x = int.Parse(coords[0]); y = int.Parse(coords[1]);
                coords = points[pointCounter + 1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                tox = int.Parse(coords[0]); toy = int.Parse(coords[1]);
                if (tox ==x ) //line is vertical, y should be different
                {
                    for (int yy = y; (y < toy ? yy<=toy : yy >= toy); yy +=(y < toy ? 1 :-1))
                    {
                        set.Add(new BlockedCoord { X = x, Y = yy });
                    }
                }
                else// implied y is the same, and line is horizontal.
                {
                    for (int xx = x; (x < tox ? xx <= tox : xx >= tox); xx += (x < tox ? 1 : -1))
                    {
                        set.Add(new BlockedCoord { X = xx, Y = y });
                    }
                }
            }
        }
        public bool AddSand(HashSet<BlockedCoord> set, bool withFloor = false)
        {
            BlockedCoord fallingSand = new BlockedCoord { X = 500, Y = 0 };
            BlockedCoord blockedPoint = set.Where(bc => bc.X == fallingSand.X && bc.Y == set.Where(bc2 => bc2.X == fallingSand.X && bc2.Y > fallingSand.Y).Min(bc2 => bc2.Y)).FirstOrDefault();
            BlockedCoord sandhole1 = new BlockedCoord { Y = 1, X = 499 };
            BlockedCoord sandhole2 = new BlockedCoord { Y = 1, X = 500 };
            BlockedCoord sandhole3 = new BlockedCoord { Y = 1, X = 501 };
            bool sandholeBlocked = set.Contains(sandhole1) && set.Contains(sandhole2) && set.Contains(sandhole3);
            //bool sandholeBlocked = set.Count(bc => bc.Y == 1 && (bc.X >= 499 && bc.X <= 501)) == 3;
            bool fullyBlocked = false;
            while (!fullyBlocked && !sandholeBlocked && blockedPoint.X!=0 && blockedPoint.Y!=0)
            {
                // if blocked down and to the left by one...
                if (set.Contains( new BlockedCoord { X = blockedPoint.X - 1, Y = blockedPoint.Y }))
                {
                    // if blocked down and to the right by one...
                    if (set.Contains(new BlockedCoord { X = blockedPoint.X + 1, Y = blockedPoint.Y }))
                    {
                        fullyBlocked = true;
                        fallingSand.Y = blockedPoint.Y-1;
                        set.Add(fallingSand);
                    }
                    else
                    {
                        fallingSand.X = blockedPoint.X + 1;
                        fallingSand.Y = blockedPoint.Y;
                        blockedPoint = set.Where(bc => bc.X == fallingSand.X && bc.Y == set.Where(bc2 => bc2.X == fallingSand.X && bc2.Y > fallingSand.Y).Min(bc2 => bc2.Y)).FirstOrDefault();
                    }
                }
                else
                {
                    fallingSand.X = blockedPoint.X - 1;
                    fallingSand.Y = blockedPoint.Y;
                    blockedPoint = set.Where(bc => bc.X == fallingSand.X && bc.Y == set.Where(bc2 => bc2.X == fallingSand.X && bc2.Y> fallingSand.Y).Min(bc2 => bc2.Y)).FirstOrDefault();
                }
                //if (withFloor && blockedPoint.X==0 && blockedPoint.Y==0)
                //{
                //    fallingSand.Y = floorLevel - 1;
                //    fullyBlocked = true;
                //    set.Add(fallingSand);
                //}
            }
            return fullyBlocked && (withFloor ? !sandholeBlocked : true); // this is backwards for ease, 
        }
    }
    struct BlockedCoord
    {
        public int X;
        public int Y;
    }
}
