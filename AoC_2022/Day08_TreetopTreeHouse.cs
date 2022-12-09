using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day08_TreetopTreeHouse : AoCodeModule
    {
        public Day08_TreetopTreeHouse()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day08_TreetopTreeHouse part 1: Trees Visible: 1546 (Process: 17.3167 ms)
            //** > Result for Day08_TreetopTreeHouse part 2: Max Scenic Score: 519064 (Process: 116.2012 ms)
            /**
             * TODO
             * SERIOUSLY MESSY and INEFFICIENT
             * REFACTOR THIS NONSENSE
             */
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            List<TreeTopRow> trees = new List<TreeTopRow>();
            foreach (string processingLine in inputFile)
            {
                if (!processingLine.Equals(string.Empty))
                    trees.Add(new TreeTopRow(processingLine));
            }
            int visibleTrees = 0;
            for (int rc =1; rc < trees.Count-1; rc++)
            {
                for(int colCounter = 1; colCounter< trees[rc].Row.Length-1; colCounter++)
                {
                    string row = trees[rc].Row;
                    int thisTree = row[colCounter];
                    visibleTrees += row.Take(colCounter).All(t => t < thisTree) 
                        || row.Skip(colCounter+1).Take(row.Length - colCounter).All(t => t < thisTree) 
                        || trees.Take(rc).All(t=> t.Row[colCounter]< thisTree )
                        || trees.Skip(rc + 1).Take(trees.Count - rc).All(t => t.Row[colCounter] < thisTree)
                        ? 1 : 0;
                }
            }
            visibleTrees += (trees[0].Row.Length * 2) + (trees.Count * 2) - 4;
            finalResult = "Trees Visible: " + visibleTrees.ToString();
            AddResult(finalResult);
            ResetProcessTimer(true);
            int rowCounter = 0;
            foreach(TreeTopRow row in trees)
            {
                foreach(TreeTopTree tree in row.trees)
                {
                    if (rowCounter == 0 || rowCounter == trees.Count - 1 || tree.Column == 0 || tree.Column == row.Row.Length - 1) { 
                        
                        tree.ScenicScore = 0;  continue; 
                    }
                    int score = 1;
                    int treeCount = 0;
                    string treeString = "";
                    treeString = new string(row.Row.Take(tree.Column).Reverse().ToArray());// ToString();
                    treeCount = treeString.IndexOf(treeString.FirstOrDefault(t => t >= tree.Height))+1;
                    score *= treeCount == 0 ? treeString.Length : treeCount;
                    treeCount = 0;
                    treeString = new string(row.Row.Skip(tree.Column + 1).Select(c => c).ToArray());// ToString();
                    treeCount = treeString.IndexOf(treeString.FirstOrDefault(t => t >= tree.Height)) + 1;
                    score *= treeCount == 0 ? treeString.Length : treeCount;
                    treeCount = 0;
                    treeString = "";
                    for(int c = 0; c<rowCounter; c++)
                    {
                        treeString += trees[c].Row[tree.Column];
                    }
                    treeString = new string(treeString.Reverse().ToArray());
                    treeCount = treeString.IndexOf(treeString.FirstOrDefault(t => t >= tree.Height)) + 1;
                    score *= treeCount == 0 ? treeString.Length : treeCount;
                    treeCount = 0;
                    treeString = "";
                    for (int c = rowCounter+1; c < trees.Count; c++)
                    {
                        treeString += trees[c].Row[tree.Column];
                    }
                    treeCount = treeString.IndexOf(treeString.FirstOrDefault(t => t >= tree.Height)) + 1;
                    score *= treeCount == 0 ? treeString.Length : treeCount;
                    treeCount = 0;
                    treeString = "";
                    tree.ScenicScore = score;
                }
                rowCounter++;
            }
            finalResult = "Max Scenic Score: " + trees.Max(treeRow => treeRow.trees.Max(tree => tree.ScenicScore)).ToString();
            AddResult(finalResult);
            ResetProcessTimer(true);
        }
    }
    public class TreeTopRow
    {
        public string Row;
        public List<TreeTopTree> trees = new List<TreeTopTree>();
        public TreeTopRow(string row)
        {
            Row = row;
            int idx = 0;
            foreach(char c in row)
            {
                trees.Add(new TreeTopTree() { Column = idx, Height = c });
                idx++;
            }
        }
    }
    public class TreeTopTree
    {
        public int Column;
        public char Height;
        public long ScenicScore;
    }
}
