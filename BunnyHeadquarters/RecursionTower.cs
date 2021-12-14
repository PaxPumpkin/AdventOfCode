using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class RecursionTower : AoCodeModule
    {
        public RecursionTower()
        {
            inputFileName = "RecursionTower.txt";
            base.GetInput();
        }

        public override void DoProcess()
        {

            //set the initial data structures
            inputFile.ForEach(line => new TowerProgram(line));
            TowerProgram.LinkThemUp();

            //Problem one - what is name of the program at the bottom? It is the only program that didn't get a reassigned BASE name during linkup...
            string bottomProgram = TowerProgram.FullTower.Where(program => program.BaseProgramName.Equals("Base")).First().Name;

            //Problem two - find the weight that the unbalanced program should be in order for the whole tower to be fully balanced.
            // this should give us the full direct chain for the unbalanced section. Unless something is wonky, it should also be in order where the names link and the first item is where the problem is.
            // ***if everything is balanced, this is unsafe because null object reference exceptions. But we know that it is unbalanced per problem spec, so I'm not bothering right now to make it safe.
            TowerProgram whereTheProblemIs = TowerProgram.FullTower.Where(p => !p.Balanced).OrderBy(p => p.TotalWeight).ToList().First();
            TowerProgram unbalancingProgram = whereTheProblemIs.UnbalancedProgram;
            int theWeightItShouldBe = unbalancingProgram.Weight + (whereTheProblemIs.CommonTotalWeight - unbalancingProgram.TotalWeight);

            //Set Output results.
            FinalOutput.Add("Bottom Program: " + bottomProgram);
            FinalOutput.Add("Program " + unbalancingProgram.Name + " is causing an imbalance. Its weight should be " + theWeightItShouldBe.ToString());
        }

    }
    class TowerProgram
    {
        // static so all instances have a local reference to the same list of all programs.
        public static List<TowerProgram> FullTower = new List<TowerProgram>();
        // instance members
        public string Name = "";
        public int Weight = 0;
        public List<string> DiscOfPrograms = new List<string>();
        public string BaseProgramName="Base"; // will get overwritten during the linkup call, except the REAL base.
        //calculated instance properties
        public int TotalWeight { get { return Weight + DiscOfPrograms.Sum(heldName => FullTower.Find(program => program.Name.Equals(heldName)).TotalWeight); } } //recursive.
        public bool Balanced { get { return DiscOfPrograms.Count == 0 || FullTower.Where(program => DiscOfPrograms.Contains(program.Name)).ToList().GroupBy(program => program.TotalWeight).Count() == 1;} }
        public TowerProgram UnbalancedProgram
        {
            get
            {
                if  (Balanced)
                    return null;
                else
                {
                    // Group them on the weights to find the odd one. The odd one is in the group that has only one member.
                    return FullTower.Where(program => DiscOfPrograms.Contains(program.Name)).GroupBy(p => p.TotalWeight).Where(group => group.Count() == 1).ToList().First().First();//first item in first group with count of 1
                }
            }
        }
        // the common total weight is the value held by the group with multiple members. Fails for none/1  in the disc of programs situation, but we'd not ever be looking for this value under that condition, so...
        public int CommonTotalWeight { get { return FullTower.Where(program => DiscOfPrograms.Contains(program.Name)).GroupBy(p => p.TotalWeight).Where(group => group.Count() > 1).ToList().First().Key; } }

        //Constructor
        public TowerProgram(string towerDefinition)
        {
            // gotta parse the string input. RegEx would be best, but people freak out around here when they see it.
            string thisTowerInfo = towerDefinition.Split(new char[] { ')' })[0]; //this tower will be in the first half of the split result.
            Name = thisTowerInfo.Split(new char[] { '(' })[0].Trim(); // first part is the name, trim off extra spaces.
            Weight = Int32.Parse(thisTowerInfo.Split(new char[] { '(' })[1].Trim()); //second part is the weight ( the closing param was split out above
            // if there was anything after the weight closing param, split it based on the markers and add to this program's list of programs it is holding. 
            DiscOfPrograms.AddRange((towerDefinition.Split(new char[] { ')' }).Length > 1) ? towerDefinition.Split(new char[] { ')' })[1].Split(new string[] { " -> ", ", " }, StringSplitOptions.RemoveEmptyEntries) : new string[] { });
            FullTower.Add(this); // put ourselves in to the list.
        }
        //Class method, not instance method. 
        public static void LinkThemUp()
        {
            //for all the programs defined in our tower, iterate through each of the programs they're holding and set that value so it links back.  
            FullTower.ForEach(tp => tp.DiscOfPrograms.ForEach(held => FullTower.Find(p => p.Name.Equals(held)).BaseProgramName = tp.Name));
        }
        //complex objects require an equality method that will accurately determine the situation for our purposes. Although I don't think I'm actually using it...
        public override bool Equals(object obj)
        {
            // in our case, name is the only piece that matters to match up (and likely only piece that would anyway ).
            return this.Name.Equals(((TowerProgram)obj).Name);
        }
        // this is just here to make the warning go away.
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
