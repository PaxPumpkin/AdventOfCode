using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day17_ConwayCubes : AoCodeModule
    {
        public Dictionary<Tuple<int,int,int,int>, ConwayCube> hypercubeDimension = new Dictionary<Tuple<int, int, int,int>, ConwayCube>();
        public Day17_ConwayCubes()
        {

            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            //inputFileName = @"InputFiles\" + this.GetType().Name + "Sample.txt";
            GetInput(); 
            OutputFileReset();
            
        }
        public override void DoProcess()
        {
            //** > Result for Day17_ConwayCubes part 1:271(Process: 349.394 ms)
            //** > Result for Day17_ConwayCubes part 2:2064(Process: 33733.7435 ms)
            ConwayCube.counter = 0;
            ConwayCube.Parent = this;
            ResetProcessTimer(true);
            int x = 0, y = 0, z = 0;
            Tuple<int, int, int,int> hypercubeAddress;
            foreach (string processingLine in inputFile)
            {
                y = 0;
                foreach (char s in processingLine)
                {
                    hypercubeAddress = new Tuple<int, int, int, int>(x, y, z, 0); 
                    hypercubeDimension.Add(hypercubeAddress, new ConwayCube(x, y, z, s == '#' ? 1 : 0));
                    y++;
                }
                x++;
            }
            // fill in the "neighbors" - 3 dimensional puzzle, each cube has 26 neighbors. 8 on its own "Z" plane, and +9 each on the z-1, z+1 planes.
            hypercubeDimension.Values.Where(cc => cc.neighbors.Count < 26).ToList().ForEach(cc => cc.FillInMissingNeighborsThreeDimensions(hypercubeDimension));
            // the above fills in for initial setup. The below will just RELINK any that were added to make sure everything matches.
            hypercubeDimension.Values.Where(cc => cc.neighbors.Count == 0).ToList().ForEach(cc => cc.FillInExistingNeighborsThreeDimensions(hypercubeDimension));
            List<ConwayCube> potentiallyTransitioning;
            for (int loop = 0; loop < 6; loop++)
            {
                Print("Loop " + (loop + 1).ToString());
                // this should prevent us from dealing with the cubes that don't actually matter....
                potentiallyTransitioning = hypercubeDimension.Values.Where(hcx => (hcx.state == 1) || (hcx.state == 0 && hcx.neighbors.Count(nhc => nhc.state == 1) > 0)).ToList();
                potentiallyTransitioning.ForEach(cc => cc.DetermineNextState());
                potentiallyTransitioning.ForEach(cc => cc.Transition());
                // transitioning can now create situations where the above list selection will need "edge" neighbors that weren't there before. So, gotta fill those in again.
                hypercubeDimension.Values.Where(cc => cc.neighbors.Count < 26).ToList().ForEach(cc => cc.FillInMissingNeighborsThreeDimensions(hypercubeDimension));
                hypercubeDimension.Values.Where(cc => cc.neighbors.Count == 0).ToList().ForEach(cc => cc.FillInExistingNeighborsThreeDimensions(hypercubeDimension));
            }
            AddResult(hypercubeDimension.Values.Count(cube => cube.state == 1).ToString());

            ResetProcessTimer(true);
            hypercubeDimension.Clear();
            ConwayCube.counter = 0;
            x = 0; y = 0; z = 0;
            foreach (string processingLine in inputFile)
            {
                y = 0;
                foreach (char s in processingLine)
                {
                    hypercubeAddress = new Tuple<int, int, int, int>(x, y, z, 0);
                    hypercubeDimension.Add(hypercubeAddress, new ConwayCube(x, y, z, s == '#' ? 1 : 0));
                    y++;
                }
                x++;
            }


            // this is essentially exactly the same code as above, just now using 4 dimensions. I've refactored the filling of neighbors methods, but I should refactor this, too...

            // 4-Dimensional hypercube has 80 neighbors. 26 as per the 3-D count, +27 each for the "w" dimension.
            hypercubeDimension.Values.Where(cc => cc.neighbors.Count < 80).ToList().ForEach(cc => cc.FillInMissingNeighborsFourDimensions(hypercubeDimension));
            // the above fills in for initial setup. The below will just RELINK any that were added to make sure everything matches.
            hypercubeDimension.Values.Where(cc => cc.neighbors.Count == 0).ToList().ForEach(cc => cc.FillInExistingNeighborsFourDimensions(hypercubeDimension));
            for (int loop = 0; loop < 6; loop++)
            {
                Print("Loop " + (loop+1).ToString());
                // this should prevent us from dealing with the cubes that don't actually matter....
                potentiallyTransitioning = hypercubeDimension.Values.Where(hc => (hc.state == 1) || (hc.state == 0 && hc.neighbors.Count(nhc => nhc.state == 1) > 0)).ToList();
                potentiallyTransitioning.ForEach(cc => cc.DetermineNextState());
                potentiallyTransitioning.ForEach(cc => cc.Transition());
                // transitioning can now create situations where the above list selection will need "edge" neighbors that weren't there before. So, gotta fill those in again.
                hypercubeDimension.Values.Where(cc => cc.neighbors.Count < 80).ToList().ForEach(cc => cc.FillInMissingNeighborsFourDimensions(hypercubeDimension));
                hypercubeDimension.Values.Where(cc => cc.neighbors.Count == 0).ToList().ForEach(cc => cc.FillInExistingNeighborsFourDimensions(hypercubeDimension));
            }
            AddResult(hypercubeDimension.Values.Count(cube => cube.state == 1).ToString());
        }
        public class ConwayCube
        {
            public static int counter=0;
            public int x { get; set; }
            public int y { get; set; }
            public int z { get; set; }
            public int w { get; set; }
            public int state { get; set; }
            int nextState { get; set; }
            public int myIndex { get; set; }
            public static AoCodeModule Parent { get; set; }
            public List<ConwayCube> neighbors = new List<ConwayCube>();
            public ConwayCube(int xc,int yc, int zc, int active)
            {
                x = xc; y = yc; z = zc; state = active; w = 0;
                myIndex=counter++;
            }
            public ConwayCube(int xc, int yc, int zc, int wc, int active)
            {
                x = xc; y = yc; z = zc; state = active; w = wc;
                myIndex = counter++;
            }
            // "Missing" Neighbors will CREATE those neighbors that should exist.
            public void FillInMissingNeighborsThreeDimensions(Dictionary<Tuple<int, int, int, int>, ConwayCube> hypercubeDimension)
            {
                FillNeighbors(hypercubeDimension, false, false); // false = NOT instantiated only(ie. CREATE them), and false = NOT four dimensions
            }
            public void FillInMissingNeighborsFourDimensions(Dictionary<Tuple<int, int, int, int>, ConwayCube> hypercubeDimension)
            {
                FillNeighbors(hypercubeDimension, false, true); // false = NOT instantiated only(ie. CREATE them), and true = YES, four dimensions
            }
            public void FillInExistingNeighborsThreeDimensions(Dictionary<Tuple<int, int, int, int>, ConwayCube> hypercubeDimension)
            {
                FillNeighbors(hypercubeDimension, true, false); // true = YES, only the ones that already exist. DO NOT CREATE NEW ONES. and false = NOT four dimensions
            }
            public void FillInExistingNeighborsFourDimensions(Dictionary<Tuple<int, int, int, int>, ConwayCube> hypercubeDimension)
            {
                FillNeighbors(hypercubeDimension, true, true); // true = YES, only the ones that already exist. DO NOT CREATE NEW ONES. and true = YES, four dimensions
            }
            private void FillNeighbors(Dictionary<Tuple<int,int,int,int>,ConwayCube> hypercubeDimension, bool instantiatedOnly, bool fourDimension)
            {
                if (neighbors.Count == (fourDimension?80:26)) return; // if this cube already has all the neighbors linked, skip this.
                int wc = -2; // dimension 4 is "w", and we will always be pre-incementing, so start one below our limit.
                Tuple<int, int, int, int> hypercubeAddress;
                ConwayCube nextNeighbor;
                while ((!fourDimension && ++wc<0) || (fourDimension && ++wc<=1)) // this is a shitty way to short circuit and re-use code, but I'm cranky today. 
                {
                    int currentWMarker = w + wc; // for four dimensions, we will be looking for neighbors at the ITERATIVE value of the "loop" w ( -1, 0, +1 ) in comparison to THIS cube's "w". this is irrelevant if the bool for fourDimension is false, no biggie....
                    for (int zc = z - 1; zc <= z + 1; zc++) // go through all three Z axes
                    {
                        for (int xc = x - 1; xc <= x + 1; xc++) // all three left-right "X" addresses for this "z"
                        {
                            for (int yc = y - 1; yc <= y + 1; yc++) // all three top-down "Y" addresses for this "z/x" combo.
                            {
                                hypercubeAddress = new Tuple<int, int, int, int>(xc, yc, zc, (fourDimension ? currentWMarker : 0)); // always use "zero" as the fourth dimension value if we're working in three dimensions.
                                if (zc == z && yc == y && xc == x && ((fourDimension ? currentWMarker : 0) == w)) // if this is our current cube's actual address, skip it. 
                                    continue;
 
                                if (neighbors.Count(neighbor => neighbor.z == zc && neighbor.x == xc && neighbor.y == yc && neighbor.w == (fourDimension ? currentWMarker : 0)) > 0) // skip if I already have this neighbor coordinate marked.
                                    continue;

                                nextNeighbor=null;
                                hypercubeDimension.TryGetValue(hypercubeAddress, out nextNeighbor); // find the neighbor in our huge map.
                                if (nextNeighbor == null && !instantiatedOnly) // if we didn't find one, and we're in "CREATE" mode, make one.
                                {
                                    nextNeighbor = fourDimension? new ConwayCube(xc, yc, zc, currentWMarker, 0):new ConwayCube(xc, yc, zc, 0); // make it appropriate to our dimensional settings. Always use "zero" for the fourth dimension in 3D mode.
                                    hypercubeDimension.Add(hypercubeAddress, nextNeighbor);
                                }

                                if (nextNeighbor != null) // if we were in create mode, or if we actually found a neighbor....
                                    if (nextNeighbor.myIndex != myIndex) // shouldn't happen, but if we found ourself, we probably shouldn't add it :)
                                        neighbors.Add(nextNeighbor);

                            }// top/down iterator
                        } // left/right iterator
                    } // z-plane iterator
                } // fourth dimension iterator(while).
            }
            public void DetermineNextState()
            {
                //If a cube is active and exactly 2 or 3 of its neighbors are also active, the cube remains active. Otherwise, the cube becomes inactive.
                //If a cube is inactive but exactly 3 of its neighbors are active, the cube becomes active. Otherwise, the cube remains inactive.
                int activeNeighbors = neighbors.Count(cc => cc.state == 1);
                if (state == 1)
                {
                    nextState = (activeNeighbors==2 || activeNeighbors==3)?1:0;
                }
                else
                {
                    nextState = (activeNeighbors == 3) ? 1 : 0;
                }

            }
            public void Transition()
            {
                state = nextState;
            }
        }
    }
}
