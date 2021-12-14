using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day22_CrabCombat : AoCodeModule
    {
        public Day22_CrabCombat()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 

        }
        public override void DoProcess()
        {
            //** > Result for Day22_CrabCombat part 1:The winner has 31269 points(Process: 0.5769 ms)
            //** > Result for Day22_CrabCombat part 2:The winner has 31151 points(Process: 8782.1425 ms)
            ResetProcessTimer(true);

            // was using linked lists. That just got SUPER stupid and messy and kept... .working ALMOST? Back to the drawing board.
            // The rules are too simple for it to keep fuckin' up. Let's stop being fancy, and just get dancy!
            // This is a complete crap result of being really annoyed. Uncle Bob, forgive me!
            List<int> player1deck = new List<int>();
            List<int> player2deck = new List<int>();
            bool loadPlayer1 = true;
            foreach (string processingLine in inputFile)
            {
                if (processingLine == "Player 2:" || processingLine == "Player 1:" || processingLine == "")
                {
                    loadPlayer1 = processingLine == "Player 1:";
                }
                else
                {
                    ((loadPlayer1) ? player1deck : player2deck).Add(int.Parse(processingLine));
                }
            }

            AddResult("The winner has " + DoExplicitPartOne(player1deck, player2deck).ToString() + " points");
            ResetProcessTimer(true);
            player1deck = new List<int>();
            player2deck = new List<int>();
            loadPlayer1 = true;

            // testing data for infinite game catch
            //inputFile.Clear();
            //inputFile.Add("Player 1:");
            //inputFile.Add("43");
            //inputFile.Add("19");
            //inputFile.Add("");
            //inputFile.Add("Player 2:");
            //inputFile.Add("2");
            //inputFile.Add("29");
            //inputFile.Add("14");

            // reload for part 2.
            foreach (string processingLine in inputFile)
            {
                if (processingLine == "Player 2:" || processingLine == "Player 1:" || processingLine == "")
                {
                    loadPlayer1 = processingLine == "Player 1:";
                }
                else
                {
                    ((loadPlayer1) ? player1deck : player2deck).Add(int.Parse(processingLine));
                }
            }
            AddResult("The winner has " + DoPoorlyFactoredPartTwo(player1deck, player2deck).ToString() +" points");

        }
        private static int DoExplicitPartOne(List<int> player1deck, List<int> player2deck)
        {
            while (true)
            {
                if (player1deck.Count == 0 || player2deck.Count == 0)
                {
                    break;
                }

                int p1Card = player1deck[0];
                int p2Card = player2deck[0];
                if (p1Card > p2Card)
                {
                    player1deck.Add(p1Card);
                    player1deck.Add(p2Card);

                    player1deck.RemoveAt(0);
                    player2deck.RemoveAt(0);
                }
                else
                {
                    player2deck.Add(p2Card);
                    player2deck.Add(p1Card);

                    player1deck.RemoveAt(0);
                    player2deck.RemoveAt(0);
                }
            }

            return CalculateWiningScore(player1deck.Count != 0 ? player1deck : player2deck);
        }

        private static int CalculateWiningScore(List<int> deck)
        {
            int result = 0;
            deck.Reverse();
            for (int i = 1; i <= deck.Count; i++)
            {
                result += deck[i-1] * i;
            }

            return result;
        }

        private static object DoPoorlyFactoredPartTwo(List<int> player1deck, List<int> player2deck)
        {
            List<List<int>> player1deckCombinations = new List<List<int>>();
            List<List<int>> player2deckCombinations = new List<List<int>>();
            while (true)
            {
                if (player1deck.Count == 0 || player2deck.Count == 0)
                {
                    break;
                }

                int p1Card = player1deck[0];
                int p2Card = player2deck[0];
                if (p1Card >= player1deck.Count || p2Card >= player2deck.Count) // someone doesn't have enough cards to recurse....
                {
                    if (p1Card > p2Card)
                    {
                        player1deck.Add(p1Card);
                        player1deck.Add(p2Card);

                        player1deck.RemoveAt(0);
                        player2deck.RemoveAt(0);
                    }
                    else
                    {
                        player2deck.Add(p2Card);
                        player2deck.Add(p1Card);

                        player1deck.RemoveAt(0);
                        player2deck.RemoveAt(0);
                    }
                    // NOW, WITH THE POWER OF LINQ and LISTS, I GIVE YOU.... SIMPLICITY!
                    if (player1deckCombinations.Any(x => x.SequenceEqual(player1deck)) || player2deckCombinations.Any(x => x.SequenceEqual(player2deck)))
                    {
                        // infinite game state reached. Player 1 wins! what is the final score?
                        return CalculateWiningScore(player1deck);// != 0 ? player1deck : player2deck);
                    }

                    player1deckCombinations.Add(new List<int>(player1deck));
                    player2deckCombinations.Add(new List<int>(player2deck));
                }
                else
                {
                    // skipping the first card, which is the one the players are "playing with" right now, take the next "X" number of cards respective of their value.
                    // as a "Range", this is a new list, and it's a list of primitives, so no object reference conflicts. They are "Copies" as per spec.
                    if (DoShitRecursionAndGetWinner(new List<int>(player1deck.GetRange(1, p1Card)), new List<int>(player2deck.GetRange(1, p2Card))))
                    {
                        // Player 1 wins!
                        player1deck.Add(p1Card);
                        player1deck.Add(p2Card);

                        player1deck.RemoveAt(0);
                        player2deck.RemoveAt(0);
                    }
                    else
                    {
                        //fucking crab wins!
                        player2deck.Add(p2Card);
                        player2deck.Add(p1Card);

                        player1deck.RemoveAt(0);
                        player2deck.RemoveAt(0);
                    }
                }
            }

            return CalculateWiningScore(player1deck.Count != 0 ? player1deck : player2deck); // what do they win, Alex?
        }
        // I was keeping score on recursive games... but it doesn't really matter. Only WHO WINS the recursion. Simple bool. True=player one. 
        private static bool DoShitRecursionAndGetWinner(List<int> player1deck, List<int> player2deck)
        {
            List<List<int>> player1deckCombinations = new List<List<int>>();
            List<List<int>> player2deckCombinations = new List<List<int>>();
            while (true)
            {
                int p1Card = player1deck[0];
                int p2Card = player2deck[0];
                if (p1Card >= player1deck.Count || p2Card >= player2deck.Count) // if either player can't recurse, just play the game. 
                {
                    if (p1Card > p2Card)
                    {
                        player1deck.Add(p1Card);
                        player1deck.Add(p2Card);

                        player1deck.RemoveAt(0);
                        player2deck.RemoveAt(0);
                    }
                    else
                    {
                        player2deck.Add(p2Card);
                        player2deck.Add(p1Card);

                        player1deck.RemoveAt(0);
                        player2deck.RemoveAt(0);
                    }
                }
                else
                {
                    // skipping the first card, which is the one the players are "playing with" right now, take the next "X" number of cards respective of their value.
                    // as a "Range", this is a new list, and it's a list of primitives, so no object reference conflicts. They are "Copies" as per spec.
                    if (DoShitRecursionAndGetWinner(new List<int>(player1deck.GetRange(1, p1Card)), new List<int>(player2deck.GetRange(1, p2Card))))
                    {
                        //player 1 wins!
                        player1deck.Add(p1Card);
                        player1deck.Add(p2Card);

                        player1deck.RemoveAt(0);
                        player2deck.RemoveAt(0);
                    }
                    else
                    {
                        // fucking crab wins!
                        player2deck.Add(p2Card);
                        player2deck.Add(p1Card);

                        player1deck.RemoveAt(0);
                        player2deck.RemoveAt(0);
                    }
                }
                // NOW, WITH THE POWER OF LINQ and LISTS, I GIVE YOU.... SIMPLICITY!
                if (player1deckCombinations.Any(x => x.SequenceEqual(player1deck)) && player2deckCombinations.Any(x => x.SequenceEqual(player2deck)))
                {
                    return true; // infinite game condition. Return true for "player one wins"
                }

                player1deckCombinations.Add(new List<int>(player1deck));
                player2deckCombinations.Add(new List<int>(player2deck));

                if (player1deck.Count == 0 || player2deck.Count == 0) //ooopsie poopsie! someone is out of cards.
                {
                    break; //bust outta the while loop.
                }
            }

            return (player1deck.Count > player2deck.Count); // if player 1 has all the cards, they win. Return true. 
        }
    }

    // original with Linked List nonsense. THIS IS WHAT HAPPENS WHEN YOU GET FANCY BOYS AND GIRLS.
    /*
    class Day22_CrabCombat : AoCodeModule
    {
        public int recursionCounter = 0;
        public int maxRecursionCounter = 0;
        public Dictionary<int, List<Tuple<LinkedList<int>, LinkedList<int>>>> infiniteGameChecker = new Dictionary<int, List<Tuple<LinkedList<int>, LinkedList<int>>>>();
        public Day22_CrabCombat()
        {

            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            //inputFileName = @"InputFiles\" + this.GetType().Name + "Sample.txt";
            GetInput();
            OutputFileReset();

        }
        public override void DoProcess()
        {

            string finalResult = "Not Set";
            ResetProcessTimer(true);
            LinkedList<int> Player1 = new LinkedList<int>();
            LinkedList<int> Player2 = new LinkedList<int>();
            bool loadPlayer1 = true;
            foreach (string processingLine in inputFile)
            {
                if (processingLine == "Player 2:" || processingLine == "Player 1:" || processingLine == "")
                {
                    loadPlayer1 = processingLine == "Player 1:";
                }
                else
                {
                    ((loadPlayer1) ? Player1 : Player2).AddLast(int.Parse(processingLine));
                }
            }

            Tuple<bool, long> gameResult = PlayAGame(CopyDeck(Player1, Player1.Count), CopyDeck(Player2, Player2.Count), false);
            finalResult = (gameResult.Item1 ? "Player 1" : "Player 2") + " wins with " + gameResult.Item2.ToString() + " points.";
            Print(finalResult);
            AddResult(finalResult);
            ResetProcessTimer(true);
            Print("--------------------------------");
            maxRecursionCounter = 0;
            Player1 = new LinkedList<int>();
            Player2 = new LinkedList<int>();
            loadPlayer1 = true;
            foreach (string processingLine in inputFile)
            {
                if (processingLine == "Player 2:" || processingLine == "Player 1:" || processingLine == "")
                {
                    loadPlayer1 = processingLine == "Player 1:";
                }
                else
                {
                    ((loadPlayer1) ? Player1 : Player2).AddLast(int.Parse(processingLine));
                }
            }
            gameResult = PlayAGame(CopyDeck(Player1, Player1.Count), CopyDeck(Player2, Player2.Count), true);
            finalResult = (gameResult.Item1 ? "Player 1" : "Player 2") + " wins with " + gameResult.Item2.ToString() + " points.";
            AddResult(finalResult);

        }
        public Tuple<bool, long> PlayAGame(LinkedList<int> Player1, LinkedList<int> Player2, bool RecursiveCombat)
        {
            recursionCounter++;
            if (recursionCounter > maxRecursionCounter)
            {
                maxRecursionCounter = recursionCounter;
                //Print("Reached new Level of Recursion: " + maxRecursionCounter.ToString());
            }
            else
            {
                //Print("Playing Game at level " + recursionCounter.ToString());
            }
            //Print("=== Game " + recursionCounter.ToString() + " ===");
            //Print("");
            int totalCards = Player2.Count + Player1.Count;
            LinkedListNode<int> Player1Card, Player2Card;
            int roundCounter = 0;
            if (RecursiveCombat && !infiniteGameChecker.ContainsKey(recursionCounter)) // it NEVER SHOULD 
            {
                infiniteGameChecker.Add(recursionCounter, new List<Tuple<LinkedList<int>, LinkedList<int>>>());
            }
            while (Player1.Count != totalCards && Player2.Count != totalCards)
            {
                if (recursionCounter == 1 && (Player1.Count + Player2.Count != 50))
                {
                    int q = 0;
                }
                roundCounter++;
                //Print("-- Round " + roundCounter.ToString() + " (Game " + recursionCounter.ToString() + ") --");
                //Print("Player 1's Deck: " + GetDeckString(Player1));
                //Print("Player 2's Deck: " + GetDeckString(Player2));
                if (RecursiveCombat)
                {
                    if (CheckPatternRepeat(recursionCounter, Player1, Player2))
                    {
                        //Print("********************Reached an infinite game at recursion level " + recursionCounter.ToString() + " turn " + roundCounter.ToString());
                        infiniteGameChecker.Remove(recursionCounter);
                        recursionCounter--;
                        long points = GetTotalPoints(Player1);
                        //Print("Returning from game " + recursionCounter.ToString() + " with a win for player 1 with " + points.ToString() + " Points");
                        return new Tuple<bool, long>(true, points);
                    }
                    else
                    {
                        infiniteGameChecker[recursionCounter].Add(new Tuple<LinkedList<int>, LinkedList<int>>(CopyDeck(Player1, Player1.Count), CopyDeck(Player2, Player2.Count)));
                        int q2 = 0;
                    }
                }
                Player1Card = Player1.First;
                Player1.RemoveFirst();
                Player2Card = Player2.First;
                Player2.RemoveFirst();
                if (RecursiveCombat && (Player1.Count >= Player1Card.Value && Player2.Count >= Player2Card.Value))
                {
                    Tuple<bool, long> gameResult = PlayAGame(CopyDeck(Player1, Player1Card.Value), CopyDeck(Player2, Player2Card.Value), RecursiveCombat);
                    //Print((gameResult.Item1 ? "Player 1" : "Player 2") + " wins the sub-game with " + gameResult.Item2.ToString() + " points");
                    if (gameResult.Item1)
                    {
                        Player1.AddLast(Player1Card);
                        Player1.AddLast(Player2Card);
                    }
                    else
                    {
                        Player2.AddLast(Player2Card);
                        Player2.AddLast(Player1Card);
                    }
                }
                else
                {
                    if (Player1Card.Value > Player2Card.Value)
                    {
                        Player1.AddLast(Player1Card);
                        Player1.AddLast(Player2Card);
                    }
                    else
                    {
                        Player2.AddLast(Player2Card);
                        Player2.AddLast(Player1Card);
                    }
                }
            }
            long finishedpoints = GetTotalPoints(Player1.Count > 0 ? Player1 : Player2);
            //Print("Returning from game " + recursionCounter.ToString() + " with a Total win for Player " + (Player1.Count > 0?"1":"2" )+ " with " + finishedpoints.ToString() + " Points");
            if (RecursiveCombat) infiniteGameChecker.Remove(recursionCounter);
            recursionCounter--;
            return new Tuple<bool, long>(Player1.Count > 0, finishedpoints);
        }
        public long GetTotalPoints(LinkedList<int> winningDeck)
        {
            long totalPoints = 0;
            int cardCount = 0;
            while (winningDeck.Count > 0)
            {
                cardCount++;
                LinkedListNode<int> lastCard = winningDeck.Last;
                winningDeck.RemoveLast();
                totalPoints += (cardCount * lastCard.Value);
            }
            return totalPoints;
        }
        public LinkedList<int> CopyDeck(LinkedList<int> OriginalDeck, int NumberToCopy, int indexToStart = 0)
        {
            LinkedList<int> NewDeck = new LinkedList<int>();
            int copiedCardCounter = 0;
            LinkedListNode<int> currentCard = OriginalDeck.First;
            while (currentCard != null && copiedCardCounter < NumberToCopy)
            {
                NewDeck.AddLast(currentCard.Value);
                currentCard = currentCard.Next;
                copiedCardCounter++;
            }
            if (NewDeck.Count != NumberToCopy)
            {
                throw new Exception("FUCK ALL!");
            }
            return NewDeck;
        }
        public bool CheckPatternRepeat(int recursiveLevel, LinkedList<int> Player1, LinkedList<int> Player2)
        {
            bool patternExists = false;
            List<Tuple<LinkedList<int>, LinkedList<int>>> setOfDecks = infiniteGameChecker[recursiveLevel];
            foreach (Tuple<LinkedList<int>, LinkedList<int>> deckSet in setOfDecks)
            {
                if (DeckMatches(Player1, deckSet.Item1) && DeckMatches(Player2, deckSet.Item2))
                {
                    patternExists = true;
                    break;
                }

            }
            return patternExists;
        }
        public bool DeckMatches(LinkedList<int> Deck1, LinkedList<int> Deck2)
        {
            bool matches = false;
            //int cardIndex = 0;
            if (Deck1.Count == Deck2.Count)
            {
                matches = true;
                LinkedListNode<int> deck1Card = Deck1.First;
                LinkedListNode<int> deck2Card = Deck2.First;

                //while (cardIndex < Deck1.Count)
                while (deck1Card != null)
                {
                    if (deck1Card.Value != deck2Card.Value)
                    {
                        matches = false;
                        break;
                    }
                    deck1Card = deck1Card.Next;
                    deck2Card = deck2Card.Next;
                    //cardIndex++;
                }
            }
            return matches;
        }
        public string GetDeckString(LinkedList<int> theDeck)
        {
            string deck = "";
            LinkedListNode<int> card = theDeck.First;
            while (card != null)
            {
                deck += card.Value.ToString() + ", ";
                card = card.Next;
            }
            return deck.Substring(0, deck.Length - 2);
        }
    }
    */
}
