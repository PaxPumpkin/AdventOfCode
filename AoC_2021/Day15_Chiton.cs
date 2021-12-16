using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AoC_2021
{
   class Day15_Chiton : AoCodeModule
   {
      public Day15_Chiton()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         //** > Result for Day15_Chiton part 1: Lowest Risk Path is 696 (Process: 172.4869 ms)
         //** > Result for Day15_Chiton part 2: Lowest Risk Path is 2952 (Process: 16166.9035 ms)
         ResetProcessTimer(true);
         CavernSpot.LoadInput(inputFile);
         AddResult("Lowest Risk Path is " + CavernSpot.Answer);
         ResetProcessTimer(true);
         CavernSpot.Widify(inputFile, 5); // can probably reuse this instead of the standard LoadInput Wide-ification Factor of 5
         AddResult("Lowest Risk Path is " + CavernSpot.Answer);
         ResetProcessTimer(true);
      }
   }
   public class CavernSpot
   {
      public int X { get; set; }
      public int Y { get; set; }
      public int RiskCost { get; set; }
      public int LowestCostFromHere { get; set; }
      public int ManhattanDistance { get { return X + Y; } }
      private List<CavernSpot> MyNeighbors = new List<CavernSpot>();

      public static Dictionary<PointDescription, CavernSpot> CavernSpotDictionary = new Dictionary<PointDescription, CavernSpot>();
      public static int HighestX { get; set; }
      public static int HighestY { get; set; }
      public static string Answer { get { return CavernSpotDictionary[new PointDescription { X = 0, Y = 0 }].LowestCostFromHere.ToString(); } }

      public static Queue<CavernSpot> queuedCostResolutions = new Queue<CavernSpot>();
      public CavernSpot(int x, int y, int risk)
      {
         X = x; Y = y; RiskCost = risk;
         CavernSpotDictionary.Add(new PointDescription { X=x, Y=y }, this);
      }
      public void LoadNeighbors()
      {
         MyNeighbors.Clear();
         SetSpot(X, Y - 1);
         SetSpot(X, Y + 1);
         SetSpot(X + 1, Y);
         SetSpot(X - 1, Y);
      }

      private void SetSpot(int x, int y)
      {
         PointDescription toGet = new PointDescription { X = x, Y = y };
         if (CavernSpotDictionary.ContainsKey(toGet))
            MyNeighbors.Add(CavernSpotDictionary[toGet]);
      }
      public void ResolveCost()
      {
         if (X == HighestX && Y == HighestY) // end point, the cost is the stated value. 
            LowestCostFromHere = RiskCost;
         else
         {
            int lowestCost;
            LowestCostFromHere = int.MaxValue;
            //iterate through neighbors and find the one with the lowest cost to reach the end. 
            // if they are unresolved values, save for resolving after. 
            MyNeighbors.OrderByDescending(spot => spot.ManhattanDistance).ToList().ForEach(spot =>
             {
                if (spot.LowestCostFromHere == 0) //not already solved. 
                {
                   queuedCostResolutions.Enqueue(spot); // because recursion is deadly at 250,000 feet.
                }
                else
                { // of the ones that actually have a solved value, get the lowest one. Add spots' own cost. 
                   // the "cost" of start is always zero. 
                   lowestCost = spot.LowestCostFromHere + ((X == 0 && Y == 0) ? 0 : RiskCost);
                   LowestCostFromHere = Math.Min(LowestCostFromHere, lowestCost);
                }
             });
         }
      }

      public static void LoadInput(List<string> inputFile) // refactor into Wide-ifier. With default loop factor of 1
      {
         CavernSpot.CavernSpotDictionary.Clear();
         HighestX = inputFile[0].Length - 1;
         HighestY = inputFile.Count - 1;
         for (int y = 0; y <= HighestY; y++)
            for (int x = 0; x <= HighestX; x++)
               new CavernSpot(x, y, int.Parse(inputFile[y][x].ToString()));

         FillConnectors();
         LoadCosts();
      }
      public static void Widify(List<string> inputFile, int WidificationFactor = 1) //Wide-ification by PaxTech™
      {
         CavernSpot.CavernSpotDictionary.Clear();
         HighestX = (inputFile[0].Length * 5) - 1;
         HighestY = (inputFile.Count * 5) - 1;
         int xInterval = inputFile[0].Length;
         int yInterval = inputFile.Count;
         for (int y = 0; y <= inputFile.Count - 1; y++)
            for (int x = 0; x <= inputFile[0].Length - 1; x++)
               for (int ywidify = 0; ywidify < WidificationFactor; ywidify++)
                  for (int widify = 0; widify < WidificationFactor; widify++)
                  {
                     int inputNumber = int.Parse(inputFile[y][x].ToString());
                     int widification = widify + ywidify;
                     int widifyNumber = (inputNumber + widification) % 9;
                     widifyNumber = widifyNumber == 0 ? 9 : widifyNumber;
                     int xModification = x + (widify * xInterval);
                     int yModification = y + (ywidify * yInterval);
                     new CavernSpot(xModification, yModification, widifyNumber);
                  }

         FillConnectors();
         LoadCosts();
      }
      private static void FillConnectors()
      {
         CavernSpotDictionary.Keys.ToList().ForEach(key => CavernSpotDictionary[key].LoadNeighbors());
      }
      private static void LoadCosts()
      {
         int leastCost, lastCost;
         lastCost = 0;
         // first time only queues empty spaces and sets basic info.
         CavernSpotDictionary.OrderByDescending(spot => spot.Value.ManhattanDistance).ToList().ForEach(spot => spot.Value.ResolveCost()); // calls to ResolveCost can generate queued entries.
         leastCost = CavernSpotDictionary[new PointDescription { X = 0, Y = 0 }].LowestCostFromHere;
         while (queuedCostResolutions.Count > 0 || !(leastCost == lastCost)) // if queued, or non-stablized least cost. 
         {
            lastCost = leastCost;
            while (queuedCostResolutions.Count > 0)
            {
               queuedCostResolutions.Dequeue().ResolveCost();
            }
            CavernSpotDictionary.OrderByDescending(spot => spot.Value.ManhattanDistance).ToList().ForEach(spot => spot.Value.ResolveCost()); //possible queued resolutions
            leastCost = CavernSpotDictionary[new PointDescription { X = 0, Y = 0 }].LowestCostFromHere;
         }
      }
   }
}
