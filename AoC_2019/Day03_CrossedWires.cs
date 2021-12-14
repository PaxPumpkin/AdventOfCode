using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2019
{
    class Day03_CrossedWires : AoCodeModule
    {
        int stepsAtFirstIntersection1 = 0, stepsAtFirstIntersection2 = 0, lowestStepsToIntersect=Int32.MaxValue;
        List<WireGridCoordinate> grid = new List<WireGridCoordinate>();
        int[,] newGrid = new int[10000, 10000];
        public Day03_CrossedWires()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
        }
        public override void DoProcess()
        {
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            int x = 0;
            
            //GetMaxXYBoundaries(inputFile);
            foreach (string processingLine in inputFile)
            {
                x++;
                //ParseWire(processingLine, x);
                //ParseWire2(processingLine, x);
                ParseWireFatCode(processingLine, x);
            }
            int md = 12000;
            for (int xx = 0; xx < 10000; xx++)
            {
                for (int y=0; y<10000; y++)
                {
                    int value = newGrid[xx, y]&3;
                    if (value==3)
                    {
                        int testx, testy;
                        if (xx >= 6000)
                        {
                            testx = xx - 6000;
                        }
                        else
                        {
                            testx = 6000 - xx;
                        }
                        if (y >= 6000)
                        {
                            testy = y - 6000;
                        }
                        else
                        {
                            testy = 6000 - y;
                        }
                        if ((testx + testy) < md) md = testx+testy;
                    }
                }
            }
            finalResult = "Smallest Manhatten distance is " + md.ToString();
            //finalResult = "Smallest Manhatten distance is " + grid.Where(gc=>gc.hasWire1 && gc.hasWire2).Min(gc=>Math.Abs(gc.x)+Math.Abs(gc.y)).ToString();
            AddResult(finalResult);
            ResetProcessTimer(true);
            finalResult = "Fewest combined steps to first intersection is " + (lowestStepsToIntersect).ToString();
            AddResult(finalResult);
        }
        void GetMaxXYBoundaries(List<string> inp)
        {
            int lowestX = 0, highestX = 0, lowestY = 0, highestY = 0;
            foreach(string definition in inp)
            {
                int x = 0, y = 0;
                List<string> directions = definition.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                directions.ForEach(direction =>
                {
                    int distance = Convert.ToInt32(direction.Substring(1));
                    switch (direction[0])
                    {
                        case 'D':
                            y-=distance;
                            break;
                        case 'U':
                            y+=distance;
                            break;
                        case 'R':
                            x+=distance;
                            break;
                        case 'L':
                            x-=distance;
                            break;
                        default:
                            throw new Exception("Direction value unparseable");
                    }
                    lowestX = Math.Min(lowestX, x);
                    lowestY = Math.Min(lowestY, y);
                    highestX = Math.Max(highestX, x);
                    highestY = Math.Max(highestY, y);
                });
            }
            Console.WriteLine("HighestX" + highestX.ToString());
            Console.WriteLine("LowestX" + lowestX.ToString());
            Console.WriteLine("HighestY" + highestY.ToString());
            Console.WriteLine("LowestY" + lowestY.ToString());

        }
        void ParseWire(string path, int wireNumber)
        {
            List<string> directions = path.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int x = 0, y = 0;
            char dir; int distance;
            if (grid.Count(gc => gc.x==x && gc.y == y) == 0) { grid.Add(new WireGridCoordinate() { x = x, y = y }); }
            WireGridCoordinate nextCoordinate;
            directions.ForEach(direction => 
            {
                dir = direction[0];
                distance = Convert.ToInt32(direction.Substring(1));
                for(int steps=0; steps<distance; steps++)
                {
                    switch (dir)
                    {
                        case 'D':
                            y--;
                            break;
                        case 'U':
                            y++;
                            break;
                        case 'R':
                            x++;
                            break;
                        case 'L':
                            x--;
                            break;
                        default:
                            throw new Exception("Direction value unparseable");
                    }
                    nextCoordinate = (grid.Count(gc => gc.x == x && gc.y == y) == 0) ? (new WireGridCoordinate() { x = x, y = y }) : grid.Where(gc=>gc.x == x && gc.y == y).First();
                    if (wireNumber == 1) nextCoordinate.hasWire1 = true; else nextCoordinate.hasWire2 = true;
                    if (grid.Count(gc => gc.x == x && gc.y == y) == 0) grid.Add(nextCoordinate);
                }
            });
        }
        void ParseWire2(string path, int wireNumber)
        {
            List<string> directions = path.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int x = 6000, y = 6000;
            char dir; int distance, totalSteps = 0;
            directions.ForEach(direction =>
            {
                dir = direction[0];
                distance = Convert.ToInt32(direction.Substring(1));
                switch (dir)
                {
                    case 'D':
                        for (int c = 0; c < distance; c++) { y--; totalSteps++;
                            newGrid[x, y] = newGrid[x, y] | (((wireNumber==1 && newGrid[x,y]==0)?(totalSteps*4):0)|wireNumber); if (wireNumber == 2 && (newGrid[x, y] & 3) == 3)
                            { stepsAtFirstIntersection2 = totalSteps; stepsAtFirstIntersection1 = ((newGrid[x, y] ^ 3) / 4); lowestStepsToIntersect = Math.Min(lowestStepsToIntersect, (stepsAtFirstIntersection1 + stepsAtFirstIntersection2)); } }
                        break;
                    case 'U':
                        for (int c = 0; c < distance; c++) { y++; totalSteps++;
                            newGrid[x, y] = newGrid[x, y] | (((wireNumber == 1 && newGrid[x, y] == 0) ? (totalSteps * 4) : 0) | wireNumber); if (wireNumber == 2 && (newGrid[x, y] & 3) == 3) 
                            { stepsAtFirstIntersection2 = totalSteps; stepsAtFirstIntersection1 = ((newGrid[x, y] ^ 3) / 4); lowestStepsToIntersect = Math.Min(lowestStepsToIntersect, (stepsAtFirstIntersection1 + stepsAtFirstIntersection2)); }
                        }
                        break;
                    case 'R':
                        for (int c = 0; c < distance; c++) { x++; totalSteps++;
                            newGrid[x, y] = newGrid[x, y] | (((wireNumber == 1 && newGrid[x, y] == 0) ? (totalSteps * 4) : 0) | wireNumber); if (wireNumber == 2 && (newGrid[x, y] & 3) == 3) 
                            { stepsAtFirstIntersection2 = totalSteps; stepsAtFirstIntersection1 = ((newGrid[x, y] ^ 3) / 4); lowestStepsToIntersect = Math.Min(lowestStepsToIntersect, (stepsAtFirstIntersection1 + stepsAtFirstIntersection2)); }
                        }
                        break;
                    case 'L':
                        for (int c = 0; c < distance; c++) { x--; totalSteps++;
                            newGrid[x, y] = newGrid[x, y] | (((wireNumber == 1 && newGrid[x, y] == 0) ? (totalSteps * 4) : 0) | wireNumber); if (wireNumber == 2 && (newGrid[x, y] & 3) == 3) 
                            { stepsAtFirstIntersection2 = totalSteps; stepsAtFirstIntersection1 = ((newGrid[x, y] ^ 3) / 4); lowestStepsToIntersect = Math.Min(lowestStepsToIntersect, (stepsAtFirstIntersection1 + stepsAtFirstIntersection2)); }
                        }
                        break;
                    default:
                        throw new Exception("Direction value unparseable");
                }
            });
            string bullshit = "";
        }

        void ParseWireFatCode(string path, int wireNumber)
        {
            List<string> directions = path.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int x = 6000, y = 6000;
            char dir; int distance, totalSteps = 0, gridValue, newValue, combinedStepsToThisIntersection, wire1StepsToThisIntersection;
            directions.ForEach(direction =>
            {
                dir = direction[0];
                distance = Convert.ToInt32(direction.Substring(1));
                // the plan is to bit-flag the grid space with the wire number as the wire passes through it by "or"-ing the wire number in.
                // wire 1 will bit mask 01, wire 2 will use 10
                // Then, we need to know IF, for WIRE ONE, whether or not this is the FIRST TIME it has been to this coordinate
                // (bit masking an int count ABOVE the flags, so... multiply by 4 to shift it, but.... might just <<2 it over instead)
                // That way, when WIRE TWO intersects it, we get the lowest number of steps from wire one.
                switch (dir)
                {
                    case 'D':
                        for (int c = 0; c < distance; c++)
                        {
                            y--; totalSteps++;
                            gridValue = newGrid[x, y];
                            // no need to bit-mask in the "1" since wire one has been here before, Doing it again changes nothing,  
                            // and if it is a second(or third) time, we don't want to change the number of steps, either.
                            // The puzzle says to use the FIRST number of steps that the wires enter the space.
                            if (wireNumber == 1 && gridValue==0) 
                            {
                                newValue = (totalSteps << 2) | 1;
                                newGrid[x, y] = newValue;
                            }
                            // for wire two, we are NOT saving the number of steps it took to get here. 
                            // But instead, if it encounters a space where wire one has already been, we will get the number of steps
                            // it took wire one to get there (the first time, from above), and add them together with how many steps
                            // it took wire two to get here. 
                            // Also, if wire 1 hasn't been here before, it technically doesn't matter about anything. We only need
                            // to know the intersecions
                            if (wireNumber == 2 && gridValue!=0)// wire 1 has been here!
                            {
                                newValue = gridValue | 2; // for puzzle one, we still have to find all intersections, so that's important to save still
                                newGrid[x, y] = newValue;

                                wire1StepsToThisIntersection = gridValue >> 2; // shift the wire bitflags out, should be left with just the number of steps it took wire 1 to get here.
                                combinedStepsToThisIntersection = wire1StepsToThisIntersection + totalSteps;
                                // if the summation to this point is lower than the last one we kept record of, change it.
                                if (combinedStepsToThisIntersection < lowestStepsToIntersect) { lowestStepsToIntersect = combinedStepsToThisIntersection; }
                            }
                        }
                        break;
                    case 'U':
                        for (int c = 0; c < distance; c++)
                        {
                            y++; totalSteps++;
                            gridValue = newGrid[x, y];
                            // no need to bit-mask in the "1" since wire one has been here before, Doing it again changes nothing,  
                            // and if it is a second(or third) time, we don't want to change the number of steps, either.
                            // The puzzle says to use the FIRST number of steps that the wires enter the space.
                            if (wireNumber == 1 && gridValue == 0)
                            {
                                newValue = (totalSteps << 2) | 1;
                                newGrid[x, y] = newValue;
                            }
                            // for wire two, we are NOT saving the number of steps it took to get here. 
                            // But instead, if it encounters a space where wire one has already been, we will get the number of steps
                            // it took wire one to get there (the first time, from above), and add them together with how many steps
                            // it took wire two to get here. 
                            // Also, if wire 1 hasn't been here before, it technically doesn't matter about anything. We only need
                            // to know the intersecions
                            if (wireNumber == 2 && gridValue != 0)// wire 1 has been here!
                            {
                                newValue = gridValue | 2; // for puzzle one, we still have to find all intersections, so that's important to save still
                                newGrid[x, y] = newValue;

                                wire1StepsToThisIntersection = gridValue >> 2; // shift the wire bitflags out, should be left with just the number of steps it took wire 1 to get here.
                                combinedStepsToThisIntersection = wire1StepsToThisIntersection + totalSteps;
                                // if the summation to this point is lower than the last one we kept record of, change it.
                                if (combinedStepsToThisIntersection < lowestStepsToIntersect) { lowestStepsToIntersect = combinedStepsToThisIntersection; }
                            }
                        }
                        break;
                    case 'R':
                        for (int c = 0; c < distance; c++)
                        {
                            x++; totalSteps++;
                            gridValue = newGrid[x, y];
                            // no need to bit-mask in the "1" since wire one has been here before, Doing it again changes nothing,  
                            // and if it is a second(or third) time, we don't want to change the number of steps, either.
                            // The puzzle says to use the FIRST number of steps that the wires enter the space.
                            if (wireNumber == 1 && gridValue == 0)
                            {
                                newValue = (totalSteps << 2) | 1;
                                newGrid[x, y] = newValue;
                            }
                            // for wire two, we are NOT saving the number of steps it took to get here. 
                            // But instead, if it encounters a space where wire one has already been, we will get the number of steps
                            // it took wire one to get there (the first time, from above), and add them together with how many steps
                            // it took wire two to get here. 
                            // Also, if wire 1 hasn't been here before, it technically doesn't matter about anything. We only need
                            // to know the intersecions
                            if (wireNumber == 2 && gridValue != 0)// wire 1 has been here!
                            {
                                newValue = gridValue | 2; // for puzzle one, we still have to find all intersections, so that's important to save still
                                newGrid[x, y] = newValue;

                                wire1StepsToThisIntersection = gridValue >> 2; // shift the wire bitflags out, should be left with just the number of steps it took wire 1 to get here.
                                combinedStepsToThisIntersection = wire1StepsToThisIntersection + totalSteps;
                                // if the summation to this point is lower than the last one we kept record of, change it.
                                if (combinedStepsToThisIntersection < lowestStepsToIntersect) { lowestStepsToIntersect = combinedStepsToThisIntersection; }
                            }
                        }
                        break;
                    case 'L':
                        for (int c = 0; c < distance; c++)
                        {
                            x--; totalSteps++;
                            gridValue = newGrid[x, y];
                            // no need to bit-mask in the "1" since wire one has been here before, Doing it again changes nothing,  
                            // and if it is a second(or third) time, we don't want to change the number of steps, either.
                            // The puzzle says to use the FIRST number of steps that the wires enter the space.
                            if (wireNumber == 1 && gridValue == 0)
                            {
                                newValue = (totalSteps << 2) | 1;
                                newGrid[x, y] = newValue;
                            }
                            // for wire two, we are NOT saving the number of steps it took to get here. 
                            // But instead, if it encounters a space where wire one has already been, we will get the number of steps
                            // it took wire one to get there (the first time, from above), and add them together with how many steps
                            // it took wire two to get here. 
                            // Also, if wire 1 hasn't been here before, it technically doesn't matter about anything. We only need
                            // to know the intersecions
                            if (wireNumber == 2 && gridValue != 0)// wire 1 has been here!
                            {
                                newValue = gridValue | 2; // for puzzle one, we still have to find all intersections, so that's important to save still
                                newGrid[x, y] = newValue;

                                wire1StepsToThisIntersection = gridValue >> 2; // shift the wire bitflags out, should be left with just the number of steps it took wire 1 to get here.
                                combinedStepsToThisIntersection = wire1StepsToThisIntersection + totalSteps;
                                // if the summation to this point is lower than the last one we kept record of, change it.
                                if (combinedStepsToThisIntersection < lowestStepsToIntersect) { lowestStepsToIntersect = combinedStepsToThisIntersection; }
                            }
                        }
                        break;
                    default:
                        throw new Exception("Direction value unparseable");
                }
            });
            string bullshit = "";
        }

        class WireGridCoordinate
        {
            public int x;
            public int y;
            public bool hasWire1 = false;
            public bool hasWire2 = false;
        }
    }
}
