using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day07_TheSumOfItsParts : AoCodeModule
    {
        public Day07_TheSumOfItsParts()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); //base class method
            OutputFileReset(); // output file will be in the same location as input, with ".output.txt" appended to the name. This clears previous output from file.
        }
        public override void DoProcess()
        {

            ResetProcessTimer(true);// part 1 start
            foreach (string processingLine in inputFile)
            {
                new InstructionStep(processingLine.Split(new string[] { "Step", " ", "must be finished before step", "can begin." }, StringSplitOptions.RemoveEmptyEntries));
            }
            string result = "Order: ";
            while (!InstructionStep.AllDone)
            {
                result += InstructionStep.all.Where(x => x.Ready == true).OrderBy(x => x.StepName).First().Do();
                
            }
            AddResult(result);
            ResetProcessTimer(true); //part 2 start
            InstructionStep.all.ForEach(x => x.done = false); // reset to not done so "ready" flag will toggle correctly. 
            result = "Order: ";
            int totalDuration = 0;
            List<Worker> workers = new List<Worker>();
            for (int x = 1; x <= 5; x++) workers.Add(new Worker(x));
            while (!InstructionStep.AllDone)
            {
                List<Worker> available = workers.Where(x => x.Available == true).ToList(); // decrements second counter for all in-progress tasks, too.
                available.ForEach(worker => {
                    List<InstructionStep> tasks = InstructionStep.all.Where(x => x.Ready == true && x.inProgress==false).OrderBy(x => x.StepName).ToList();
                    if (tasks.Count > 0)
                    {
                        worker.DoTask(tasks.First());
                    }
                });
                //Console.Write(totalDuration.ToString().PadLeft(4, '0') + " - ");
                //workers.ForEach(worker => { Console.Write(worker.OnTask==null?".":worker.OnTask.StepName); });
                //Console.Write("\n");
                totalDuration++;
            }
            totalDuration--; // that last second should have all workers idle. 
            AddResult("Total Seconds: " + totalDuration.ToString());
            InstructionStep.all.Clear();
        }
    }
    public class Worker
    {
        public bool Available { get
            {// each worker is checked for availability through each loop/second. Any in-progress tasks should decrement remaining time 
                if (SecondsOccupied > 0) { SecondsOccupied--; return false; }
                if (OnTask != null) { OnTask.done = true; OnTask = null; } // seconds occupied is now zero, if there was an active task, mark it done and remove it from this worker.
                return true; // seconds occupied is zero, so yeah... put him to work!
            }
        }
        public int Number { get; set; }
        public int SecondsOccupied { get; set; }
        public InstructionStep OnTask { get; set; }
        public Worker(int id)
        {
            Number = id;
            SecondsOccupied = 0;
            OnTask = null;
        }
        public void DoTask(InstructionStep task)
        {
            OnTask = task;
            task.inProgress = true;
            SecondsOccupied = task.SecondsToComplete - 1; // since technically this is the same second it was assigned...
        }
    }
    public class InstructionStep
    {
        public static List<InstructionStep> all = new List<InstructionStep>();
        public static bool AllDone { get {
                return all.Count(x => x.done == false) == 0;
            } }
        public string StepName { get; set; }
        public List<InstructionStep> PrecedesStep { get; set; }
        public bool done = false;
        public bool inProgress = false;
        public int SecondsToComplete
        {
            get
            {
                return ((int)StepName[0])-4; // Char A is int 65, subtract 4 to get 61 --- our desired representation. 
            }
        }

        public bool Ready
        {
            get
            {
                List<string> stepsWithPrecedants = new List<string>();
                all.ForEach(x => stepsWithPrecedants.AddRange(x.PrecedesStep.Select(y => y.StepName).ToList()));
                return !done && all.Count(x => x.PrecedesStep.Contains(this) && x.done == false) == 0;
            }
        }
        public InstructionStep(string[] parts)
        {
            InstructionStep meMaybe = all.Where(x => x.StepName.Equals(parts[0].Trim())).FirstOrDefault();
            StepName = parts[0].Trim();
            PrecedesStep = new List<InstructionStep>();
            if (parts.Length > 1)
            {
                InstructionStep existing = all.Where(x => x.StepName.Equals(parts[1].Trim())).FirstOrDefault();
                if (existing == null)
                {
                    (meMaybe ?? this).PrecedesStep.Add(new InstructionStep(new string[] { parts[1].Trim() }));
                }
                else
                {
                    (meMaybe ?? this).PrecedesStep.Add(existing);
                }
                (meMaybe ?? this).PrecedesStep.Sort((x, y) => x.StepName.CompareTo(y.StepName));
            }
            if (meMaybe==null) all.Add(this);
        }
        public string Do()
        {
            this.done = true;
            return this.StepName;
        }
        public override bool Equals(object other)
        {
            InstructionStep otherIS = other as InstructionStep;
            return otherIS.StepName == this.StepName;
        }
        public override int GetHashCode()
        {
            return this.StepName.GetHashCode() ^ this.PrecedesStep.GetHashCode();
        }
    }
    public class InstructionStepComparer : IEqualityComparer<InstructionStep>
    {

        public bool Equals(InstructionStep first, InstructionStep second)
        {
            return (first.StepName == second.StepName);
        }

        public int GetHashCode(InstructionStep obj)
        {
            return obj.StepName.GetHashCode() ^ obj.PrecedesStep.GetHashCode();
        }
    }
}
