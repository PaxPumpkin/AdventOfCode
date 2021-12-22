using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day19_BeaconScanner : AoCodeModule
   {
      public Day19_BeaconScanner()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput(); 
         OutputFileReset(); 
      }
      public override void DoProcess()
      {
         
         ResetProcessTimer(true);
         foreach (string processingLine in inputFile)
         {

         }
         AddResult("You Gotta be Kidding Me. Come Back to this. I got a job.");

      }
   }
}
