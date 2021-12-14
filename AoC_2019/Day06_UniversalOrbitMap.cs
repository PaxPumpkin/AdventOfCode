using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunUtilities;

namespace AoC_2019
{
    class Day06_UniversalOrbitMap : AoCodeModule
    {
        int orbitCount = 0;
        List<Orbiter> allOrbiters = new List<Orbiter>();
        public Day06_UniversalOrbitMap()
        {
            
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput();
            OutputFileReset();

        }
        public override void DoProcess()
        {
            //** > Result for Day06_UniversalOrbitMap part 1:Total of direct and indirect orbits: 270768(Process: 51 ms)
            //** > Result for Day06_UniversalOrbitMap part 2:Orbital Transfers Required: 451(Process: 0 ms)
            

            ResetProcessTimer(true);
            char[] splitters = new char[] { ')' };
            
            foreach (string processingLine in inputFile)
            {
                string[] line = processingLine.Split(splitters);
                allOrbiters.Add(new Orbiter() { Name = line[1], OrbitsName = line[0] });
            }
            Orbiter COM = new Orbiter(){ Name = "COM", orbitsToCOM = 0, OrbitsMe = allOrbiters.Where(x => x.OrbitsName == "COM").ToList() };
            allOrbiters.Add(COM);
            RecurseOrbits(COM.OrbitsMe);
            AddResult("Total of direct and indirect orbits: " + allOrbiters.Sum(x=>x.orbitsToCOM).ToString());

            ResetProcessTimer(true);

            Orbiter ME = allOrbiters.Where(x => x.Name == "YOU").First();
            Orbiter SAN = allOrbiters.Where(x => x.Name == "SAN").First();

            AddResult("Orbital Transfers Required: " + CalculateMinimumTransfers(ME,SAN).ToString());

        }
        private void RecurseOrbits(List<Orbiter> orbiters)
        {
            orbitCount++;
            foreach(Orbiter orbiter in orbiters)
            {
                orbiter.orbitsToCOM = orbitCount;
                orbiter.OrbitsMe = allOrbiters.Where(x => x.OrbitsName == orbiter.Name).ToList();
                orbiter.IOrbit = allOrbiters.Where(x => x.Name == orbiter.OrbitsName).First();
                RecurseOrbits(orbiter.OrbitsMe);
            }
            orbitCount--;
        }

        private int CalculateMinimumTransfers(Orbiter first, Orbiter second)
        {
            // These lists will be ordered where the first item in the list is the farthest away.
            // iterating until there is a common point in both, will give us the intersection.
            // The distance in transferring would be (endpoint1.orbitsToCOM-1 + endpoint2.orbitsToCOM-1) - (2* the intersection's Orbits to COM)
            // subtract 1 from each endpoint because puzzle says NOT between endpoints, but between the objects the enpoints themselves orbit. That's -1 each.
            // subtract the count of orbits to COM for each path at the instersection point, one... because we're not going there, and two, it's been added twice in the totals.
            List<string> firstPath = NamesToCOM(first);
            List<string> secondPath = NamesToCOM(second);
            string intersectionName = "NOPERZ";
            for (int x=0; x<firstPath.Count; x++)
            {
                if (secondPath.Contains(firstPath[x]))
                {
                    intersectionName = firstPath[x];
                    break;
                }
            }
            Orbiter intersection = allOrbiters.Where(x => x.Name == intersectionName).First();
            int xfers = (first.orbitsToCOM-1) + (second.orbitsToCOM-1) - (2*intersection.orbitsToCOM);

            return xfers;
        }
        private List<string> NamesToCOM(Orbiter endpoint)
        {
            List<string> path = new List<string>();
            Orbiter next = endpoint.IOrbit;
            while (next.Name != "COM")
            {
                path.Add(next.Name);
                next = next.IOrbit;
            }
            path.Add("COM");
            return path;
        }
        class Orbiter
        {
            public Orbiter IOrbit = null;
            public List<Orbiter> OrbitsMe = new List<Orbiter>();
            public int orbitsToCOM = 0;
            public string Name = "";
            public string OrbitsName = "";
        }
    }
}
