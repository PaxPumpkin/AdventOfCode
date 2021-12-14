using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day12_SubterraneanSustainability : AoCodeModule
    {
        public Day12_SubterraneanSustainability()
        {

            //inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            inputFileName = @"InputFiles\" + this.GetType().Name + "jmcarter17.txt";
            //inputFileName = @"InputFiles\" + this.GetType().Name + "focus75.txt";
            GetInput(); 
            OutputFileReset(); 
        }
        public override void DoProcess()
        {
            string initialState = "#.#.#....##...##...##...#.##.#.###...#.##...#....#.#...#.##.........#.#...#..##.#.....#..#.###"; //me
            //initialState = "#..#.#..##......###...###"; //sample data
            initialState = ".##..##..####..#.#.#.###....#...#..#.#.#..#...#....##.#.#.#.#.#..######.##....##.###....##..#.####.#"; //jmcarter17
            //initialState = "##.##.##..#..#.#.#.#...#...#####.###...#####.##..#####.#..#.##..#..#.#...#...##.##...#.##......####."; //focus75
            ResetProcessTimer(true);
            List<PlantPattern> patterns = new List<PlantPattern>();
            LinkedList<PlantPot> plantPots = new LinkedList<PlantPot>(); // for ease of adding and moving
            List<PlantPot> indexedPlants = new List<PlantPot>(); // for ease of addressing
 
            foreach (char x in initialState)
            {
                plantPots.AddLast(new PlantPot(x.ToString(),plantPots.Count));
            }
            //start with some extra empty pots, similar to the example. I can see how it would matter
            for (int x = 0; x < 5; x++) { plantPots.AddFirst(new PlantPot(".", plantPots.First.Value.index - 1)); }
            // then a few more empty pots at the end. 
            for (int x = 0; x < 5; x++) { plantPots.AddLast(new PlantPot(".",plantPots.Last.Value.index+1)); }
            foreach (string processingLine in inputFile)
            {
                string[] patternparts = processingLine.Split(new string[] { " => " }, StringSplitOptions.RemoveEmptyEntries);
                patterns.Add(new PlantPattern() { pattern = patternparts[0], result = patternparts[1] });
            }
            int totalPlants = 0;
            int lastChange = 0;
            int lastLastChange = 0;
            int changeIteration = 0;
            int lastTotal = 0;
            int generation20 = 0;
            LinkedListNode<PlantPot> currentPot;
            string thisGenerationState;
            thisGenerationState = "";
            currentPot = plantPots.First;
            indexedPlants = new List<PlantPot>();
            while (currentPot != null)
            {
                thisGenerationState += currentPot.Value.current; // build the pretty picture of pots all in a row.
                indexedPlants.Add(currentPot.Value);
                currentPot = currentPot.Next;
            }
            for (int x = 0; x < 300; x++)
            {

                if (x == 0) { Print(x.ToString().PadLeft(2,' ') + ": " + thisGenerationState); }
                patterns.ForEach(pattern =>
                {
                    int startIndex = 0;
                    bool keepLooking = true;
                    while (keepLooking)
                    {
                        int foundIndex = thisGenerationState.IndexOf(pattern.pattern, startIndex);
                        if (foundIndex >= 0)
                        {
                            indexedPlants[foundIndex+2].pending = pattern.result; //foundIndex++;
                            startIndex = foundIndex +1;
                        }
                        else
                        {
                            // no more match the pattern
                            keepLooking = false;
                        }
                    }
                });


                int thisGenerationPlantCount = 0;
                thisGenerationState = "";
                currentPot = plantPots.First;
                while (currentPot != null)
                {
                    currentPot.Value.Transition();
                    thisGenerationPlantCount += (currentPot.Value.current.Equals("#")) ? currentPot.Value.index : 0; // not a COUNT OF PLANTS! A SUMMATION OF INDICES! OOPS!
                    thisGenerationState += currentPot.Value.current; // build the pretty picture of pots all in a row.
                    currentPot = currentPot.Next;
                }
                if (!thisGenerationState.StartsWith("....."))
                {
                    for (int z = 0; z < 5; z++) { plantPots.AddFirst(new PlantPot(".", plantPots.First.Value.index - 1)); }
                }
                if (!thisGenerationState.EndsWith("....."))
                {
                    for (int z = 0; z < 5; z++) { plantPots.AddLast(new PlantPot(".", plantPots.Last.Value.index + 1)); }
                }
                indexedPlants = new List<PlantPot>();
                thisGenerationState = "";
                currentPot = plantPots.First;
                while (currentPot != null)
                {
                    thisGenerationState += currentPot.Value.current; // build the pretty picture of pots all in a row.
                    indexedPlants.Add(currentPot.Value);
                    currentPot = currentPot.Next;
                }
                Print("Change in plant count: " + (thisGenerationPlantCount-totalPlants) + " iteration " + x);
                if (lastChange == (thisGenerationPlantCount - totalPlants))
                {
                    lastLastChange++;
                    changeIteration = x+1;
                    lastTotal = thisGenerationPlantCount;
                    //break;
                }
                else { lastLastChange = 0; lastChange = (thisGenerationPlantCount - totalPlants); }
                if (lastLastChange > 5) { break; }
                totalPlants = thisGenerationPlantCount; // not summing each iteration, final is just the last iteration's index summation
                if (x < 20) { generation20 = totalPlants; }
                //Print((x+1).ToString().PadLeft(2, ' ') + ": " + thisGenerationState + " ( " + thisGenerationPlantCount.ToString() + " plants)");
            }


            AddResult(generation20.ToString() + " in 20 generations"); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);
            //long iterationsPastPattern = ((50000000000 - 111) * 20) + 2728;
            //long iterationsPastPattern = ((50000000000 - 123) * 58) + 8990;
            long iterationsPastPattern = ((50000000000 - changeIteration) * lastChange) + lastTotal;
            AddResult(iterationsPastPattern.ToString() + " in fifty billion generations");
        }
    }
    public class PlantPot
    {
        public string current = "";
        public string pending = "";
        public int index = 0;
        public PlantPot(string initial,int number)
        {
            current =  initial;
            pending = "."; 
            index = number;
        }
        public void Transition()
        {
            current = pending;
            pending = ".";
        }

    }
    public class PlantPattern
    {
        public string pattern;
        public string result;

    }
}
