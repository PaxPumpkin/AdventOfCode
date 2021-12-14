using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2019
{
    class Day02_1202ProgramAlarm : AoCodeModule
    {
        public Day02_1202ProgramAlarm()
        {

            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day02_1202ProgramAlarm part 1:Value at position 0 after execution is: 4138687(Process: 1 ms)
            //** > Result for Day02_1202ProgramAlarm part 2:The noun and verb to produce the desired result are 66, 35(Process: 24 ms)
            //** > Result for Day02_1202ProgramAlarm part 2:Answer: 6635(Process: 24 ms)
  
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            List<long> intCodeProgram = new List<long>();
            foreach (string processingLine in inputFile) // should just be 1
            {
                string[] programNumbers = processingLine.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string pn in programNumbers)
                {
                    intCodeProgram.Add(Convert.ToInt64(pn));
                }
            }

            IntCodeComputer compy = new IntCodeComputer(intCodeProgram);
            compy.SetProgramInstruction(1, 12);
            compy.SetProgramInstruction(2, 2);
            compy.RunProgram();

            finalResult = "Value at position 0 after execution is: " + compy.GetProgramValue(0).ToString();
            AddResult(finalResult);

            ResetProcessTimer(true);
            bool completed = false;
            int noun=0, verb=0;
            for (noun=0; noun<=99 && !completed; noun++)
            {
                for (verb=0; verb<=99 && !completed; verb++)
                {
                    compy.ResetProgram();
                    compy.SetProgramInstruction(1, noun);
                    compy.SetProgramInstruction(2, verb);
                    compy.RunProgram();
                    completed = compy.GetProgramValue(0) == 19690720;
                }
            }
            if (completed) { noun--; verb--; } 
            finalResult = "The noun and verb to produce the desired result are " + noun.ToString() + ", " + verb.ToString();
            AddResult(finalResult);
            finalResult = "Answer: " + ((100*noun)+verb).ToString();
            AddResult(finalResult);
        }
    }
}
