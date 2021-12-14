using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class Program
    {
        static string data = "L4, L3, R1, L4, R2, R2, L1, L2, R1, R1, L3, R5, L2, R5, L4, L3, R2, R2, L5, L1, R4, L1, R3, L3, R5, R2, L5, R2, R1, R1, L5, R1, L3, L2, L5, R4, R4, L2, L1, L1, R1, R1, L185, R4, L1, L1, R5, R1, L1, L3, L2, L1, R2, R2, R2, L1, L1, R4, R5, R53, L1, R1, R78, R3, R4, L1, R5, L1, L4, R3, R3, L3, L3, R191, R4, R1, L4, L1, R3, L1, L2, R3, R2, R4, R5, R5, L3, L5, R2, R3, L1, L1, L3, R1, R4, R1, R3, R4, R4, R4, R5, R2, L5, R1, R2, R5, L3, L4, R1, L5, R1, L4, L3, R5, R5, L3, L4, L4, R2, R2, L5, R3, R1, R2, R5, L5, L3, R4, L5, R5, L3, R1, L1, R4, R4, L3, R2, R5, R1, R2, L1, R4, R1, L3, L3, L5, R2, R5, L1, L4, R3, R3, L3, R2, L5, R1, R3, L3, R2, L1, R4, R3, L4, R5, L2, L2, R5, R1, R2, L4, L4, L5, R3, L4";
        

        static void Main(string[] args)
        {
            Position1 position = new Position1("North");
            string[] moves = data.Split(new char[] { ',' });
            foreach (string move in moves)
            {
                if (!position.Go(move.Trim().ToLower()))
                {
                    break; // we crossed a path. We are now somewhere we've been before (may not have completed the move entirely. Just until we hit the same spot)
                }
            }
            Console.WriteLine("Position is now " + position.distance.ToString() + " blocks away");
        }
    }
    class Coordinate1
    {
        public int xpos = 0;
        public int ypos = 0;
        public Coordinate1(int x, int y)
        {
            xpos = x; ypos = y;
        }
    }
    class Position1
    {
        public int xpos = 0;
        public int ypos = 0;
        string orientation = "North";
        List<string> directions = (new string[] { "North", "East", "South", "West" }).ToList();
        List<Coordinate1> visitedSpots = new List<Coordinate1>();
        public int distance
        {
            get
            {
                int totalBlocks =0;
                totalBlocks = Math.Abs(xpos) + Math.Abs(ypos);
                return totalBlocks;
            }
        }
        public Position1(string direction)
        {
            if (direction == null || direction.Trim().Equals("")) direction = "North";
            orientation = direction;
            visitedSpots.Add(new Coordinate1(0, 0));
        }
        public bool Go(string direction)
        {
            orientation = ParseOrientation(direction.Trim().ToLower().Substring(0, 1));
            int modifier = ((orientation.Equals("West") || orientation.Equals("South")) ? -1 : 1);
            int blocks = (Int32.Parse(direction.Trim().Substring(1)));// *((orientation.Equals("West") || orientation.Equals("South")) ? -1 : 1);

            for (int x = 1; x <= blocks; x++)
            {
                // first move
                if (orientation.Equals("North") || orientation.Equals("South"))
                {
                    ypos += modifier; // one block at a time....
                }
                else
                {
                    xpos += modifier;
                }
                //then check
                if (visitedSpots.FindIndex(q => q.xpos == xpos && q.ypos == ypos) >= 0)
                {
                    // already been here before
                    return false;
                }
                else
                {
                    // never been here before so add it to the path
                    visitedSpots.Add(new Coordinate1(xpos, ypos));
                }
            }
            return true; // did not cross any paths.

            //if (orientation.Equals("North") || orientation.Equals("South"))
            //{
            //    ypos += blocks;
            //}
            //else
            //{
            //    xpos += blocks;
            //}
        }
        private string ParseOrientation(string turn)
        {
            int pointerIndex = directions.FindIndex(x=>x.Equals(orientation));
            pointerIndex += (turn.Equals("l") ? -1 : 1);
            pointerIndex = (pointerIndex < 0) ? 3 : (pointerIndex > 3 ? 0 : pointerIndex);
            return directions[pointerIndex];
        }
    }
}
