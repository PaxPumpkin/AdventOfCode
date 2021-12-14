using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class HexGrid : AoCodeModule
    {

        public HexGrid()
        {
            inputFileName = "hexgrid.txt";
            base.GetInput();
        }
        public override void DoProcess()
        {
            List<string> path = new List<string>();

            GridHex whereWeBe = new GridHex(0, 0); // starting spot.

            // parse out the path.
            inputFile.ForEach(x => path.AddRange(x.Split(new char[] { ',' })));

            int maxDistanceEver = 0;
            path.ForEach(direction => { whereWeBe = whereWeBe.Move(direction); maxDistanceEver = Math.Max(whereWeBe.DistanceFromZero,maxDistanceEver); });
            // now that we've moved everywhere, let's see how far away we are.


            FinalOutput.Add("Ok. We be this many away: " + whereWeBe.DistanceFromZero.ToString());
            FinalOutput.Add("Ok. The furthest ever wandered away: " + maxDistanceEver.ToString());
        }
    }
    public class GridHex
    {
        // set this up, but it's really not necessary at all. 
        public static List<GridHex> InfiniteGrid = new List<GridHex>();
        public int x = 0;
        public int y = 0;

        public int DistanceFromZero
        {
            get
            {
                int a = Math.Abs(x);
                int b = Math.Abs(y);
                // amount of total (absolute) whole steps, plus one for every half step.
                return ((int)((a + b) / 10)) + (((a + b) % 10) / 5);
            }
        }
        public GridHex()
        {
        }
        public GridHex(int x, int y)
        {
            this.x = x; this.y =y;
            if (!InfiniteGrid.Contains(this)) InfiniteGrid.Add(this);
        }
        public override bool Equals(object obj)
        {
            GridHex comparison= (GridHex)obj;
            return comparison.x == this.x && comparison.y == this.y;
        }

        public GridHex Move(string direction) // returns the hex to which we moved.
        {
            int toX = this.x;
            int toY = this.y;
            switch (direction) // the value of 5 equals a "half step" since hexes look that way to me.
            {
                case "n":
                    toY += 10;
                    break;
                case "ne":
                    toX += 5;
                    toY += 5;
                    break;
                case "se":
                    toX += 5;
                    toY -= 5;
                    break;
                case "s":
                    toY-=10;
                    break;
                case "sw":
                    toX -= 5;
                    toY -= 5;
                    break;
                case "nw":
                    toX -= 5;
                    toY += 5;
                    break;

                default:
                    throw new Exception("Uhm. Move where???");
            }
            GridHex nextHex = InfiniteGrid.Where(hex => hex.x == toX && hex.y == toY).FirstOrDefault();// should be only 1 or 0 found.
            if (nextHex == null) { nextHex = new GridHex(toX, toY); }
            return nextHex;
        }
    }
}
