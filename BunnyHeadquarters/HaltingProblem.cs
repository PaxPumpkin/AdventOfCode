using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class HaltingProblem : AoCodeModule
    {
        char currentState = 'A';
        long currentSlot = 50;
        short[] tape = new short[101];

        public HaltingProblem()
        {
            for (int x = 0; x <= tape.GetUpperBound(0); x++) { tape[x] = 0; }

        }

        public override void DoProcess()
        {
            for (int x = 0; x < 12172063; x++)
            {
                switch (currentState)
                {
                    case 'A':
  //                      If the current value is 0:
  //  - Write the value 1.
  //  - Move one slot to the right.
  //  - Continue with state B.
  //If the current value is 1:
  //  - Write the value 0.
  //  - Move one slot to the left.
  //  - Continue with state C.
                        if (tape[currentSlot] == 0)
                        {
                            tape[currentSlot] = 1;
                            currentSlot++;
                            currentState='B';
                        }
                        else
                        {
                            tape[currentSlot] = 0;
                            currentSlot--;
                            currentState = 'C';
                        }

                        break;
                    case 'B':
  //                      In state B:
  //If the current value is 0:
  //  - Write the value 1.
  //  - Move one slot to the left.
  //  - Continue with state A.
  //If the current value is 1:
  //  - Write the value 1.
  //  - Move one slot to the left.
  //  - Continue with state D.
                        if (tape[currentSlot] == 0)
                        {
                            tape[currentSlot] = 1;
                            currentSlot--;
                            currentState = 'A';
                        }
                        else
                        {
                            tape[currentSlot] = 1;
                            currentSlot--;
                            currentState = 'D';
                        }
                        break;
                    case 'C':
  //                      In state C:
  //If the current value is 0:
  //  - Write the value 1.
  //  - Move one slot to the right.
  //  - Continue with state D.
  //If the current value is 1:
  //  - Write the value 0.
  //  - Move one slot to the right.
  //  - Continue with state C.
                        if (tape[currentSlot] == 0)
                        {
                            tape[currentSlot] = 1;
                            currentSlot++;
                            currentState = 'D';
                        }
                        else
                        {
                            tape[currentSlot] = 0;
                            currentSlot++;
                            currentState = 'C';
                        }
                        break;
                    case 'D':
  //                      In state D:
  //If the current value is 0:
  //  - Write the value 0.
  //  - Move one slot to the left.
  //  - Continue with state B.
  //If the current value is 1:
  //  - Write the value 0.
  //  - Move one slot to the right.
  //  - Continue with state E.
                        if (tape[currentSlot] == 0)
                        {
                            tape[currentSlot] = 0;
                            currentSlot--;
                            currentState = 'B';
                        }
                        else
                        {
                            tape[currentSlot] = 0;
                            currentSlot++;
                            currentState = 'E';
                        }
                        break;
                    case 'E':
  //                      In state E:
  //If the current value is 0:
  //  - Write the value 1.
  //  - Move one slot to the right.
  //  - Continue with state C.
  //If the current value is 1:
  //  - Write the value 1.
  //  - Move one slot to the left.
  //  - Continue with state F.
                        if (tape[currentSlot] == 0)
                        {
                            tape[currentSlot] = 1;
                            currentSlot++;
                            currentState = 'C';
                        }
                        else
                        {
                            tape[currentSlot] = 1;
                            currentSlot--;
                            currentState = 'F';
                        }
                        break;
                    case 'F':
  //                      In state F:
  //If the current value is 0:
  //  - Write the value 1.
  //  - Move one slot to the left.
  //  - Continue with state E.
  //If the current value is 1:
  //  - Write the value 1.
  //  - Move one slot to the right.
  //  - Continue with state A.
                        if (tape[currentSlot] == 0)
                        {
                            tape[currentSlot] = 1;
                            currentSlot--;
                            currentState = 'E';
                        }
                        else
                        {
                            tape[currentSlot] = 1;
                            currentSlot++;
                            currentState = 'A';
                        }
                        break;
                    default:
                        Console.Write("WTF!");
                        break;

                }
                if (currentSlot < 0 || currentSlot > tape.GetUpperBound(0))
                    reDimensionArray();
            }
            FinalOutput.Add("Ughm, I'm done.");
            int checksum = 0;
            for (int x=0; x<=tape.GetUpperBound(0); x++)
            {
                if (tape[x] == 1) checksum++;
            }
            FinalOutput.Add("checksum:" + checksum);
        }
        private void reDimensionArray()
        {
            short[] newTape = new short[((int)((tape.GetUpperBound(0) + 1) * 1.5))];
            tape.CopyTo(newTape,((int)((newTape.GetUpperBound(0)-tape.GetUpperBound(0))/2)));
            long newZero = ((int)((newTape.GetUpperBound(0) - tape.GetUpperBound(0)) / 2));
            currentSlot += newZero;
            tape = newTape;
        }
    }
}
