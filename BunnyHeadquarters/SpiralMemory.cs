using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class SpiralMemory
    {
        static int[,] spiral;
        static void Main(string[] args)
        {
            int result = 0;
            int magicNumber = 289326; // my target number that was provided as input.
            // cheap and easy way to get ints from doubles and I don't gotta mess with exceptions.
            int rowsquare = Int32.Parse(Math.Floor(Math.Sqrt(magicNumber)).ToString()); // number of rows/cols I need to have this many squares to work with. This should represent either EXACTLY the proper number of rows OR one row too few.
            // since it's likely that it's actually one row too few, we need to add one. 
            // ...and adding more space for edge-buffering on the summation problem. I'll want empty squares on the border for expediency. 
            // Also ensures odd numbers of rows for center-finding calculations. Laziness.
            rowsquare += (rowsquare%2==0)?3:2;  // if it's even, I need to make it odd. Add odd, but then I need the extra rows for buffering either way.
            spiral = new int[rowsquare,rowsquare]; // init. Primitive array, will automatically be filled with 0s.
            int currentx = (spiral.GetUpperBound(0) ) / 2; // establish I'm starting at the center.
            int currenty = (spiral.GetUpperBound(1) ) / 2; // both will be the same value for this particular problem, but separating in case the next problem has an irregular shape.
            // second problem doesn't use these values after all, but leaving in. 
            int initialX = currentx;
            int initialY = currenty;
            int iterator = 1; // until we've filled in everything to get to our spot, the first square is numbered 1. Increment the iterator AFTER assignation to the array.
            bool showNumber = true; // flag to display the result of summation
            spiral[currentx, currenty] = 1; // init the center space value.
            int totalloops = (rowsquare+1)/2; // for now, this is true. ( shifting a zero-based array to a 1-based counter, and the additional loop I added for buffering which won't work to iterate for summation.
            int previousloopssquares = 0; // for counting how many steps to move through, I need to know how many are inside. Probably an easier way to do this (x^2 - (x-1)^2) comes to mind...
            for (int looprow = 0; looprow < totalloops; looprow++)//oscillator pointer. 
            {
                // loop row is zero-based, so adding one to keep me from recalculating how I'm doing stuff. 
                int thisLoopsSquares = Int32.Parse(Math.Pow(looprow*2+1 , 2).ToString())-previousloopssquares;
                int thisLoopsOscillationLength = thisLoopsSquares / 4;//should truncate to the remainder, but each side is made up of equal numbers of squares 
                bool firstTimeThrough = true; // marker that this is the beginning of the next loop to avoid advancing the pointer on the x-axis initially.
                for (int legCounter = 1; legCounter <= 4; legCounter++) // four sides to the loop
                {
                    // squares to step through on each loop through....
                    for (int stepCounter = 1; stepCounter <= thisLoopsOscillationLength; stepCounter++)
                    {
                        // first time through each loop, we don't move the pointer, since it is the very first square of the loop and we moved the pointer at the END of the previous loop.
                        if (!firstTimeThrough)
                        {
                            // silly way to do this, I think, but it works for quick coding.
                            switch (legCounter)
                            {
                                case 1:
                                    currenty--; //go "up"
                                    break;
                                case 2:
                                    currentx--; //go "left"
                                    break;
                                case 3:
                                    currenty++; //"down"
                                    break;
                                case 4:
                                    currentx++; //"right"
                                    break;
                                default:
                                    Console.WriteLine("WHAT THE ACTUAL F???");
                                    break;
                            }
                        }// end check if we're moving the pointers. 

                        ////for original problem, this just set to the iterator of total squares...
                        //spiral[currentx, currenty] = iterator;
                        //if (iterator == magicNumber) // did we hit our target space?
                        //{
                        //    // so... summation of absolute of currentx-initialx and absolute of currenty-initialy should be step count away! Don't count the initial center, so -1
                        //    int totalStepsAway = Int32.Parse((Math.Abs(currentx - initialX) + Math.Abs(currenty - initialY)).ToString()) - 1;
                        //    Console.WriteLine("Steps away: " + totalStepsAway.ToString());
                        //    Console.ReadLine();
                        //    // note - easier way AFTER I did the dumb stuff. If I know how many loops I need in my square, I ALREADY know the base step distance. 
                        //    //  -- then I just need to know which leg it is in, 1/3 or 2/4. This would be differential on X or Y. It's all just a stupid math problem and I'm ACTUALLY WRITING THESE STUPID SQUARES.
                        //    //  -----   OK, well, problem two apparently will need these squares written after all, so I guess it wasn't a total waste, but I'm betting that is a simple math problem too. 
                        //}
                        //iterator++;

                        int AdjacentSquaresSum = GetAdjacentSquaresSum(currentx, currenty); //look all around, add it up. Keep separate for checking if we're done.
                        spiral[currentx, currenty] = AdjacentSquaresSum; //set this square's value. 
                        if (AdjacentSquaresSum > magicNumber && showNumber) // we done?  "first number written LARGER than our magic number"
                        {
                            showNumber = false; // we're showing it. After readline, just run to finish.
                            Console.WriteLine("The value is: " + AdjacentSquaresSum.ToString());
                            Console.ReadLine();
                        }
                        else
                        {
                            // debugging, but ignore this noise.
                            if (showNumber) { Console.WriteLine("Loop: " + looprow.ToString() + " leg: " + legCounter.ToString() + " step: " + stepCounter.ToString() + " is value " + AdjacentSquaresSum.ToString()); }
                        }
                        firstTimeThrough = false; // ok, we've done the first square of the next loop. Next time, move pointers.
                    }

                }
                previousloopssquares=thisLoopsSquares+previousloopssquares;
                // at the end of every loop, reset initial starting point by +1 on the x axis.
                currentx++;
            }


            Console.WriteLine("Done: " + result.ToString());
            Console.ReadLine();
        }

        // second problem wants to add stuff. 
        public static int GetAdjacentSquaresSum(int x, int y)
        {
            int summation = 0;
            // this aggravates me, but I can't be bothered to figure out a better way right now. 
            summation += GetSquareValue(x + 1, y);
            summation += GetSquareValue(x + 1, y-1);
            summation += GetSquareValue(x , y-1);
            summation += GetSquareValue(x -1, y-1);
            summation += GetSquareValue(x -1, y);
            summation += GetSquareValue(x - 1, y+1);
            summation += GetSquareValue(x , y +1);
            summation += GetSquareValue(x + 1, y +1);
            return summation;
        }
        public static int GetSquareValue(int x, int y)
        {
            // boundary protection, but the looping and setup should avoid this ever happening. 
            if (x < 0 || y < 0 || x > spiral.GetUpperBound(0) || y > spiral.GetUpperBound(1)) return 0;
            else return spiral[x, y];
        }
    }
}
