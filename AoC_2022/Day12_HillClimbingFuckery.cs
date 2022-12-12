using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day12_HillClimbingFuckery : AoCodeModule
    {
        public Day12_HillClimbingFuckery()
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
            // totally stole someone's code. I can't do pathfinding, so I got one and modified it to work (had to implement priority queue functions)
            ResetProcessTimer(true);
            Y2022D12 thingy = new Y2022D12();
            
            AddResult(thingy.SolvePartOne(inputFile).ToString());
            ResetProcessTimer(true);
            AddResult(thingy.SolvePartTwo(inputFile).ToString());
            ResetProcessTimer(true);
        }
    }



    public class Y2022D12 
    {
        
        public class Cell
        {
            public char Value { get; }
            public int X { get; }
            public int Y { get; }

            public Cell(char value, int x, int y)
            {
                Value = value;
                X = x;
                Y = y;
            }
        }

        private class Graph
        {
            public List<Cell> Cells { get; } = new List<Cell>();

            public Graph(char[,] grid)
            {
                for (int row = 0; row <= grid.GetUpperBound(0); row++)
                {
                    for (int col = 0; col <= grid.GetUpperBound(1); col++)
                    {
                        Cells.Add(new Cell(grid[row,col], col, row));
                    }
                }
            }

            public void PrintPath(List<Cell> path)
            {
                Dictionary<int, HashSet<int>> cellsOnPath = path.GroupBy(c => c.Y)
                                                                .ToDictionary(
                                                                    g => g.Key, g => new HashSet<int>(g.Select(c => c.X)));

                foreach (var cell in Cells)
                {
                    if (cell.X == 0)
                    {
                        Console.WriteLine();
                    }

                    if (cellsOnPath.ContainsKey(cell.Y) && cellsOnPath[cell.Y].Contains(cell.X))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(cell.Value);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(cell.Value);
                    }
                }

                Console.WriteLine();
            }

            public IEnumerable<Cell> GetNeighbors(Cell cell)
            {
                return Cells.Where(c => c.Y == cell.Y && Math.Abs(cell.X - c.X) == 1 ||
                                        c.X == cell.X && Math.Abs(cell.Y - c.Y) == 1);
            }

            public List<Cell> AStar(Cell start, Cell end, Func<Cell, Cell, int> heuristic,
                                    Func<Cell, Cell, int?> edgeWeight)
            {
                var openSet = new PriorityQueue();//<Cell, int>();
                openSet.Enqueue(new QueueItem() { cell = start, priority = 0 });

                Dictionary<Cell, Cell> cameFrom = new Dictionary<Cell,Cell>();

                Dictionary<Cell, int> gScore = new Dictionary<Cell,int>()
                {
                    { start, 0 }
                };

                Dictionary<Cell, int> fScore = new Dictionary<Cell, int>()
                {
                    { start, heuristic(start, end) }
                };

                while (openSet.Count > 0)
                {
                    var current = openSet.Dequeue();
                    if (current.cell == end)
                    {
                        return ReconstructPath(cameFrom, current.cell);
                    }

                    foreach (Cell neighbor in GetNeighbors(current.cell))
                    {
                        var weight = edgeWeight(current.cell, neighbor);
                        if (weight is null)
                        {
                            continue;
                        }

                        var tentativeGScore = gScore[current.cell] + weight.Value;
                        if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                        {
                            cameFrom[neighbor] = current.cell;
                            gScore[neighbor] = tentativeGScore;
                            fScore[neighbor] = tentativeGScore + heuristic(neighbor, end);
                            openSet.Enqueue(new QueueItem() { cell = neighbor, priority = fScore[neighbor] });
                        }
                    }
                }

                return new List<Cell>();
            }

            private List<Cell> ReconstructPath(Dictionary<Cell, Cell> cameFrom, Cell current)
            {
                List<Cell> path = new List<Cell>() { current };
                while (cameFrom.ContainsKey(current))
                {
                    current = cameFrom[current];
                    path.Add(current);
                }

                path.Reverse();
                return path;
            }
        }

        private int EffectiveValue(char c)
        {
            char r;
            switch (c) {
                case 'S':
                    r = 'a';
                    break;
                case 'E':
                    r = 'z';
                    break;
                default:
                    r = c;
                    break;
            }
            return r;
        }

        public object SolvePartOne(List<string> input)
        {
            char[,] grid = new char[input.Count, input.First().Length];

            int rowCounter = 0;
            foreach(string line in input)
            {
                int colCounter = 0;
                foreach (char c in line)
                {
                    grid[rowCounter, colCounter] = c;
                    colCounter++;
                }
                rowCounter++;
            }
            var graph = new Graph(grid);
            var path = graph.AStar(graph.Cells.Single(c => c.Value == 'S'),
                                   graph.Cells.Single(c => c.Value == 'E'),
                                   (curr, end) => Math.Abs(curr.X - end.X) + Math.Abs(curr.Y - end.Y),
                                   (curr, neighbor) =>
                                   {
                                       var nEff = EffectiveValue(neighbor.Value);
                                       var cEff = EffectiveValue(curr.Value);

                                       var score = nEff > cEff + 1
                                           ? null
                                           : (int?)cEff - nEff + 1;

                                       return score;
                                   });

            //graph.PrintPath(path);
            return "Shortest path from S to E is " + (path.Count - 1).ToString();
        }

        public object SolvePartTwo(List<string> input)
        {
            char[,] grid = new char[input.Count, input.First().Length];

            int rowCounter = 0;
            foreach (string line in input)
            {
                int colCounter = 0;
                foreach (char c in line)
                {
                    grid[rowCounter, colCounter] = c;
                    colCounter++;
                }
                rowCounter++;
            }
            var graph = new Graph(grid);

            var hikeEnd = graph.Cells.Single(c => c.Value == 'E');
            var possibleStarts = graph.Cells
                                      .Where(c => c.Value == 'a')
                                      .Where(c => graph.GetNeighbors(c).Any(n => n.Value == 'b'))
                                      .ToList();

            Console.WriteLine($"{possibleStarts.Count} possible starts");

            var paths = new List<List<Cell>>();
            int startCounter = 0;
            foreach (var start in possibleStarts)
            {
                startCounter++;
                var path = graph.AStar(start,
                                       hikeEnd,
                                       (curr, end) => Math.Abs(curr.X - end.X) + Math.Abs(curr.Y - end.Y),
                                       (curr, neighbor) =>
                                       {
                                           var nEff = EffectiveValue(neighbor.Value);
                                           var cEff = EffectiveValue(curr.Value);

                                           var score = nEff > cEff + 1
                                               ? null
                                               : (int?)cEff - nEff + 1;

                                           return score;
                                       });

                Console.WriteLine($"{startCounter}:Start at {start.X},{start.Y}, path = {path.Count}");

                if (path.Count > 0)
                {
                    paths.Add(path);
                }
            }

            var shortest = paths.OrderBy(p => p.Count).First();
            //graph.PrintPath(shortest);
            return $"Start at {shortest.First().X},{shortest.First().Y}, path = {shortest.Count - 1}";
        }
        public class QueueItem
        {
            public Cell cell;
            public int priority;
        }
        public class PriorityQueue 
        {
            private List<QueueItem> _pq = new List<QueueItem>();

            public void Enqueue(QueueItem item)
            {
                _pq.Add(item);
                _pq = _pq.OrderBy(x => x.priority).ToList();
            }

            public QueueItem Dequeue()
            {
                var item = _pq[0];
                _pq.RemoveAt(0);

                return item;
            }
            public int Count { get { return _pq.Count; } }
        }
    }





}
