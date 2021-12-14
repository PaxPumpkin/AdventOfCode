using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day12_PassagePathing : AoCodeModule
   {
      public Day12_PassagePathing()
      {
         //Sample, Sample2, Sample3 - all check out both parts. 
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput(); 
         OutputFileReset(); 
      }
      public override void DoProcess()
      {
         //** > Result for Day12_PassagePathing part 1: Total Number of paths is 3485 (Process: 29.6712 ms)
         //** > Result for Day12_PassagePathing part 2: Total Number of paths is 85062 (Process: 42228.1797 ms)
         ResetProcessTimer(true);
         CaveConnector.CaveConnections.Clear();
         string[] caveConnectorDefinition;
         foreach (string processingLine in inputFile)
         {
            caveConnectorDefinition = processingLine.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            new CaveConnector(caveConnectorDefinition[0], caveConnectorDefinition[1]);
         }
         CaveConnector.EstablishBackPaths();
         CaveConnector start = CaveConnector.CaveConnections.Where(c => c.Name == "start").First(); // start at the very beginning. That's a very good place to start.
         CaveConnector.ResetVisits();
         List<string> allPaths = start.FindAllPathsTo("end").Where(p => p.EndsWith("end")).OrderBy(p=>p).ToList(); // remove deadend paths by including only those that end on "end". Match order of samples on AoC for ease of comparison.
         if (inputFileName.Contains("Sample")) allPaths.ForEach(p => Print(p));
         AddResult("Total Number of paths is " + allPaths.Count.ToString());
         ResetProcessTimer(true);
         if (inputFileName.Contains("Sample"))
         {
            Print("");
            Print("Part 2");
         }
         allPaths.Clear();
         // repeat pathfinding, allowing all "small" caves to be permitted to be traversed twice instead of only once. 
         // however, at any given pathfinding iteration, only a *single* "small" cave can be traversed twice. All the others will still be only once.
         // so loop through all "small" caves, redoing the pathfinding for each one as it is permitted 2 traversals.
         // Each set of paths must end at "end" (no dead-ends). Only add those found paths to the final list if they haven't already been found in a previous iteration. 
         // "start" and "end" can only ever be traversed once. No going back to start, and once hitting "end", the path is completed. Exclude them from the list of small caves.
         CaveConnector.CaveConnections.Where(c => !c.Big && c.Name != "start" && c.Name != "end").ToList().ForEach(smallCave => 
         {
            CaveConnector.currentlyAllowedTwoVisits = smallCave.Name;
            if (inputFileName.Contains("Sample")) Print("Allowing 2 Visits to " + smallCave.Name);
            CaveConnector.ResetVisits();
            List<string> pathsForIteration = start.FindAllPathsTo("end", true).Where(p => p.EndsWith("end")).ToList(); // remove dead-ends
            pathsForIteration = pathsForIteration.Where(p => !allPaths.Contains(p)).ToList(); // only add paths that hadn't already been saved
            allPaths.AddRange(pathsForIteration);

            if (inputFileName.Contains("Sample")) pathsForIteration.OrderBy(p => p).ToList().ForEach(p => Print(p)); // output sample data in same order as AoC page for ease of comparison.
         });
         
         if (inputFileName.Contains("Sample"))
         {
            Print("");
            Print("Full List");
            allPaths.OrderBy(p => p).ToList().ForEach(p => Print(p));
         }
         AddResult("Total Number of paths is " + allPaths.Count.ToString());
         ResetProcessTimer(true);
      }
   }
   public class CaveConnector
   {
      public string Name { get; set; }
      public List<CaveConnector> Connections { get; set; }
      public bool Big { get; set; }
      public int VisitCounter { get; set; }
      public static string currentlyAllowedTwoVisits = "";

      public static List<CaveConnector> CaveConnections = new List<CaveConnector>();
      
      public CaveConnector(string name)
      {
         Name = name;
         Big = name == name.ToUpper();
         VisitCounter = 0;
         Connections = new List<CaveConnector>();
         if (CaveConnections.Count(c=>c.Name==Name)==0)
            CaveConnections.Add(this);
      }
      public CaveConnector(string name, string connector) :this(name)
      {
         CaveConnector cave = CaveConnections.Where(c => c.Name == name).FirstOrDefault();
         if (cave == null) throw new Exception("Cave instantiation failed!");
         CaveConnector connectedTo = CaveConnections.Where(c => c.Name == connector).FirstOrDefault() ?? new CaveConnector(connector);
         if (cave.Connections.Count(c => c.Name == connector) == 0)
            cave.Connections.Add(connectedTo);
      }
      public List<string> FindAllPathsTo(string endPoint, bool allowTwice = false)
      {
         string pathBase = Name + ",";
         List<string> paths = new List<string>();
         VisitCounter++;
         List<CaveConnector> availableToVisit = Connections.Where(c => c.Big || c.VisitCounter==0 || (allowTwice && c.Name==currentlyAllowedTwoVisits && c.VisitCounter==1)).ToList();
         if (availableToVisit.Count == 0 || Name=="end")
         {
            paths.Add(Name);
         }
         else
         {
            foreach (CaveConnector c in availableToVisit)
            {
               
               if (c.Name == endPoint)
               {
                  paths.Add(pathBase + c.Name);
               }
               else
               {
                  List<string> foundPaths = c.FindAllPathsTo(endPoint, allowTwice);
                  for (int x = 0; x < foundPaths.Count; x++)
                     foundPaths[x] = pathBase + foundPaths[x];

                  paths.AddRange(foundPaths);
               }
            }
         }
         VisitCounter--;
         return paths;
      }
      public static void EstablishBackPaths()
      {
         //Instantiation only includes forward connections. But the pathing algorithm does permit conditional backwards traversal, so fill in those backwards paths.
         CaveConnections.ForEach(c => c.Connections.ForEach(cave => { if (cave.Connections.Count(cv => cv.Name == c.Name) == 0) { cave.Connections.Add(c); } }));
      }
      public static void ResetVisits()
      {
         CaveConnections.ForEach(c => { c.VisitCounter = 0; }); // techincally, all SHOULD be zero already.
      }
   }
}
