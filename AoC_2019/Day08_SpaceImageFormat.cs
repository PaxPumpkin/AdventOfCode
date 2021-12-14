using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2019
{
   class Day08_SpaceImageFormat : AoCodeModule
   {
      public Day08_SpaceImageFormat()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput(); 
         OutputFileReset(); 
      }
      public override void DoProcess()
      {
         /*
          * Image Output:
            X    XXXX   XX X  X  XX
            X    X       X X X  X  X
            X    XXX     X XX   X
            X    X       X X X  X
            X    X    X  X X X  X  X
            XXXX XXXX  XX  X  X  XX

            **************************************************************
            //** > Result for Day08_SpaceImageFormat part 1: Least Zeroes layer, One-count times Two-count is 1340 (Process: 0.2604 ms)
            //** > Result for Day08_SpaceImageFormat part 2: Done (Process: 15.83 ms)
            (Total including instantiation: 16.1054 ms)
            **************************************************************
            **************************************************************
         */

         ResetProcessTimer(true);// true also iterates the section marker
         string allImageBits = inputFile[0];
         int width = 25, height = 6;
         int layerBitCount = width * height;
         int layerPointer = 0;
         List<string> layers = new List<string>();
         while (layerPointer + layerBitCount <= allImageBits.Length)
         {
            layers.Add(allImageBits.Substring(layerPointer, layerBitCount));
            layerPointer += layerBitCount;
         }
         int zeroBitCount = int.MaxValue;
         string leastZeroesLayer = "";
         layers.ForEach(x => { int zc = x.Count(c => c == '0'); if (zc < zeroBitCount) { zeroBitCount = zc; leastZeroesLayer = x; } });
         AddResult("Least Zeroes layer, One-count times Two-count is " + (leastZeroesLayer.Count(c => c == '1') * leastZeroesLayer.Count(c => c == '2')).ToString());
         ResetProcessTimer(true);
         char[] resultingImageStream = new string('2', layerBitCount).ToCharArray();
         layers.ForEach(
            x =>
            {
               for (int i = 0; i < layerBitCount; i++)
               {
                  resultingImageStream[i] = resultingImageStream[i] == '2' ? x[i] : resultingImageStream[i];
               }
            });
         Console.WriteLine("Image Output:");
         char block = 'X'; //  (char)178; // 178 is supposed to be "block", but it looks wrong... a superscript 2?
         for (int row = 0; row < height; row++)
         {
            for (int x = 0; x < width; x++)
            {
               Console.Write(resultingImageStream[(row * width) + x]=='1'? block : ' ');
            }
            Console.WriteLine("");
         }
         AddResult("Done");
         ResetProcessTimer(true);
      }
   }
}
