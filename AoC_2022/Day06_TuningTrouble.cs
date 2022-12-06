using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day06_TuningTrouble : AoCodeModule
    {
        public Day06_TuningTrouble()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day06_TuningTrouble part 1: Unique Signal Pattern Found at character 1235 (Process: 0.5717 ms)
            //** > Result for Day06_TuningTrouble part 2: Start of Message Marker Found at character 3051 (Process: 6.5706 ms)
            string finalResult, signal = inputFile[0];
            int PACKET_MARKER = 4, MESSAGE_MARKER = 14; // unique char count to qualify as marker
            ResetProcessTimer(true);

            finalResult = "Start of Packet Marker Found at character: " + ScanSignal(signal, PACKET_MARKER); 
            // saw this style on reddit, but over multiple runs, it is consistently and notably slower.
            //finalResult = signal.Select((x, i) => (signal.Skip(i).Take(PACKET_MARKER).Distinct().Count() == PACKET_MARKER, i + PACKET_MARKER)).First(x => x.Item1).Item2.ToString();
            AddResult(finalResult); ResetProcessTimer(true);

            finalResult = "Start of Message Marker Found at character: " + ScanSignal(signal, MESSAGE_MARKER); 
            //finalResult = signal.Select((x, i) => (signal.Skip(i).Take(MESSAGE_MARKER).Distinct().Count() == MESSAGE_MARKER, i + MESSAGE_MARKER)).First(x => x.Item1).Item2.ToString();
            AddResult(finalResult); ResetProcessTimer(true);

        }
        public string ScanSignal(string stream, int markerLength)
        {
            Queue<char> signalStream = new Queue<char>();
            int streamCounter = 0;
            string processedStreamLength = "Not Found";
            foreach (char c in stream)
            {
                streamCounter++;
                signalStream.Enqueue(c);
                if (signalStream.Count > markerLength) signalStream.Dequeue(); // keep window on stream processing to exact size of marker
                //if (signalStream.Distinct().Count() == markerLength) // saw this on reddit, it also had odd standouts in taking longer to run than the group by (which I really don't get, at all, but it's true)
                if (signalStream.GroupBy(ch => ch).Count() == markerLength) // group by gives number of unique characters in queue, if it matches our target length, we found it. 
                {
                    processedStreamLength = streamCounter.ToString();
                    signalStream.Clear();
                    break;
                }
            }
            return processedStreamLength;
        }
    }
}
