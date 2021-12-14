using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day11_DumboOctopuses : AoCodeModule
   {
      public Day11_DumboOctopuses()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         //** > Result for Day11_DumboOctopuses part 1: Total Flashes after 100 steps: 1647 (Process: 8.9734 ms)
         //** > Result for Day11_DumboOctopuses part 2: All Octopi flash simulaneously at step 348 (Process: 28.1196 ms)
         ResetProcessTimer(true);
         DumboOctopus.LoadOctopi(inputFile);
         AddResult("Total Flashes after 100 steps: " + DumboOctopus.DoCycles(100).ToString());

         ResetProcessTimer(true);
         DumboOctopus.LoadOctopi(inputFile);
         AddResult("All Octopi flash simulaneously at step " + DumboOctopus.FlashUntilSynchronized().ToString());
      }
   }
   public class DumboOctopus
   {
      public static List<DumboOctopus> DumboOctopi = new List<DumboOctopus>();
      public static long TotalFlashesGenerated { get; set; }
      public int X { get; set; }
      public int Y { get; set; }
      public int EnergyLevel { get; set; }
      public bool Flashed { get; set; }
      public DumboOctopus(int x, int y, int energyLevel)
      {
         X = x; Y = y; EnergyLevel = energyLevel;
         DumboOctopi.Add(this);
      }
      public int Flash()
      {
         Flashed = true;
         DumboOctopi.Where(octopus => octopus.X >= X - 1 && octopus.X <= X + 1 && octopus.Y >= Y - 1 && octopus.Y <= Y + 1)
            .ToList()
            .ForEach(octopus => octopus.EnergyLevel++);
         return 1;
      }
      public void Reset()
      {
         Flashed = false;
         EnergyLevel = 0;
      }
      public static void LoadOctopi(List<string> octopusInput)
      {
         DumboOctopi.Clear();
         TotalFlashesGenerated = 0;
         for (int y = 0; y < 10; y++)
            for (int x = 0; x < 10; x++)
               new DumboOctopus(x, y, int.Parse(octopusInput[y][x].ToString()));
      }
      public static void IncreaseAllOctopusEnergy()
      {
         DumboOctopi.ForEach(octopus => octopus.EnergyLevel++);
      }
      public static int FlashAllOverchargedOctopuses()
      {
         List<DumboOctopus> primedOctopi = DumboOctopi.Where(octopus => octopus.EnergyLevel > 9 && octopus.Flashed == false).ToList();
         int totalFlashed = 0;
         while (primedOctopi.Count > 0)
         {
            primedOctopi.ForEach(octopus => totalFlashed += octopus.Flash());
            primedOctopi = DumboOctopi.Where(octopus => octopus.EnergyLevel > 9 && octopus.Flashed == false).ToList();
         }
         TotalFlashesGenerated += totalFlashed;
         return totalFlashed;
      }
      public static long DoCycles(int numberOfCycles)
      {
         for (int x = 0; x < numberOfCycles; x++)
         {
            DumboOctopus.IncreaseAllOctopusEnergy();
            DumboOctopus.FlashAllOverchargedOctopuses();
            DumboOctopus.ResetAllFlashedOctopi();
         }
         return TotalFlashesGenerated;
      }
      public static int FlashUntilSynchronized()
      {
         int stepCounter = 1; 
         DumboOctopus.IncreaseAllOctopusEnergy();
         // With our input, it is implied that this will EVERYTUALLY stop. Otherwise, I'd bound it somehow.
         while (DumboOctopus.FlashAllOverchargedOctopuses() < DumboOctopus.DumboOctopi.Count)
         {
            stepCounter++;
            DumboOctopus.ResetAllFlashedOctopi();
            DumboOctopus.IncreaseAllOctopusEnergy();
         }
         return stepCounter;
      }
      public static void ResetAllFlashedOctopi()
      {
         DumboOctopi.Where(octopus => octopus.Flashed).ToList().ForEach(octopus => octopus.Reset());
      }
      public static void DrawOctopiMap(bool pauseOnDraw = false)
      {
         DumboOctopi.ForEach(octopus =>
         {
            Console.SetCursorPosition(octopus.X, octopus.Y);
            Console.Write(octopus.EnergyLevel > 9 ? 'X' : octopus.EnergyLevel.ToString()[0]);
         });
         if (pauseOnDraw) Console.ReadKey();
      }
   }
}
