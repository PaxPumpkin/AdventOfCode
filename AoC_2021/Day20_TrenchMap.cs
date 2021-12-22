using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day20_TrenchMap : AoCodeModule
   {
      public Day20_TrenchMap()
      {

         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         //** > Result for Day20_TrenchMap part 1: Pixels lit after 2 enhancements is 5218 (Process: 206.4787 ms)
         //** > Result for Day20_TrenchMap part 2: Pixels lit after 50 enhancements is 15527 (Process: 11704.6307 ms)
         ResetProcessTimer(true);
         ImageEnhancer enhancer = new ImageEnhancer(inputFile);
         AddResult("Pixels lit after 2 enhancements is " + enhancer.RunEnhancement(2).ToString());
         ResetProcessTimer(true);
         AddResult("Pixels lit after 50 enhancements is " + enhancer.RunEnhancement(48).ToString()); // 48 more times to make 50 total.
         ResetProcessTimer(true);
      }
   }
   public class ImageEnhancer
   {
      string enhancementAlgorithm { get; set; }
      HashSet<PointDescription> LitPixels = new HashSet<PointDescription>();

      public ImageEnhancer(List<string> input)
      {
         enhancementAlgorithm = input[0];
         int inputPointer = 0;
         while (++inputPointer < input.Count)
         {
            if (input[inputPointer] != "")
            {
               int x = 0, y = inputPointer - 2;
               while (x < input[inputPointer].Length)
               {
                  if (input[inputPointer][x] == '#') LitPixels.Add(new PointDescription { X = x, Y = y });
                  x++;
               }
            }
         }

      }
      public int RunEnhancement(int thisManyTimes)
      {
         // I am a bonehead. the edges of the image are infinite and will ALSO toggle according to the algorithms. So, they always have to be accounted 
         // for as they flip. No wonder everything is so messed up. And I mean toggle. And it is all because of what is in the "0" position of the algorithm.
         // If the zero position is to stay un-lit, we don't have to worry about it ( like the Sample input ).
         // But my input has a LIT indicator for algorithm[0], so that causes trouble. I'm willing to bet EVERYONEs "real input" has a Lit indicator there because it's 
         // clearly meant to say "up yours" once you try this with the real deal. 
         // Anyway, since this only happens on alternating enhancements, we are using a boolean to say "Messed Up Time" based upon the state of algorithm[0] AND which 
         // alternative state this enhancement is in(every other one is messed up, so even/odd. Odd is bad). 
         bool ThisIterationIsGonnaBeWeird;
         for (int x = 0; x < thisManyTimes; x++)
         {
            // short circuit on whether or not our algorithm is messed up, first. because if it isn't a bad algorithm, the iteration flip-flop doesn't matter. 
            ThisIterationIsGonnaBeWeird = enhancementAlgorithm[0] == '#' && (x % 2) != 0;
            Enhance(ThisIterationIsGonnaBeWeird);
         }
         return LitPixels.Count();
      }

      private void Enhance(bool MessedUpIteration)
      {
         // as each pixel is processed, in order to avoid contaminating our source, store the results here. Save them off at the end.
         HashSet<PointDescription> resultingEnhancement = new HashSet<PointDescription>();

         // this changes on every enchancement iteration, so need to find the boundaries for this iteration
         int firstRow = LitPixels.Min(point => point.Y),
               firstColumn = LitPixels.Min(point => point.X),
               lastRow = LitPixels.Max(point => point.Y),
               lastColumn = LitPixels.Max(point => point.X);

         // since we must account for the boundaries' new edges, start one before the first row, and go one row beyond the last
         for (int rowCounter = firstRow - 1; rowCounter <= lastRow + 1; rowCounter++)
         {
            // so for each row, move through the columns. Again, start one column before our current image, and process to one column further
            for (int columnCounter = firstColumn - 1; columnCounter <= lastColumn + 1; columnCounter++)
            {
               // for the pixel at this row/column, determine the algorithm lookup based upon the pixels in the 9 spots immediately around it. 
               string binaryLookupBuilder = "";
               for (int examineRow = -1; examineRow <= 1; examineRow++) //iterate from the row immediately above our current row to the end below
               {
                  for (int examineColumn = -1; examineColumn <= 1; examineColumn++) // same with columns - immediately to the left, through to immediately to the right.
                  {
                     PointDescription examinePoint = new PointDescription { X = (columnCounter + examineColumn), Y = (rowCounter + examineRow) };
                     // check to see if the state of the infinite edges actually matter. If the spot we're looking at is in our source map for this iteration, 
                     //    we don't have to worry about the state of the infinite values
                     // This negative logic is funky, but aesthetically pleasing to me. 
                     bool ConsiderInfinity = !(examinePoint.Y >= firstRow && examinePoint.Y <= lastRow && examinePoint.X >= firstColumn && examinePoint.X <= lastColumn);
                     if (!MessedUpIteration || !ConsiderInfinity)
                     {
                        binaryLookupBuilder += LitPixels.Contains(examinePoint) ? '1' : '0';
                     }
                     else
                     {
                        // this IS a messed up iteration, and we're looking outside into the inifinite plane that is now funky-toggled. Which will always be 1.
                        binaryLookupBuilder += '1';
                     }

                  }
               }
               bool thisPixelIsLITson = enhancementAlgorithm[Convert.ToInt32(binaryLookupBuilder, 2)] == '#';
               if (thisPixelIsLITson)
                  resultingEnhancement.Add(new PointDescription { X = columnCounter, Y = rowCounter });
            }
         }
         // put the result back into the source for continued enhancement.
         LitPixels = resultingEnhancement;
      }
   }
}
