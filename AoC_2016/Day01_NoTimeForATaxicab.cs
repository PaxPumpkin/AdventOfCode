using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day01_NoTimeForATaxicab : AoCodeModule
    {
        public Day01_NoTimeForATaxicab()
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
            List<string> inputItems = inputFile[0].Split(new char[] { ',' }).ToList<string>();
            //string finalResult = "Not Set";
            ResetProcessTimer(true);// true also iterates the section marker
            Directions me = new Directions();
            
            foreach (string processingLine in inputItems)
            {
                me.Move(processingLine.Trim());
            }
            AddResult(me.BlocksAway.ToString()); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);
            DirectionsTracker you = new DirectionsTracker();
            foreach (string processingLine in inputItems)
            {
                you.Move(processingLine.Trim());
                if (you.CrossedPath != null) { break; }
            }
            AddResult(you.BlocksAway.ToString());
        }
    }
    public class Directions{
        char[] directions = new char[] { 'N', 'E', 'S', 'W' };
        char currentDirection = 'N';
        int dPointer = 0;
        int x = 0, y = 0;
        public int BlocksAway { get {
                return Math.Abs(x) + Math.Abs(y);
            } }
        public Directions() { }
        public void Turn(char direction)
        {
            dPointer += (direction == 'R') ? 1 : -1;
            dPointer = (dPointer < 0) ? directions.GetUpperBound(0) : ((dPointer > directions.GetUpperBound(0)) ? 0 : dPointer);
            currentDirection = directions[dPointer];
        }
        public void Move(string command)
        {
            Turn(command[0]);
            int distance = int.Parse(command.Substring(1));
            distance *= (currentDirection == 'S' || currentDirection == 'W') ? -1 : 1;
            x+=((currentDirection == 'W' || currentDirection == 'E') ? distance : 0);
            y += ((currentDirection == 'N' || currentDirection == 'S') ? distance : 0);
        }
    }
    public class BlockPoint {
        public int x;
        public int y;
        public BlockPoint(int xCoord, int yCoord)
        {
            x = xCoord;
            y = yCoord;
        }
        public override bool Equals(object other)
        {
            BlockPoint otherBP = other as BlockPoint;
            return otherBP.x == this.x && otherBP.y == this.y;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class BlockPointComparer : IEqualityComparer<BlockPoint>
    {

        public bool Equals(BlockPoint first, BlockPoint second)
        {
            return (first.x==second.x && first.y==second.y);
        }

        public int GetHashCode(BlockPoint obj)
        {
            return obj.x.GetHashCode() ^ obj.y.GetHashCode();
        }
    }
   


    public class DirectionsTracker
    {
        char[] directions = new char[] { 'N', 'E', 'S', 'W' };
        char currentDirection = 'N';
        int dPointer = 0;
        int x = 0, y = 0;
        List<BlockPoint> beenHere = new List<BlockPoint>();
        public BlockPoint CrossedPath;
        public int BlocksAway
        {
            get
            {
                return Math.Abs(x) + Math.Abs(y);
            }
        }
        public DirectionsTracker() {
            beenHere.Add(new BlockPoint(x, y));
        }
        public void Turn(char direction)
        {
            dPointer += (direction == 'R') ? 1 : -1;
            dPointer = (dPointer < 0) ? directions.GetUpperBound(0) : ((dPointer > directions.GetUpperBound(0)) ? 0 : dPointer);
            currentDirection = directions[dPointer];
        }
        public void Move(string command)
        {
            Turn(command[0]);
            int distance = int.Parse(command.Substring(1));
            //distance *= (currentDirection == 'S' || currentDirection == 'W') ? -1 : 1;
            int moveAmount = (currentDirection == 'S' || currentDirection == 'W') ? -1 : 1;
            for (int i = 0; i <distance; i++) {
                x += ((currentDirection == 'W' || currentDirection == 'E') ? moveAmount : 0);
                y += ((currentDirection == 'N' || currentDirection == 'S') ? moveAmount : 0);
                BlockPoint newPoint = new BlockPoint(x, y);
                if (beenHere.Contains(newPoint))
                {
                    CrossedPath = newPoint;
                    break;
                }
                beenHere.Add(newPoint);
            }
        }
    }
}
