using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day15_BeverageBandits : AoCodeModule
    {
        public Day15_BeverageBandits()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        public override void DoProcess()
        {

            string finalResult = "Not Set";
            ResetProcessTimer(true);
            BattleDude.map = new char[inputFile[0].Length, inputFile.Count];
            int y = 0;
            foreach (string processingLine in inputFile)
            {
                int x = 0;
                foreach (char mappiece in processingLine)
                {
                    BattleDude.map[x, y] = (mappiece == 'E' || mappiece == 'G') ? '.' : mappiece;
                    if (mappiece == 'E' || mappiece == 'G')
                    {
                        BattleDude.Dudes.Add(new BattleDude(x, y, mappiece == 'E'));
                    }
                    x++;
                }
                y++;
            }
            BattleDude.ReorderDudes();
            int battleRounds = 0;
            BattleDude.Output();
            while (BattleDude.Dudes.Count(x => x.isElf && !x.isDead) > 0 && BattleDude.Dudes.Count(x => !x.isElf && !x.isDead) > 0)
            {
                int dudeCounter = 0;
                while (dudeCounter < BattleDude.Dudes.Count)
                {
                    BattleDude dude = BattleDude.Dudes[dudeCounter];
                    if (!dude.isDead)
                    {
                        dude.TakeTurn();
                    }
                    if (!(BattleDude.Dudes.Count(x => x.isElf && !x.isDead) > 0 && BattleDude.Dudes.Count(x => !x.isElf && !x.isDead) > 0))
                    {
                        break;
                    }
                    dudeCounter++;
                    //BattleDude.Output();
                }

                if (!(BattleDude.Dudes.Count(x => x.isElf && !x.isDead) > 0 && BattleDude.Dudes.Count(x => !x.isElf && !x.isDead) > 0))
                {
                    break;
                }
                // last thing to do since this is FULL rounds. If everyone dies in the middle of a round, it doesn't count
                battleRounds++;
                BattleDude.ReorderDudes();
            }
            int remainingHitPoints = BattleDude.Dudes.Where(x => !x.isDead).Sum(x => x.hitpoints);
            //int remainingHitPoints = BattleDude.Dudes.Sum(x => x.hitpoints);
            finalResult = "Battle Outcome: " + (remainingHitPoints * battleRounds).ToString();
            AddResult(finalResult);
            ResetProcessTimer(true);

        }
    }
    public class BattleDude
    {
        public static char[,] map;
        public static List<BattleDude> Dudes = new List<BattleDude>();
        public List<MapPoint> currentTraversal;
        //public List<MapPoint> currentPathFinding = new List<MapPoint>();
        public List<MapPoint> bestMoves;
        public int x;
        public int y;
        public int hitpoints;
        public int power;
        public bool isElf;
        public bool isDead = false;
        public BattleDude(int xc, int yc, bool elf) : this(xc, yc, elf, 200, 3) { }
        public BattleDude(int xc, int yc, bool elf, int hp, int attack)
        {
            x = xc;y = yc;isElf = elf;hitpoints = hp;power = attack;
        }
        public static void Output() { Output(null,false); }
        public static void Output(MapPoint highlight) { Output(highlight, true); }
        public static void Output(MapPoint highlight,bool pathMarker)
        {
            if (highlight != null) {
                Console.SetCursorPosition(highlight.x, highlight.y);
                if (pathMarker)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("*");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    if (Dudes.Count(dude => dude.x == highlight.x && dude.y == highlight.y) > 0)
                    {
                        Console.Write(Dudes.Where(dude => !dude.isDead && dude.x == highlight.x && dude.y == highlight.y).First().isElf ? "E" : "G");
                    }
                    else
                    {
                        Console.Write(map[highlight.x, highlight.y]);
                    }
                }
                return;
            }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            for (int i = 0; i <= map.GetUpperBound(1); i++)
            {
                for (int j = 0; j <= map.GetUpperBound(0); j++)
                {
                    if (Dudes.Count(dude => !dude.isDead && dude.x == j && dude.y == i) > 0)
                    {
                        Console.BackgroundColor=(highlight!=null && j==highlight.x && i==highlight.y)?ConsoleColor.Red:ConsoleColor.Black;
                        Console.Write(Dudes.Where(dude => !dude.isDead && dude.x == j && dude.y == i) .First().isElf?"E":"G");
                    }
                    else
                    {
                        Console.Write(map[j, i]);
                    }
                }
                Console.WriteLine();
            }
        }
        public static void ReorderDudes()
        {
            Dudes = Dudes.OrderBy(x=>!x.isDead).ThenBy(x => x.y).ThenBy(x => x.x).ToList();
        }
        public void TakeTurn()
        {
            //Console.SetCursorPosition(0, map.GetUpperBound(1) + 3);
            //Console.WriteLine("                                                                                          ");
            //Console.WriteLine("                                                                                          ");
            //Console.WriteLine("                                                                                          ");
            //Console.WriteLine("                                                                                          ");
            //Console.WriteLine("                                                                                          ");
            // unit attacks first if any adjacent sqaure already contains an enemy unit
            if (Dudes.Count(dude => (((dude.x == this.x - 1 || dude.x == this.x + 1) && dude.y == this.y) || ((dude.y == this.y - 1 || dude.y == this.y + 1) && dude.x == this.x)) && !dude.isDead && dude.isElf != this.isElf) > 0)
            {
                //Console.SetCursorPosition(0, map.GetUpperBound(1) + 3);
                //Console.WriteLine((this.isElf ? "Elf" : "Goblin") + " @ " + this.x.ToString() + "," + this.y.ToString() + " has an attack to make.");
                BattleDude weakestEnemy = Dudes.Where(dude => (((dude.x == this.x - 1 || dude.x == this.x + 1) && dude.y == this.y) || ((dude.y == this.y - 1 || dude.y == this.y + 1) && dude.x == this.x)) && !dude.isDead && dude.isElf != this.isElf).OrderBy(dude => dude.hitpoints).ThenBy(dude=>dude.y).ThenBy(dude=>dude.x).First();
                weakestEnemy.hitpoints -= this.power;
                weakestEnemy.isDead = weakestEnemy.hitpoints <= 0;
            }
            else
            {
                //Console.SetCursorPosition(0, map.GetUpperBound(1) + 3);
                //Console.WriteLine((this.isElf ? "Elf" : "Goblin") + " @ " + this.x.ToString() + "," + this.y.ToString() + " is going to move.");
                // move, then possibly attack.
                // first, all possible targets -- not dead, and of the other type.
                List<BattleDude> allAliveEnemies = Dudes.Where(dude => !dude.isDead && dude.isElf != this.isElf).ToList();
                List<Tuple<BattleDude,MapPoint>> enemyAdjacentSpots = new List<Tuple<BattleDude,MapPoint>>();
                if (allAliveEnemies.Count > 0)
                {
                    //Console.WriteLine("There are " + allAliveEnemies.Count.ToString() + " enemies");
                    allAliveEnemies.ForEach(dude =>
                    {
                        List<MapPoint> tests = new List<MapPoint>();
                        tests.Add(new MapPoint(dude.x - 1, dude.y));
                        tests.Add(new MapPoint(dude.x + 1, dude.y));
                        tests.Add(new MapPoint(dude.x, dude.y-1));
                        tests.Add(new MapPoint(dude.x , dude.y+1));
                        tests.ForEach(p =>
                        {
                            if (map[p.x, p.y] == '.' && Dudes.Count(d => !d.isDead && d.x == p.x && d.y == p.y) == 0)
                            {
                                enemyAdjacentSpots.Add(new Tuple<BattleDude,MapPoint>(dude,p));
                            }
                        });
                    });
                    // ok, to sum up... this dude was not already adjacent to an enemy, or else the first code block would take over.
                    // we've gotten all alive enemies, and collected all OPEN points immediately adjacent(non-diagonal) to them.
                    // to quote the puzzle: "If unit is not already in range...and there are no open spaces.. unit ends turn"
                    // so only continue if enemyAdjancentSpots has at least one...
                    if (enemyAdjacentSpots.Count > 0)
                    {
                        //Console.WriteLine("There are " + enemyAdjacentSpots.Count.ToString() + " open spots next to enemies");
                        //time to move. First find REACHABLE spots. This is, quote, based upon CURRENT positions, not predictive on their likely later moves this round.
                        List<Tuple<BattleDude, MapPoint>> reachable = new List<Tuple<BattleDude, MapPoint>>();
                        MapPoint me = new MapPoint(this.x, this.y);
                        bestMoves = new List<MapPoint>();
                        enemyAdjacentSpots.ForEach(p => 
                        {
                            // old way.....
                            //currentTraversal = new List<MapPoint>();
                            ////currentPathFinding = new List<MapPoint>();
                            //FindPath(me, p,22);
                            //if (p.steps > 0) reachable.Add(p);
                            MapPoint path = AStarPath(me, p.Item2);
                            if (path.steps >= 0) { p.Item2.steps = path.steps; reachable.Add(new Tuple<BattleDude, MapPoint>(p.Item1,path)); }
                        });
                        if (reachable.Count > 0)
                        {
                            //Console.WriteLine((this.isElf ? "Elf" : "Goblin") + " @ " + this.x.ToString() + "," + this.y.ToString() + " has a path to " + reachable.Count.ToString() + " of them.");
                            //old Way.....
                            //// ok, we have reachable goals. Find the closest, ties broken by "reading order"
                            //MapPoint goal = reachable.OrderBy(p => p.steps).ThenBy(p => p.y).ThenBy(p => p.x).First();
                            //if (goal.steps == 0)
                            //{
                            //    int wtf = 0;
                            //}
                            //// now find all paths to that goal so we can determine the correct move. 
                            ////List<Path> paths = FindAllPaths(me, goal);
                            //// paths with same step counts (1 down, 1 right vs 1 right, 1 down) get broken by moving in reading order
                            ////Path chosen = paths.OrderBy(p => p.steps.First.Value.y).ThenBy(p => p.steps.First.Value.x).First();
                            //bestMoves = new List<MapPoint>();
                            ////currentPathFinding = new List<MapPoint>();
                            //FindAllPaths(me, goal);
                            //if (bestMoves.Count == 0)
                            //{
                            //    int seriously = 0;
                            //}
                            //MapPoint chosen = bestMoves.OrderBy(m => m.steps).ThenBy(m => m.y).ThenBy(m => m.x).First();
                            // reachable is now a list of FIRST MOVES THAT LEAD TO SOMETHING. The lowest step count automatically takes us to the closest
                            //MapPoint chosen = reachable.OrderBy(m => m.steps).ThenBy(m => m.y).ThenBy(m => m.x).First();
                            MapPoint chosen = reachable.OrderBy(m => m.Item1.hitpoints).ThenBy(m => m.Item2.y).ThenBy(m => m.Item2.x).First().Item2;
                            //Console.WriteLine("The closest one is " + chosen.steps.ToString() + " steps away and the first step to get there is " + chosen.x.ToString() + "," + chosen.y.ToString());
                            this.x = chosen.x;
                            this.y = chosen.y;
                            // Now, if we find ourselves next to an enemy after moving once, ATTACK!
                            if (Dudes.Count(dude => (((dude.x == this.x - 1 || dude.x == this.x + 1) && dude.y == this.y) || ((dude.y == this.y - 1 || dude.y == this.y + 1) && dude.x == this.x)) && !dude.isDead && dude.isElf != this.isElf) > 0)
                            {
                                //Console.WriteLine("I can now attack!");
                                BattleDude weakestEnemy = Dudes.Where(dude => (((dude.x == this.x - 1 || dude.x == this.x + 1) && dude.y == this.y) || ((dude.y == this.y - 1 || dude.y == this.y + 1) && dude.x == this.x)) && !dude.isDead && dude.isElf != this.isElf).OrderBy(dude => dude.hitpoints).ThenBy(dude => dude.y).ThenBy(dude => dude.x).First();
                                weakestEnemy.hitpoints -= this.power;
                                weakestEnemy.isDead = weakestEnemy.hitpoints <= 0;
                            }
                        }
                    }
                }
                else
                {
                    // no alive enemies. This is just a placeholder for doing nothing. Turn is over. 
                }
            }
        }
        public MapPoint AStarPath(MapPoint start, MapPoint goal)
        {
            MapPoint result = new MapPoint(0, 0) { steps = -1 };
            List<MapPoint> pathPoints = new List<MapPoint>();
            pathPoints.Add(goal);
            Queue<MapPoint> toScan = new Queue<MapPoint>();
            int stepCounter = 0;
            goal.steps = stepCounter;
            toScan.Enqueue(goal);
            bool found = false;
            List<MapPoint> potentials = new List<MapPoint>();
            while (!found && toScan.Count>0)
            {
                MapPoint nextPoint = toScan.Dequeue();
                //stepCounter++;
                stepCounter = nextPoint.steps + 1;
                potentials = new List<MapPoint>();
                if ((start.x == nextPoint.x && start.y == nextPoint.y - 1) || (map[nextPoint.x, nextPoint.y - 1] == '.' && Dudes.Count(dude => !dude.isDead && dude.x == nextPoint.x && dude.y == (nextPoint.y - 1)) == 0)) potentials.Add(new MapPoint(nextPoint.x, nextPoint.y - 1) { steps = stepCounter });
                if ((start.x == nextPoint.x - 1 && start.y == nextPoint.y) || (map[nextPoint.x - 1, nextPoint.y] == '.' && Dudes.Count(dude => !dude.isDead && dude.x == (nextPoint.x - 1) && dude.y == nextPoint.y) == 0)) potentials.Add(new MapPoint(nextPoint.x - 1, nextPoint.y) { steps = stepCounter });
                if ((start.x == nextPoint.x + 1 && start.y == nextPoint.y) || (map[nextPoint.x + 1, nextPoint.y] == '.' && Dudes.Count(dude => !dude.isDead && dude.x == (nextPoint.x + 1) && dude.y == nextPoint.y) == 0)) potentials.Add(new MapPoint(nextPoint.x + 1, nextPoint.y) { steps = stepCounter });
                if ((start.x == nextPoint.x && start.y == nextPoint.y + 1) || (map[nextPoint.x, nextPoint.y + 1] == '.' && Dudes.Count(dude => !dude.isDead && dude.x == nextPoint.x && dude.y == (nextPoint.y + 1)) == 0)) potentials.Add(new MapPoint(nextPoint.x, nextPoint.y + 1) { steps = stepCounter });
                foreach (MapPoint test in potentials)
                {
                    if (pathPoints.Count(q => q.x == test.x && q.y == test.y && q.steps <= test.steps) == 0)
                    {
                        pathPoints.Add(test);
                        toScan.Enqueue(test);
                    }

                    pathPoints.RemoveAll(q => q.x == test.x && q.y == test.y && q.steps > test.steps);

                }
                //pathPoints.ForEach(q => Output(q));
                if (pathPoints.Count(q => q.x == start.x && q.y == start.y) > 0)
                {
                    found = true;
                }
            }
            if (found) // vs just run out of stuff to scan.
            {
                result = pathPoints.Where(q =>(
                    (start.x==q.x && (q.y==start.y-1 || q.y==start.y+1)) ||
                    (start.y == q.y && (q.x == start.x - 1 || q.x == start.x + 1))
                )).OrderBy(q=>q.steps).ToList().First();
            }
            return result; 
        }
        public void FindPath(MapPoint start, MapPoint goal) { FindPath(start, goal, -1); }
        public void FindPath(MapPoint start, MapPoint goal, int limitTo)
        {
            if (limitTo >= 0 && start.steps > limitTo) { goal.steps = 0; return; }
            if (start.x == goal.x && start.y == goal.y)
            {
                goal.steps = start.steps;
                return;
            }
            if (currentTraversal.Count(x => x.x == start.x && x.y == start.y) > 0)
            {
                goal.steps = 0;
                return;
            }
            //if (currentPathFinding.Count(x => x.x == start.x && x.y == start.y) > 0)
            //{
            //    goal.steps = 0;
            //    return;
            //}
            //if (currentTraversal.Count == 0)
            //{
            //    currentPathFinding.Clear();
            //}
            //currentPathFinding.Add(start);
            //int myPathPointer = currentPathFinding.Count; // this is technically 1 AFTER "me" in the path, but that's the point I want...
            Output(start);
            // if reachable, goal.steps>0. Only return shortest path ( which may be multiple paths of equally short step counts)
            int minimumSteps = int.MaxValue;
            List<MapPoint> possibleMoves = new List<MapPoint>();
            if (map[start.x, start.y - 1] == '.' && Dudes.Count(dude => !dude.isDead && dude.x == start.x && dude.y == (start.y - 1)) == 0) possibleMoves.Add(new MapPoint(start.x, start.y - 1));
            if (map[start.x-1, start.y ] == '.' && Dudes.Count(dude => !dude.isDead && dude.x == (start.x-1) && dude.y == start.y) == 0) possibleMoves.Add(new MapPoint(start.x-1, start.y));
            if (map[start.x+1, start.y] == '.' && Dudes.Count(dude => !dude.isDead && dude.x == (start.x+1) && dude.y == start.y) == 0) possibleMoves.Add(new MapPoint(start.x+1, start.y ));
            if (map[start.x, start.y + 1] == '.' && Dudes.Count(dude => !dude.isDead && dude.x == start.x && dude.y == (start.y + 1)) == 0) possibleMoves.Add(new MapPoint(start.x, start.y + 1));
            possibleMoves = possibleMoves.OrderBy(move => (Math.Abs(goal.x - move.x) + Math.Abs(goal.y - move.y))).ToList(); 
            possibleMoves.ForEach(move =>
            {

                //currentPathFinding.RemoveRange(myPathPointer, currentPathFinding.Count - myPathPointer);


                move.steps = start.steps + 1;
                currentTraversal.Add(start);
                FindPath(move, goal,Math.Min(limitTo,minimumSteps));
                Output(currentTraversal[currentTraversal.Count - 1],false);
                currentTraversal.RemoveAt(currentTraversal.Count - 1);
                if (goal.steps > 0)
                {
                    
                    minimumSteps = Math.Min(goal.steps, minimumSteps);
                    if (goal.steps == minimumSteps && currentTraversal.Count == 0)
                    {
                        move.steps = minimumSteps;
                        bestMoves.Add(move);
                        bestMoves = bestMoves.Where(m => m.steps <= minimumSteps).ToList();
                        //currentPathFinding = new List<MapPoint>();
                    }
                }
            });
            if (minimumSteps == int.MaxValue) minimumSteps = 0;
            goal.steps = minimumSteps;

        }
        public List<Path> FindAllPaths(MapPoint start, MapPoint goal)
        {
            // limit to the number of steps in goal
            List<Path> paths = new List<Path>();
            //currentPathFinding.Clear();
            FindPath(start, goal, goal.steps);
            return paths;
        }
    }
    public class MapPoint
    {
        public int x;
        public int y;
        public int steps;
        public MapPoint(int xc, int yc)
        {
            x = xc;y = yc;
        }

    }
    public class Path
    {
        public LinkedList<MapPoint> steps = new LinkedList<MapPoint>();
    }
}
