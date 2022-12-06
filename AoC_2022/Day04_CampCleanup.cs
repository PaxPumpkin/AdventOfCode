using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day04_CampCleanup : AoCodeModule
    {
        public Day04_CampCleanup()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        public override void DoProcess()
        {
            //** > Result for Day04_CampCleanup part 1: Number of assignments where one fully contains the other: 651 (Process: 0.876 ms)
            //** > Result for Day04_CampCleanup part 2: Number of assignments where one overlaps at all: 956 (Process: 0.0359 ms)
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            List<CleanupAssignment> assignments = new List<CleanupAssignment>();
            string[] assignmentPair;
            foreach (string processingLine in inputFile)
            {
                assignmentPair = processingLine.Split(new char[] { ',' });
                assignments.Add(new CleanupAssignment(assignmentPair[0], assignmentPair[1]));
            }
            finalResult = "Number of assignments where one fully contains the other: " + assignments.Count(ass => ass.FullyOverlapped).ToString();
            AddResult(finalResult); 
            ResetProcessTimer(true);
            finalResult = "Number of assignments where one overlaps at all: " + assignments.Count(ass => ass.OverlapAtAll).ToString();
            AddResult(finalResult); 
            ResetProcessTimer(true);
        }
    }
    public class CleanupAssignment
    {
        (int, int) SectionOne;
        (int, int) SectionTwo;

        public bool FullyOverlapped
        {
            get
            {
                return (SectionOne.Item1 <= SectionTwo.Item1 && SectionOne.Item2 >= SectionTwo.Item2) || (SectionTwo.Item1 <= SectionOne.Item1 && SectionTwo.Item2 >= SectionOne.Item2);
            }
        }
        public bool OverlapAtAll
        {
            get
            {
                return (SectionOne.Item1 >= SectionTwo.Item1 && SectionOne.Item1 <= SectionTwo.Item2) || (SectionOne.Item2 >= SectionTwo.Item1 && SectionOne.Item2 <= SectionTwo.Item2) ||
                     (SectionTwo.Item1 >= SectionOne.Item1 && SectionTwo.Item1 <= SectionOne.Item2) || (SectionTwo.Item2 >= SectionOne.Item1 && SectionTwo.Item2 <= SectionOne.Item2);
            }
        }
        public CleanupAssignment(string sectionOne, string sectionTwo)
        {
            string[] s1 = sectionOne.Split(new char[] { '-' });
            string[] s2 = sectionTwo.Split(new char[] { '-' });
            SectionOne = (int.Parse(s1[0]), int.Parse(s1[1]));
            SectionTwo = (int.Parse(s2[0]), int.Parse(s2[1]));
        }
    }
}
