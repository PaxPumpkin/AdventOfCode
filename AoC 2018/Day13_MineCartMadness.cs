using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day13_MineCartMadness : AoCodeModule
    {
        public Day13_MineCartMadness()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 

        }
        public override void DoProcess()
        {

            string finalResult = "Not Set";
            ResetProcessTimer(true);// true also iterates the section marker
            // the mine map is an x/y grid with extra component representing something I may not need later, but I'm starting with it.
            // if ^,v,>,< chars show up indicating a cart, then the underlying map piece is straight track either | or - 
            // but I am not certain I'll want to go that far.
            // maybe keep the carts separate???
            char[,,] mine = new char[ inputFile.Count, inputFile.Max(x => x.Length), 2];
            List<MineCart> carts = new List<MineCart>();
            SetFreshMineMap(mine, carts);
            bool boom = false;
            int boomX = 0, boomY = 0;
            int timeTick = 0;
            while (!boom)
            {
                carts.OrderBy(cart => cart.xcoord).ThenBy(cart => cart.ycoord).ToList().ForEach(cart =>
                    {
                        if (!boom) // we don't move any more carts after the collision.
                        {
                            boom = cart.Move(mine,carts);
                            if (boom)
                            {
                                boomX = cart.xcoord;
                                boomY = cart.ycoord;
                            }
                        }
                    });
                timeTick++;
            }
            finalResult = "First collision at tick " + timeTick.ToString() + " at coordinates " + boomY.ToString() + "," + boomX.ToString();

            AddResult(finalResult);
            ResetProcessTimer(true);
            SetFreshMineMap(mine, carts);
            boom = false;
            boomX = 0; boomY = 0;
            timeTick = 0;
            while (carts.Count(cart=>cart.crashed==false)>1)
            {
                carts.OrderBy(cart => cart.xcoord).ThenBy(cart => cart.ycoord).ToList().ForEach(cart =>
                {
                        boom = cart.Move(mine, carts);


                });
                timeTick++;
            }
            MineCart lastCart = carts.Where(cart => cart.crashed == false).First();
            finalResult = "Location of last cart at tick " + timeTick.ToString() + " is at coordinates " + lastCart.ycoord.ToString() + "," + lastCart.xcoord.ToString();
            AddResult(finalResult);
        }
        public void SetFreshMineMap(char[,,] mine,List<MineCart> carts)
        {
            carts.Clear();
            int rowCounter = 0;
            foreach (string processingLine in inputFile)
            {
                int colCounter = 0;
                foreach (char mapPiece in processingLine)
                {
                    char mineSpaceType = mapPiece;
                    if (mapPiece == '<' || mapPiece == '>')
                    {
                        mineSpaceType = '-';
                        carts.Add(new MineCart(rowCounter, colCounter, (mapPiece == '<' ? "left" : "right")));
                    }
                    else if (mapPiece == '^' || mapPiece == 'v')
                    {
                        mineSpaceType = '|';
                        carts.Add(new MineCart(rowCounter, colCounter, (mapPiece == '^' ? "up" : "down")));
                    }
                    mine[rowCounter, colCounter, 1] = mineSpaceType;
                    colCounter++;
                }
                rowCounter++;
            }
        }
    }
    public class MineCart
    {
        public int xcoord;
        public int ycoord;
        public string direction;
        public string nextTurn = "left"; // then straight, then right
        public bool crashed = false;
        public MineCart(int x, int y, string dir)
        {
            xcoord = x;
            ycoord = y;
            direction = dir;
        }
        public bool Move(char[,,] mine, List<MineCart> carts)
        {
            bool boom = false;
            if (crashed) { return boom; }
            int nextX=-1, nextY=-1;
            switch (direction) {
                case "left":
                    nextY = ycoord - 1;
                    nextX = xcoord;
                    break;
                case "right":
                    nextY = ycoord + 1;
                    nextX = xcoord;
                    break;
                case "up":
                    nextY = ycoord;
                    nextX = xcoord-1;
                    break;
                case "down":
                    nextY = ycoord;
                    nextX = xcoord+1;
                    break;
            }
            // ok, first check to see if there is a cart there already! this cart won't show up there in this selector because it hasn't changed x,y internally yet.
            if (carts.Count(cart => cart.xcoord == nextX && cart.ycoord == nextY && cart.crashed==false) > 0)
            {
                // mark the cart we hit before doing the next bits (it may prevent it from moving in the iteration!
                carts.Where(cart => cart.xcoord == nextX && cart.ycoord == nextY).First().crashed = true; // first, because there should only be one!
                xcoord = nextX;
                ycoord = nextY;
                boom = true;
                crashed = true;

            }
            else// we are clear to move
            {
                xcoord = nextX; ycoord = nextY;
                // now resolve any possible new directions
                if (mine[nextX, nextY, 1] == '+')
                {
                    // intersection! 
                    if (nextTurn == "left")
                    {
                        direction = (direction == "up") ? "left" : (direction == "down" ? "right" : (direction == "left" ? "down" : "up"));
                        nextTurn = "straight";
                    }
                    else if (nextTurn == "right")
                    {
                        direction = (direction == "up") ? "right" : (direction == "down" ? "left" : (direction == "left" ? "up" : "down"));
                        nextTurn = "left";
                    }
                    else
                    {
                        nextTurn = "right";
                    }

                }
                else if (mine[nextX, nextY, 1] == '\\' || mine[nextX, nextY, 1] == '/')
                {
                    if (mine[nextX, nextY, 1] == '\\') // top-right corner or bottom left....
                    {
                        direction = (direction == "right" ? "down" : (direction == "up" ? "left" : (direction == "down" ? "right" : "up")));
                    }
                    else// top-left or bottom-right
                    {
                        direction = (direction == "right" ? "up" : (direction == "up" ? "right" : (direction == "down" ? "left" : "down")));
                    }
                }
            }

            return boom;
        }
    }
}
