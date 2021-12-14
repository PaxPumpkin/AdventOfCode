using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace BunnyHeadquarters
{
    class DigitalPlumber : AoCodeModule
    {
        public DigitalPlumber()
        {
            inputFileName = "digitalplumber.txt";
            base.GetInput();
        }

        public override void DoProcess()
        {
            inputFile.ForEach(line => new PipedProgram(line)); // for each line of input, create an object instance.
            int result = PipedProgram.allPrograms.Where(x => x.id.Equals("0")).FirstOrDefault().DownstreamConnectionCount()+1 ; // starting with program 0, find all connections. Add one for the starting point, since the recursion won't count itself. 
            FinalOutput.Add("Program count to 0: " + result.ToString());
            int totalGroups = PipedProgram.GroupCount();
            FinalOutput.Add("Total Program Groups: " + totalGroups.ToString());
        }

    }
    class PipedProgram
    {
        public static List<PipedProgram> allPrograms = new List<PipedProgram>(); // single list for all programs defined
        public static List<string> alreadyExamined = new List<string>(); // prevents infinite recursion while examining any given starting point.
        
        public string id = ""; // my id
        public List<string> connections = new List<string>(); // list of ids I'm connected to.

        public PipedProgram(string definition) // "0 <-> 1, 2, 3" for example
        {
            // bust it out to the individual units: [id,connection,connection,connection]
            string[] pieces = definition.Split(new string[] { " <-> ", ", " }, StringSplitOptions.RemoveEmptyEntries);
            this.id = pieces[0]; // id is first piece
            connections = new List<string>(pieces.Skip(1)); // connections are all the rest of the pieces
            allPrograms.Add(this); // keep track of me for everyone else to hook into
        }
        // starting from "me" how many other programs can reach me? Recursive function. 
        public int DownstreamConnectionCount()
        {
            if (!alreadyExamined.Contains(this.id)) alreadyExamined.Add(this.id); // if anyone points back at me during the recursion, no need to go through my lists...
            int counter = 0; // don't count me. The initial starting point will be added outside this recursion.
            // for each ID that was listed as having a direct connection to me...
            connections.ForEach(x => { 
                if (!alreadyExamined.Contains(x)) // if we haven't already been down this path. 
                {
                    counter++; // connected to this program in the iteration, so that counts as 1 connection.
                    alreadyExamined.Add(x); // keep track that we will have already traversed that path.
                    PipedProgram connected = allPrograms.Where(y => y.id.Equals(x)).FirstOrDefault(); // get that program id's instance
                    counter += connected.DownstreamConnectionCount(); // and traverse ITs connections, add it to our count
                }
            });
            return counter; // a value representing all "connections" and their recursively determined connections as well. 

        }
        // static method for all groupings ( uniquely interconnected sets of program ids )
        public static int GroupCount()
        {
            // a list of lists. Each inner list represents the grouping that is created by recursively traversing an ID
            List<List<string>> groupings = new List<List<string>>();
            // for all programs, we need to find its grouping
            allPrograms.ForEach(x =>
            {
                // if any list in the list of lists contains my ID, I already belong to a group, so don't do it again.
                if (!groupings.Any(list => list.Contains(x.id)))
                {
                    alreadyExamined = new List<string>();// init the group list for traversal
                    x.DownstreamConnectionCount(); // recursively traverse the group starting from me.
                    groupings.Add(alreadyExamined); // add the result to my list of groups.
                }
            });
            alreadyExamined = new List<string>(); // reinitialize the recursion throttle list since we are done with it. 
            return groupings.Count;
        }
    }
}
