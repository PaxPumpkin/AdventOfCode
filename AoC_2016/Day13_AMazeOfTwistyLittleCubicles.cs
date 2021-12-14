using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day13_AMazeOfTwistyLittleCubicles : AoCodeModule
    {
        public Day13_AMazeOfTwistyLittleCubicles()
        {

            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); //base class method
            OutputFileReset(); // output file will be in the same location as input, with ".output.txt" appended to the name. This clears previous output from file.
        }
        public override void DoProcess()
        {
            //string finalResult = "Not Set";
            ResetProcessTimer(true);// part 1
            CubicleMaze.MinimumMovesFoundAtSomePoint = 200; // deepest recursion I'll permit for this traversal
            Console.Clear();
            Console.CursorTop = 0;
            Console.CursorLeft = 00;
            CubicleMaze.hsf.Print("Checking Location: 0000:0000 Move: 0000", true);
            MazeCoordinate.MagicNumber = 1362; // static value to use to compute all the wall/space for coordinate stuff whenever a new coordinate is tested.
            CubicleMaze.StartAt(new MazeCoordinate(1, 1));

            int result;//= CubicleMaze.GetTo(new MazeCoordinate(31, 39,true), CubicleMaze.Start, null);

            //AddResult("Total Moves: " + result.ToString());
            ResetProcessTimer(true);// part 2 start
            CubicleMaze.MinimumMovesFoundAtSomePoint = 52;
            CubicleMaze.PlacesIveSeen.Clear();
            result = CubicleMaze.GetTo(new MazeCoordinate(31, 39, true), CubicleMaze.Start, null); // should never get there and end with 0
            AddResult("Unique Spots in fifty moves: " + CubicleMaze.PlacesIveSeen.Count.ToString() + " does it match this? " + CubicleMaze.Maze.Count(x=>x.Traversed==true).ToString());
            Console.CursorTop = 2;
            Console.CursorLeft = 0;
        }

    }
    public class CubicleMaze
    {
        public static List<MazeCoordinate> Maze = new List<MazeCoordinate>();
        public static MazeCoordinate Start;
        public static MazeCoordinate Current;
        public static List<MazeCoordinate> CurrentPath = new List<MazeCoordinate>();
        public static List<MazeCoordinate> PlacesIveSeen = new List<MazeCoordinate>();
        public static int MinimumMovesFoundAtSomePoint = 500;// sets a limit on how far we will let recursion roam...
        public static int RecursionCounter = 0;
        public static bool AbortPath = false;
        public static HackerScannerPrint hsf = new HackerScannerPrint("Checking Location: 0000:0000 Move: 0000", 'q');
        public static void StartAt(MazeCoordinate start)
        {
            Start = start;
            Current = start;
            CurrentPath.Add(start);
        }
        public static int TraverseIt()
        {

            return 0;

        }
        public static int GetTo(MazeCoordinate goal, MazeCoordinate me, MazeCoordinate from)
        {
            int pathCounter = from == null ? 0 : 1;
            // ok, I need to be able to keep track of how many moves are in the current path iteration 
            // because if we are currently at move X where that is larger than a path that was already discovered, there's no reason to continue!
            RecursionCounter++;
            Console.CursorTop = 0;
            Console.CursorLeft = 39;
            hsf.Print("Checking Location: " + me.X.ToString().PadLeft(4, ' ') + ":" + me.Y.ToString().PadRight(4, ' ') + " Move: " + RecursionCounter.ToString().PadLeft(4, ' '), false);
            if (RecursionCounter + pathCounter > MinimumMovesFoundAtSomePoint) {
                //Console.WriteLine();
                //Console.WriteLine("Aborting Attempt... at move " + RecursionCounter + " at location " + me.X + ":" + me.Y );
                //hsf.Print("Checking Location: " + me.X.ToString().PadLeft(4, ' ') + ":" + me.Y.ToString().PadRight(4, ' '), true);
                RecursionCounter--; AbortPath = false; return 0; }
            me.Traversed = true; // marker so that if another path brings us here again, we don't go in circles...
            me.Draw();
            if (!PlacesIveSeen.Contains(me)) { PlacesIveSeen.Add(me); }
            // first scan all around for open space to determine our possible paths from here.
            List<MazeCoordinate> possibilities = new List<MazeCoordinate>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int targetX = x + me.X, targetY = y + me.Y;
                    // if we're going negative, we're scanning ourselves, or we are scanning where we came from, skip it. 
                    if (targetX < 0 || targetY< 0 || (targetX == me.X && targetY == me.Y) || (from!=null && targetX==from.X && targetY==from.Y)) continue;
                    else
                    {
                        if (!((targetX == me.X - 1 && targetY == me.Y - 1) ||
                            (targetX == me.X - 1 && targetY == me.Y + 1) ||
                            (targetX == me.X + 1 && targetY == me.Y - 1) ||
                            (targetX == me.X + 1 && targetY == me.Y + 1)) && targetX == goal.X && targetY == goal.Y) {
                            me.Draw(true);
                            RecursionCounter--; return pathCounter + 1; } // our goal is right here? Add the move it would take to get there and bug out.
                        MazeCoordinate test = new MazeCoordinate(targetX,targetY);// just adds to Maze if hasn't been scanned before. 
                        // now get the object from the maze list (protects against overwriting the object properties)
                        if ((targetX == me.X - 1 && targetY == me.Y - 1) ||
                            (targetX == me.X - 1 && targetY == me.Y + 1) ||
                            (targetX == me.X + 1 && targetY == me.Y - 1) ||
                            (targetX == me.X + 1 && targetY == me.Y + 1)) { test = null; }
                        else
                        {
                            //test = Maze.Where(space => (space.Traversed == false && space.X == targetX && space.Y == targetY)).FirstOrDefault();
                            test = Maze.Where(space => (space.X == targetX && space.Y == targetY)).FirstOrDefault();
                            if (CurrentPath.Contains(test)){ test = null; }
                        }
                        // if test is null here, it is only because Traversed==true and we've been down that road already. 
                        if (test != null && test.SpaceType!=1) possibilities.Add(test); // only add if it is open space.
                    }
                }
            }// all possibilities are loaded.
            if (possibilities.Count == 0) pathCounter = 0;
            else
            {
                int MinimumFound = int.MaxValue;
                for (int i = 0; i < possibilities.Count; i++)
                {
                    CurrentPath.Add(possibilities[i]);
                    int result = GetTo(goal, possibilities[i], me);
                    CurrentPath.Remove(possibilities[i]);
                    // we have traversed all the way from start and gotten there at least once.
                    if (result != 0 && from == null)
                    {
                        Console.SetCursorPosition(0, 1);
                        Console.Write("Back at Start with " + result.ToString() + " Moves!");
                        if (result < MinimumMovesFoundAtSomePoint) { Console.WriteLine("   NEW RECORD!"); } else { Console.WriteLine(" Nope."); }
                        MinimumMovesFoundAtSomePoint = Math.Min(result, MinimumMovesFoundAtSomePoint); }
                    if (result != 0) me.Draw(true);
                    MinimumFound = result!=0?Math.Min(result, MinimumFound):MinimumFound;// if multiple paths can get there from here, only return the smallest number of moves. 
                }
                pathCounter += MinimumFound != int.MaxValue ? MinimumFound : 0; // only increment the counter if a realistic value was found.
            }
            // since we know that we are not the goal (never would have entered this method )
            // then result must be greater than the move it took to get here. If it is not, we should return zero to indicate a dead path. 
            RecursionCounter--;
            return pathCounter > 1? pathCounter : 0;
        }
    }
    public class MazeCoordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int SpaceType { get; set; }
        public bool Traversed { get; set; }
        public static int MagicNumber { get; set; }
        public MazeCoordinate(int x, int y):this(x,y,false)
        {
        }
        public MazeCoordinate(int x, int y, bool isGoal)
        {
            X = x; Y = y; SpaceType = MagicTransform(x, y); Traversed = false;
            if (!CubicleMaze.Maze.Contains(this))
            {
                CubicleMaze.Maze.Add(this);
                ConsoleColor original = Console.ForegroundColor;
                Console.ForegroundColor = (isGoal) ? ConsoleColor.Red : original;
                Console.CursorTop = 10 + x;
                Console.CursorLeft = y;
                Console.Write(SpaceType == 0 ? "." : "#");
                Console.ForegroundColor = original;
            }
        }
        public void Draw()
        {
            Draw(false);
        }
        public void Draw(bool path)
        {
            ConsoleColor original = Console.ForegroundColor;
            Console.ForegroundColor = path?ConsoleColor.Green:ConsoleColor.Blue;
            Console.CursorTop = 10 + this.X;
            Console.CursorLeft = this.Y;
            Console.Write("@");
            Console.ForegroundColor = original;
        }
        public override bool Equals(object other)
        {
            MazeCoordinate otherBP = other as MazeCoordinate;
            return otherBP.X == this.X && otherBP.Y == this.Y;
        }
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }
        private static int MagicTransform(int x, int y)
        {
            return (Convert.ToString((((x * x) + (3 * x) + (2 * x * y) + y + (y * y)) + MagicNumber), 2).Count(a => a == '1')) % 2;
        }
    }
    public class MazeCoordinateComparer : IEqualityComparer<MazeCoordinate>
    {

        public bool Equals(MazeCoordinate first, MazeCoordinate second)
        {
            return (first.X == second.X && first.Y == second.Y);
        }

        public int GetHashCode(MazeCoordinate obj)
        {
            return obj.X.GetHashCode() ^ obj.Y.GetHashCode();
        }
    }
}
