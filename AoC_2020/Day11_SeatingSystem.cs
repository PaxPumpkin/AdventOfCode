using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day11_SeatingSystem : AoCodeModule
    {
        public Day11_SeatingSystem()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day11_SeatingSystem part 1:Data Setup Complete(Process: 4621 ms)
            //** > Result for Day11_SeatingSystem part 2:After 116 iterations...(Process: 456 ms)
            //** > Result for Day11_SeatingSystem part 2:The number of occupied seats is: 2126(Process: 456 ms)
            //** > Result for Day11_SeatingSystem part 3:Data Reset Complete(Process: 5333 ms)
            //** > Result for Day11_SeatingSystem part 4:After 84 iterations...(Process: 328 ms)
            //** > Result for Day11_SeatingSystem part 4:The number of occupied seats is: 1914(Process: 328 ms)
                        

            ResetProcessTimer(true);
            int[,] basicGrid = new int[inputFile.Count,inputFile[0].Length];
            int row = 0, column = 0;
            foreach (string processingLine in inputFile)
            {
                column = 0;
                foreach (char c in processingLine)
                {
                    basicGrid[row, column] = (c == '.') ? 0 : ((c == 'L') ? 1 : 2);
                    column++;
                }
                row++;
            }
            List<SeatingSpace> allSpaces = new List<SeatingSpace>();
            row = 0; column = 0;
            for (row = 0; row <= basicGrid.GetUpperBound(0); row++)
            {
                for (column = 0; column <= basicGrid.GetUpperBound(1); column++)
                {
                    allSpaces.Add(new SeatingSpace() { Row = row, Column = column, Type = basicGrid[row, column], ImpendingType = -1 });
                }
            }
            allSpaces.ForEach(a => {
                if (a.Row > 0)
                    a.Neighbors.AddRange(allSpaces.Where(x => x.Row == a.Row - 1 && x.Column >= (a.Column > 0 ? a.Column - 1 : a.Column) && x.Column <= (a.Column < basicGrid.GetUpperBound(1) ? a.Column + 1 : a.Column)).ToList());
                if (a.Row < basicGrid.GetUpperBound(0))
                    a.Neighbors.AddRange(allSpaces.Where(x => x.Row == a.Row + 1 && x.Column >= (a.Column > 0 ? a.Column - 1 : a.Column) && x.Column <= (a.Column < basicGrid.GetUpperBound(1) ? a.Column + 1 : a.Column)).ToList());
                if (a.Column > 0)
                    a.Neighbors.Add(allSpaces.Where(x => x.Row == a.Row && x.Column == a.Column - 1).First());
                if (a.Column < basicGrid.GetUpperBound(1))
                    a.Neighbors.Add(allSpaces.Where(x => x.Row == a.Row && x.Column == a.Column + 1).First());
            });
            AddResult("Data Setup Complete");
            ResetProcessTimer(true);
            bool completed = false;
            string lastIteration = SeatingSpace.GetSnapShot(allSpaces), thisIteration="";
            int counter = 0;
            while (!completed)
            {
                allSpaces.ForEach(a => a.DetermineShift());
                allSpaces.ForEach(a => a.Shift());
                counter++;
                thisIteration = SeatingSpace.GetSnapShot(allSpaces);
                completed = thisIteration.Equals(lastIteration);
                lastIteration = thisIteration;
            }
            AddResult("After " + counter.ToString() + " iterations...");
            long occupied = allSpaces.Count(x => x.Type == 2);
            AddResult("The number of occupied seats is: " + occupied.ToString());
            ResetProcessTimer(true);
            allSpaces.Clear();
            row = 0; column = 0;
            for (row = 0; row <= basicGrid.GetUpperBound(0); row++)
            {
                for (column = 0; column <= basicGrid.GetUpperBound(1); column++)
                {
                    allSpaces.Add(new SeatingSpace() { Row = row, Column = column, Type = basicGrid[row, column], ImpendingType = -1 });
                }
            }
            SeatingSpace potentialSpace;
            bool found = false;
            int lookingRow, lookingColumn;
            allSpaces.ForEach(a => {
                found = false;
                lookingRow = a.Row; lookingColumn = a.Column;
                // look "up"
                while (!found && lookingRow >= 0)
                {
                    lookingRow--;
                    if (lookingRow >= 0)
                    {
                        potentialSpace = allSpaces.Where(x => x.Row == lookingRow && x.Column == lookingColumn && x.Type == 1).FirstOrDefault();
                        if (potentialSpace != null)
                        {
                            a.Neighbors.Add(potentialSpace);
                            found = true;
                        }
                    }

                }

                found = false;
                lookingRow = a.Row; lookingColumn = a.Column;
                // look "down"
                while (!found && lookingRow <= basicGrid.GetUpperBound(0))
                {
                    lookingRow++;
                    if (lookingRow <= basicGrid.GetUpperBound(0))
                    {
                        potentialSpace = allSpaces.Where(x => x.Row == lookingRow && x.Column == lookingColumn && x.Type == 1).FirstOrDefault();
                        if (potentialSpace != null)
                        {
                            a.Neighbors.Add(potentialSpace);
                            found = true;
                        }
                    }

                }

                found = false;
                lookingRow = a.Row; lookingColumn = a.Column;
                // look "right"
                while (!found && lookingColumn <= basicGrid.GetUpperBound(1))
                {
                    lookingColumn++;
                    if (lookingColumn <= basicGrid.GetUpperBound(1))
                    {
                        potentialSpace = allSpaces.Where(x => x.Row == lookingRow && x.Column == lookingColumn && x.Type == 1).FirstOrDefault();
                        if (potentialSpace != null)
                        {
                            a.Neighbors.Add(potentialSpace);
                            found = true;
                        }
                    }

                }

                found = false;
                lookingRow = a.Row; lookingColumn = a.Column;
                // look "left"
                while (!found && lookingColumn >=0)
                {
                    lookingColumn--;
                    if (lookingColumn >=0)
                    {
                        potentialSpace = allSpaces.Where(x => x.Row == lookingRow && x.Column == lookingColumn && x.Type == 1).FirstOrDefault();
                        if (potentialSpace != null)
                        {
                            a.Neighbors.Add(potentialSpace);
                            found = true;
                        }
                    }

                }

                found = false;
                lookingRow = a.Row; lookingColumn = a.Column;
                // look "diagonal up/left"
                while (!found && lookingColumn >= 0 && lookingRow>=0)
                {
                    lookingColumn--;
                    lookingRow--;
                    if (lookingColumn >= 0 && lookingRow>=0)
                    {
                        potentialSpace = allSpaces.Where(x => x.Row == lookingRow && x.Column == lookingColumn && x.Type == 1).FirstOrDefault();
                        if (potentialSpace != null)
                        {
                            a.Neighbors.Add(potentialSpace);
                            found = true;
                        }
                    }

                }

                found = false;
                lookingRow = a.Row; lookingColumn = a.Column;
                // look "diagonal down/left"
                while (!found && lookingColumn >= 0 && lookingRow <= basicGrid.GetUpperBound(1))
                {
                    lookingColumn--;
                    lookingRow++;
                    if (lookingColumn >= 0 && lookingRow <= basicGrid.GetUpperBound(1))
                    {
                        potentialSpace = allSpaces.Where(x => x.Row == lookingRow && x.Column == lookingColumn && x.Type == 1).FirstOrDefault();
                        if (potentialSpace != null)
                        {
                            a.Neighbors.Add(potentialSpace);
                            found = true;
                        }
                    }

                }

                found = false;
                lookingRow = a.Row; lookingColumn = a.Column;
                // look "diagonal up/right"
                while (!found && lookingColumn <= basicGrid.GetUpperBound(0) && lookingRow >= 0)
                {
                    lookingColumn++;
                    lookingRow--;
                    if (lookingColumn<= basicGrid.GetUpperBound(0) && lookingRow >= 0)
                    {
                        potentialSpace = allSpaces.Where(x => x.Row == lookingRow && x.Column == lookingColumn && x.Type == 1).FirstOrDefault();
                        if (potentialSpace != null)
                        {
                            a.Neighbors.Add(potentialSpace);
                            found = true;
                        }
                    }

                }

                found = false;
                lookingRow = a.Row; lookingColumn = a.Column;
                // look "diagonal down/right"
                while (!found && lookingColumn <=basicGrid.GetUpperBound(1) && lookingRow <= basicGrid.GetUpperBound(0))
                {
                    lookingColumn++;
                    lookingRow++;
                    if (lookingColumn <= basicGrid.GetUpperBound(1) && lookingRow <= basicGrid.GetUpperBound(0))
                    {
                        potentialSpace = allSpaces.Where(x => x.Row == lookingRow && x.Column == lookingColumn && x.Type == 1).FirstOrDefault();
                        if (potentialSpace != null)
                        {
                            a.Neighbors.Add(potentialSpace);
                            found = true;
                        }
                    }

                }
            });
            AddResult("Data Reset Complete");
            ResetProcessTimer(true);
            completed = false;
            lastIteration = SeatingSpace.GetSnapShot(allSpaces); thisIteration = "";
             counter = 0;
            while (!completed)
            {
                allSpaces.ForEach(a => a.DetermineShiftRule2());
                allSpaces.ForEach(a => a.Shift());
                counter++;
                thisIteration = SeatingSpace.GetSnapShot(allSpaces);
                completed = thisIteration.Equals(lastIteration);
                lastIteration = thisIteration;
            }
            AddResult("After " + counter.ToString() + " iterations...");
            occupied = allSpaces.Count(x => x.Type == 2);
            AddResult("The number of occupied seats is: " + occupied.ToString());
        }
    }
    class SeatingSpace
    {
        // 0= floor, 1= empty seat, 2= occupied seat, -1 = unset
        public int Type { get; set; }
        public int ImpendingType { get; set; }
        public List<SeatingSpace> Neighbors = new List<SeatingSpace>();
        public int Row { get; set; }
        public int Column { get; set; }
        public void Shift()
        {
            if (ImpendingType != -1)
            {
                Type = ImpendingType;
                ImpendingType = -1;
            }
            else { throw new Exception("This Seating Space was not set to a shift to a new type!"); }

        }
        public void DetermineShift()
        {
            /*
             * If a seat is empty (L) and there are no occupied seats adjacent to it, the seat becomes occupied.
             * If a seat is occupied (#) and four or more seats adjacent to it are also occupied, the seat becomes empty.
             * Otherwise, the seat's state does not change.
             * Floor (.) never changes; seats don't move, and nobody sits on the floor
            */
            switch (Type)
            {
                case 0: //floor
                    ImpendingType = 0;
                    break;
                case 1: //empty
                    ImpendingType = (Neighbors.Count(x => x.Type == 2) == 0) ? 2 : 1;
                    break;
                case 2: //occupied
                    ImpendingType = (Neighbors.Count(x => x.Type == 2) >= 4 ) ? 1 : 2;
                    break;
                default:
                    throw new Exception("This Seating Area's type is not set!");
            }
        }

        public void DetermineShiftRule2()
        {
            /*
             * If a seat is empty (L) and there are no occupied seats adjacent to it, the seat becomes occupied.
             * If a seat is occupied (#) and four or more seats adjacent to it are also occupied, the seat becomes empty.
             * Otherwise, the seat's state does not change.
             * Floor (.) never changes; seats don't move, and nobody sits on the floor
            */
            switch (Type)
            {
                case 0: //floor
                    ImpendingType = 0;
                    break;
                case 1: //empty
                    ImpendingType = (Neighbors.Count(x => x.Type == 2) == 0) ? 2 : 1;
                    break;
                case 2: //occupied
                    ImpendingType = (Neighbors.Count(x => x.Type == 2) >= 5) ? 1 : 2;
                    break;
                default:
                    throw new Exception("This Seating Area's type is not set!");
            }
        }
        public static string GetSnapShot(List<SeatingSpace> allSpaces)
        {
            StringBuilder mapSnapshot = new StringBuilder("");
            allSpaces.OrderBy(x => x.Row).ThenBy(x => x.Column).ToList().ForEach(x => mapSnapshot.Append(x.Type.ToString()));
            return mapSnapshot.ToString();
        }
    }
}
