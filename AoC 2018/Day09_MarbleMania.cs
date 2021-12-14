using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day09_MarbleMania : AoCodeModule
    {
        public Day09_MarbleMania()
        {
            // If you always save input file in the /InputFiles/ subfolder and name it the same as the class processing it, this will work.
            // if you put it elsewhere or name it differently, just change below. 
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); //base class method
            OutputFileReset(); // output file will be in the same location as input, with ".output.txt" appended to the name. This clears previous output from file.
            //Print("Did Something");// outputs to console immediately
            //Print("DidSomethingElse", FileOutputAlso); // both console and output file
            //Print("Another Thing", FileOutputOnly); // output file only.
        }
        public override void DoProcess()
        {
            ResetProcessTimer(true);
            //419 players; last marble is worth 71052 points
            List<long> MarbleRing = new List<long>();

            int playerPointer = 0, marbleCounter = 1, marblePointer = 0, marble1 = 0, marble2 = 0;
            int maxPlayers = 419, lastMarble=71052 ;
            long[] players = new long[maxPlayers]; // changed to long from int for part two. overflow!
            MarbleRing.Add(0);
            FunUtilities.HackerScannerPrint hs = new FunUtilities.HackerScannerPrint("Marble: 0000000", 'q');
            hs.Print("Marble: 0000000", true);
            while (marbleCounter <= lastMarble)
            {
                hs.Print("Marble: " + marbleCounter.ToString().PadLeft(7,'0')); // if we wanted the "hacker" random fill, we would pad with the setup char 'q'
                if (marble1 == marble2)
                {//only happens at first move
                    MarbleRing.Add(marbleCounter); marblePointer = 1;
                    marble1 = marblePointer + 1; marble2 = marblePointer + 2;
                    marble1 = (marble1 >= MarbleRing.Count) ? 0 : marble1;
                    marble2 = (marble2 >= MarbleRing.Count) ? marble2 - MarbleRing.Count : marble2;
                }
                else
                {
                    //always insert at marble2, not sure marbel1 is important
                    if (marbleCounter % 23 == 0)
                    {
                        players[playerPointer] += marbleCounter;
                        marblePointer += -7;
                        marblePointer = (marblePointer < 0) ? MarbleRing.Count - Math.Abs(marblePointer) : marblePointer;
                        players[playerPointer] += MarbleRing[marblePointer];
                        MarbleRing.RemoveAt(marblePointer);
                        marblePointer = (marblePointer == MarbleRing.Count) ? 0 : marblePointer;
                    }
                    else
                    {
                        if (marble2 > 0)
                        {
                            MarbleRing.Insert(marble2, marbleCounter);
                            marblePointer = marble2;
                        }
                        else
                        {
                            MarbleRing.Add(marbleCounter);
                            marblePointer = MarbleRing.Count - 1;
                        }

                    }
                    marble1 = marblePointer + 1; marble2 = marblePointer + 2;
                    marble1 = (marble1 >= MarbleRing.Count) ? 0 : marble1;
                    marble2 = (marble2 >= MarbleRing.Count) ? marble2 - MarbleRing.Count : marble2;
                }

                marbleCounter++; playerPointer++; playerPointer = (playerPointer == maxPlayers) ? 0 : playerPointer;
            }
            Console.WriteLine();
            AddResult(players.Max().ToString());
            ResetProcessTimer(true);
            lastMarble = lastMarble * 100;
            //The old way using straight lists is going WAY overboard. Quit after 15 mins of running...  Trying a new way. linked list seems to be enjoying a lot of attention on reddit?
            // never used it though. What the heck, let's try....

            players = new long[maxPlayers]; //reset scores
            LinkedList<int> marbleRing2 = new LinkedList<int>();
            LinkedListNode<int> current = marbleRing2.AddFirst(0);
            marblePointer = 1; // was using a counter and a pointer to the list index, don't need that anymore with linked lists. just the counter.
            while (marblePointer<=lastMarble)
            {
                if (marblePointer % 23 == 0)
                {
                    players[marblePointer % maxPlayers] += marblePointer; // give the dude his points.
                    //move back in the node list 7 times
                    for (int j = 0; j < 7; j++) { current = current.Previous ?? marbleRing2.Last; } // apparently when the linked list is at the beginning, previous returns null... so loop to the last in the list.
                    players[marblePointer % maxPlayers] += current.Value;// add the 7th previous marble value.
                    LinkedListNode<int> oneToRemove = current; // we are done with it.
                    current = oneToRemove.Next; // repoint current to the one that will now be in this spot.
                    marbleRing2.Remove(oneToRemove); // and unlink it. 
                }
                else
                {
                    current = marbleRing2.AddAfter(current.Next ?? marbleRing2.First, marblePointer); // add it between one marble ahead and two marbles ahead. If Next is null, we passed the end of the list, repoint to the beginning.
                }
                marblePointer++;
            }
            AddResult( players.Max().ToString());
            //Holy crap that was fast......

        }
    }
    
}
