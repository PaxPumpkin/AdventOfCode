using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day13_TransparentOrigami : AoCodeModule
   {
      public Day13_TransparentOrigami()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         //** > Result for Day13_TransparentOrigami part 1: After one fold there are 610 dots (Process: 21.7785 ms)
         //** > Result for Day13_TransparentOrigami part 2: See Display for part 2 (Process: 87.2589 ms)
         // Display shows PZFJHRFZ
         ResetProcessTimer(true);
         TransparentPaper.Reset(); // clear all static lists to allow multiple runs
         foreach (string processingLine in inputFile)
         {
            TransparentPaper.ParseInputLine(processingLine);
         }
         TransparentPaper.FoldOnce(); // part 1, fold once to count resulting dots. 
         AddResult("After one fold there are " + TransparentPaper.CurrentDots + " dots."); 
         ResetProcessTimer(true);
         TransparentPaper.FinishFolding(); // part 2, do all remaining folds.
         TransparentPaper.Display();
         AddResult("See Display for part 2. Expected: PZFJHRFZ"); // added result to output for future reference, if i ever look at this again.
         ResetProcessTimer(true);
      }
   }

   public class TransparentPaper
   {
      public struct Fold // this struct is pretty useless beyond the context of TransparentPaper, so it's internal.
      {
         public int Boundary;
         public int Index;
      }
      private static List<PointDescription> MarkedPoints = new List<PointDescription>();
      private static Queue<Fold> Folds = new Queue<Fold>();
      public static string CurrentDots { get { return MarkedPoints.Count.ToString(); } }

      public TransparentPaper()
      {
         // never actually instatiated. Everything is static.
      }
      public static void Reset()
      {
         MarkedPoints.Clear();
         Folds.Clear();
      }
      public static void ParseInputLine(string inputLine)
      {
         if (inputLine.Contains(","))
         {
            ParsePoint(inputLine);
         }
         else if(inputLine.Contains("fold"))
         {
            ParseFold(inputLine);
         }
      }
      private static void ParsePoint(string inputLine)
      {
         string[] parsedLine = inputLine.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
         InstantiatePoint(int.Parse(parsedLine[0]), int.Parse(parsedLine[1]));
      }
      private static void ParseFold(string inputLine)
      {
         string[] parsedLine = inputLine.Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
         Folds.Enqueue(new Fold { Boundary = parsedLine[2] == "x" ? 0 : 1, Index = int.Parse(parsedLine[3]) });
      }
      public static void InstantiatePoint(int x, int y)
      {
         PointDescription newPoint = new PointDescription { X = x, Y = y };
         if (!MarkedPoints.Contains(newPoint)) MarkedPoints.Add(newPoint);
         else throw new Exception("Tried to add the same point twice!");
      }
      private static void MarkPoint(PointDescription point)
      {
         // When folding, points can overlap, so no errors if already marked, just don't add twice to avoid counting errors (even though display will still work even with duplicates)
         if (!MarkedPoints.Contains(point))
         {
            MarkedPoints.Add(point);
         }
      }
      public static void FoldOnce()
      {
         if (Folds.Count > 0) DoFold(Folds.Dequeue());
      }
      public static void FinishFolding()
      {
         while (Folds.Count > 0) DoFold(Folds.Dequeue());
      }
      private static void DoFold(Fold instruction)
      {
         List<PointDescription> foldedPoints = MarkedPoints.Where(point => instruction.Boundary == 0 ? point.X > instruction.Index : point.Y > instruction.Index).ToList();
         MarkedPoints.RemoveAll(point => foldedPoints.Contains(point));
         foldedPoints.ForEach(point => MarkPoint(new PointDescription { X = instruction.Boundary == 0 ? NewFoldedIndex(point.X, instruction.Index) : point.X, Y = instruction.Boundary == 0 ? point.Y : NewFoldedIndex(point.Y, instruction.Index) }));
      }
      private static int NewFoldedIndex(int pointIndex, int foldIndex)
      {
         return pointIndex - ((pointIndex - foldIndex) * 2);
      }
      public static void Display()
      {
         Console.Clear();
         ConsoleColor originalFG = Console.ForegroundColor; //save for after
         ConsoleColor originalBG = Console.BackgroundColor;
         Console.ForegroundColor = Console.BackgroundColor = ConsoleColor.Red; // make pretty, and solid blocks
         foreach (PointDescription point in MarkedPoints)
         {
            Console.SetCursorPosition(point.X, point.Y);
            Console.Write('#');
         }
         Console.ForegroundColor = originalFG;//put back
         Console.BackgroundColor = originalBG;
         Console.SetCursorPosition(0, MarkedPoints.Max(point => point.Y) + 1); // set appropriate position for result output
      }
   }
}
