using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day07_TheTreacheryOfWhales : AoCodeModule
   {
      public Day07_TheTreacheryOfWhales()
      {

         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         //** > Result for Day07_TheTreacheryOfWhales part 1: Position 354, fuel cost: 342730 (Process: 35.0555 ms)
         //** > Result for Day07_TheTreacheryOfWhales part 2: Position 476, Crab Engineering fuel cost: 92335207 (Process: 44.338 ms)
         inputFile = inputFile[0].Split(new char[] { ',' }).ToList<string>();
         ResetProcessTimer(true);// true also iterates the section marker
         List<CrabSub> crabSubs = new List<CrabSub>();
         foreach (string processingLine in inputFile)
         {
            crabSubs.Add(new CrabSub(processingLine));
         }
         CalculationResult results = CalculateFuelCosts(crabSubs, FuelCalculationStyle.Standard);
         AddResult("Position " + results.Position.ToString() + ", fuel cost: " + results.FuelCost.ToString());
         ResetProcessTimer(true);
         results = CalculateFuelCosts(crabSubs, FuelCalculationStyle.CrabEngineering);
         AddResult("Position " + results.Position.ToString() + ", Crab Engineering fuel cost: " + results.FuelCost.ToString());
         ResetProcessTimer(true);
      }
      public enum FuelCalculationStyle { Standard, CrabEngineering }
      public struct CalculationResult
      {
         public int Position;
         public long FuelCost;
      }
      public CalculationResult CalculateFuelCosts(List<CrabSub> crabSubs, FuelCalculationStyle style)
      {
         //This is pretty brute force. There's probably some magic formula to do it. 
         var crabGroups = crabSubs.GroupBy(x => x.HorizontalPosition).OrderByDescending(x => x.Count());

         long minimumFuel = long.MaxValue;
         long fuelCheck = 0;
         int targetPosition = 0;
         foreach (var grouping in crabGroups) // check for narrowing by smaller increments in already existing target locations where fuel cost may be zero for some.
         {
            crabSubs.ForEach(x => x.TargetPosition = grouping.Key);
            fuelCheck = crabSubs.Sum(x => style == FuelCalculationStyle.Standard ? x.FuelCost : x.CrabEngineeringFuelCost);
            if (fuelCheck < minimumFuel) { minimumFuel = fuelCheck; targetPosition = grouping.Key; }
         }
         int scanRange = 5;
         int starter = Math.Max(0, targetPosition - scanRange);
         for (int spaces = starter; spaces <= targetPosition + scanRange; spaces++) // work with a quick check around the cheapest area. Is +/-5 big enough?
         {
            crabSubs.ForEach(x => x.TargetPosition = spaces);
            fuelCheck = crabSubs.Sum(x => style == FuelCalculationStyle.Standard ? x.FuelCost : x.CrabEngineeringFuelCost);
            if (fuelCheck < minimumFuel) { minimumFuel = fuelCheck; targetPosition = spaces; }
         }
         return new CalculationResult { Position = targetPosition, FuelCost = minimumFuel };
      }
   }
   public class CrabSub
   {
      public int HorizontalPosition { get; set; }
      public int TargetPosition { get; set; }
      public int FuelCost { get { return Math.Abs(HorizontalPosition - TargetPosition); } }
      public int CrabEngineeringFuelCost { get { int difference = FuelCost; return ((difference * (difference + 1)) / 2); } }
      public CrabSub(string Position)
      {
         HorizontalPosition = int.Parse(Position);
      }
   }
}
