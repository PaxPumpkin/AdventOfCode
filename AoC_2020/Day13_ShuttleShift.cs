using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day13_ShuttleShift : AoCodeModule
    {
        public Day13_ShuttleShift()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + "Sample.txt";
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        public override void DoProcess()
        {

            //** > Result for Day13_ShuttleShift part 1:Bus ID * Number of Minutes: 5946(Process: 2.8147 ms)
            //** > Result for Day13_ShuttleShift part 2:Timestamp where all buses leave at their respective offset: 645338524823718(Process: 17.1142 ms)

            // a bit of golfing:
            //** > Result for Day13_ShuttleShift part 1:Bus ID * Number of Minutes: 5946(Process: 0.0413 ms)
            //** > Result for Day13_ShuttleShift part 2:Timestamp where all buses leave at their respective offset with CRT: 645338524823718(Process: 0.0473 ms)
            //** > Result for Day13_ShuttleShift part 3:Timestamp where all buses leave at their respective offset non CRT: 645338524823718(Process: 0.036 ms)

            // more golfing
            //** > Result for Day13_ShuttleShift part 1:Bus ID * Number of Minutes: 5946(Process: 0.0161 ms)
            //** > Result for Day13_ShuttleShift part 2:Timestamp where all buses leave at their respective offset with CRT: 645338524823718(Process: 0.0338 ms)
            //** > Result for Day13_ShuttleShift part 2:Timestamp where all buses leave at their respective offset non CRT: 645338524823718(Process: 0.0303 ms)
            //** > Result for Day13_ShuttleShift part 2:Timestamp where all buses leave at their respective offset non CRT with Tuples: 645338524823718(Process: 0.0272 ms)
            //(Total: 0.1169 ms)


            //PART 1
            ResetProcessTimer(true);
            long firstTimestampICanRide=long.Parse(inputFile[0]);
            List<int> BusIDs = new List<int>();
            inputFile[1].Split(new char[] { ',' }).ToList().ForEach(x => { if (x != "x") { BusIDs.Add(Int32.Parse(x)); } });
            int minutes = 0;

            while (BusIDs.Count(x => (firstTimestampICanRide + minutes) % x == 0) == 0) { minutes++; }
            AddResult("Bus ID * Number of Minutes: " + (minutes * BusIDs.Where(x => (firstTimestampICanRide + minutes) % x == 0).First()).ToString()); // includes elapsed time from last ResetProcessTimer
            
            
            // PART 2 with CRT
            ResetProcessTimer(true);
            BusIDs.Clear();
            inputFile[1].Split(new char[] { ',' }).ToList().ForEach(x => { if (x != "x") { BusIDs.Add(Int32.Parse(x)); } else { BusIDs.Add(0); } });
            // full disclosure: I'm not getting anywhere with this. I stole this code from Reddit. 
            // How the hell was I supposed to figure out the Chinese Remainder Theorem??
            long answer = ChineseRemainderTheorem(
                BusIDs
                    .Where(x => x > 0)
                    .Select(x => (long)x)
                    .ToArray(),
                BusIDs
                    .Select((x, i) => new { i, x })
                    .Where(x => x.x > 0)
                    .Select(x => (long)(x.x - x.i) % x.x) //(Bus ID - Position) % Bus ID
                    .ToArray()
            );
            AddResult("Timestamp where all buses leave at their respective offset with CRT: " + answer.ToString());


            /*
            // all my crap figuring, and trying to be fancy. 
            // I think it's the Tuples that made it too hard to read and figure out what was going wrong. 
            List<Tuple<int, int>> BusID = new List<Tuple<int, int>>();
            int index = 0;
            inputFile[2].Split(new char[] { ',' }).ToList().ForEach(x => { if (x != "x") { BusID.Add(new Tuple<int, int>(Int32.Parse(x), index)); } index++; });
            long ts = 1;
            BusID.ForEach(x => ts *= x.Item1 - x.Item2);
            //BusID.ForEach(x => ts *= ((x.Item1 - x.Item2)+ (x.Item1-x.Item2+1)%(x.Item2==0?1:x.Item2)));
            //BusID.ForEach(x => { int result = ((x.Item1 - x.Item2) % x.Item1); ts *= result > 0 ? result : 1; });
            while (BusID.Count(x => (ts + x.Item2) % x.Item1 == 0) != BusID.Count())// ||(x.Item1 % ts) - x.Item1) == 0) != BusID.Count()) 
            {
                ts += BusID[0].Item1;
            }

            */

            //PART 2 NON-CRT, non-tupled 
            ResetProcessTimer(); // reset timer, do NOT increment "part number"
            BusIDs.Clear();
            // Load all schedules, but do NOT exclude the "X" schedules this time so that we don't have to fuck with Tuples.
            inputFile[1].Split(new char[] { ',' }).ToList().ForEach(x => { if (x != "x") { BusIDs.Add(Int32.Parse(x)); } else { BusIDs.Add(0); } });
            // this accumulator is the currently determined PERIOD
            //  by which all buses will always return to their index as an offset.
            long minimumTimeIterationUntilNextMatch = BusIDs[0]; //start with first bus, period = ID
            long numberOfMinutes = 0; // time click accumulator
            int BusIDIndex = 1; // next Bus to re-calibrate

            //examine all Bus schedules.
            while (BusIDIndex < BusIDs.Count)
            {
                //Skip all the "X" schedules that we included to keep indices in a nicer way. 
                if (BusIDs[BusIDIndex] != 0 )
                {
                    //Increment the current time accumulator 
                    // with the latest determined value of 
                    // how many minutes until every Bus we've already examined
                    // will always be at its determined offset from zero. 
                    numberOfMinutes += minimumTimeIterationUntilNextMatch;

                    //Check if the current time results in positioning the current
                    //  Bus as the appropriate offset to match its index.
                    //If not, loop to keep incrementing the clock until it does. 
                    //   Remember, the BusIDIndex is respresenting the "offset from zero"
                    //   so this modulo ensures that BusID in X index is AT that offset at this time check. 
                    if ((numberOfMinutes + BusIDIndex) % BusIDs[BusIDIndex] == 0)
                    {
                        //IT MATCHED! FACTOR THAT PUPPY! 
                        // We are multiplying by the bus ID because for any X, 
                        // it will always return to this position EVERY X time clicks
                        minimumTimeIterationUntilNextMatch *= BusIDs[BusIDIndex];

                        //OK, start again with the next bus in our list. 
                        BusIDIndex++;
                    }
                }
                else BusIDIndex++; // this bus was a "X" bus. go to the next one.
            }
            AddResult("Timestamp where all buses leave at their respective offset non CRT: " + numberOfMinutes.ToString());

            /*
            //PART 2 NON-CRT, TUPLED, with explanation
            ResetProcessTimer(); // reset timer, do NOT increment "part number" for display
            // Tuple- first item with be the BusID, second item is the index in the list.
            List<Tuple<int, int>> BusSchedules = new List<Tuple<int, int>>();
            int index = 0; // counter for tuple Item2/Index indicator.
            // Load all relevant schedules only, not the X schedules.
            inputFile[1].Split(new char[] { ',' }).ToList().ForEach(x => 
                { 
                    if (x != "x") BusSchedules.Add(new Tuple<int,int>(Int32.Parse(x),index)); 
                    index++; 
                });
            // this accumulator is the currently determined PERIOD
            //  by which all buses will always return to their index as an offset.
            long minimumTimeClicksUntilNextMatch = BusSchedules[0].Item1; //first bus, period = ID = Item1
            long totalElapsedMinutes = 0; // time click accumulator

            //examine all Bus schedules except the first one, which doesn't require fiddlin' around.
            BusSchedules.Where(schedule=>schedule.Item2!=0).ToList().ForEach(BusSchedule =>
            {
                //Check if the current time results in positioning this next
                //      bus at the appropriate offset to match its index.
                //If not, loop to keep incrementing the clock until it does. 
                //      Remember, Item2 is the BusID *Index* 
                //      and is respresenting the "offset from zero"
                //      so this modulo ensures that BusID(Item1) in X index 
                //      is AT that offset at this time check.
                //   and the minimumTimeClicksUntilNextMatch represents a PERIOD 
                //      by which all previously examined buses will 
                //      ALSO modulo to 0 at their indices.
                while (((totalElapsedMinutes + BusSchedule.Item2) % BusSchedule.Item1 != 0))
                {
                    totalElapsedMinutes += minimumTimeClicksUntilNextMatch;
                }
                //WE GOT A MATCH! FACTOR THAT PUPPY! 
                //We are multiplying by the bus ID(Item1) because for any iteration of this
                //      time period going forward, this bus (and all previously determined buses)
                //      will always return to this desired position EVERY X time clicks
                minimumTimeIterationUntilNextMatch *= BusSchedule.Item1; 
            });
            AddResult("Timestamp where all buses leave at their respective offset non CRT with Tuples: " + numberOfMinutes.ToString());

            */
            //PART 2 NON-CRT, TUPLED, Cleaned
            ResetProcessTimer(); 
            List<Tuple<int, int>> BusSchedules = new List<Tuple<int, int>>();
            int index = 0; 
            inputFile[1].Split(new char[] { ',' }).ToList().ForEach(x =>
            {
                if (x != "x") BusSchedules.Add(new Tuple<int, int>(Int32.Parse(x), index));
                index++;
            });
            long minimumTimeClicksUntilNextMatch = BusSchedules[0].Item1; //first bus, period = ID = Item1
            long totalElapsedMinutes = 0; 
            BusSchedules.Where(schedule => schedule.Item2 != 0).ToList().ForEach(BusSchedule =>
            {
                while (((totalElapsedMinutes + BusSchedule.Item2) % BusSchedule.Item1 != 0))
                {
                    totalElapsedMinutes += minimumTimeClicksUntilNextMatch;
                }
                minimumTimeIterationUntilNextMatch *= BusSchedule.Item1;
            });
            AddResult("Time Tick : " + numberOfMinutes.ToString());

            //ResetProcessTimer();
            //List<int> BusScheduleList = new List<int>();
            //inputFile[1].Split(new char[] { ',' }).ToList().ForEach(x =>
            //{
            //    BusScheduleList.Add(Int32.Parse((x == "x") ? "0" : x));
            //});
            //long minimumTimeClicksPerPeriod = BusScheduleList[0]; //first bus, period = ID
            //long totalElapsedTimeClicks = 0; // time click accumulator
            //BusScheduleList.Where(schedule => schedule != 0)
            //    .Select((schedule, idx) => new { schedule, idx })
            //    .ToList().ForEach(BusSchedule =>
            //    {
            //        while (((totalElapsedTimeClicks + BusSchedule.idx) % BusSchedule.schedule != 0))
            //        {
            //            totalElapsedTimeClicks += minimumTimeClicksPerPeriod;
            //        }
            //        minimumTimeClicksPerPeriod *= BusSchedule.schedule;
            //    });
            //AddResult("Timestamp where all buses leave at their respective offset non CRT with out Tuples: " + numberOfMinutes.ToString());
        }
        private static long ChineseRemainderTheorem(long[] n, long[] a)
        {

            long prod = n.Aggregate(1, (long i, long j) => i * j);
            long sm = 0;

            for (int i = 0; i < n.Length; i++)
            {
                var p = prod / n[i];

                sm += a[i] * ModularMultiplicativeInverse(p, n[i]) * p;
            }

            return sm % prod;
        }
        static long ModularMultiplicativeInverse(long a, long mod)
        {
            long b = a % mod;

            for (int x = 1; x < mod; x++)
            {
                if ((b * x) % mod == 1)
                {
                    return x;
                }
            }

            return 1;
        }
        public long LeavesAtZero(int ID, int IDX)
        {
            int nextTimeAtZero = 0;
            int leavesAtZero = (IDX % ID);
            if (leavesAtZero != 0){
                int wholeTimes = (int)(IDX / ID);
                int largestIntValue = wholeTimes * ID;
                int difference = IDX - largestIntValue;
                nextTimeAtZero = IDX + difference;
            }
            else
            {
                nextTimeAtZero = IDX;
            }
            return nextTimeAtZero;
        }

        public bool thinking(int ts, int id, int idx)
        {
            //ts = 0;
            //int idx = 0;
            //int id = 17;
            bool test = ts % id == ((idx==0)?0:(id - idx));// *(ts%id);
            return test;
        }
    }
}
