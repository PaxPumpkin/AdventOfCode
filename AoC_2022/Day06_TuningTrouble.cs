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
            string finalResult;
            ResetProcessTimer(true);
            finalResult = "Unique Signal Pattern Found at character " + ScanSignal(inputFile[0], 4);
            AddResult(finalResult); 
            ResetProcessTimer(true);
            finalResult = "Start of Message Marker Found at character " + ScanSignal(inputFile[0], 14);
            AddResult(finalResult); 
            ResetProcessTimer(true);
        }
        public string ScanSignal(string stream, int headerMarkerSize)
        {
            Queue<char> signalStream = new Queue<char>();
            int charCounter = 0;
            string processedStreamLength = "Not Found";
            foreach (char c in stream)
            {
                charCounter++;
                signalStream.Enqueue(c);
                if (signalStream.Count > headerMarkerSize)
                {
                    signalStream.Dequeue();
                }
                if (signalStream.GroupBy(ch => ch).Count() == headerMarkerSize)
                {
                    processedStreamLength = charCounter.ToString();
                    break;
                }
            }
            signalStream.Clear();
            return processedStreamLength;
        }
    }
}
