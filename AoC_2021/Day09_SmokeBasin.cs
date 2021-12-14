using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day09_SmokeBasin : AoCodeModule
   {
      public Day09_SmokeBasin()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput(); //base class method
         OutputFileReset();
      }
      public override void DoProcess()
      {
         //** > Result for Day09_SmokeBasin part 1: Sum of risk levels for all low points: 502 (Process: 0.8162 ms)
         //** > Result for Day09_SmokeBasin part 2: Product of three largest basins: 1330560 (Process: 22.5862 ms)
         ResetProcessTimer(true);
         int[,] heightMap = new int[inputFile[0].Length, inputFile.Count];
         int y = 0;
         foreach (string processingLine in inputFile)
         {
            for (int x = 0; x < processingLine.Length; x++)
            {
               heightMap[x, y] = processingLine[x] - 48;

            }
            y++;
         }
         List<int> lowPoints = new List<int>();
         List<PointDescription> basinLowPoints = new List<PointDescription>();
         bool lowPoint = false;
         int pointValue = 0;
         for (y = 0; y <= heightMap.GetUpperBound(1); y++)
         {
            for (int x = 0; x <= heightMap.GetUpperBound(0); x++)
            {
               lowPoint = false;
               pointValue = heightMap[x, y];
               if (x == 0)
               {
                  if (y == 0)
                  {
                     lowPoint = pointValue < heightMap[x + 1, y] && pointValue < heightMap[x, y + 1];
                  }
                  else if (y == heightMap.GetUpperBound(1))
                  {
                     lowPoint = pointValue < heightMap[x + 1, y] && pointValue < heightMap[x, y - 1];
                  }
                  else
                  {
                     lowPoint = pointValue < heightMap[x + 1, y] && pointValue < heightMap[x, y - 1] && pointValue < heightMap[x, y + 1];
                  }
               }
               else if (y == 0)
               {
                  if (x == heightMap.GetUpperBound(0))
                  {
                     lowPoint = pointValue < heightMap[x - 1, y] && pointValue < heightMap[x, y + 1];
                  }
                  else
                  {
                     lowPoint = pointValue < heightMap[x - 1, y] && pointValue < heightMap[x, y + 1] && pointValue < heightMap[x + 1, y];
                  }
               }
               else if (y == heightMap.GetUpperBound(1))
               {
                  if (x == heightMap.GetUpperBound(0))
                  {
                     lowPoint = pointValue < heightMap[x - 1, y] && pointValue < heightMap[x, y - 1];
                  }
                  else
                  {
                     lowPoint = pointValue < heightMap[x - 1, y] && pointValue < heightMap[x, y - 1] && pointValue < heightMap[x + 1, y];
                  }
               }
               else if (x == heightMap.GetUpperBound(0))
               {
                  lowPoint = pointValue < heightMap[x - 1, y] && pointValue < heightMap[x, y + 1] && pointValue < heightMap[x, y - 1];
               }
               else
               {
                  lowPoint = pointValue < heightMap[x - 1, y] && pointValue < heightMap[x + 1, y] && pointValue < heightMap[x, y + 1] && pointValue < heightMap[x, y - 1];
               }
               if (lowPoint)
               {
                  lowPoints.Add(pointValue + 1);
                  basinLowPoints.Add(new PointDescription { X = x, Y = y });
               }
            }
         }
         AddResult("Sum of risk levels for all low points: " + lowPoints.Sum().ToString());
         ResetProcessTimer(true);
         List<int> basinSize = new List<int>();
         basinLowPoints.ForEach(bottom => basinSize.Add((new BasinFill(bottom,heightMap)).Fill()));

         long product = 1;
         basinSize.OrderByDescending(x => x).ToList().Take(3).ToList().ForEach(x => product *= x);
         AddResult("Product of three largest basins: " + product.ToString());
         ResetProcessTimer(true);
      }
   }
   public class BasinFill
   {
      List<PointDescription> examinedPoints = new List<PointDescription>();
      PointDescription basinLowPoint;
      int[,] heightMap;
      public BasinFill(PointDescription lowPoint, int[,] mapping)
      {
         //examinedPoints.Add(lowPoint);
         basinLowPoint = lowPoint;
         heightMap = mapping;
      }
      public int Fill()
      {
         Fill(basinLowPoint);
         return examinedPoints.Count();
      }
      public void Fill(PointDescription fromPoint)
      {
         if (!examinedPoints.Contains(fromPoint))
         {
            examinedPoints.Add(fromPoint);

            List<PointDescription> surroundingPoints = new List<PointDescription>();
            if (fromPoint.X > 0 && heightMap[fromPoint.X - 1, fromPoint.Y] != 9)
            {
               surroundingPoints.Add(new PointDescription { X = fromPoint.X - 1, Y = fromPoint.Y });
            }
            if (fromPoint.X < heightMap.GetUpperBound(0) && heightMap[fromPoint.X + 1, fromPoint.Y] != 9)
            {
               surroundingPoints.Add(new PointDescription { X = fromPoint.X + 1, Y = fromPoint.Y });
            }
            if (fromPoint.Y > 0 && heightMap[fromPoint.X, fromPoint.Y-1] != 9)
            {
               surroundingPoints.Add(new PointDescription { X = fromPoint.X, Y = fromPoint.Y-1 });
            }
            if (fromPoint.Y < heightMap.GetUpperBound(1) && heightMap[fromPoint.X, fromPoint.Y+1] != 9)
            {
               surroundingPoints.Add(new PointDescription { X = fromPoint.X, Y = fromPoint.Y +1 });
            }
            surroundingPoints.ForEach(sPoint => Fill(sPoint));
         }
      }
   }
}
