using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day04_ReposeRecord : AoCodeModule
    {
        public Day04_ReposeRecord()
        {
            /// AoCodeModule boilerplate --- 
            // If you always save input file in the /InputFiles/ subfolder and name it the same as the class processing it, this will work.
            // if you put it elsewhere or name it differently, just change below. 
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); //base class method
            OutputFileReset(); // output file will be in the same location as input, with ".output.txt" appended to the name. This clears previous output from file.
            //Print("Did Something");// outputs to console immediately
            //Print("DidSomethingElse", FileOutputAlso); // both console and output file
            //Print("Another Thing", FileOutputOnly); // output file only.
        }
        public override void DoProcess()
        {
            //If Comma Delimited on a single input line in file, uncomment this and use inputItems instead of inputfile
            //List<string> inputItems = inputFile[0].Split(new char[] { ',' }).ToList<string>();
            string finalResult = "Not Set";
            ResetProcessTimer(true);// true also iterates the section marker
            string[] record;
            List<ActivityRecord> records = new List<ActivityRecord>();
            List<GuardRecord> guardRecords = new List<GuardRecord>();
            // load up data raw
            foreach (string processingLine in inputFile)
            {
                record = processingLine.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                records.Add(new ActivityRecord(record[0], record[1]));
            }
            // put it in time order
            records = records.OrderBy(x => x.RecordDate).ToList();
            GuardRecord lastGuard = new GuardRecord(DateTime.Now); // this record is not ever used, just prevents unitialized variable compiler error.
            // creates a new guard record each time the guard comes on duty, records minutes asleep
            records.ForEach(x =>
            {
                if (x.Activity.Contains("Guard"))
                {
                    lastGuard = new GuardRecord(x.RecordDate)
                    { // stupid split, wordy, but whatever.
                        ID = int.Parse(x.Activity.Split(new string[] { "Guard", "#", " ", "begins", "shift" }, StringSplitOptions.RemoveEmptyEntries)[0]),
                    };
                    guardRecords.Add(lastGuard);
                }
                else if (x.Activity.Contains("asleep"))
                {
                    lastGuard.Sleep(x.RecordDate);
                }
                else //wakes up only other option
                {
                    lastGuard.Wake(x.RecordDate);
                }
            });
            var MostSleepyGuard = guardRecords.GroupBy(x => x.ID).Select(group => new { ID = group.First().ID, TotalMinutes = group.Sum(g => g.TotalMinutesAsleep) }).OrderByDescending(x => x.TotalMinutes).First();
            // get all the activity records for the sleepiest guard so we can determine which minute is the most common to be asleep.
            List<GuardRecord> sleepiestGuardGraph = guardRecords.Where(x => x.ID == MostSleepyGuard.ID).ToList();
            // MostCommonMinutesAsleep returns a 3-part tuple from all of a Guard's records (graph)
            // 1st the minute most often asleep
            // 2nd the number of times in the graph this minute was found to be asleep (part 2 useful info)
            // 3rd returning the input ID of the guard to save a step in the second problem's solution. 
            int mostCommonAsleepMinute = GuardRecord.MostCommonMinuteAsleep(sleepiestGuardGraph).Item1;
            finalResult = (mostCommonAsleepMinute * MostSleepyGuard.ID).ToString();
            AddResult(finalResult); 

            //Part 2
            ResetProcessTimer(true);
            // list of all guards' graphs for all guards' logs to find who has the highest frequency of a given minute
            List<Tuple<int, int, int>> totalGraph = new List<Tuple<int, int,int>>();
            // get graphs for everyone. 
            guardRecords.Select(x => new { x.ID }).Distinct().ToList().ForEach(x => { totalGraph.Add(GuardRecord.MostCommonMinuteAsleep(guardRecords.Where(y => y.ID == x.ID).ToList())); });
            // of all the graphs, get the one with the highest frequency (item 2). The guard's ID is in the returned object already. 
            var finalFilter = totalGraph.Where(y => y.Item2 == totalGraph.Max(x => x.Item2)).ToList(); // should be a list of one
            AddResult((finalFilter[0].Item1 * finalFilter[0].Item3).ToString());
        }
    }
    public class ActivityRecord
    {
        public DateTime RecordDate { get; set; }
        public string Activity { get; set; }
        public ActivityRecord(string dt, string act)
        {
            RecordDate = DateTime.Parse(dt);
            Activity = act;
        }
    }
    public class GuardRecord
    {
        public int ID { get; set; } // for each guarding incident, the guard in question
        public string Date { get; set; } // the date the guard came on duty
        public int[] hour = new int[60]; // graph of the hour between midnight and 1am
        public int TotalMinutesAsleep
        {
            get {
                return hour.Sum(); // refactored to this.
                //int tot = 0; hour.Sum()
                //for (int x = 0; x <= hour.GetUpperBound(0); x++) { tot += hour[x]; }
                //return tot;
            }
        }
        public GuardRecord(DateTime when)
        {
            // if they start at 23:58pm on Jan 1, the shift really applies to Jan2 12:00am-1am
            if (when.ToString("HH") != "00")
            {
                when = when.AddDays(1);
            }
            this.Date = when.ToString("MM-dd"); // match the formatting. 
        }
        public void Sleep(DateTime when)
        {
            if (this.Date == null || this.Date == "") // leftover from when I wasn't capturing this earlier
            {
                this.Date = when.ToString("MM-dd");
            }
            int fromMinute = int.Parse(when.ToString("mm"));
            // set all minutes after sleepy time to asleep. Waking up will clear the appropriate elements.
            for (int x = fromMinute; x <= hour.GetUpperBound(0); x++) { hour[x] = 1; } // probably easier way to do this.
        }
        public void Wake(DateTime when)
        {
            int fromMinute = int.Parse(when.ToString("mm"));
            // reset all elements to awake from the minute the guard woke up. 
            for (int x = fromMinute; x <= hour.GetUpperBound(0); x++) { hour[x] = 0; }
        }
        public string GetPrintRecord() // because I wanted to make an output like the problem had. Not useful except for troubleshooting. 
        {
            string result = "";
            result += this.Date + " #" + ID.ToString().PadRight(4,' ') + " ";
            for (int x = 0; x <= hour.GetUpperBound(0); x++) { result += (hour[x] == 0) ? "." : "#"; }
            return result;
        }
        public static string GetHeader()
        {
            string result = "";
            result += "DATE  ID    MINUTE" + (new string(' ', 53)) + Environment.NewLine;
            result += (new string(' ', 12));
            for (int x = 0; x < 60; x++) { result += (x < 10) ? "0" : (x.ToString()[0].ToString()); }
            result += Environment.NewLine + (new string(' ', 12));
            for (int x = 0; x < 60; x++) { result += (x < 10) ? x.ToString() : (x.ToString()[1].ToString()); }
            return result;
        }
        /// <summary>
        /// All the work is done here!
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Tuple<int,int,int> MostCommonMinuteAsleep(List<GuardRecord> input)
        {
            int[] hoursOverlap = new int[60]; // keeps an overlapped counter for when each minute is mentioned in a particular guard's graph
            int gId = 0;
            input.ForEach(x =>  // for each activity log for a given guard, increment the counters for all cells. Since 1=asleep, only asleep minutes increment.
            {
                gId = x.ID;
                for (int i = 0; i < 60; i++) { hoursOverlap[i] += x.hour[i]; }
            });
            int maxIndex = -1;
            int maxMinutes = 0;
            // find out which minute is most asleep.
            for (int i = 0; i < 60; i++) { if (hoursOverlap[i] > maxMinutes) { maxMinutes = hoursOverlap[i]; maxIndex = i; } }
            // return which minute it was, the number of times it happened, and the guard's ID
            return new Tuple<int,int,int>(maxIndex,maxMinutes,gId);
        }
    }
}
