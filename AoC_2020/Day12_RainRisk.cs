using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020 // I keep every year's puzzles, so this just separates them by year.
{
    // every day's puzzle is an extension of this "AoCodeModule" type. 
    class Day12_RainRisk : AoCodeModule 
    {
        public Day12_RainRisk() //constructor.... Main Timer was started before this is called.
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); // primary "input" file loading. No massaging or manipulation done. 
            OutputFileReset();
            
        }
        public override void DoProcess() // called to solve the puzzles.
        {
            //** > Result for Day12_RainRisk part 1:Manhattan Distance is 362(Process: 0.0003 ms)
            //** > Result for Day12_RainRisk part 2:Manhattan Distance is 29895(Process: 0.1696 ms)
            ResetProcessTimer(true); // start "Part 1" Timer. Everything related to "Part" is after this.
            Ship ferryBoat = new Ship();
            // if serious manipulation was needed, it's part of the timing cycle.
            // This puzzle doesn't require it.
            foreach (string processingLine in inputFile) 
            {
                ferryBoat.MoveTheShip(processingLine);
            }
            // AddResult method will spit out text at ANY POINT I need, and will include the "Part" timer's
            //  elapsed time up to that point. It does NOT reset the timer.
            AddResult("Manhattan Distance is " + ferryBoat.ManhattanDistance);


            //This resets the timer for Part 2. All data manipulation,etc for part 2 is counted.
            ResetProcessTimer(true);             
            ferryBoat = new Ship();
            foreach (string processingLine in inputFile)
            {
                ferryBoat.WaypointNavigation(processingLine);
            }
            AddResult("Manhattan Distance is " + ferryBoat.ManhattanDistance);
            // class exits and the shell checks the Main Timer after disposal. 
        }
        class Waypoint
        {
            public int XShift { get; set; }
            public int YShift { get; set; }
            public Waypoint(int xOffset,int yOffset)
            {
                XShift = xOffset; YShift = yOffset;
            }
            public void RotateTheWaypoint(string instruction)
            {
                char dir = instruction[0];
                int qty = Int32.Parse(instruction.Substring(1));
                while (qty > 0)
                {
                    int x = XShift, y = YShift;
                    if (dir == 'L')
                    {
                        YShift = x;
                        XShift = -y;
                    }
                    else
                    {
                        YShift = -x;
                        XShift = y;
                    }
                    qty -= 90;
                }
            }
        }
        class Ship
        {
            int CurrentX = 0;
            int CurrentY = 0;
            LinkedList<char> Compass = new LinkedList<char>();
            LinkedListNode<char> CurrentHeading;
            Waypoint navigationalWaypoint; 
            public int ManhattanDistance { get { return Math.Abs(CurrentY) + Math.Abs(CurrentX); } }
            public Ship()
            {
                CurrentX = 0; CurrentY = 0;
                Compass.Clear();
                LinkedListNode<char> position = Compass.AddFirst('E');
                position = Compass.AddAfter(position, 'S');
                position = Compass.AddAfter(position, 'W');
                position = Compass.AddAfter(position, 'N');
                CurrentHeading = Compass.First;
                navigationalWaypoint = new Waypoint(10, 1);
            }
            public void TurnTheShip(string instruction)
            {
                char dir = instruction[0];
                int qty = Int32.Parse(instruction.Substring(1));
                while (qty > 0)
                {
                    if (dir == 'R')
                    {
                        CurrentHeading = CurrentHeading.Next==null?Compass.First:CurrentHeading.Next;
                    }
                    else
                    {
                        CurrentHeading = CurrentHeading.Previous == null ? Compass.Last : CurrentHeading.Previous;
                    }
                    qty -= 90;
                }
            }
            public void MoveTheShip(string instruction)
            {
                char dir = instruction[0];
                if (dir =='L' || dir == 'R') { TurnTheShip(instruction); return; }
                dir = (dir == 'F') ? CurrentHeading.Value : dir;
                int qty = Int32.Parse(instruction.Substring(1));
                switch (dir)
                {
                    case 'N':
                        CurrentY += qty;
                        break;
                    case 'S':
                        CurrentY -= qty;
                        break;
                    case 'E':
                        CurrentX += qty;
                        break;
                    case 'W':
                        CurrentX -= qty;
                        break;
                    default:
                        throw new Exception("Fucked Direction");
                }
            }

            public void WaypointNavigation(string instruction)
            {
                char dir = instruction[0];
                if (dir=='L' || dir=='R') { navigationalWaypoint.RotateTheWaypoint(instruction); return; }
                int qty = Int32.Parse(instruction.Substring(1));
                switch (dir)
                {
                    case 'N':
                        navigationalWaypoint.YShift += qty;
                        break;
                    case 'S':
                        navigationalWaypoint.YShift -= qty;
                        break;
                    case 'E':
                        navigationalWaypoint.XShift += qty;
                        break;
                    case 'W':
                        navigationalWaypoint.XShift -= qty;
                        break;
                    case 'F':
                        for (int x=0; x<qty; x++)
                        {
                            CurrentX += navigationalWaypoint.XShift; CurrentY += navigationalWaypoint.YShift;
                        }
                        break;
                    default:
                        throw new Exception("Fucked Direction");
                }
            }
        }
    }
    //** > Result for Day12_RainRisk part 1:Manhattan Distance is 362(Process: 0.0003 ms)
    //** > Result for Day12_RainRisk part 2:Manhattan Distance is 29895(Process: 0.1696 ms)
}
