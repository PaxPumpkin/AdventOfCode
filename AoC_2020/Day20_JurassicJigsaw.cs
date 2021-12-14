using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day20_JurassicJigsaw : AoCodeModule
    {
        public static List<SatelliteImage> placedImages = new List<SatelliteImage>();
        public Day20_JurassicJigsaw()
        {
            
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            //inputFileName = @"InputFiles\" + this.GetType().Name + "Sample.txt";
            GetInput(); 
            OutputFileReset();
            
        }
        public override void DoProcess()
        {
            //** > Result for Day20_JurassicJigsaw part 1:All four corners multiplied together = 4006801655873(Process: 101.3731 ms)
            //** > Result for Day20_JurassicJigsaw part 2:The number of sea monsters is 21 and the choppy sea count is 1838(Process: 5167.6138 ms)
            ResetProcessTimer(true);
            List<string> imageFile = new List<string>();
            List<SatelliteImage> images = new List<SatelliteImage>();
            long imageNumber = 0;
            long result = 1;
            foreach (string processingLine in inputFile)
            {
                if (processingLine.StartsWith("Tile"))
                {
                    imageFile.Clear();
                    imageNumber = long.Parse(processingLine.Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries)[1]);
                }
                else if (processingLine.Trim() != "")
                {
                    imageFile.Add(processingLine.Trim());
                }
                else
                {
                    images.Add(new SatelliteImage(imageNumber, imageFile.ToArray()));
                    imageNumber = -1;
                    imageFile.Clear();
                }
            }
            // iterator for each image, check matches with all other images (not itself, which will get a 4-sided match, natch )
            images.ForEach(si => images.Where(other => other.imageID != si.imageID).ToList().ForEach(other => si.MatchWith(other)));
            // corners only have two matches, so multiply the values of the images that only have two matches. 
            images.Where(si => si.myMatches.Count == 2).ToList().ForEach(si => result *= si.imageID);
            AddResult("All four corners multiplied together = " + result.ToString());
            ResetProcessTimer(true);
            // some double-checking the results....
            List<SatelliteImage> corners = images.Where(si => si.myMatches.Count == 2).ToList();
            List<SatelliteImage> edges = images.Where(si => si.myMatches.Count == 3).ToList();
            // 144 images... should be 4 corners and 40 edges.
            List<SatelliteImage> fillers = images.Where(si => si.myMatches.Count == 4).ToList();
            // this leaves 100 center pieces. OK, this checks out. 
            // we should be able to build a basic frame since we know the corners and edges and they each say who they match to.

            //adjusting for doing this with sample data.... grrrrrr....
            int bigGridSize = (int)Math.Sqrt(images.Count);
            //SatelliteImage[,] grid = new SatelliteImage[12, 12]; // to do sample data, a 12/12 grid isn't gonna break anything, but I wanna do it right.
            SatelliteImage[,] grid = new SatelliteImage[bigGridSize, bigGridSize];
            // at the moment, I'm not sure of orientation, so it may not matter and we can copy out later once it's figured or
            // adjust our scheme for iteration. 
            // For now, just pick a corner, and go through the directions
            grid[0, 0] = corners[0];
            placedImages.Add(grid[0, 0]); 
            corners[0].ExpandEdges(grid, 0, 0, images); // expands up or down and then left or right as applicable. Does both, though, from the corner.
            // we now have the grid filled on two edges, and it contains 3 of the 4 corners. Find the corner that isn't represented and expand those edges.
            grid[bigGridSize-1, bigGridSize-1] = corners.Where(corner => corner.imageID != grid[0, 0].imageID && corner.imageID != grid[bigGridSize-1, 0].imageID && corner.imageID != grid[0, bigGridSize-1].imageID).First();
            placedImages.Add(grid[bigGridSize-1, bigGridSize-1]);
            grid[bigGridSize-1, bigGridSize-1].ExpandEdges(grid, bigGridSize-1, bigGridSize-1, images);
            // now to fill in all the middle. Lotta fluff in here, but i'm just trying to chug through it. 
            Dictionary<long, int> foundSoFar = new Dictionary<long, int>();
            //for (int x=1; x<11; x++)// lines 0 and 11 are already filled. now do the ones in the middle. 
            for (int x = 1; x < bigGridSize-1; x++)// lines 0 and 11 are already filled. now do the ones in the middle. 
            {
                foundSoFar.Clear();
                grid[x, 0].ExpandLine(grid, x, 0, images);
                placedImages.Select(img => img.imageID).ToList().ForEach(f => { if (foundSoFar.ContainsKey(f)) { throw new Exception("Duplicate detected"); } else { foundSoFar.Add(f, 0); } });
                
            }
            PrintGrid(grid); // this is just indexing the rows/columns with an image number... again, for a lot of debuggering and sanity checking.
            foundSoFar.Clear();
            placedImages.Select(img => img.imageID).ToList().ForEach(f => { if (foundSoFar.ContainsKey(f)) { throw new Exception("Duplicate detected"); } else { foundSoFar.Add(f, 0); } });
            // ok, if we get here, everything should be filled in and there are zero re-used satellite images. 
            // now all we have to do it iterate through them all and flip/rotate their image data to actually match the orientation necessary. 
            // NOTE FOR LATER... if we find any sea monsters at all, but the answers are wrong, it may be that the overall grid orientation needs to be flipped/rotated... 
            // I just saw that in the description. sheesh. 

            for (int x = 0; x < bigGridSize; x++) 
            {
                for (int y=0; y< bigGridSize; y++)
                {
                    // ok, one by one, tell each satellite image to orient itself such that it will match properly with the image left, right, above, and/or below it in the grid as has already been figured
                    grid[x, y].OrientImageData(grid, x, y, images);
                }
            }

            // now we have to build the image all together without borders
            // the edges don't count any more. so instead of 10x10 image data, it's 8x8
            // so that makes an overall "image" of 96x96 "pixels" for our input data.
            TotalImage bigImage = new TotalImage(grid);
            for (int x = 0; x < bigGridSize; x++)
            {
                for (int y = 0; y < bigGridSize; y++)
                {
                    bigImage.AddBlock(grid[x, y], x, y); //strips out edges/spaces as per spec.
                }
            }
            // mark out all the sea monster matches. Can sea monsters overlap, I wonder? ( answer, didn't find any, but I allowed for it )
            bigImage.FindSeaMonsters(); // also count the # symbols that are left. That's chop/rough seas, whatever he called it.


            AddResult("The number of sea monsters is " + bigImage.SeaMonsters.ToString() + " and the choppy sea count is " + bigImage.JustChop.ToString());
            //1886 is too high.
        }
        public void PrintGrid(SatelliteImage[,] grid) // just indices of image ids in the correct places. NOT the contents of the images.
        {
            Print("");
            for (int x = 0; x <= grid.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= grid.GetUpperBound(1); y++)
                {
                    Console.Write("(" + (x < 10 ? " " : "") + x.ToString() + "," + (y < 10 ? " " : "") + y.ToString() + ") " + (grid[x, y] != null ? grid[x, y].imageID.ToString() : "0000") + "  ");
                }
                Print("");
            }
            Print("");
        }
    }
    class TotalImage
    {
        public char[,] overallGrid = new char[96, 96];
        public char[,] overallGridOriented = new char[96, 96]; // holder for fiddling with orientation
        public int SeaMonsters = 0;
        public int JustChop = 0;
        public TotalImage()
        {

        }
        public TotalImage(SatelliteImage[,] grid)
        {
            int dimensions = (grid.GetUpperBound(0)+1)*8;
            overallGrid = new char[dimensions, dimensions]; //base image
            overallGridOriented = new char[dimensions, dimensions]; //playground for flipping/rotating.
        }
        public void AddBlock(SatelliteImage img, int x, int y)
        {
            x *= 8;
            y *= 8;
            for (int xoffset=0; xoffset<8; xoffset++)
            {
                for (int yoffset=0; yoffset<8; yoffset++)
                {
                    overallGrid[x + xoffset, y + yoffset] = img.imageData[xoffset + 1][yoffset + 1];
                }
            }
        }
        public void FindSeaMonsters()
        {
            // will have to search for the sea monster pattern. 
            // I guess, it's said, that if none are found, we will have to flip/rotate the image, too
            // and again, I'm assuming that we will only find sea monsters at all when it is properly oriented. I hope.
            //                  # 
            //#    ##    ##    ###
            // #  #  #  #  #  #   

            bool fullyFiddledWith = false;
            overallGridOriented = (char[,])overallGrid.Clone(); //can't COPYTO with multidimensional arrays in this version of C#, and if I assign directly, it's a reference pointer... so it changes the original.
            char[,] holder = (char[,])overallGrid.Clone(); ;
            bool foundOne = false;
            int rotationCount = 0; //debuggering flags, unnecessary for solving. This is just so I can see what is happening. 
            bool flipped = false;
            JustChop = 0;
            SeaMonsters = 0;
            while (SeaMonsters== 0 && !fullyFiddledWith) // bool flag to prevent perpetual execution. 
            {
                //gonna check all four rotation orientations, once as-is, then again flipped. 
                // unless we find sea monsters!
                for (int inversions = 0; inversions < 2 && SeaMonsters == 0; inversions++)
                {
                    rotationCount = 0;
                    for (int rotations = 0; rotations < 4 && SeaMonsters == 0; rotations++)
                    {
                        // store our current orientation in the holder for graphical manipulation
                        // doing this in case sea monsters CAN overlap, I don't want to arrange it in our matching space
                        holder = (char[,])overallGridOriented.Clone();
                        // search by blocks of 3 lines, so gotta start at x=2
                        // the sea monster is 20 chars long, so y only iterates to 76 ( y + [0-19] )
                        //for (int x = 2; x < 96; x++)
                        for (int x = 2; x <= overallGridOriented.GetUpperBound(0); x++)
                        {
                            //for (int y = 0; y <= 76; y++)
                            for (int y = 0; y <= overallGridOriented.GetUpperBound(1) -19; y++)
                            {
                                foundOne = false;
                                //find pattern in overallGridORIENTED . doing this longhand for now, will golf it down later if it works at all.
                                //     OR maybe not. Why? I HATE REGEX.
                                if (overallGridOriented[x - 1, y] == '#') // first sign of sea-monster!
                                {
                                    if (overallGridOriented[x , y + 1] == '#')
                                    {
                                        if (overallGridOriented[x, y + 4] == '#')
                                        {
                                            if (overallGridOriented[x - 1, y + 5] == '#' && overallGridOriented[x - 1, y + 6] == '#')
                                            {
                                                if (overallGridOriented[x, y + 7] == '#' & overallGridOriented[x, y + 10] == '#')
                                                {
                                                    if (overallGridOriented[x - 1, y + 11] == '#' && overallGridOriented[x - 1, y + 12] == '#')
                                                    {
                                                        if (overallGridOriented[x, y + 13] == '#' & overallGridOriented[x, y + 16] == '#')
                                                        {
                                                            if (overallGridOriented[x - 1, y + 17] == '#' && overallGridOriented[x - 1, y + 18] == '#' && overallGridOriented[x - 1, y + 19] == '#')
                                                            {
                                                                if (overallGridOriented[x-2, y + 18] == '#')
                                                                {
                                                                    foundOne = true;
                                                                    SeaMonsters++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                // Replace the pattern with Os in the holder array if full pattern matches
                                if (foundOne)
                                {
                                    holder[x - 1, y] = 'O';
                                    holder[x , y+1] = 'O';
                                    holder[x, y+4] = 'O';
                                    holder[x - 1, y+5] = 'O';
                                    holder[x - 1, y+6] = 'O';
                                    holder[x , y+7] = 'O';
                                    holder[x , y+10] = 'O';
                                    holder[x - 1, y+11] = 'O';
                                    holder[x - 1, y+12] = 'O';
                                    holder[x, y+13] = 'O';
                                    holder[x, y+16] = 'O';
                                    holder[x - 1, y+17] = 'O';
                                    holder[x - 1, y+18] = 'O';
                                    holder[x - 1, y+19] = 'O';
                                    holder[x - 2, y+18] = 'O';
                                }
                            }
                        } // one rotation check.
                        if (SeaMonsters == 0)
                        {
                            Rotate();
                            rotationCount++;
                        }
                    }// all rotations checked.
                    if (SeaMonsters == 0)
                    {
                        Invert();
                        flipped = true;
                    }
                }// both inversions and all rotations checked. set flag to prevent infinite running. 
                fullyFiddledWith = true;
            }
            if (SeaMonsters > 0)
            {
                OutputSeaMonsters(holder);// prints to console the FULL image contents, and uses the holder array that has the sea monster chars changed.

                // iterate through and count the choppy sea tiles.
                for (int x = 0; x <= holder.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= holder.GetUpperBound(1); y++)
                    {
                        if (holder[x, y] == '#')
                        {
                            JustChop++;
                        }
                    }
                }
            }
        }
        public void Rotate() // rotates the full image grid.
        {      
            char[,] holder = new char[96, 96];
            holder = (char[,])overallGridOriented.Clone();
            for (int x = 0; x <= holder.GetUpperBound(0); x++)
            {
                for (int y = holder.GetUpperBound(1); y >= 0; y--)
                {
                    holder[overallGrid.GetUpperBound(0) - y, overallGrid.GetUpperBound(0) - x] = overallGridOriented[x, overallGrid.GetUpperBound(0) - y];
                }
            }
            overallGridOriented = (char[,])holder.Clone();
            //PrintComparison(); //outputs original and rotated versions side-by-side to the console for sanity checking. 
        }
        public void Invert() // mirror-flips the grid.
        {
            for (int x = 0; x <= overallGridOriented.GetUpperBound(0); x++)
            {
                for (int y = overallGridOriented.GetUpperBound(1); y >= 0; y--)
                {
                    overallGridOriented[x, y] = overallGrid[x, overallGrid.GetUpperBound(0) - y];
                }
            }
            //PrintComparison(); //outputs original and rotated versions side-by-side to the console for sanity checking. 
        }
        public void PrintComparison() // sanity-checking the rotation/inversion code. I did it differently for the whole grid vs individual image objects.
        {
            Console.WriteLine("");
            char[] line = (new string(' ', 197)).ToCharArray();
            for (int x = 0; x < 96; x++) 
            {
                line = (new string(' ', 197)).ToCharArray();
                for (int y=0; y<96; y++)
                {
                    line[y] = overallGrid[x, y];
                    line[y + 100] = overallGridOriented[x, y];
                }
                Console.WriteLine(line);
            }
            Console.WriteLine("");
        }
        public void OutputSeaMonsters(char[,] markedUp) // prints full image with color markers for the sea monster spaces.
        {
            Console.WriteLine("");
            ConsoleColor original = Console.ForegroundColor;
            for (int x = 0; x <= markedUp.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= markedUp.GetUpperBound(1); y++)
                {
                    Console.ForegroundColor = markedUp[x, y] == 'O' ? ConsoleColor.Blue : original;
                    Console.Write(markedUp[x, y]);
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
            Console.ResetColor();
        }
    }

    class SatelliteImage
    {
        public enum MatchSides { Top, Right, Bottom, Left }
        public long imageID = 0;
        public string[] imageData = new string[10];
        public string[] orientedImageData = new string[10];
        public string TOP = "";
        public string BOTTOM = "";
        public string LEFT = "";
        public string RIGHT = "";
        public int rotated = 0;
        public bool flipped = false;

        public Dictionary<MatchSides,long> myMatches = new Dictionary<MatchSides,long>();
        public SatelliteImage(long number, string[] image)
        {
            if (image.Length == 0) throw new Exception("0-Length image file. Cannot Parse.");
            imageID = number;
            image.CopyTo(imageData, 0);
            TOP = image[0];
            BOTTOM = image[image.Length - 1];
            for (int x=0; x<image.Length; x++)
            {
                LEFT += image[x][0];
                RIGHT += image[x][image[0].Length - 1];
            }
            for (int x = 0; x < 10; x++)
            {
                orientedImageData[x] = "          ";//10 spaces .string(' ',10) woulda worked. Duh. 
            }
        }
        public void MatchWith(SatelliteImage otherImage)
        {
            // this is fucked up and hard to read, but it works....  doesn't change the orientation of the original image since we don't know yet where it goes in the overall BIG image.
            if (TOP == otherImage.BOTTOM || TOP == reverse(otherImage.TOP) || TOP == reverse(otherImage.BOTTOM) || TOP == otherImage.TOP) { myMatches.Add(MatchSides.Top, otherImage.imageID); }
            if (BOTTOM == otherImage.TOP || BOTTOM == reverse(otherImage.BOTTOM) || BOTTOM == reverse(otherImage.TOP) || BOTTOM == otherImage.BOTTOM) { myMatches.Add(MatchSides.Bottom, otherImage.imageID); }
            if (LEFT == otherImage.RIGHT || LEFT == reverse(otherImage.LEFT) || LEFT == reverse(otherImage.RIGHT) || LEFT==otherImage.LEFT) { myMatches.Add(MatchSides.Left, otherImage.imageID); }
            if (RIGHT == otherImage.LEFT || RIGHT == reverse(otherImage.RIGHT) || RIGHT == reverse(otherImage.LEFT) || RIGHT==otherImage.RIGHT) { myMatches.Add(MatchSides.Right, otherImage.imageID); }

            if (TOP == otherImage.RIGHT || TOP == reverse(otherImage.RIGHT) || TOP == reverse(otherImage.LEFT) || TOP == otherImage.LEFT) { myMatches.Add(MatchSides.Top, otherImage.imageID); }
            if (BOTTOM == otherImage.RIGHT || BOTTOM == reverse(otherImage.RIGHT) || BOTTOM == reverse(otherImage.LEFT) || BOTTOM == otherImage.LEFT) { myMatches.Add(MatchSides.Bottom, otherImage.imageID); }
            if (LEFT == otherImage.TOP || LEFT == reverse(otherImage.TOP) || LEFT == reverse(otherImage.BOTTOM) || LEFT==otherImage.BOTTOM) { myMatches.Add(MatchSides.Left, otherImage.imageID); }
            if (RIGHT == otherImage.TOP || RIGHT == reverse(otherImage.TOP) || RIGHT == reverse(otherImage.BOTTOM) || RIGHT==otherImage.BOTTOM) { myMatches.Add(MatchSides.Right, otherImage.imageID); }
        }
        public static string reverse(string s)// helper function... the char[] reverse wasn't working right as an in-line statement, so...
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        public void ExpandEdges(SatelliteImage[,] grid, int myX, int myY, List<SatelliteImage> allImages)
        {
            // with a given corner, expand the lines left or right and up or down as necessary until hitting an edge. 
            bool traversingX = true;
            long nextID = 0;
            long backstopID = imageID; // this.imageID, since we're starting here.
            int traversalX = myX, traversalY = myY;
            foreach(KeyValuePair<MatchSides,long> match in myMatches)
            {
                if (!traversingX) { traversalX = myX; }
                // setting up for starting with a corner, will need to adjust for edges....
                // because corners only attach to edges, but edges attach to corners(have two matches), other edges(three matches) and FILLER(4 matches)
                // we need to make sure that we are following a straight line and only set up with "threes" until we hit the last "two"(next corner)
                SatelliteImage connected = allImages.Find(img=> img.imageID== match.Value);
                while (connected!=null && connected.myMatches.Count==3)// || connected.myMatches.Count == 2)
                {
                    if (traversingX) { if (myX == 0) { traversalX++; } else { traversalX--; } }  else { if (myY == 0) { traversalY++; } else { traversalY--; } }
                    if (grid[traversalX, traversalY]!=null && grid[traversalX, traversalY].imageID != connected.imageID) { throw new Exception("About to replace a grid space with a different image!");  }
                    grid[traversalX, traversalY] = connected;
                    if (Day20_JurassicJigsaw.placedImages.Count(img => img.imageID == grid[traversalX, traversalY].imageID) != 0)
                    {
                        throw new Exception("About to place an image in the grid for a second time!");
                    }
                    Day20_JurassicJigsaw.placedImages.Add(grid[traversalX, traversalY]);
                    nextID = connected.myMatches.Where(aMatch => aMatch.Value != backstopID && (new int[] { 2, 3 }).Contains(allImages.Find(img => img.imageID == aMatch.Value).myMatches.Count)).First().Value;
                    backstopID = connected == null ? backstopID = imageID : connected.imageID;
                    connected = allImages.Find(nextOne => nextOne.imageID == nextID);
                }
                //if (traversingX) { traversalX++; } else { traversalY++; } // leaving this as a reminder to myself that if you're going to make stupid loops, you'll pay stupid prices later. 
                if (traversingX) { if (myX == 0) { traversalX++; } else { traversalX--; } } else { if (myY == 0) { traversalY++; } else { traversalY--; } }
                grid[traversalX, traversalY] = connected;
                if (Day20_JurassicJigsaw.placedImages.Count(img=>img.imageID== grid[traversalX, traversalY].imageID)==0)
                    Day20_JurassicJigsaw.placedImages.Add(grid[traversalX, traversalY]);
                traversingX = false;
                backstopID = imageID;
            }

        }

        public void ExpandLine(SatelliteImage[,] grid, int myX, int myY, List<SatelliteImage> allImages)
        {
            // this fills in all the "filler" pieces in the middle. I basically copied the "edges" code. Should refactor, but I'm too lazy. 
            bool traversingX = false;
            long backstopID = imageID; // this.imageID, since we're starting here.
            int traversalX = myX, traversalY = myY;
            int targetx, targety;

            SatelliteImage connected = allImages.Find(img=>myMatches.Where(kvp => Day20_JurassicJigsaw.placedImages.Count(pi => pi.imageID == kvp.Value) == 0).First().Value==img.imageID);
            SatelliteImage nextImage;
            while (connected != null && connected.myMatches.Count == 4)
            {
                if (traversingX) { if (myX == 0) { traversalX++; } else { traversalX--; } } else { if (myY == 0) { traversalY++; } else { traversalY--; } }
                if (grid[traversalX, traversalY] != null && grid[traversalX, traversalY].imageID != connected.imageID) { throw new Exception("About to replace a grid space with a different image!"); }
                grid[traversalX, traversalY] = connected;
                if (Day20_JurassicJigsaw.placedImages.Count(img => img.imageID == grid[traversalX, traversalY].imageID) != 0)
                {
                    throw new Exception("About to place an image in the grid for a second time!");
                }
                Day20_JurassicJigsaw.placedImages.Add(grid[traversalX, traversalY]);
                targetx = traversalX - 1; targety = traversalY + 1;
                // ok, so find an image that isn't the one we started from, is a "filler" (4 matches), will match the space immediately above itself as should have already been placed in the grid
                // AND isn't the one that matches that above space on the line above that was already placed...
                // there is probably a better way to figure that, but right now, I just don't care...
                nextImage = allImages.Where(img => img.imageID != backstopID && img.myMatches.Count == 4 && img.myMatches.Values.Contains(grid[targetx, targety].imageID) && Day20_JurassicJigsaw.placedImages.Count(pi=>pi.imageID==img.imageID)==0).FirstOrDefault();
                backstopID = nextImage == null ? imageID : nextImage.imageID;
                connected = nextImage;
            }
        }

        public void OrientImageData(SatelliteImage[,] grid, int myX, int myY, List<SatelliteImage> allImages)
        {
            // two times through. 
            // First with original data, just rotate three times ( the fourth would just return to original state )
            // then "invert" and then go through ALL FOUR rotations again. 
            // play with a copy.
            SatelliteImage imageToOrient = grid[myX, myY];
            SatelliteImage playground = new SatelliteImage(imageToOrient.imageID, imageToOrient.imageData);
            bool foundMatchingAlternativeOrientation = false;
            for (int inversions=0; inversions<2 && !foundMatchingAlternativeOrientation; inversions++)
            {
                for (int rotations=0; rotations<(inversions==0?3:4) && !foundMatchingAlternativeOrientation; rotations++)
                {
                    if (inversions == 0 || (inversions > 0 && rotations > 0)) // prevents rotation on first iteration of inverted image. we have to check it as-is.
                    { 
                        playground = Rotate(playground); 
                        playground = new SatelliteImage(playground.imageID, playground.imageData); 
                        playground.rotated = rotations + (inversions==0?1:0); 
                    }
                    playground.flipped = inversions > 0;
                    playground.myMatches.Clear();
                    //iteration of current matches hopefully will result in the SAME values(hopefully in proper directions this time)
                    foreach (KeyValuePair<MatchSides,long> kvp in imageToOrient.myMatches)
                    {
                        playground.MatchWith(allImages.Find(img => img.imageID == kvp.Value));
                    }
                    bool matchedAll = true;
                    // this goes through and makes certain that THIS orientation not only matches the same images(which it always should)
                    // but that it ALSO matches them in the grid direction where they are found adjacently.
                    foreach (KeyValuePair<MatchSides, long> kvp in playground.myMatches)
                    {
                        switch (kvp.Key)
                        {
                            case MatchSides.Top:
                                matchedAll = matchedAll && (myX > 0 && grid[myX - 1, myY].imageID == kvp.Value);
                                break;
                            case MatchSides.Right:
                                matchedAll = matchedAll && (myY < grid.GetUpperBound(1) && grid[myX, myY + 1].imageID == kvp.Value);
                                break;
                            case MatchSides.Bottom:
                                matchedAll = matchedAll && (myX < grid.GetUpperBound(0) && grid[myX + 1, myY].imageID == kvp.Value);
                                break;
                            case MatchSides.Left:
                                matchedAll = matchedAll && (myY > 0 && grid[myX , myY-1].imageID == kvp.Value);
                                break;
                            default:
                                throw new Exception("Messed up match");
                        }
                    }
                    foundMatchingAlternativeOrientation = matchedAll;
                }
                // invert on first pass only
                if (inversions == 0 && !foundMatchingAlternativeOrientation)
                {
                    // start fresh. 
                    playground = new SatelliteImage(imageToOrient.imageID, imageToOrient.imageData);
                    playground = Invert(playground);
                    playground = new SatelliteImage(playground.imageID, playground.imageData); // resets the object reference with the newly oriented data
                }
            }

            if (foundMatchingAlternativeOrientation) // if so, we should be the CURRENT orientation in "playground" that WORKED. If not, the original orientation was the correct one all along. 
            {
                playground.imageData.CopyTo(imageToOrient.imageData, 0);
                imageToOrient.rotated = playground.rotated;
                imageToOrient.flipped = playground.flipped;
            }
        }
        public static SatelliteImage Rotate(SatelliteImage img)
        {
            for (int x=0; x<10; x++)
            {
                img.orientedImageData[x] = "          ";//10 spaces. string(' ',10), duh...
            }
            int xPointer = 0;
            for (int y = 0; y < 10; y++)
            {
                string newLine = "";
                for (int x = 9; x >= 0; x--)
                {
                    newLine += img.imageData[x][y];
                }
                img.orientedImageData[xPointer] = newLine;
                xPointer++;
            }
            //PrintComparison(img); // side by side output in the console for sanity-checking.
            img.orientedImageData.CopyTo(img.imageData, 0);
            return img;
        }
        public static SatelliteImage Invert(SatelliteImage img)
        {
            for (int x = 0; x < 10; x++)
            {
                img.orientedImageData[x] = reverse(img.imageData[x]);
            }
            //PrintComparison(img); // side by side output in the console for sanity-checking.
            img.orientedImageData.CopyTo(img.imageData, 0);
            return img;
        }
        public static void PrintComparison(SatelliteImage img) // prints side/side of original with new orientation to check the rotation/inversion code is working.
        {
            Console.WriteLine("");
            for (int x=0; x < 10; x++)
            {
                Console.WriteLine(img.imageData[x] + "   " + img.orientedImageData[x]);
            }
            Console.WriteLine("");
        }

    }
}
