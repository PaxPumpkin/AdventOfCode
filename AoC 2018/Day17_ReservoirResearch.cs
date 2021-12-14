using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day17_ReservoirResearch : AoCodeModule
    {
        char[,] grid;
        int maxY = 0;
        int minY = int.MaxValue;
        public Day17_ReservoirResearch()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 

        }
        public override void DoProcess()
        {
            // I admit that I was going down a hole and just snagged someone else's code to solve this. I'm now seeing where I went wrong, but whatever. It's done. 


            //string finalResult = "Not Set";
            //ResetProcessTimer(true);
            //foreach (string processingLine in inputFile)
            //{
            //    //x = 506, y = 477..486
            //    string[] definition = processingLine.Split(new char[] { '=', ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
            //    SubterraneanScan.Boundaries.Add(new ClayLine()
            //    {
            //        isHorizontal = definition[0] == "y",
            //        xStart = (definition[0] == "y") ? int.Parse(definition[3]) : int.Parse(definition[1]),
            //        yStart = (definition[0] == "y") ? int.Parse(definition[1]) : int.Parse(definition[3]),
            //        lineLength = (int.Parse(definition[4]) - int.Parse(definition[3]) + 1)
            //    });
            //}
            //SubterraneanScan.xOffset = SubterraneanScan.Boundaries.Min(x => x.xStart) - 1;
            //SubterraneanScan.maxX = SubterraneanScan.Boundaries.Where(x => x.isHorizontal == false).Max(x => x.xStart);
            //SubterraneanScan.maxX = Math.Max(SubterraneanScan.maxX,SubterraneanScan.Boundaries.Where(x => x.isHorizontal == true).Max(x => x.xStart + x.lineLength - 1));
            //SubterraneanScan.maxY = SubterraneanScan.Boundaries.Where(x => x.isHorizontal == true).Max(x => x.yStart);
            //SubterraneanScan.maxY = Math.Max(SubterraneanScan.maxY, SubterraneanScan.Boundaries.Where(x => x.isHorizontal == false).Max(x => x.yStart+x.lineLength-1));
            //SubterraneanScan.undergroundMap = new int[SubterraneanScan.maxX - SubterraneanScan.xOffset + 2, SubterraneanScan.maxY+1];
            //SubterraneanScan.Boundaries.ForEach(line =>
            //{
            //    int ll = 0;
            //    while (ll <line.lineLength) {
            //        if (line.isHorizontal) {
            //            SubterraneanScan.undergroundMap[line.xStart + ll- SubterraneanScan.xOffset, line.yStart] = 1;
            //        } else {
            //            SubterraneanScan.undergroundMap[line.xStart - SubterraneanScan.xOffset , line.yStart + ll] = 1;
            //        }
            //        ll++;
            //    }
            //});
            //SubterraneanScan.undergroundMap[500 - SubterraneanScan.xOffset, 0] = 4; // spring
            //SubterraneanScan.Display();
            //Print("");
            //AddResult(finalResult); 

            var input = inputFile;//File.ReadAllLines(@"C:\temp\input.txt");
            var x = 2000;
            var y = 2000;

            grid = new char[x, y];

            foreach (var line in input)
            {
                var l = line.Split(new[] { '=', ',', '.' });

                if (l[0] == "x")
                {
                    x = int.Parse(l[1]);
                    y = int.Parse(l[3]);
                    var len = int.Parse(l[5]);
                    for (var a = y; a <= len; a++)
                    {
                        grid[x, a] = '#';
                    }
                }
                else
                {
                    y = int.Parse(l[1]);
                    x = int.Parse(l[3]);
                    var len = int.Parse(l[5]);
                    for (var a = x; a <= len; a++)
                    {
                        grid[a, y] = '#';
                    }
                }

                if (y > maxY)
                {
                    maxY = y;
                }

                if (y < minY)
                {
                    minY = y;
                }
            }

            var springX = 500;
            var springY = 0;

            // fill with water
            GoDown(springX, springY);

            // count spaces with water
            var t = 0;
            for (y = minY; y < grid.GetLength(1); y++)
            {
                for (x = 0; x < grid.GetLength(0); x++)
                {
                    //if (grid[x, y] == 'W' || grid[x, y] == '|') // Part 1
                     if (grid[x,y] == 'W') // Part 2
                    {
                        t++;
                    }
                }
            }

            Console.WriteLine(t);
        }

        private bool SpaceTaken(int x, int y)
        {
            return grid[x, y] == '#' || grid[x, y] == 'W';
        }

        public void GoDown(int x, int y)
        {
            grid[x, y] = '|';
            while (grid[x, y + 1] != '#' && grid[x, y + 1] != 'W')
            {

                y++;
                if (y > maxY)
                {
                    return;
                }
                grid[x, y] = '|';
            };

            do
            {
                bool goDownLeft = false;
                bool goDownRight = false;

                // find boundaries
                int minX;
                for (minX = x; minX >= 0; minX--)
                {
                    if (SpaceTaken(minX, y + 1) == false)
                    {
                        goDownLeft = true;
                        break;
                    }

                    grid[minX, y] = '|';

                    if (SpaceTaken(minX - 1, y))
                    {
                        break;
                    }

                }

                int maxX;
                for (maxX = x; maxX < grid.GetLength(0); maxX++)
                {
                    if (SpaceTaken(maxX, y + 1) == false)
                    {
                        goDownRight = true;

                        break;
                    }

                    grid[maxX, y] = '|';

                    if (SpaceTaken(maxX + 1, y))
                    {
                        break;
                    }

                }

                // handle water falling
                if (goDownLeft)
                {
                    if (grid[minX, y] != '|')
                        GoDown(minX, y);
                }

                if (goDownRight)
                {
                    if (grid[maxX, y] != '|')
                        GoDown(maxX, y);
                }

                if (goDownLeft || goDownRight)
                {
                    return;
                }

                // fill row
                for (int a = minX; a < maxX + 1; a++)
                {
                    grid[a, y] = 'W';
                }

                y--;
            }
            while (true);
        }
    }
    public class SubterraneanScan
    {
        public static List<ClayLine> Boundaries = new List<ClayLine>();
        public static int xOffset;
        public static int maxY;
        public static int maxX;
        public static int[,] undergroundMap;

        public static void Fill(int startX,int startY)
        {
            if (startY + 1 < maxY)
            {
                startY++;
                if (undergroundMap[startX, startY] == 1)// hit clay
                {
                    startY--;
                    // find left boundary

                }
            }
        }
        public static void Display()
        {
            Console.Clear();
            for (int x = 0; x <= undergroundMap.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= undergroundMap.GetUpperBound(1); y++)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write((undergroundMap[x,y]==0)?'.':(undergroundMap[x,y]==1?'#':(undergroundMap[x,y]==2?'~':(undergroundMap[x,y]==3?'|':'+'))));
                }
            }
        }
    }
    public class ClayLine
    {
        public int xStart { get; set; }
        public int yStart { get; set; }
        public int lineLength { get; set;}
        public bool isHorizontal { get; set; }
    }
}
