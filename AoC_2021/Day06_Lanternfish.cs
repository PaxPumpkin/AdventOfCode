using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day06_Lanternfish : AoCodeModule
   {
      public Day06_Lanternfish()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput(); 
         OutputFileReset(); 
      }
      public override void DoProcess()
      {
         //** > Result for Day06_Lanternfish part 1: After 80 days, using buckets, there are 387413 (Process: 0.083 ms)
         //** > Result for Day06_Lanternfish part 2: After 256 days, using buckets, there are 1738377086345 (Process: 0.0074 ms)
         ResetProcessTimer(true);
         List<string> inputItems = inputFile[0].Split(new char[] { ',' }).ToList<string>(); // comma delimited single line input
         ulong[] fishBuckets = new ulong[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // all buckets empty
         foreach (string cycle in inputItems)
         {
            fishBuckets[int.Parse(cycle)]++; // add each fish to the appropriate bucket
         }
         CycleFish(fishBuckets, 80);
         AddResult("After 80 days, using buckets, there are " + fishBuckets.Sum().ToString());
         ResetProcessTimer(true);
         // do 176 more times to make 256 total days for part 2
         CycleFish(fishBuckets, 176);
         AddResult("After 256 days, using buckets, there are " + fishBuckets.Sum().ToString());
         ResetProcessTimer(true);
      }
      public void CycleFish(ulong[] fishBuckets, int iterations)
      {
         for (int x = 0; x < iterations; x++)
         {
            // anything in the zero bucket each cycle spawns into the 8 bucket, and moves back to the six bucket.
            ulong spawnedThisCycle = fishBuckets[0];
            // move them all down a bucket (bucket = bucket ahead). Except the 8 bucket. We leave it alone for now because there is no 9 bucket.
            for (int i = 0; i < 8; i++)
            {
               fishBuckets[i] = fishBuckets[i + 1];
            }
            // bucket 8 is now set to however many got spawned from the start of the cycle.
            fishBuckets[8] = spawnedThisCycle;
            // put all the ones that were in zero back into 6, but don't overwrite. Because 6 can now have x number that cycled down from 7. Add them to whatever got moved in.
            fishBuckets[6] += spawnedThisCycle;
         }
      }
   }
}
