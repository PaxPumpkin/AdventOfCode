using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class FractalArt : AoCodeModule
    {
        public FractalArt()
        {
            inputFileName = "fractalart.txt";
            base.GetInput();
        }

        public override void DoProcess()
        {
            char[,] startingGrid= new char[3,3];
            char[,] transformingGrid;
            int x=0; int y=0;
            ".#.".ToList<char>().ForEach(c => { startingGrid[x, y] = c; y++; }); y = 0; x++;
            "..#".ToList<char>().ForEach(c => { startingGrid[x, y] = c; y++; }); y = 0; x++;
            "###".ToList<char>().ForEach(c => { startingGrid[x, y] = c; y++; }); y = 0; x = 0;
            List<PatternFilter> rules = new List<PatternFilter>();
            inputFile.ForEach(rule => rules.Add(new PatternFilter(rule)));

            /*
            // proving that my flipflops work right.
            char[,] test;
            PatternFilter.Print(startingGrid);
            test = PatternFilter.RotateRight(startingGrid);
            PatternFilter.Print(test);
            test = PatternFilter.RotateRight(test);
            PatternFilter.Print(test);
            test = PatternFilter.RotateRight(test);
            PatternFilter.Print(test);
            test = PatternFilter.RotateRight(test); // should result in the starting grid
            PatternFilter.Print(test);
            test = PatternFilter.Flip(startingGrid, true);
            PatternFilter.Print(test);
            test = PatternFilter.Flip(startingGrid, false);
            PatternFilter.Print(test);
            */
            // TESTING 2 iterations ONLY! RESET ALL THE RULES and use the ones that they gave as an example
            //rules = new List<PatternFilter>();
            //rules.Add(new PatternFilter("../.# => ##./#../..."));
            //rules.Add(new PatternFilter(".#./..#/### => #..#/..../..../#..#"));

            for (int iteration = 0; iteration < 18; iteration++) // five iterations for part 1(answer 125), two for test data, 18 iterations for part 2 (answer is 1782917)
            {
                Console.WriteLine("Starting Iteration: " + iteration.ToString());
                int patternSize = ((startingGrid.GetUpperBound(0)+1 )% 2 == 0) ? 2 : 3; // always by 2 or 3
                int numberOfPatterns = (startingGrid.GetUpperBound(0)+1) / patternSize; // in each row, but since it's a square, we can use the same # for iteration of columns/rows
                int newGridSize = numberOfPatterns * (patternSize+1); // patterns input size of 2 always comes out with a new grid size of 3.
                transformingGrid = new char[newGridSize, newGridSize];
                List<PatternFilter> matchedRules = rules.Where(r => r.TwoCharPattern == (patternSize == 2)).ToList();
                char[,] patternPiece;
                for (x = 0; x < numberOfPatterns; x ++)  
                {
                    for (y = 0; y < numberOfPatterns; y ++)
                    {
                        patternPiece = PatternFilter.GetPatternAt(startingGrid, x*patternSize, y*patternSize, patternSize);
                        bool gotIt=false;
                        Tuple<bool, char[,]> result;
                        for (int ruleCounter = 0; ruleCounter < matchedRules.Count && !gotIt; ruleCounter++)
                        {

                            // for each rotation, try it, then try it flipped left and then up/down
                            for (int rotation = 0; rotation < 4; rotation++)
                            {
                                if (rotation > 0) patternPiece = PatternFilter.RotateRight(patternPiece); // only rotate after testing the unrotated pattern
                                result = PatternFilter.Transform(patternPiece, matchedRules[ruleCounter]);
                                gotIt = result.Item1;
                                // this is some CHEAP ASS CODE. Maybe one day I'll optimize it. 
                                if (gotIt)
                                {
                                    transformingGrid = PatternFilter.InsertResult(transformingGrid, x * (patternSize + 1), y * (patternSize + 1), result.Item2);
                                    break;
                                }
                                else
                                {
                                    result = PatternFilter.Transform(PatternFilter.Flip(patternPiece, false), matchedRules[ruleCounter]);
                                    gotIt = result.Item1;
                                    if (gotIt)
                                    {
                                        transformingGrid = PatternFilter.InsertResult(transformingGrid, x * (patternSize + 1), y * (patternSize + 1), result.Item2);
                                    }
                                    else
                                    {
                                        result = PatternFilter.Transform(PatternFilter.Flip(patternPiece, true), matchedRules[ruleCounter]);
                                        gotIt = result.Item1;
                                        if (gotIt)
                                        {
                                            transformingGrid = PatternFilter.InsertResult(transformingGrid, x * (patternSize + 1), y * (patternSize + 1), result.Item2);
                                        }
                                    }
                                }
                            }
                            

                        }
                    }
                }
                startingGrid = transformingGrid;
            }
            //Console.WriteLine("Resulting grid: ");
           // PatternFilter.Print(startingGrid);
            FinalOutput.Add("Lit after iterations: " + PatternFilter.LitCount(startingGrid));
            
            
        }
    }
    class PatternFilter
    {
        public bool TwoCharPattern = false;
        public char[,] pattern;
        public char[,] output;
        public PatternFilter(string rule)
        {
            string[] pieces = rule.Split(new string[] { " => " }, StringSplitOptions.RemoveEmptyEntries);
            string[] inputPattern = pieces[0].Split(new char[] { '/' });
            string[] outputPattern = pieces[1].Split(new char[] { '/' });
            this.pattern = new char[inputPattern.Length, inputPattern.Length];
            this.output = new char[outputPattern.Length, outputPattern.Length];
            int y = 0;
            for (int x = 0; x < inputPattern.Length; x++)
            {
                inputPattern[x].ToList<char>().ForEach(c => { this.pattern[x, y] = c; y++; });
                y = 0;
            }
            for (int x = 0; x < outputPattern.Length; x++)
            {
                outputPattern[x].ToList<char>().ForEach(c => { this.output[x, y] = c; y++; });
                y = 0;
            }
            TwoCharPattern=(inputPattern.Length == 2);
        }
        public static Tuple<bool, char[,]> Transform(char[,] inputPattern, PatternFilter rule)
        {
            bool matched = false;
            char[,] result = inputPattern;
            // will be a 2x2 or 3x3 input array.
            if (PatternFilter.PatternMatch(inputPattern, rule.pattern))
            {
                matched = true;
                result = rule.output;
            }
            return new Tuple<bool, char[,]>(matched, result);
        }
        public static bool PatternMatch(char[,] doesThis, char[,] matchThat)
        {
            if (doesThis.Length != matchThat.Length) return false;
            bool failure = false;
            for (int x = 0; x <= doesThis.GetUpperBound(0) && !failure; x++)
            {
                for (int y = 0; y <= doesThis.GetUpperBound(1) && !failure; y++)
                {
                    failure = doesThis[x, y] != matchThat[x, y];
                }
            }

            return !failure;
        }
        public static char[,] GetPatternAt(char[,] wholeThing, int x, int y, int patternSize)
        {
            char[,] result = new char[patternSize,patternSize];
            int resultx = 0;
            int resulty = 0; 
            for (int i=x; i<x+patternSize; i++){
                for (int j=y; j<y+patternSize; j++){
                    result[resultx, resulty] = wholeThing[i, j];
                    resulty++;
                }
                resultx++;
                resulty = 0;
            }
            return result;
        }
        public static char[,] RotateRight(char[,] inputArray)
        {
            int size = inputArray.GetUpperBound(0) + 1; // upper bound is 2? the size for new array should be 3
            char[,] outputArray = new char[size, size];
            int x2 = 0;
            int y2 = size - 1;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    outputArray[x2, y2] = inputArray[x, y];
                    x2++;
                }
                y2--; x2 = 0;
            }

            return outputArray;
        }
        public static char[,] Flip(char[,] inputArray, bool yAxis)
        {
            int size = inputArray.GetUpperBound(0) + 1; // upper bound is 2? the size for new array should be 3
            char[,] outputArray = new char[size, size];
            int x2 = size - 1;
            int y2=x2;
            if (yAxis)//left/right flip
            {
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        outputArray[x, y2] = inputArray[x, y];
                        y2--;
                    }
                    y2 = x2;
                }
            }
            else // top/bottom flip
            {
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        outputArray[y2, y] = inputArray[x, y];
                        
                    }
                    y2--;
                }
            }

            return outputArray;
        }
        public static string Print(char[,] inputPattern)
        {
            StringBuilder line = new StringBuilder();
            StringBuilder theWholeShow = new StringBuilder();
            line.Append("".PadLeft(inputPattern.GetUpperBound(0) + 3, '-'));
            Console.WriteLine(line.ToString());
            theWholeShow.AppendLine(line.ToString());
            line = new StringBuilder();

            for (int x = 0; x <= inputPattern.GetUpperBound(0); x++)
            {
                line.Append("|");
                for (int y = 0; y <= inputPattern.GetUpperBound(1); y++)
                {
                    line.Append(inputPattern[x, y]);
                }
                line.Append("|");
                Console.WriteLine(line.ToString());
                theWholeShow.AppendLine(line.ToString());
                line = new StringBuilder();
            }
            line.Append("".PadLeft(inputPattern.GetUpperBound(0) + 3, '-'));
            Console.WriteLine(line.ToString());
            theWholeShow.AppendLine(line.ToString());
            Console.WriteLine("");
            return theWholeShow.ToString();
        }
        //PatternFilter.InsertResult(transformingGrid, x / patternSize, y / patternSize, result.Item2);
        public static char[,] InsertResult(char[,] resultGrid, int atX, int atY, char[,] patternToInsert)
        {
            for (int x = 0; x <= patternToInsert.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= patternToInsert.GetUpperBound(1); y++)
                {
                    //now this doesn't work.
                    resultGrid[atX+x, atY+y] = patternToInsert[x, y];
                }
            }
            return resultGrid;
        }
        public static string LitCount(char[,] grid)
        {
            int litcounter = 0;
            for (int x = 0; x <= grid.GetUpperBound(0); x++)
            {
                for (int y=0; y<=grid.GetUpperBound(1); y++)
                    litcounter+=(grid[x,y]=='#')?1:0;
            }

            return litcounter.ToString();
        }
    }
}
