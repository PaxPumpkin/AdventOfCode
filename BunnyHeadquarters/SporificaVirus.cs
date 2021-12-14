using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class SporificaVirus : AoCodeModule
    {
        //initial map is 25x25.
        //char[,] map = new char[575, 575];
        char[,] map = new char[25, 25];
        int x = 0;
        int y = 0;
        char direction = 'U';
        int reallocatedArrayCounter = 0;
        public SporificaVirus()
        {
            inputFileName = "sporificavirus.txt";
            base.GetInput();
        }

        public override void DoProcess()
        {
            //for (x = 0; x < 575; x++) { for (y = 0; y < 575; y++) { map[x, y] = '.'; } }
            int cleanToInfectedTransitionCount = 0;
            inputFile.ForEach(l => { l.ToCharArray().ToList().ForEach(c => { map[x, y] = c; x++; }); x = 0; y++; });
            x = 12; y = 12; // center of current map
            // rules for first part ( answer 5565 )
            //for (int iteration = 0; iteration < 10000; iteration++)
            //{
            //    direction = Turn(((map[x, y] == '#') ? 'R' : 'L'), direction);
            //    if (map[x, y] == '#') map[x, y] = '.'; else { map[x, y] = '#'; cleanToInfectedTransitionCount++; } 
            //    Move();
            //}
            //FinalOutput.Add("After 10000, transitions were: " + cleanToInfectedTransitionCount.ToString());
            // rules for second part  (2319013 is too low )
            for (int iteration = 0; iteration < 10000000; iteration++)
            {

                direction = Turn(((map[x, y] == '.' || map[x, y] == '\0') ? 'L' : (map[x, y] == 'W' ? 'S' : (map[x, y] == '#' ? 'R' : 'B'))), direction);
                if (map[x, y] == '.' || map[x, y]=='\0') map[x, y] = 'W';
                else if (map[x, y] == 'W') { map[x, y] = '#'; cleanToInfectedTransitionCount++; }
                else if (map[x, y] == '#') map[x, y] = 'F';
                else if (map[x, y] == 'F') map[x, y] = '.';
                else
                {
                    Console.WriteLine("I don't think I should get here...");
                }
                Move();
            }
            FinalOutput.Add("After 10000000, transitions were: " + cleanToInfectedTransitionCount.ToString());
        }
        public char Turn(char whichWay,char current)
        {
            if (whichWay == 'S') return current; // straight ahead, so same direction as now
            if (whichWay == 'B')
            {  // go "Back"
                if (current == 'U') current = 'D';
                else if (current == 'D') current = 'U';
                else if (current == 'R') current = 'L';
                else current = 'R';
            }
            else
            {
                current = (whichWay == 'R') ?
                    ((current == 'U') ? 'R' : ((current == 'R') ? 'D' : ((current == 'D') ? 'L' : 'U')))
                    : ((current == 'U') ? 'L' : ((current == 'L') ? 'D' : ((current == 'D') ? 'R' : 'U')));
            }
            return current;
        }
        public void Move()
        {
            if (((direction == 'U' && y == 0) || (direction == 'D' && y == map.GetUpperBound(1))) || ((direction == 'L' && x == 0) || (direction == 'R' && x == map.GetUpperBound(0))))
            {
                reallocatedArrayCounter++;
                // gotta expand the map grid. Gonna say by about 50%
                int newDimensions = Convert.ToInt32(map.GetUpperBound(0)*1.5);
                char[,] newGrid = new char[newDimensions,newDimensions];
                // add half of the new space as the buffer. 
                int newX = Convert.ToInt32((newDimensions - map.GetUpperBound(0)) / 2);
                int newY = newX; // and start our copy-over there
                for (int i = 0; i <= map.GetUpperBound(0); i++)
                {
                    for (int j = 0; j <= map.GetUpperBound(1); j++)
                    {
                        newGrid[newX + i, newY + j] = map[i, j];
                    }
                }
                //reset our map pointers
                x += newX;
                y += newY;
                //reset our map
                map = newGrid;
            }
            switch (direction)
            {
                case 'U':
                    y--;
                    break;
                case 'D':
                    y++;
                    break;
                case 'L':
                    x--;
                    break;
                case 'R':
                    x++;
                    break;                
                default:
                    break;
            }
        }
    }
}
