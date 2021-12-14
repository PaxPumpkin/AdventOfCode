using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day02_Dive : AoCodeModule
   {
      public Day02_Dive()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput(); 
         OutputFileReset(); 
      }
      public override void DoProcess()
      {
         //** > Result for Day02_Dive part 1: Multiplied result of current position: 1480518 (Process: 0.3397 ms)
         //** > Result for Day02_Dive part 2: Multiplied result of current position: 1282809906 (Process: 0.3302 ms)
         ResetProcessTimer(true);
         AddResult("Multiplied result of current position: " + new Submarine(inputFile, SolutionStyle.Part1).Solve());
         ResetProcessTimer(true);
         AddResult("Multiplied result of current position: " + new Submarine(inputFile, SolutionStyle.Part2).Solve());
      }
   }
   public class Submarine
   {
      private long Depth { get; set; }
      private long Horizontal { get; set; }
      private long Aim { get; set; }
      private SolutionStyle SolutionMethod { get; set; }
      private List<String> MovementCommands { get; set; }
      private string Solution { get { return (Depth * Horizontal).ToString(); } }

      public Submarine() : this (new List<string>()){}
      public Submarine(List<string> listOfCommands) : this (listOfCommands, SolutionStyle.Part1){}
      public Submarine(SolutionStyle methodology) : this (new List<string>(), methodology){}
      public Submarine(List<string> listOfCommands, SolutionStyle methodology)
      {
         MovementCommands = listOfCommands;
         ResetPosition(methodology);
      }
      private void ResetPosition(SolutionStyle methodology)
      {
         Depth = 0;
         Horizontal = 0;
         Aim = 0;
         SolutionMethod = methodology;
      }
      public string Solve()
      {
         ResetPosition(SolutionMethod);
         MovementCommands.ForEach(cmd => Move(cmd));
         return Solution;
      }
      private void Move(string command)
      {
         ApplyCommand(DecodeCommand(command));
      }
      private Command DecodeCommand(string command)
      {
         string[] commandPieces = command.StandardSplit();
         return new Command() 
         { 
            Direction = commandPieces[0].ToLower(), 
            Distance = long.Parse(commandPieces[1]) 
         };
      }
      private void ApplyCommand(Command movement)
      {
         switch (movement.Direction)
         {
            case "up":
               if (SolutionMethod == SolutionStyle.Part1) Depth -= movement.Distance; else Aim -= movement.Distance;
               break;
            case "forward":
               Horizontal += movement.Distance;
               if (SolutionMethod == SolutionStyle.Part2) Depth += Aim * movement.Distance;
               break;
            case "down":
               if (SolutionMethod == SolutionStyle.Part1) Depth += movement.Distance; else Aim += movement.Distance;
               break;
            default:
               throw new Exception("No match for command: " + movement.Direction);
         }
      }
   }
   public struct Command
   {
      public string Direction { get; set; }
      public long Distance { get; set; }
   }
   public enum SolutionStyle { Part1, Part2 }
}
