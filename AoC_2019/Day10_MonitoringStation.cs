using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2019
{
   class Day10_MonitoringStation : AoCodeModule
   {
      public Day10_MonitoringStation()
      {
         // all 5 sample files complete successfully..
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt"; 
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         //** > Result for Day10_MonitoringStation part 1: Best is 23,20 with 334 other asteroids detected. (Process: 6587.1541 ms)
         //** > Result for Day10_MonitoringStation part 2: The 200th asteroid to be vaporized is at  11,19 with a score of 1119 (Process: 14.5518 ms)
         ResetProcessTimer(true);// true also iterates the section marker
         object[,] asteroidMap = new object[inputFile[0].Length, inputFile.Count];
         int y = 0;
         Asteroid.Asteroids.Clear();
         Console.Clear();
         foreach (string processingLine in inputFile)
         {
            for (int x = 0; x < processingLine.Length; x++)
            {
               Console.SetCursorPosition(x, y);
               Console.Write(processingLine[x]);
               if (processingLine[x] == '#')
               {
                  Asteroid asteroid = new Asteroid(x, y);
                  asteroidMap[x, y] = asteroid;
               }
               else
               {
                  asteroidMap[x, y] = ".";
               }
            }
            y++;
         }
         Asteroid.ResetEverything();
         Asteroid.consoleColumn = asteroidMap.GetUpperBound(0) + 3;
         Asteroid.showDisplay = false;
         Asteroid.pauseOnDisplay = false;
         ConsoleColor original = Console.ForegroundColor;
         for (int x = 0; x <= asteroidMap.GetUpperBound(0); x++)
         {
            for (y = 0; y <= asteroidMap.GetUpperBound(1); y++)
            {
               if (asteroidMap[x, y] is Asteroid)
               {
                  if (Asteroid.showDisplay)
                  {
                     Console.SetCursorPosition(x, y);
                     Console.ForegroundColor = ConsoleColor.Red;
                     Console.Write('#');
                     Console.ForegroundColor = original;
                  }
                  Asteroid asteroid = (Asteroid)asteroidMap[x, y];
                  Asteroid.ResetAllMarkers();
                  Asteroid.Asteroids.Where(a => a != asteroid).ToList().ForEach(a => a.CheckLineOfSightTo(x, y));
                  asteroid.AsteroidsInLineOfSight = Asteroid.Asteroids.Count(a => a != asteroid && !a.isBlocked);
                  if (Asteroid.showDisplay)
                  {
                     Console.ForegroundColor = original;
                     Console.SetCursorPosition(x, y);
                     Console.Write('#');
                  }
               }
            }
         }
         Console.SetCursorPosition(0, asteroidMap.GetUpperBound(1) + 1);
         Console.WriteLine();
         AddResult("Best is " + Asteroid.BestLocation.X.ToString() + "," + Asteroid.BestLocation.Y.ToString() + " with " + Asteroid.MaximumAsteroidsViewable.ToString() + " other asteroids detected.");
         ResetProcessTimer(true);
         Asteroid.ResetEverything();
         Asteroid.Asteroids.Where(a => a != Asteroid.BestLocation).ToList().ForEach(a => a.CheckLineOfSightTo(Asteroid.BestLocation.X, Asteroid.BestLocation.Y));
         Asteroid.BestLocation.AsteroidsInLineOfSight = Asteroid.Asteroids.Count(a => a != Asteroid.BestLocation && !a.isBlocked);
         List<Asteroid> orderOfDestruction = Asteroid.Asteroids.Where(a => a != Asteroid.BestLocation && !a.isBlocked).OrderBy(a => a.slopeCalc).ToList();
         if (orderOfDestruction.Count < 200)
         {
            AddResult("This sample input doesn't have 200 asteroids already set.");

         }
         else
         {
            Asteroid Number200 = orderOfDestruction[199];
            AddResult("The 200th asteroid to be vaporized is at  " + Number200.X.ToString() + "," + Number200.Y.ToString() + " with a score of " + ((Number200.X * 100) + Number200.Y).ToString());
         }
         ResetProcessTimer(true);
      }
   }
   public class Asteroid
   {
      public static List<Asteroid> Asteroids = new List<Asteroid>();
      public static int MaximumAsteroidsViewable { get; set; }
      public static Asteroid BestLocation { get; set; }
      public bool isBlocked { get; set; }
      public int X { get; set; }
      public int Y { get; set; }
      private int _asteroidsInLineOfSight;
      public int AsteroidsInLineOfSight
      {
         get { return _asteroidsInLineOfSight; }
         set
         {
            _asteroidsInLineOfSight = value;
            if (value > MaximumAsteroidsViewable)
            {
               MaximumAsteroidsViewable = value;
               BestLocation = this;
            }
         }
      }
      public decimal slopeCalc { get; set; }
      public static int consoleColumn { get; set; }
      public static bool showDisplay { get; set; }
      public static bool pauseOnDisplay { get; set; }
      public Asteroid(int x, int y)
      {
         X = x; Y = y;
         isBlocked = false;
         AsteroidsInLineOfSight = 0;
         if (!Asteroids.Contains(this))
         {
            Asteroids.Add(this);
         }
         else
         {
            throw new Exception("HEY! Asteroid " + x.ToString() + ", " + y.ToString() + " is already in the list!");
         }
      }
      public void Reset()
      {
         isBlocked = false;
      }

      public void CheckLineOfSightTo(int x, int y)
      {
         decimal slopeCalcBase = x >= X && y <= Y ? 180M : (x >= X && y >= Y ? 270M : (x <= X && y <= Y ? 90M : 0M));
         if (isBlocked) return;
         ConsoleColor original = Console.ForegroundColor;
         if (showDisplay)
         {
            Console.SetCursorPosition(consoleColumn, 0);
            Console.Write("Examing Blue Asteroid at " + X.ToString() + "," + Y.ToString());
            Console.SetCursorPosition(consoleColumn, 1);
            Console.Write("Examining LOS to Red at " + x.ToString() + "," + y.ToString());
            Console.SetCursorPosition(X, Y);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write('#');
            Console.ForegroundColor = original;
         }
         // since it can't be the same X AND Y, this should fall through correctly
         // either it is on the same column(x), the same row(y), or... it's at a diagonal.
         if (x == X)
         {
            isBlocked = Asteroids.Any(a => a.X == X && a != this && a.Y != y && (y < Y ? (a.Y < Y && a.Y > y) : (a.Y > Y && a.Y < y)));
            if (showDisplay)
            {
               Console.SetCursorPosition(consoleColumn, 2);
               Console.Write("Target is in same COLUMN");
               Console.SetCursorPosition(consoleColumn, 7);
               Console.Write("This has blocked LOS: " + isBlocked.ToString());
               if (pauseOnDisplay)
               {
                  Console.ReadKey();
               }
               Console.SetCursorPosition(X, Y);
               Console.ForegroundColor = original;
               Console.Write('#');
            }
            if (!isBlocked)
            {
               slopeCalc = y<Y?180M:0M;
            }
         }
         else if (y == Y)
         {
            isBlocked = Asteroids.Any(a => a.Y == Y && a != this && a.X != x && (x < X ? (a.X < X && a.X > x) : (a.X > X && a.X < x)));
            if (showDisplay)
            {
               Console.SetCursorPosition(consoleColumn, 2);
               Console.Write("Target is in same ROW");
               Console.SetCursorPosition(consoleColumn, 7);
               Console.Write("This has blocked LOS: " + isBlocked.ToString());
               if (pauseOnDisplay)
               {
                  Console.ReadKey();
               }
               Console.SetCursorPosition(X, Y);
               Console.ForegroundColor = original;
               Console.Write('#');
            }
            if (!isBlocked)
            {
               slopeCalc = x < X ? 90M : 270M;
            }
         }
         else
         {
            
            int xDiff = x - X; // could be negative
            int yDiff = y - Y;
            int xFactor = xDiff > 0 ? -1 : 1;
            int yFactor = yDiff > 0 ? -1 : 1;
            if (showDisplay)
            {
               Console.SetCursorPosition(consoleColumn, 2);
               Console.Write("Target is at an ANGLE");
               Console.SetCursorPosition(consoleColumn, 3);
               Console.Write("Relative Coord Diff: " + xDiff.ToString() + "," + yDiff.ToString());
               Console.SetCursorPosition(consoleColumn, 4);
               Console.Write("With initial factors of : " + xFactor.ToString() + "/" + yFactor.ToString());
            }
            // if not on a direct 1 to 1 diagonal, we need to reduce it down to a prime factoring
            if (Math.Abs(xDiff) != Math.Abs(yDiff))
            {
               int minVal = Math.Min(Math.Abs(xDiff), Math.Abs(yDiff));
               int maxVal = Math.Max(Math.Abs(xDiff), Math.Abs(yDiff));
               decimal divisor = ((decimal)maxVal / (decimal)minVal);
               while (minVal != 1 && divisor == Decimal.Round(divisor))
               {
                  xDiff /= minVal; 
                  yDiff /= minVal; 
                  minVal = Math.Min(Math.Abs(xDiff), Math.Abs(yDiff));
                  maxVal = Math.Max(Math.Abs(xDiff), Math.Abs(yDiff));
                  divisor = ((decimal)maxVal / (decimal)minVal);
               }
               xFactor = -xDiff;
               yFactor = -yDiff;
               if (Math.Abs(xDiff) > 1 && Math.Abs(yDiff) > 1)
               {
                  for (int checker = Math.Min(Math.Abs(xDiff), Math.Abs(yDiff)); checker >=2; checker--)
                  {
                     decimal factoringX = (decimal)Math.Abs(xDiff) / (decimal)checker;
                     decimal factoringY = (decimal)Math.Abs(yDiff) / (decimal)checker;
                     if (Decimal.Round(factoringX) == factoringX && Decimal.Round(factoringY) == factoringY)
                     {
                        xFactor = -(xDiff / checker);
                        yFactor = -(yDiff / checker);
                        break;
                     }
                  }
               }

            }
            x += xFactor; y += yFactor;
            if (showDisplay)
            {
               Console.SetCursorPosition(consoleColumn, 5);
               Console.Write("Slope is " + xFactor.ToString() + "/" + yFactor.ToString());
            }
            while (!isBlocked && x != X && y != Y)
            {
               isBlocked = Asteroids.Any(a => a.X == x && a.Y == y);
               if (showDisplay)
               {
                  Console.SetCursorPosition(consoleColumn, 6);
                  Console.Write("Examining Location " + x.ToString() + "," + y.ToString());
                  Console.SetCursorPosition(x, y);
                  Console.ForegroundColor = ConsoleColor.Green;
                  Console.Write(isBlocked ? '#' : '.');
                  Console.ForegroundColor = original;
                  Console.SetCursorPosition(consoleColumn, 7);
                  Console.Write("This will block LOS: " + isBlocked.ToString());
                  if (pauseOnDisplay)
                  {
                     Console.ReadKey();
                  }
                  Console.SetCursorPosition(x, y);
                  Console.ForegroundColor = original;
                  Console.Write(isBlocked ? '#' : '.');
               }
               x += xFactor; y += yFactor;
            }
            if (showDisplay)
            {
               Console.SetCursorPosition(X, Y);
               Console.ForegroundColor = original;
               Console.Write('#');
            }
            if (!isBlocked)
            {
               slopeCalc = slopeCalcBase +  ( slopeCalcBase<270 ? (decimal)Math.Abs(xFactor) / (decimal)Math.Abs(yFactor) : (decimal)Math.Abs(yFactor) / (decimal)Math.Abs(xFactor));
            }
         }
         if (showDisplay)
         {
            for (int row = 0; row <= 8; row++)
            {
               Console.SetCursorPosition(consoleColumn, row); Console.Write("                                    ");
            }
            
         }
      }


      public static void ResetAllMarkers()
      {
         Asteroids.ForEach(a => a.Reset());
      }
      public static void ResetEverything()
      {
         Asteroids.ForEach(a => { a.AsteroidsInLineOfSight = 0; a.slopeCalc = 0; });
         ResetAllMarkers();
         MaximumAsteroidsViewable = 0;
      }

      #region EqualityStuff
      public bool Equals(Asteroid other)
      {
         return Equals(other, this);
      }

      public override bool Equals(object obj)
      {
         if (!(obj is Asteroid))
         {
            return false;
         }

         var objectToCompareWith = (Asteroid)obj;

         return objectToCompareWith.X == X && objectToCompareWith.Y == Y;

      }

      public override int GetHashCode()
      {
         var calculation = X + Y;
         return calculation.GetHashCode();
      }
      public static bool operator ==(Asteroid c1, Asteroid c2)
      {
         return c1.Equals(c2);
      }

      public static bool operator !=(Asteroid c1, Asteroid c2)
      {
         return !c1.Equals(c2);
      }
      #endregion
   }
}
