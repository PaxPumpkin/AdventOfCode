using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class PermutationPromenade : AoCodeModule
    {
        public PermutationPromenade()
        {
            inputFileName = "permutationpromenade.txt";
            base.GetInput();
        }
        public override void DoProcess()
        {
            
            List<string> DanceMoves = inputFile[0].Split(new char[] { ',' }).ToList<string>();
            char[] programs = "abcdefghijklmnop".ToCharArray();
            List<string> permutations = new List<string>();
            permutations.Add(new string(programs));
            bool NotFound = true;
            int howMany = 1000000000; // 1 billion. but we're not going to go that route. Just keep going until we see something again. Once we do that, we know the length of the cycle and can just move forward from that point the remainder of cycles necessary.
            string finalResult = "Not Set";
            for (int q1 = 1; q1 <= howMany && NotFound; q1++) // this was my downfall at first. Iteration was 0<1000000000, when it should have been 1<=1000000000. The modulo was off by one. 
            {
                DanceMoves.ForEach(move =>
                {
                    int position1 = 0;
                    int position2 = 0;
                    char holder;
                    char type = move[0];
                    switch (type)
                    {
                        case 's':
                            position1 = Int32.Parse(move.Substring(1));
                            char[] temp = new char[programs.Length];
                            programs.ToList().Skip(programs.Length - position1).ToArray<char>().CopyTo(temp, 0);
                            programs.Take(programs.Length - position1).ToArray<char>().CopyTo(temp, position1);
                            programs = temp;
                            break;
                        case 'x':
                            position1 = Int32.Parse(move.Substring(1).Split(new char[] { '/' })[0]);
                            position2 = Int32.Parse(move.Substring(1).Split(new char[] { '/' })[1]);
                            holder = programs[position1];
                            programs[position1] = programs[position2];
                            programs[position2] = holder;
                            break;
                        case 'p':
                            position1 = (new string(programs)).IndexOf(move.Substring(1, 1));
                            position2 = (new string(programs)).IndexOf(move.Substring(3, 1));
                            holder = programs[position1];
                            programs[position1] = programs[position2];
                            programs[position2] = holder;
                            break;
                        default:
                            throw new Exception("BAD MOVE DETECTED! -- " + move);
                            break;
                    }
                });
                if (q1 == 1) { FinalOutput.Add("Result of the dance first 1 cycle:" + (new string(programs))); permutations.Add(new string(programs)); } // no need to check if we match at the first iteration!
                else{
                    if (permutations[0].Equals(new string(programs))){
                        NotFound = false;
                        FinalOutput.Add("we hit a cycle loop at iteration:" + q1.ToString() + " when the permutation became " + (new string(programs))); // found to be 60 cycles.
                        finalResult = permutations[howMany % q1];  // just the result that would happen from this point for the remainder of the iterations...
                    }
                    else{
                        permutations.Add(new string(programs));
                    }
                }
            }
            FinalOutput.Add("Result of the billion dances:" + finalResult);
        }
    }
}
