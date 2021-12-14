using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day04_GiantSquid : AoCodeModule
   {
      public Day04_GiantSquid()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput(); 
         OutputFileReset();
      }
      public override void DoProcess()
      {
         //** > Result for Day04_GiantSquid part 1: Score of first card to win: 8442 (Process: 4.4158 ms)
         //** > Result for Day04_GiantSquid part 2: Score of last card to win: 4590 (Process: 12.7221 ms)
         ResetProcessTimer(true);// true also iterates the section marker
         string NumberCallOrder = inputFile[0]; // comma delimited
         List<BingoCard> bingoCards = new List<BingoCard>();
         int nextCardDefinition = 2;
         while (nextCardDefinition < inputFile.Count - 1)
         {
            BingoCard newCard = new BingoCard();
            for (int x=0; x<5; x++)
            {
               newCard.AddRow(inputFile[nextCardDefinition + x]);
            }
            newCard.Reset();
            bingoCards.Add(newCard);
            nextCardDefinition += 6; //5 rows of numbers, 1 blank line.
         }
         List<string> numbersToCall = NumberCallOrder.Split(new char[] { ',' }).ToList();
         bool gotAWinner = false;
         long score = 0;
         foreach (string number in numbersToCall)
         {
            int callingNumber = int.Parse(number);
            foreach(BingoCard card in bingoCards)
            {
               gotAWinner = card.MarkNumber(callingNumber);
               if (gotAWinner)
               {
                  score = card.Score;
                  break;
               }
            }
            if (gotAWinner) break;
         }
         AddResult("Score of first card to win: " + score.ToString()); // includes elapsed time from last ResetProcessTimer
         ResetProcessTimer(true);
         bingoCards.ForEach(x => x.Reset());
         BingoCard lastWinningBingoCard = null;
         foreach (string number in numbersToCall)
         {
            int callingNumber = int.Parse(number);
            foreach (BingoCard card in bingoCards.Where(x=>!x.IsWinner))
            {
               gotAWinner = card.MarkNumber(callingNumber);
               if (gotAWinner)
               {
                  lastWinningBingoCard = card;
               }
            }
            if (bingoCards.Count(x => !x.IsWinner) == 0) break;
         }
         AddResult("Score of last card to win: " + lastWinningBingoCard.Score.ToString());
         ResetProcessTimer(true);

      }
   }
   public class BingoCard
   {
      // just quick and messy at first
      List<int> originalNumbers = new List<int>();
      List<int> unmarkedNumbers = new List<int>();
      List<BingoStrand> Rows = new List<BingoStrand>();
      List<BingoStrand> Columns = new List<BingoStrand>();
      List<BingoStrand> AllRowsAndColumns = new List<BingoStrand>();

      int lastCalledNumber = -1;
      private int loadingCounter = 0;
      public bool IsWinner { get { return AllRowsAndColumns.Any(x => x.IsWinner); } }

      public BingoCard()
      {
         loadingCounter = 0;
         for (int x = 0; x < 5; x++) { Columns.Add(new BingoStrand()); }
      }
      public void AddRow(string rowNumbers)
      {
         List<string> row = rowNumbers.StandardSplit().ToList();
         BingoStrand newRow = new BingoStrand();
         loadingCounter = 0;
         row.ForEach(x => { int num = Int32.Parse(x); newRow.AddNumber(num); Columns[loadingCounter].AddNumber(num); loadingCounter++; });
         Rows.Add(newRow);
         originalNumbers.AddRange(newRow.RowColumn);
         unmarkedNumbers.AddRange(newRow.RowColumn);
      }
      public void Reset()
      {
         unmarkedNumbers = originalNumbers.ToList();
         Rows.ForEach(x => x.Reset());
         Columns.ForEach(x => x.Reset());
         AllRowsAndColumns = Rows.ToList();
         AllRowsAndColumns.AddRange(Columns.ToList());
         lastCalledNumber = -1;
      }
      public bool MarkNumber(int calledNumber)
      {
         bool madeAWinner = false;
         if (!IsWinner)
         {
            lastCalledNumber = calledNumber;
            unmarkedNumbers.Remove(calledNumber);
            AllRowsAndColumns.ForEach(x => madeAWinner |= x.MarkSpot(calledNumber));
         }
         return madeAWinner;
      }
      public long Score
      {
         get
         {
            return (long)(unmarkedNumbers.Sum()) * (long)lastCalledNumber;
         }
      }


   }
   public class BingoStrand
   {
      public List<int> RowColumn { get; set; }
      private List<int> Marked;
      public bool IsWinner { get { return RowColumn.Count == Marked.Count; } }
      public BingoStrand()
      {
         RowColumn = new List<int>();
         Marked = new List<int>();
      }
      public void AddNumber(int newNumber)
      {
         RowColumn.Add(newNumber);
      }
      public void Reset()
      {
         Marked.Clear();
      }
      public bool MarkSpot(int calledNumber)
      {
         if (RowColumn.Contains(calledNumber) && !Marked.Contains(calledNumber))
         {
            Marked.Add(calledNumber);
         }
         return RowColumn.Count == Marked.Count;
      }
   }
}
