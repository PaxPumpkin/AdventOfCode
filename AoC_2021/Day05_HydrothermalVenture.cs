using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day05_HydrothermalVenture : AoCodeModule
   {
      public Day05_HydrothermalVenture()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         // using List<HydrothermalVentSpot> :
         //** > Result for Day05_HydrothermalVenture part 1: Points where 2 or more lines overlap: 5167 (Process: 71145.5224 ms)
         //** > Result for Day05_HydrothermalVenture part 2: Points where 2 or more lines overlap: 17604 (Process: 120534.2569 ms)
         // Refactored with Dictionary:
         //** > Result for Day05_HydrothermalVenture part 1: Line Parsing Done. (Process: 0.7553 ms)
         //** > Result for Day05_HydrothermalVenture part 1: Horizontal Vertical Lines - Points where 2 or more lines overlap: 5167 (Process: 592.2007 ms)
         //** > Result for Day05_HydrothermalVenture part 2: Added Diagonals - Points where 2 or more lines overlap: 17604 (Process: 1130.3594 ms)

         ResetProcessTimer(true);// true also iterates the section marker
         Dictionary<PointDescription, HydrothermalVentSpot> hydrothermalVents = new Dictionary<PointDescription, HydrothermalVentSpot>();
         List<int[]> horizontalVerticals = new List<int[]>();
         List<int[]> diagonals = new List<int[]>();
         foreach (string processingLine in inputFile)
         {
            int[] lineDefinition = ParseVentVector(processingLine);
            if (lineDefinition[0] == lineDefinition[2] || lineDefinition[1] == lineDefinition[3]) //part 1 - only horizontal or vertical lines
            {
               horizontalVerticals.Add(lineDefinition); // for part 1
            }
            else
            { 
               diagonals.Add(lineDefinition); // for part 2
            }
         }
         AddResult("Line Parsing Done.");
         ResetProcessTimer();

         //Part 1, Horizontal and Vertical only.
         horizontalVerticals.ForEach(x => AddVents(hydrothermalVents, x));
         int atLeastTwoOverlaps = hydrothermalVents.Count(x => x.Value.LineIntersection >= 2);
         AddResult("Horizontal Vertical Lines - Points where 2 or more lines overlap: " + atLeastTwoOverlaps.ToString());
         ResetProcessTimer(true);

         // Part 2, add diagonals.
         diagonals.ForEach(x => AddVents(hydrothermalVents, x));
         atLeastTwoOverlaps = hydrothermalVents.Count(x => x.Value.LineIntersection >= 2);
         AddResult("Added Diagonals - Points where 2 or more lines overlap: " + atLeastTwoOverlaps.ToString());
         ResetProcessTimer();
      }
      public int[] ParseVentVector(string ventDefinition)
      {
         ventDefinition = ventDefinition.Replace(" -> ", ",");
         List<string> points = ventDefinition.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
         List<int> ventVectors = new List<int>();
         points.ForEach(x => ventVectors.Add(int.Parse(x)));
         return ventVectors.ToArray();
      }
 
      public void AddVents(Dictionary<PointDescription, HydrothermalVentSpot> spotList, int[] lineDefinition)
      {
         if (lineDefinition[0] == lineDefinition[2])
         {
            int OurX = lineDefinition[0];
            int startingPoint = lineDefinition[1], endingPoint = lineDefinition[3];
            if (lineDefinition[1] > lineDefinition[3])
            {
               startingPoint = lineDefinition[3];
               endingPoint = lineDefinition[1];
            }
            for (int OurY = startingPoint; OurY <= endingPoint; OurY++)
            {
               AddOrIncrementSpot(spotList, new PointDescription { X = OurX, Y = OurY });
            }
         }
         else if (lineDefinition[1] == lineDefinition[3])
         {
            int OurY = lineDefinition[1];
            int startingPoint = lineDefinition[0], endingPoint = lineDefinition[2];
            if (lineDefinition[0] > lineDefinition[2])
            {
               startingPoint = lineDefinition[2];
               endingPoint = lineDefinition[0];
            }
            for (int OurX = startingPoint; OurX <= endingPoint; OurX++)
            {
               AddOrIncrementSpot(spotList, new PointDescription { X = OurX, Y = OurY });
            }
         }
         else
         {
            int startingx = lineDefinition[0], endingx = lineDefinition[2];
            int xincrementor = startingx < endingx ? 1 : -1;
            int yincrementor = lineDefinition[1] < lineDefinition[3] ? 1 : -1;
            int OurY = lineDefinition[1];
            for (int OurX = startingx; (xincrementor > 0 ? OurX <= endingx : OurX >= endingx); OurX += xincrementor)
            {
               AddOrIncrementSpot(spotList, new PointDescription { X = OurX, Y = OurY });
               OurY += yincrementor;
            }
         }
      }
      public void AddOrIncrementSpot(Dictionary<PointDescription, HydrothermalVentSpot> spotList, PointDescription point)
      {
         spotList.TryGetValue(point, out HydrothermalVentSpot spot);
         if (spot != null)
         {
            spot.IncrementLineIntersection();
         }
         else
         {
            spotList.Add(point, new HydrothermalVentSpot(point));
         }
      }
   }
   public class HydrothermalVentSpot
   {
      public int X { get; set; }
      public int Y { get; set; }
      public int LineIntersection { get; set; }
      public HydrothermalVentSpot(PointDescription point)
      {
         X = point.X; Y = point.Y; LineIntersection = 1;
      }
      public void IncrementLineIntersection()
      {
         LineIntersection++;
      }
   }
   public struct PointDescription // hashes better for dictionary than the straight int[] which kept messing up.
   {
      public int X { get; set; }
      public int Y { get; set; }


      public bool Equals(PointDescription other)
      {
         return Equals(other, this);
      }

      public override bool Equals(object obj)
      {
         if (!(obj is PointDescription))
         {
            return false;
         }

         var objectToCompareWith = (PointDescription)obj;

         return objectToCompareWith.X == X && objectToCompareWith.Y == Y;

      }

      public override int GetHashCode()
      {
         var calculation = X + Y;
         return calculation.GetHashCode();
      }
      public static bool operator ==(PointDescription c1, PointDescription c2)
      {
         return c1.Equals(c2);
      }

      public static bool operator !=(PointDescription c1, PointDescription c2)
      {
         return !c1.Equals(c2);
      }
   }
}
