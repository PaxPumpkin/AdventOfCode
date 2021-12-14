using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day10_SyntaxScoring : AoCodeModule
   {
      public Day10_SyntaxScoring()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         // refactored scores
         //** > Result for Day10_SyntaxScoring part 1: Score for corrupted lines is 392043 (Process: 0.496 ms)
         //** > Result for Day10_SyntaxScoring part 2: Score for completed lines is 1605968119 (Process: 0.0334 ms)
         ResetProcessTimer(true);
         long totalScore = 0;
         List<long> completionScores = new List<long>();
         foreach (string processingLine in inputFile)
         {
            SyntaxChecker syntaxChecker = new SyntaxChecker(processingLine);
            totalScore += syntaxChecker.CheckAndScoreCorruption();
            if (syntaxChecker.IsIncomplete) completionScores.Add(syntaxChecker.CompleteAndScore());
         }
         AddResult("Score for corrupted lines is " + totalScore.ToString());
         ResetProcessTimer(true);

         AddResult("Score for completed lines is " + completionScores.OrderBy(x => x).ToList()[((completionScores.Count - 1) / 2)].ToString());
         ResetProcessTimer(true);
      }
   }
   public class SyntaxChecker
   {
      string CodeLine { get; set; }
      public bool IsIncomplete { get; set; }

      private Stack<char> syntaxStack = new Stack<char>(); // stack will only ever contain "start" chars

      // using lists instead of arrays for convenience of "IndexOf" method. Indices of start/stop/score should match.
      static readonly List<char> startChars = new List<char>() { '(', '[', '{', '<' };
      static readonly List<char> endChars = new List<char>() { ')', ']', '}', '>' };
      static readonly List<int> corruptionScores = new List<int>() { 3, 57, 1197, 25137 };

      public SyntaxChecker(string codeLine)
      {
         CodeLine = codeLine; 
      }
      public long CheckAndScoreCorruption()
      {
         foreach (char chunkMarker in CodeLine)
         {
            if (startChars.Contains(chunkMarker)) syntaxStack.Push(chunkMarker);
            else
            {
               if (syntaxStack.Count > 0 && syntaxStack.Pop() != startChars[endChars.IndexOf(chunkMarker)]) //remove from stack, not "peek"
                  return corruptionScores[endChars.IndexOf(chunkMarker)];
            }
         }
         IsIncomplete = syntaxStack.Count > 0;
         return 0;
      }
      public long CompleteAndScore()
      {
         long score = 0;
         while (syntaxStack.Count > 0)       
            score = (score * 5) + (startChars.IndexOf(syntaxStack.Pop()) + 1);

         return score;
      }
   }
}
