using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2020
{
    class Day06_CustomCustoms : AoCodeModule
    {
        public Day06_CustomCustoms()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput();
            OutputFileReset();        }
        public override void DoProcess()
        {
            //** > Result for Day06_CustomCustoms part 1:The summation is 6249(Process: 0 ms)
            //** > Result for Day06_CustomCustoms part 2:The summation is 3103(Process: 0 ms)
 
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            int summation = 0;
            List<char> answers = new List<char>();
            foreach (string processingLine in inputFile)
            {
                if (processingLine.Trim().Equals(""))
                {
                    summation += answers.Count();
                    answers = new List<char>();
                }
                else
                {
                    foreach(char c in processingLine)
                    {
                        if (!answers.Contains(c))
                        {
                            answers.Add(c);
                        }
                    }
                }
            }
            finalResult = "The summation is  " + summation.ToString();
            AddResult(finalResult); 
            ResetProcessTimer(true);
            summation = 0;
            answers = new List<char>();
            bool newgroup = true;
            List<char> baddies = new List<char>();
            List<char> processingLineList = new List<char>();
            foreach (string processingLine in inputFile)
            {
                if (processingLine.Trim().Equals(""))
                {
                    summation += answers.Count();
                    answers.Clear();
                    newgroup = true;
                }
                else
                {
                    if (newgroup)
                    {
                        foreach (char c in processingLine)
                        {
                            answers.Add(c);
                        }
                        newgroup = false;
                    }
                    else
                    {
                        if (answers.Count > 0)
                        {
                            baddies.Clear();
                            processingLineList = processingLine.ToList();
                            foreach (char c in answers)
                            {
                                if (!processingLineList.Contains(c))
                                {
                                    baddies.Add(c);
                                }
                            }
                            baddies.ForEach(c => answers.Remove(c));
                        }
                    }
                }
            }
            finalResult = "The summation is  " + summation.ToString();
            AddResult(finalResult); 
        }
    }
}
