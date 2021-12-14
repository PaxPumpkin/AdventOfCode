using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day24_LobbyLayout : AoCodeModule
    {
        public static int TotalTileCount = 0;
        public static Dictionary<Tuple<int,int>, HexTile> tiles = new Dictionary<Tuple<int,int>, HexTile>();
        public Day24_LobbyLayout()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            //inputFileName = @"InputFiles\" + this.GetType().Name + "Sample.txt";
            GetInput();
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day24_LobbyLayout part 1:The number of tiles with the black side up: 438(Process: 26.2873 ms)
            //** > Result for Day24_LobbyLayout part 2:The number of tiles with the black side up after 100 turns: 4038(Process: 1659.4988 ms)
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            TotalTileCount = 0;
            tiles.Clear();
            HexTile referenceTile = new HexTile(0,0);
            foreach (string processingLine in inputFile)
            {
                referenceTile.Move(processingLine);
            }
            finalResult = "The number of tiles with the black side up: " + tiles.Values.Count(t => t.WhiteSideUp == false).ToString();
            AddResult(finalResult);
            Print(finalResult);
            ResetProcessTimer(true);
            // OK.... this fixes the error I mention below. 
            // The assumption exists that after the initial traversal, the holes that actually exist in our floor map right now
            //  really *shouldn't* be holes. Fill them in. Now things work.
            //   Side Note: it just occurred to me that if the path was circuitous enough to leave a "hole" more than two tiles wide, this might still leave a hole. Let's over-kill it in the final game check.
            tiles.Values.ToList().ForEach(hex => hex.FillNeighbors()); 
            for (int x = 0; x < 100; x++)
            {
                tiles.Values.ToList().ForEach(hex => hex.DetermineImpendingChange());
                tiles.Values.ToList().ForEach(hex => hex.Change());
                //if (x > 95) // print the last five for error-checking. I'm off by a small number... Gotta find that error!
                {
                    //Print("Turn " + (x+1).ToString() + ":" + tiles.Values.Count(t => t.WhiteSideUp == false).ToString());
                }
            }
            finalResult = "The number of tiles with the black side up after 100 turns: " + tiles.Values.Count(t => t.WhiteSideUp == false).ToString();
            AddResult(finalResult);
        }

    }
    class HexTile
    {
        public enum Directions { e, se, sw, w, nw, ne }
        Dictionary<Directions, HexTile> neighbors = new Dictionary<Directions, HexTile>();
        int index = 0;
        public int TileIndex { get { return index; } }
        public int row = 0;
        public int column = 0;
        public bool WhiteSideUp = true;
        public bool ImpendingFlip = false;
        public HexTile()
        {
            Day24_LobbyLayout.TotalTileCount++;
            index = Day24_LobbyLayout.TotalTileCount;
            Day24_LobbyLayout.tiles.Add(new Tuple<int, int>(row, column), this);
        }
        public HexTile(int r, int c)
        {
            Day24_LobbyLayout.TotalTileCount++;
            index = Day24_LobbyLayout.TotalTileCount;
            row = r; column = c;
            Day24_LobbyLayout.tiles.Add(new Tuple<int, int>(row, column), this);
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row + 1, column + 1)) && !neighbors.ContainsKey(Directions.se)) { neighbors.Add(Directions.se, Day24_LobbyLayout.tiles[new Tuple<int, int>(row + 1, column + 1)]); }
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row + 1, column + 1))) Day24_LobbyLayout.tiles[new Tuple<int, int>(row + 1, column + 1)].CheckNeighbors(); 
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row - 1, column + 1)) && !neighbors.ContainsKey(Directions.ne)) { neighbors.Add(Directions.ne, Day24_LobbyLayout.tiles[new Tuple<int, int>(row - 1, column + 1)]); }
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row - 1, column + 1))) Day24_LobbyLayout.tiles[new Tuple<int, int>(row - 1, column + 1)].CheckNeighbors(); 
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row + 1, column - 1)) && !neighbors.ContainsKey(Directions.sw)) { neighbors.Add(Directions.sw, Day24_LobbyLayout.tiles[new Tuple<int, int>(row + 1, column - 1)]); }
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row + 1, column - 1))) Day24_LobbyLayout.tiles[new Tuple<int, int>(row + 1, column - 1)].CheckNeighbors(); 
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row - 1, column - 1)) && !neighbors.ContainsKey(Directions.nw)) { neighbors.Add(Directions.nw, Day24_LobbyLayout.tiles[new Tuple<int, int>(row - 1, column - 1)]); }
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row - 1, column - 1))) Day24_LobbyLayout.tiles[new Tuple<int, int>(row - 1, column - 1)].CheckNeighbors(); 
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row, column + 2)) && !neighbors.ContainsKey(Directions.e)) { neighbors.Add(Directions.e, Day24_LobbyLayout.tiles[new Tuple<int, int>(row, column + 2)]); }
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row, column + 2))) Day24_LobbyLayout.tiles[new Tuple<int, int>(row , column + 2)].CheckNeighbors(); 
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row, column - 2)) && !neighbors.ContainsKey(Directions.w)) { neighbors.Add(Directions.w, Day24_LobbyLayout.tiles[new Tuple<int, int>(row, column - 2)]); }
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row , column -2))) Day24_LobbyLayout.tiles[new Tuple<int, int>(row , column - 2)].CheckNeighbors(); 
        }
        public void CheckNeighbors() // same as above, but used for "re-checking" when a new tile has been placed. I may be over-killing this concept, but I'm off somewhere....
        {
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row + 1, column + 1)) && !neighbors.ContainsKey(Directions.se)) { neighbors.Add(Directions.se, Day24_LobbyLayout.tiles[new Tuple<int, int>(row + 1, column + 1)]); }
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row - 1, column + 1)) && !neighbors.ContainsKey(Directions.ne)) { neighbors.Add(Directions.ne, Day24_LobbyLayout.tiles[new Tuple<int, int>(row - 1, column + 1)]); }
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row + 1, column - 1)) && !neighbors.ContainsKey(Directions.sw)) { neighbors.Add(Directions.sw, Day24_LobbyLayout.tiles[new Tuple<int, int>(row + 1, column - 1)]); }
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row - 1, column - 1)) && !neighbors.ContainsKey(Directions.nw)) { neighbors.Add(Directions.nw, Day24_LobbyLayout.tiles[new Tuple<int, int>(row - 1, column - 1)]); }
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row, column + 2)) && !neighbors.ContainsKey(Directions.e)) { neighbors.Add(Directions.e, Day24_LobbyLayout.tiles[new Tuple<int, int>(row, column + 2)]); }
            if (Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(row, column - 2)) && !neighbors.ContainsKey(Directions.w)) { neighbors.Add(Directions.w, Day24_LobbyLayout.tiles[new Tuple<int, int>(row, column - 2)]); }
        }
        public void AddNeighbor(Directions backPath, HexTile neighbor)
        {
            if (!neighbors.ContainsKey(backPath))
            {
                neighbors.Add(backPath, neighbor);
            }
        }
        public void Move(string path)
        {
            if (path.Length == 0)
            {
                WhiteSideUp = !WhiteSideUp;
                return;
            }
            string step = (path.StartsWith("s") || path.StartsWith("n")) ? path.Substring(0, 2) : path.Substring(0, 1);

            path = path.Length==step.Length?"":path.Substring(step.Length == 1 ? 1 : 2);
            Directions direction = step == "e" ? Directions.e : (step == "se" ? Directions.se : (step == "sw" ? Directions.sw : (step == "w" ? Directions.w : (step == "nw" ? Directions.nw : Directions.ne))));
            if (!neighbors.ContainsKey(direction))
            {
                int nextRow = direction==Directions.se || direction== Directions.sw?row+1:(direction==Directions.e || direction==Directions.w?row:row-1);
                int nextColumn = direction== Directions.se || direction == Directions.ne ? column + 1 : (direction == Directions.nw || direction == Directions.sw ? column-1 : (direction==Directions.e?column+2:column-2)); 
                HexTile newNeighbor = Day24_LobbyLayout.tiles.ContainsKey(new Tuple<int, int>(nextRow, nextColumn))? Day24_LobbyLayout.tiles[new Tuple<int, int>(nextRow, nextColumn)] : new HexTile(nextRow,nextColumn);
                // creating the new neighbor SHOULD now auto-add us to their neighbors.
                if (!neighbors.ContainsKey(direction)) throw new Exception("Neighbor not auto-added!");
                Directions backPath = direction == Directions.e ? Directions.w : (direction == Directions.w ? Directions.e : (direction == Directions.se ? Directions.nw : (direction == Directions.nw ? Directions.se : (direction == Directions.sw ? Directions.ne : Directions.sw))));
                newNeighbor.AddNeighbor(backPath, this); // this is also likely now unecessary
            }
            neighbors[direction].Move(path);
        }
        public void DetermineImpendingChange()
        {
            FillNeighbors(); // ALWAYS START WITH A NON-HOLEY FLOOR. I think the other changes I put in fix the need for this here, but I'm a tad past actually caring at this point. 
            if (WhiteSideUp)
            {
                ImpendingFlip = neighbors.Values.Count(n => n.WhiteSideUp == false) == 2;
            }
            else
            {
                int tileCount = neighbors.Values.Count(n => n.WhiteSideUp == false);
                ImpendingFlip = (tileCount == 0 || tileCount > 2);
            }
        }
        public void FillNeighbors()
        {

            if (neighbors.Count < 6)
            {
                if (!neighbors.ContainsKey(Directions.ne)) MakeNeighbor(Directions.ne);
                if (!neighbors.ContainsKey(Directions.se)) MakeNeighbor(Directions.se);
                if (!neighbors.ContainsKey(Directions.nw)) MakeNeighbor(Directions.nw);
                if (!neighbors.ContainsKey(Directions.sw)) MakeNeighbor(Directions.sw);
                if (!neighbors.ContainsKey(Directions.e)) MakeNeighbor(Directions.e);
                if (!neighbors.ContainsKey(Directions.w)) MakeNeighbor(Directions.w);
                neighbors.Values.ToList().ForEach(n => n.CheckNeighbors()); // if we made any, we have to re-link them in all directions.
            }
        }
        public void Change()
        {
            if (ImpendingFlip)
            {
                WhiteSideUp = !WhiteSideUp;
                ImpendingFlip = false;
            }
        }
        public HexTile MakeNeighbor(Directions whichWay)
        {
            int nextRow = whichWay == Directions.se || whichWay == Directions.sw ? row + 1 : (whichWay == Directions.e || whichWay == Directions.w ? row : row - 1);
            int nextColumn = whichWay == Directions.se || whichWay == Directions.ne ? column + 1 : (whichWay == Directions.nw || whichWay == Directions.sw ? column - 1 : (whichWay == Directions.e ? column + 2 : column - 2));
            HexTile newNeighbor = new HexTile(nextRow, nextColumn);
            return newNeighbor;
        }
    }

}
