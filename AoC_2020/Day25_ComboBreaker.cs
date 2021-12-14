using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day25_ComboBreaker : AoCodeModule
    {
        public Day25_ComboBreaker()
        {
            
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
            
        }
        public override void DoProcess()
        {
            //** > Result for Day25_ComboBreaker part 1:The encryption key matches: True(Process: 382.0249 ms)
            //** > Result for Day25_ComboBreaker part 1:The encryption key is: 6198540(Process: 382.0309 ms)
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            List<Tuple<long, long>> decryptions = new List<Tuple<long, long>>();
            foreach (string processingLine in inputFile)
            {
                Tuple<long,long> set = GetLoopSize(long.Parse(processingLine));
                decryptions.Add(set);
            }
            long result1 = GetEncryptionKey(decryptions[0].Item1, decryptions[1].Item2);
            long result2 = GetEncryptionKey(decryptions[1].Item1, decryptions[0].Item2);

            AddResult("The encryption key matches: " + (result1==result2).ToString());
            AddResult("The encryption key is: " + result1.ToString());

        }
        private Tuple<long,long> GetLoopSize(long target)
        {
            long result=0;
            long subjectNumber = 7;
            long iterator = 1;
            while (iterator != target)
            {
                iterator *= subjectNumber;
                iterator = iterator % 20201227;
                result++;
            }

            return new Tuple<long,long>(iterator,result);
        }
        private long GetEncryptionKey(long subjectNumber, long loopSize)
        {
            //long result = 0;
            long iterator = 1;
            for (int x=0; x<loopSize; x++)
            {
                iterator *= subjectNumber;
                iterator = iterator % 20201227;
            }

            return iterator;
        }
    }
}
