using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Drawing;

namespace AoC_2018
{
    class Day10_TheStarsAlign : AoCodeModule
    {
        public Day10_TheStarsAlign()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); //base class method
            OutputFileReset(); // output file will be in the same location as input, with ".output.txt" appended to the name. This clears previous output from file.
        }
        public override void DoProcess()
        {
            ResetProcessTimer(true);
            string[] parsed;
            foreach (string processingLine in inputFile)
            {
                //EG
                //position =< 30916,  41135 > velocity =< -3, -4 >
                parsed=processingLine.Split(new string[] { "position", "=", "<", ">", " ", "velocity", "," }, StringSplitOptions.RemoveEmptyEntries);
                SkyBox.TheStars.Add(new PointMovement() { Initial = new Point(int.Parse(parsed[0]), int.Parse(parsed[1])), Velocity = new Point(int.Parse(parsed[2]), int.Parse(parsed[3])) });
            }
            // THIS MADE MUCH MORE SENSE USING LONG INSTEAD OF INT!!!!
            //10230-10250 when incrementing by ten is the range where I should reset to go one at a time....
            // Hey, guess what, turns out that this isn't super-intensive time wise. I can go one at a time the whole way. 
            int counter = 0;// 10230; /// first time through, started at zero until the message about getting bigger again happened. Then refined and started counting at x-10, using increments of 1 instead of 10.
            long oldBox = SkyBox.BoxSize(counter);
            counter += 1; //10
            long newBox = SkyBox.BoxSize(counter);

            while (newBox < oldBox )
            {
                if (counter>10200) SkyBox.DisplayRational(counter); // this is just for fun, now that I know when it resolves. Maximize the console window before choosing module. Slows it WAY DOWN to do it the whole time and you don't start seeing ANYTHING move until the thosands...
                counter += 1; //10 for initial increment, 1 to refine.
                oldBox = newBox;
                newBox = SkyBox.BoxSize(counter);
                //Print("Box Size: " + newBox.ToString() + " at time " + counter.ToString());
                if (newBox > oldBox) {
                    //Print ("GETTING BIGGER!!!!"); 
                    counter--;
                } // marked when it starts expanding again. Go back one time-index to when it was smaller. 
            }
            Console.Clear();
            // displayRational after LONG conversion- Coordinates are actually usable now.
            SkyBox.DisplayRational(counter); // display(x) - tried to average and reset points. Once I used LONG instead of INT, the numbers became normalized.
            Console.SetCursorPosition(0,15);// just guessing.
            ResetProcessTimer();
            AddResult("At time index: " + counter.ToString()); // includes elapsed time from last ResetProcessTimer

        }
    }
    public class SkyBox
    {
        public static List<PointMovement> TheStars = new List<PointMovement>();
        public static long BoxSize(int AtTime)
        {
            // I was a dummy at first and failed to put parentheses around the Max-Min calcs.That caused all kinds of fun. 
            return (TheStars.Max(x => x.Get(AtTime).X) - TheStars.Min(x => x.Get(AtTime).X)) * (TheStars.Max(x => x.Get(AtTime).Y) - TheStars.Min(x => x.Get(AtTime).Y));
        }
        public static void Display(int AtTime)
        {
            long minX = TheStars.Min(x => x.Get(AtTime).X);
            long minY = TheStars.Min(x => x.Get(AtTime).Y);
            Console.Clear();
            TheStars.ForEach(x =>
            {
                Point b = x.Get(AtTime);
                long bufferX = (b.X - minX) / (long)Console.BufferWidth;
                long bufferY = (b.Y - minY) / (long)Console.BufferHeight;
                if (bufferX <= Console.BufferWidth && bufferY <= Console.BufferHeight)
                {
                    Console.SetCursorPosition((int)bufferX, (int)bufferY);
                    Console.Write("#");
                }
            });
        }
        public static void DisplayRational(int AtTime)
        {
            long minX = TheStars.Min(x => x.Get(AtTime).X);
            long minY = TheStars.Min(x => x.Get(AtTime).Y);
            Console.Clear();
            TheStars.ForEach(x =>
            {
                Point b = x.Get(AtTime);
                long bufferX = (b.X - minX);
                long bufferY = (b.Y - minY);
                if (bufferX < Console.BufferWidth && bufferY < Console.BufferHeight && bufferX>=0 && bufferY>=0)
                {
                    Console.SetCursorPosition((int)bufferX, (int)bufferY);
                    Console.Write("#");
                }
            });
        }
    }
    public class PointMovement
    {
        public Point Initial;
        public Point Velocity;
        public Point Get(int timeIndex)
        {
            return new Point(Initial.X + (Velocity.X * timeIndex), Initial.Y + (Velocity.Y * timeIndex));
        }
    }
    public class Point // was using System.Drawing.Point, but the ints inside caused problems.
    {
        public long X;
        public long Y;
        public Point(long x, long y)
        {
            X = x; Y = y;
        }
    }
}
