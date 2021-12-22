using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day21_DiracDice : AoCodeModule
   {
      public Day21_DiracDice()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         //** > Result for Day21_DiracDice part 1: Losing score multiplied by die roll is 518418 (Process: 0.0304 ms)
         //** > Result for Day21_DiracDice part 2: Winner had a total of 116741133558209 (Process: 642.9872 ms)
         ResetProcessTimer(true);
         List<DiracPlayer> players = new List<DiracPlayer>();
         foreach (string processingLine in inputFile)
         {
            string[] playerInput = processingLine.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            players.Add(new DiracPlayer(int.Parse(playerInput[1])));
         }
         DeterministicDie.ResetDie();
         bool gameWon = false;
         while (!gameWon)
            players.ForEach(player => { if (!gameWon) { gameWon = player.TakeTurn(); } });

         AddResult("Losing score multiplied by die roll is " + (DeterministicDie.NumberOfRolls * players.Where(player => !player.WonGame).First().TotalScore).ToString());
         ResetProcessTimer(true);
         players.ForEach(player => player.Reset());
         DiracPlayer.TakeQuantumPathsToWin(players[0], players[1]);
         AddResult("Winner had a total of " + DiracPlayer.QuantumGameWins.ToString());
         ResetProcessTimer(true);
      }
   }
   public static class DeterministicDie
   {
      static int diePointer = 0;
      static int dieIncrementer = 1;
      static int dieMinValue = 1;
      static int dieMaxValue = 100;
      // I set up these properties in case part 2 changed the parameters of the die. Turns out, it's not used a second time. Oh well. 
      public static long NumberOfRolls { get; set; }
      public static int DieIncrementer { get { return dieIncrementer; } set { dieIncrementer = value; } }
      public static int DieMinValue { get { return dieMinValue; } set { dieMinValue = value; } }

      public static int DieMaxValue { get { return dieMaxValue; } set { dieMaxValue = value; } }
      public static int RollDie(int numberOfRolls)
      {
         int dieTotal = 0;
         for (int x = 0; x < numberOfRolls; x++)
         {
            diePointer += dieIncrementer;
            if (diePointer > dieMaxValue) diePointer = dieMinValue;
            dieTotal += diePointer;
         }
         NumberOfRolls += numberOfRolls;
         return dieTotal;
      }
      public static void ResetDie(int toNextRoll = 1)
      { // will part 2 have us roll this 4 bazillion times? or change where the die roll starts?
         diePointer = dieMinValue - 1;
         NumberOfRolls = 0;
      }

   }
   public class DiracPlayer
   {
      int startingPosition;
      int score;
      int currentPosition;

      static long TotalPlayer1Wins = 0, TotalPlayer2Wins = 0;
      public int CurrentPosition { get { return currentPosition; } }
      public bool WonGame { get; set; }
      public long TotalScore { get { return score; } } // part 1 - each player's individual score
      public static long QuantumGameWins { get { return Math.Max(TotalPlayer1Wins, TotalPlayer2Wins); } }
      public static List<int> QuantumGameRollCombinations = new List<int>(); // will hold the ordered list of 27 combinations of 3 rolls of a 3 sided die.
      // the key is:
      // For Player 1's current position and score (first two ints in the tuple)
      // and Player 2's current position and score (second two ints in the tuple)
      // and one of the 7 possible dice rolls[27 combinations] (fifth int in the tuple)
      // and which player gets the next turn (sixth int)
      // This is to keep track of how many times downstream of the combos that player one won, versus how many time player two won(longs in the value tuple). The results can clearly be huge!
      // so that if we come across a combination that has already been used and resolved a second time, we don't have to do all the paths over again. 
      public static Dictionary<(int, int, int, int, int, int), (long, long)> QuantumPathDeterminations = new Dictionary<(int, int, int, int, int, int), (long, long)>();
      public DiracPlayer(int gameStartingPosition)
      {
         startingPosition = currentPosition = gameStartingPosition;
      }
      public void Reset()
      {
         currentPosition = startingPosition;
         score = 0;
         WonGame = false;

         // for part 2
         if (QuantumGameRollCombinations.Count == 0) // setup all dice roll combinations for part 2. 27 combinations of the roll of the roll of the roll of the dice.
            for (int die1 = 1; die1 <= 3; die1++)
               for (int die2 = 1; die2 <= 3; die2++)
                  for (int die3 = 1; die3 <= 3; die3++)
                     QuantumGameRollCombinations.Add(die1 + die2 + die3);

         // just playing to see if the order of the rolls matters - it doesn't
         /// and then checking to see if all 27 are actually necessary - they are (because, and i'm a bonehead, even if the combo is played before, we must still accumulate the results a second or third time)
         //QuantumGameRollCombinations = QuantumGameRollCombinations.OrderBy(roll => roll).ToList();
         //QuantumGameRollCombinations = QuantumGameRollCombinations.OrderBy(roll => roll).Distinct().ToList();
         
         // clear the hashed result nonsense for a fresh start.
         QuantumPathDeterminations = new Dictionary<(int, int, int, int, int, int), (long, long)>();
      }
      public bool TakeTurn() // part 1
      {
         currentPosition += DeterministicDie.RollDie(3); // three times
         currentPosition = currentPosition % 10; // positions 1-10 ( or 1-0, but it's the same result)
         score += currentPosition == 0 ? 10 : currentPosition; // score for position 0 is 10, not zero.
         WonGame = score >= 1000;
         return WonGame;
      }
      public static void TakeQuantumPathsToWin(DiracPlayer player1, DiracPlayer player2) // part 2
      {
         // each set of rolls spawns a different path. 
         // a player can roll during their turn a total value of 3-9, inclusive. so each quantum dice roll will spawn up to 7 different results
         // Note to self, that's a naive look at it. There must be 27 total rolls. 
         // The first dice roll has three possibilities. All of them must be examined. But for each of those three....
         // The second dice roll also produces three possibilities. So for each of the original 3, there are three more. 9 combinations.
         // Same with the third. So for each of first two dice's 9 combinations, there are now three more. 3^3 rolls = 27.
         // 
         // since we are only playing to 21, hopefully this won't turn into a huge data map, but as the rolls progress, they should generate a pattern
         // whereby at the start of a turn we can see if this combination of starting scores at this given position, with this variation of the dice roll, for this player
         // already has a result, and use it without recursing further.
         (long player1Wins, long player2Wins) WinAccumulations = (0, 0);
         TotalPlayer1Wins = 0; TotalPlayer2Wins = 0;

         // all 27 combinations are set during init, in the order they generate. hmmm.... I wonder if order matters?
         QuantumGameRollCombinations.ForEach(roll =>
         {
            // kick it off with starting positions and score for each possible dice roll, but always start with player 1
            WinAccumulations = TakeAQuantumTurn(player1.currentPosition, player1.score, player2.currentPosition, player2.score, roll, 1); 
            TotalPlayer1Wins += WinAccumulations.player1Wins; TotalPlayer2Wins += WinAccumulations.player2Wins;
         });
      }
      // For this set of positions, starting scores, dice roll option, and the player's turn recurse until someone wins
      // Recursion will call for the alternative player in the loop.
      public static (long, long) TakeAQuantumTurn(int Player1CurrentPosition, int Player1CurrentScore, int Player2CurrentPosition, int Player2CurrentScore, int RollOfTheDice, int WhichPlayersTurn)
      {
         (long player1Wins, long player2Wins) WinnerAccumulation = (0, 0);
         long totalPlayer1Wins = 0, totalPlayer2Wins = 0; // used in recursion loop for this turn's possible spawning point.
         (int, int, int, int, int, int) thisTurnsKey = (Player1CurrentPosition, Player1CurrentScore, Player2CurrentPosition, Player2CurrentScore, RollOfTheDice, WhichPlayersTurn);
         // first, see if we've already played this tune
         if (!QuantumPathDeterminations.TryGetValue(thisTurnsKey, out WinnerAccumulation)) // if we don't have it, gotta determine it. 
         {
            // check the results to see if the player's turn results in a win
            int turnResultingPosition = ((WhichPlayersTurn == 1 ? Player1CurrentPosition : Player2CurrentPosition) + RollOfTheDice) % 10; //move on the board, wrap around at 10.
            int turnScore = ((WhichPlayersTurn == 1 ? Player1CurrentScore : Player2CurrentScore) + (turnResultingPosition == 0 ? 10 : turnResultingPosition)); // board is 1234567890, but zero is 10 points.
            if (turnScore >= 21)
            {
               //resulted in a win, add one to the accumulators for this player, nothing for the loser.
               if (WhichPlayersTurn == 1) WinnerAccumulation.player1Wins++; else WinnerAccumulation.player2Wins++;
               //WinnerAccumulation.player1Wins += WhichPlayersTurn == 1 ? 1 : 0;
               //WinnerAccumulation.player2Wins += WhichPlayersTurn == 2 ? 1 : 0;
            }
            else
            {
               // nobody won yet. Add the score to the player whose turn it is, and their the resulting position 
               if (WhichPlayersTurn == 1) Player1CurrentScore = turnScore; else Player2CurrentScore = turnScore;
               if (WhichPlayersTurn == 1) Player1CurrentPosition = turnResultingPosition; else Player2CurrentPosition = turnResultingPosition;
               // Next Player is up!
               // note, see original note, above.  27 combinations of rolls
               // this will recurse in a tree that will continue to spawn 27 rolls until someone wins every combination
               QuantumGameRollCombinations.ForEach(roll =>
               {
                  // reuse WinnerAccumulation. Set final results at end.
                  WinnerAccumulation = TakeAQuantumTurn(Player1CurrentPosition, Player1CurrentScore, Player2CurrentPosition, Player2CurrentScore, roll, WhichPlayersTurn == 1 ? 2 : 1); 
                  totalPlayer1Wins += WinnerAccumulation.player1Wins; totalPlayer2Wins += WinnerAccumulation.player2Wins;
               });
               // set this spawning point's results for future lookup
               WinnerAccumulation.player1Wins = totalPlayer1Wins;
               WinnerAccumulation.player2Wins = totalPlayer2Wins;
            }
         }
         // record the results to allow for short-circuiting should this combination occur again
         QuantumPathDeterminations[thisTurnsKey] = WinnerAccumulation;
         return WinnerAccumulation;
      }
   }
}
