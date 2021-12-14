using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day23_CrabCups : AoCodeModule
    {
        public Day23_CrabCups()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 

        }
        public override void DoProcess()
        {
            //** > Result for Day23_CrabCups part 1:The order of the game after 100 moves is: 53248976(Process: 0.1093 ms)
            //** > Result for Day23_CrabCups part 2:The product of the two cups clockwise from 1 is : 418819514477(Process: 33510.1408 ms)
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            string input = "784235916";
            //input = "389125467"; // Sample
            LinkedList<int> crabCups= new LinkedList<int>(); // hey! a legit reason to use linked lists! ( don't look at day 22 )
            foreach(char c in input)
            {
                crabCups.AddLast(int.Parse(c.ToString()));
            }
            LinkedListNode<int> currentCup = crabCups.First, targetCup;
            LinkedList<int> pickedUpCups = new LinkedList<int>();
            int targetDestination = 0, highestInList=0;
            for (int turnCounter=1; turnCounter<=100; turnCounter++)
            {
                pickedUpCups.Clear();
                for (int nextThree = 0; nextThree<3; nextThree++)
                {
                    pickedUpCups.AddLast(currentCup.Next!=null?currentCup.Next.Value:crabCups.First.Value);
                    crabCups.Remove(currentCup.Next!=null?currentCup.Next:crabCups.First);
                }
                highestInList = (!pickedUpCups.Contains(9) ? 9 : (!pickedUpCups.Contains(8) ? 8 : (!pickedUpCups.Contains(7) ? 7 : 6)));
                targetDestination = currentCup.Value - 1;
                if (targetDestination < 1) targetDestination = highestInList;
                //while (crabCups.Count(aCup => aCup == targetDestination) == 0)
                while (pickedUpCups.Count(aCup => aCup == targetDestination) > 0)
                {
                    targetDestination--;
                    if (targetDestination < 1) targetDestination = highestInList;
                }
                targetCup = crabCups.Find(targetDestination);
                for (int nextThree = 0; nextThree < 3; nextThree++)
                {
                    crabCups.AddAfter(targetCup, pickedUpCups.Last.Value);
                    pickedUpCups.RemoveLast();
                }
                currentCup = currentCup.Next!=null?currentCup.Next:crabCups.First;
            }
            finalResult = "";
            LinkedListNode<int> outputCup = crabCups.Find(1);
            for (int cupCount = 0; cupCount<8; cupCount++)
            {
                outputCup = outputCup.Next==null?crabCups.First:outputCup.Next;
                finalResult += outputCup.Value.ToString();
            }
            AddResult("The order of the game after 100 moves is: " + finalResult);
            Print("The order of the game after 100 moves is: " + finalResult);
            ResetProcessTimer(true);
            input = "784235916";
            //input = "389125467"; // Sample
            crabCups.Clear();
            foreach (char c in input)
            {
                crabCups.AddLast(int.Parse(c.ToString()));
            }
            for(int stupidCrab=10; stupidCrab<=1000000; stupidCrab++)
            {
                crabCups.AddLast(stupidCrab);
            }


            // this is taking too fucking long. DEREFERENCE ALL THE NODES!
            Dictionary<int, LinkedListNode<int>> fuckingCrab= new Dictionary<int, LinkedListNode<int>>();
            currentCup = crabCups.First;
            fuckingCrab.Add(currentCup.Value, currentCup);
            while (currentCup.Next != null)
            {
                currentCup = currentCup.Next;
                fuckingCrab.Add(currentCup.Value, currentCup);
            }

            currentCup = crabCups.First;
            targetDestination = 0; highestInList = 0;
            for (int turnCounter = 1; turnCounter <= 10000000; turnCounter++)
            {
                pickedUpCups.Clear();
                for (int nextThree = 0; nextThree < 3; nextThree++)
                {
                    pickedUpCups.AddLast(currentCup.Next != null ? currentCup.Next.Value : crabCups.First.Value);
                    crabCups.Remove(currentCup.Next != null ? currentCup.Next : crabCups.First);
                }
                // this is the fastest way. Seaching the original list was a killer!
                highestInList = (!pickedUpCups.Contains(1000000) ? 1000000 : (!pickedUpCups.Contains(999999) ? 999999 : (!pickedUpCups.Contains(999998) ? 999998 : 999997)));
                targetDestination = currentCup.Value - 1;
                if (targetDestination < 1) targetDestination = highestInList;
                while (pickedUpCups.Count(aCup => aCup == targetDestination) > 0)
                {
                    targetDestination--;
                    if (targetDestination < 1) targetDestination = highestInList;
                }
                targetCup = fuckingCrab[targetDestination]; //crabCups.Find(targetDestination); // OH NO YOU DON'T! Not searching a bazillion element list... DICTIONARY TO THE RESCUE!
                for (int nextThree = 0; nextThree < 3; nextThree++)
                {
                    crabCups.AddAfter(targetCup, pickedUpCups.Last.Value);
                    fuckingCrab[pickedUpCups.Last.Value] = targetCup.Next; // when the node was removed from the node list, it ceased to be linked. We have to add it back to the dictionary with the new object
                    pickedUpCups.RemoveLast();
                }
                currentCup = currentCup.Next != null ? currentCup.Next : crabCups.First;
            }
            finalResult = "";
            outputCup = crabCups.Find(1);
            outputCup = outputCup.Next == null ? crabCups.First : outputCup.Next;
            long cupMultiplication = outputCup.Value;
            outputCup = outputCup.Next == null ? crabCups.First : outputCup.Next;
            cupMultiplication *= outputCup.Value;
            
            AddResult("The product of the two cups clockwise from 1 is : " + cupMultiplication.ToString());



        }
    }
}
