using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class GarbageInTheStream : AoCodeModule
    {
        public GarbageInTheStream()
        {
            inputFileName = "garbageinthestream.txt";
            GetInput();
        }

        public override void DoProcess()
        {
            inputFile.ForEach(streamOfGarbage => { string gc; string score = Process(streamOfGarbage, out gc).ToString(); FinalOutput.Add("Final Score: " + score); FinalOutput.Add("Garbage Count: " + gc); });
            
        }
        private int Process(string streamOfGarbage, out string gc)
        {
            int totalScore = 0;
            int currentScore = 0;
            int groupCount=0;
            int garbageCount = 0;
            bool inGarbage = false;
            bool ignoreNext = false;
            foreach (char flotsam in streamOfGarbage)
            {
                if (!ignoreNext)
                {
                    //Look for special chars
                    switch (flotsam)
                    {
                        case '{':
                            if (!inGarbage)
                            {
                                currentScore++;
                            }
                            else garbageCount++;
                            break;
                        case '}':
                            if (!inGarbage)
                            {
                                totalScore += currentScore;
                                currentScore--;
                                groupCount++;
                            }
                            else garbageCount++;
                            break;
                        case '<':
                            if (!inGarbage) inGarbage = true;
                            else garbageCount++;
                            break;
                        case '>':
                            inGarbage = false;
                            break;
                        case '!':
                            ignoreNext = true;
                            break;
                        default:
                            if (inGarbage)garbageCount++;
                            break;
                    }
                }
                else ignoreNext = false;
            }
            gc = garbageCount.ToString();
            return totalScore;
        }
    }
}
