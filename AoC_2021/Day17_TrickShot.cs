using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day17_TrickShot : AoCodeModule
   {
      public Day17_TrickShot()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         //** > Result for Day17_TrickShot part 1: Highest y reached is 3916 (Process: 14.3511 ms)
         //** > Result for Day17_TrickShot part 2: Velocities found that will hit target 2986 (Process: 0.2584 ms)
         ResetProcessTimer(true);
         // split on colon. Take the right side and remove all characters that don't matter. Should get 4 numbers for x and y ranges.
         string[] input = inputFile[0].Split(new char[] {':'})[1].Split(new char[] { ' ', 'x','y', ',', '=', '.' }, StringSplitOptions.RemoveEmptyEntries);
         List<int> xRange = new List<int> { int.Parse(input[0]), int.Parse(input[1]) }, yRange = new List<int> { int.Parse(input[2]), int.Parse(input[3]) };

         int highestYPosition = 0; // highest Y throughout all test conditions.
         List<string> foundVelocities = new List<string>(); // save all velocities that hit the target at all.

         int xLower = 1; // find the lowest x-velocity that actually reaches the min target edge before reaching value of zero. Fancy sigma. 
         while ((xLower * (xLower + 1)) / 2 < xRange.First()) xLower++; 

         for (int x = xLower; x <= xRange.Last(); x++) // iterate from lowest x Velocity that reaches target through highest velocity that doesn't overshoot.
         {
            for (int y = yRange.First(); y < Math.Abs(yRange.First()); y++) // from the fastest negative velocity that doesn't overshoot, through the fastest positive that doesn't accumulate gravity increases and overshoot.
            {
               int xVel = x, yVel = y; // test conditions. Will get changed during test.
               int xPos = 0, yPos = 0; // tracking trajectory positions to see when we hit the target
               int highestYThisCombo = 0; // highest y position reached for this test condition combination.
               while (yPos >= yRange.First() && xPos <= xRange.Last())// while we haven't gone farther than our target outer edges...
               {
                  xPos += xVel; yPos += yVel; // change position
                  highestYThisCombo = Math.Max(highestYThisCombo, yPos); //check to see if we got any higher than before during this velocity combination test
                  if (xPos >= xRange.First() && xPos <= xRange.Last() && yPos <= yRange.Last() && yPos >= yRange.First()) // if we hit the target...
                  {
                     //save the combo that worked.
                     foundVelocities.Add(x + "," + y); // can add duplicates. Costs more to check each before adding than just counting distinct after, so just stuff it in there.
                     highestYPosition = Math.Max(highestYPosition, highestYThisCombo); // see if our highest Y for this combination is highest of any combination.
                  }
                  xVel = (xVel == 0) ? 0 : xVel - 1; // apply drag to forward velocity. Can't go lower than zero. 
                  yVel--; // apply arbitrary gravity increases towards faster negative velocity. Unbounded. 
               }
            }
         }
         AddResult("Highest y reached is " + highestYPosition);
         ResetProcessTimer(true); // just for the part 2 marker in the result. All the processing is done, so the timer value for part 2 isn't terribly relevant. 
         AddResult("Velocities found that will hit target " + foundVelocities.Distinct().Count());
         ResetProcessTimer(true);

      }
   }
}
