using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day18_SettlersOfTheNorthPole : AoCodeModule
    {
        public Day18_SettlersOfTheNorthPole()
        {
 
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day18_SettlersOfTheNorthPole part 1:Data Setup Complete(Process: 430 ms)
            //** > Result for Day18_SettlersOfTheNorthPole part 1:Resource value after 10 iterations: 506160(Process: 440 ms)
            //** > Result for Day18_SettlersOfTheNorthPole part 2:Data Setup Complete(Process: 446 ms)
            //** > Result for Day18_SettlersOfTheNorthPole part 2:After 1000000000 iterations it would be: 189168(Process: 1074 ms)

              
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            int[,] basicGrid = new int[50, 50];
            int row = 0, column=0;
            foreach (string processingLine in inputFile)
            {
                column = 0;
                foreach (char c in processingLine)
                {
                    basicGrid[row, column] = (c == '.') ? 0 : ((c=='|')?1:2);
                    column++;
                }
                row++;
            }
            List<Acre> allAcres = new List<Acre>();
            row = 0;column = 0;
            for (row=0; row<50; row++)
            {
                for (column=0; column<50; column++)
                {
                    allAcres.Add(new Acre() {Row=row, Column=column, Type=basicGrid[row,column],ImpendingType=-1 });
                }
            }
            allAcres.ForEach(a => {
                if (a.Row > 0)
                    a.Neighbors.AddRange(allAcres.Where(x => x.Row == a.Row - 1 && x.Column >= (a.Column>0?a.Column-1:a.Column) && x.Column <= (a.Column<49?a.Column+1:a.Column)).ToList());
                if (a.Row < 49)
                    a.Neighbors.AddRange(allAcres.Where(x => x.Row == a.Row + 1 && x.Column >= (a.Column > 0 ? a.Column - 1 : a.Column) && x.Column <= (a.Column < 49 ? a.Column + 1 : a.Column)).ToList());
                if (a.Column > 0)
                    a.Neighbors.Add(allAcres.Where(x=>x.Row==a.Row && x.Column==a.Column-1).First());
                if (a.Column < 49)
                    a.Neighbors.Add(allAcres.Where(x => x.Row == a.Row && x.Column == a.Column + 1).First());
            });
            AddResult("Data Setup Complete");
            for (int x=0; x<10; x++)
            {
                allAcres.ForEach(a => a.DetermineShift());
                allAcres.ForEach(a => a.Shift());
            }
            long resourceValue = allAcres.Count(x => x.Type == 1) * allAcres.Count(x => x.Type == 2);
            AddResult("Resource value after 10 iterations: " + resourceValue.ToString());
            ResetProcessTimer(true);

            // ok, let's see if we can find a pattern with a few thousand iterations, first....
            // so reset to initial conditions. 
            basicGrid = new int[50, 50];
            row = 0; column = 0;
            foreach (string processingLine in inputFile)
            {
                column = 0;
                foreach (char c in processingLine)
                {
                    basicGrid[row, column] = (c == '.') ? 0 : ((c == '|') ? 1 : 2);
                    column++;
                }
                row++;
            }
            allAcres = new List<Acre>();
            row = 0; column = 0;
            for (row = 0; row < 50; row++)
            {
                for (column = 0; column < 50; column++)
                {
                    allAcres.Add(new Acre() { Row = row, Column = column, Type = basicGrid[row, column], ImpendingType = -1 });
                }
            }
            allAcres.ForEach(a => {
                if (a.Row > 0)
                    a.Neighbors.AddRange(allAcres.Where(x => x.Row == a.Row - 1 && x.Column >= (a.Column > 0 ? a.Column - 1 : a.Column) && x.Column <= (a.Column < 49 ? a.Column + 1 : a.Column)).ToList());
                if (a.Row < 49)
                    a.Neighbors.AddRange(allAcres.Where(x => x.Row == a.Row + 1 && x.Column >= (a.Column > 0 ? a.Column - 1 : a.Column) && x.Column <= (a.Column < 49 ? a.Column + 1 : a.Column)).ToList());
                if (a.Column > 0)
                    a.Neighbors.Add(allAcres.Where(x => x.Row == a.Row && x.Column == a.Column - 1).First());
                if (a.Column < 49)
                    a.Neighbors.Add(allAcres.Where(x => x.Row == a.Row && x.Column == a.Column + 1).First());
            });
            AddResult("Data Setup Complete");

            // let's do 3000 iterations, and see if we can find a common pattern. 
            List<IterationState> iterations = new List<IterationState>();

            //iterations.Add(new IterationState() { iterationNumber = 0, mapSnapshot = Acre.GetSnapShot(allAcres) });
            // this is the iteration that repeats. It shows up after the initial 480 iterations, so this will be our trigger to modulo out the remaining iterations. 
            iterations.Add(new IterationState() { iterationNumber = -1, mapSnapshot = "0000000000000000000001111111110000000000000000000000000000000000000001111112111111000000000000000000000000000000000001111112222211111100000000000000000000000000000001111112222022221111110000000000000000000000000001111112222000002222111111000000000000000000000001111112222000000000222211111100000000000000000001111112222000000000000022221111110000000000000001111112222000000000000000002222111111000000000001111112222000000000000000000000222211111100000001111112222000000000000000000000000022221111110001111112222000000000000000000000000000002222111111111112222000000000000000000000000000000000222211111112222000000000000000000000000000000000000022221112222000000000000000000110000000000000000000002222222000000000000000000111111111000000000000000000222200000000000000000111111111111100000000000000000000000000000000000111111211111111110000000000000000000000000000000111111222221111111111000000000000000000000000000111111222202222111111111100000000000000000000000111111222200000222211111111110000000000000000000111111222200000000022221111111111000000000000000011111222200000000000002222222211111000000000000001111222200000000000000000222222221110000000000000011122200000000000000000000000002221100000000000001112200000000000000000000000000000200000000000000011122000000000000000000000000000000000000000000001112200000000000000000000000000000000000000000000011122000000000000011000000000000000000000000000001112200000000000011111100000000000000000000000000011122000000000011111111110000000000000000000220001112200000000011111121111111000000000000000222200011122000000000111122222111111100000000000002211001112200000000011122220222211111110000000000022110011122000000000111220000022221111100000000000221101112200000000011122000000002222111100000000002211011122000000000111220000000000222111000000000022110111220000000001112220000000000221111000000000221100111220000000001112200000000000221110000000002211001112200000000011111000000000002211110000000022110001112200000000001110000000000002211100000000022100011122000000000000000000000000022111100000000221000011122000000000000000000000000221110000000002210000111220000000000000001110000022111100000000221100000111220000000000000011120000221110000000002211000001112200000000000001111220222111100000000221110000001112200000000000001112222211110000000002211100000011122000000000000011111211111100000000221111000000011122000000000000011111111110000000002211100000000111220000000000000011111110000000000221111000000000111200000000000000001110000000000002211100" });
            //A map configuration repeated at iteration 480
            //A map configuration repeated at iteration 508
            // so it repeats at this point every 28 iterations. 
            for (int x = 0; x < 600; x++) // 600 is just buffer from 480, it could be 508 at this point, whatever...
            {
                allAcres.ForEach(a => a.DetermineShift());
                allAcres.ForEach(a => a.Shift());
                string thisMapIteration = Acre.GetSnapShot(allAcres);
                // do all the initial iterations until repeating (480, although we COULD reload the map with the repeating condition.....)
                if (iterations.Count(x=>x.mapSnapshot==thisMapIteration)>0)
                {
                    // then reset our iteration counter to the remaining number of iterations to do. 
                    // Target iterations minus the number we've already done, and get the modulo of that from the repeating pattern count. 
                    x = 600 - ((1000000000-x)%28);
                }
                // was adding all the fingerprint maps into the iteration list here, but not needed any more. 
            }
            
            AddResult("After 1000000000 iterations it would be: " + (allAcres.Count(x => x.Type == 1) * allAcres.Count(x => x.Type == 2)).ToString());


        }
    }
    class IterationState
    {
        public string mapSnapshot { get; set; }
        public int iterationNumber { get; set; }

    }
    class Acre
    {
        // 0= open, 1= trees, 2= lumberyard, -1 = unset
        public int Type { get; set; }
        public int ImpendingType { get; set; }
        public List<Acre> Neighbors = new List<Acre>();
        public int Row { get; set; }
        public int Column { get; set; }
        public void Shift()
        {
            if (ImpendingType != -1)
            {
                Type = ImpendingType;
                ImpendingType = -1;
            }
            else { throw new Exception("This Acre was not set to a shift to a new type!"); }

        }
        public void DetermineShift()
        {
            /*
             * An open acre will become filled with trees if three or more adjacent acres contained trees. Otherwise, nothing happens.
             * An acre filled with trees will become a lumberyard if three or more adjacent acres were lumberyards. Otherwise, nothing happens.
             * An acre containing a lumberyard will remain a lumberyard if it was adjacent to at least one other lumberyard and at least one acre containing trees. Otherwise, it becomes open.
            */
            switch (Type)
            {
                case 0: //open
                    ImpendingType = (Neighbors.Count(x=>x.Type==1)>=3) ? 1 : 0;
                    break;
                case 1: //trees
                    ImpendingType = (Neighbors.Count(x => x.Type == 2) >= 3) ? 2 : 1;
                    break;
                case 2: //lumberyard
                    ImpendingType = (Neighbors.Count(x => x.Type == 2) >= 1 && Neighbors.Count(x => x.Type == 1) >= 1) ? 2 : 0;
                    break;
                default:
                    throw new Exception("This Acre's type is not set!");
            }
        }
        public static string GetSnapShot(List<Acre> allAcres)
        {
            StringBuilder mapSnapshot = new StringBuilder("");
            allAcres.OrderBy(x => x.Row).ThenBy(x => x.Column).ToList().ForEach(x => mapSnapshot.Append(x.Type.ToString()));
            return mapSnapshot.ToString();
        }
    }
}
