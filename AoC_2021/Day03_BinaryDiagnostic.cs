using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day03_BinaryDiagnostic : AoCodeModule
   {
      public Day03_BinaryDiagnostic()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset(); 
      }
      public override void DoProcess()
      {
         //** > Result for Day03_BinaryDiagnostic part 1: Power Consumption = 3885894 (Process: 0.2814 ms)
         //** > Result for Day03_BinaryDiagnostic part 2: Life Support Rating = 4375225 (Process: 0.8149 ms)
         ResetProcessTimer(true);// true also iterates the section marker
         AnalyzeList(inputFile, out int gamma, out int epsilon);
         AddResult("Power Consumption = " + (gamma*epsilon).ToString());
         ResetProcessTimer(true);
         int oxygenGenerationRating = Convert.ToInt32(ProcessListMatching(inputFile,Commonality.MostCommon), 2);
         int co2ScrubberRating = Convert.ToInt32(ProcessListMatching(inputFile, Commonality.LeastCommon), 2);
         AddResult("Life Support Rating = " + (oxygenGenerationRating * co2ScrubberRating).ToString());
         ResetProcessTimer(true);
      }
      public void AnalyzeList(List<string> binaryOutput, out int MaxMatching, out int MinMatching)
      {
         int binaryPositions = binaryOutput.Count > 0 ? binaryOutput[0].Length : 0;
         int ones, zeroes;
         string max = "", min = "";
         for (int x = 0; x < binaryPositions; x++)
         {
            ones = 0; zeroes = 0;
            inputFile.ForEach(binary => { if (binary[x] == '1') ones++; else zeroes++; });
            if (Math.Max(ones, zeroes) == ones) { max += "1"; min += "0"; } else { max += "0"; min += "1"; }
         }
         MaxMatching = Convert.ToInt32(max,2); MinMatching = Convert.ToInt32(min,2);
      }
      public enum Commonality { MostCommon, LeastCommon }
      public string ProcessListMatching(List<string> startingList, Commonality processingType)
      {
         List<string> matches = startingList.ToList();
         int matchPointer = 0, ones, zeroes;
         string matchString = "";
         while (matches.Count > 1)
         {
            ones = 0; zeroes = 0;
            matches.ForEach(binary => { if (binary[matchPointer] == '1') ones++; else zeroes++; });
            matchString += (ones == zeroes ? (processingType==Commonality.MostCommon?"1":"0") : ((Math.Max(ones, zeroes) == ones) ? (processingType == Commonality.MostCommon?"1":"0") : (processingType == Commonality.MostCommon ? "0":"1")));
            matchPointer++;
            matches = matches.Where(x => x.StartsWith(matchString)).ToList();
         }
         return matches[0];
      }
   }
}
