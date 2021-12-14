using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day08_HandheldHalting : AoCodeModule
    {
        public Day08_HandheldHalting()
        {
            
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); //base class method
            OutputFileReset(); // output file will be in the same location as input, with ".output.txt" appended to the name. This clears previous output from file.
            
        }
        public override void DoProcess()
        {
            // this is under first-run conditions, unmodified input program.
            //** > Result for Day08_HandheldHalting part 0:Data setup completed(Process: 1 ms)
            //** > Result for Day08_HandheldHalting part 1:When an instruction hit the infinite loop condition, the accumulator was at the value: 1548(Process: 0 ms)
            //** > Result for Day08_HandheldHalting part 2:Instruction pointer that sent program into infinite loop: 191(Process: 0 ms)
             
            string finalResult = "Not Set";
            long accumulator = 0;
            int instructionPointer = 0;
            List<Instruction> Program = new List<Instruction>();
            char[] split = new char[] { ' ' };
            foreach (string processingLine in inputFile)
            {
                string[] inst = processingLine.Split(split);
                inst[1] = inst[1].StartsWith("+") ? inst[1].Substring(1) : inst[1];
                Program.Add(new Instruction(inst[0], Convert.ToInt32(inst[1])));
            }
            AddResult("Data setup completed");
            ResetProcessTimer(true);
            while(instructionPointer<Program.Count() && Program[instructionPointer].HasExecuted==false)
            {
                Program[instructionPointer].HasExecuted = true;
                switch(Program[instructionPointer].Command)
                {
                    case "acc":
                        accumulator += Program[instructionPointer].Operand;
                        instructionPointer++;
                        break;
                    case "jmp":
                        instructionPointer += Program[instructionPointer].Operand;
                        break;
                    default: //"NOP"
                        instructionPointer++;
                        break;
                }

            }
            finalResult = ((instructionPointer < Program.Count())?"When an instruction hit the infinite loop condition, the accumulator was at the value: ":"Normal execution, accumulator: ") + accumulator.ToString();
            AddResult(finalResult);



            //** > Result for Day08_HandheldHalting part 0:Data setup completed(Process: 1 ms)
            //** > Result for Day08_HandheldHalting part 1:When an instruction hit the infinite loop condition, the accumulator was at the value: 1548(Process: 0 ms)
            //** > Result for Day08_HandheldHalting part 2:Completed! Swapped out encounter 68, jmp to nop, at input line 227(Process: 18 ms)
            //** > Result for Day08_HandheldHalting part 2:Accumulator value is 1375(Process: 18 ms)

            ResetProcessTimer(true);
            int swapCounter = 0, nopJmpEncounterCounter = 0, swappedInstructionLine = 0;
            instructionPointer = 0;
            string swappedInstruction = "";
            
            while (instructionPointer < Program.Count() || swapCounter>Program.Count()) // either we finish normally, or somehow we've targeted to swap the nth encounter of an instruction that is technically never ever possible.
            {
                //Reset entirely for new test condition execution
                // each time this test runs, change a nop to jmp (or jmp to nop). We are brute-forcing this via iteration.
                // the first time, it will be the first encounter of a nop/jmp. Increment the swappointer and if it ended because of infinite loop, the next time it will be the second.
                // then the third, etc. 
                instructionPointer = 0; accumulator = 0; nopJmpEncounterCounter = 0;
                Program = new List<Instruction>();
                foreach (string processingLine in inputFile)
                {
                    string[] inst = processingLine.Split(split);
                    inst[1] = inst[1].StartsWith("+") ? inst[1].Substring(1) : inst[1];
                    Program.Add(new Instruction(inst[0], Convert.ToInt32(inst[1])));
                }
                while (instructionPointer < Program.Count() && Program[instructionPointer].HasExecuted == false)
                {

                    Program[instructionPointer].HasExecuted = true;
                    if (Program[instructionPointer].Command == "nop" || Program[instructionPointer].Command == "jmp")
                    {
                        if (nopJmpEncounterCounter == swapCounter)
                        {
                            Program[instructionPointer].Command = (Program[instructionPointer].Command == "nop") ? "jmp" : "nop";
                            swappedInstructionLine = instructionPointer;
                            swappedInstruction = (Program[instructionPointer].Command == "nop") ? "jmp to nop" : "nop to jmp";
                        }
                    }
                    switch (Program[instructionPointer].Command)
                    {
                        case "acc":
                            accumulator += Program[instructionPointer].Operand;
                            instructionPointer++;
                            break;
                        case "jmp":
                            instructionPointer += Program[instructionPointer].Operand;
                            nopJmpEncounterCounter++;
                            break;
                        default: //"NOP"
                            instructionPointer++;
                            nopJmpEncounterCounter++;
                            break;
                    }

                }
                swapCounter++; // change a different one next time.
            }
            finalResult = (instructionPointer < Program.Count()) ? "Infinite Loop Result " : "Completed! Swapped out encounter " + swapCounter.ToString() + ", " + swappedInstruction + ", at input line " + (swappedInstructionLine+1).ToString() ;
            AddResult(finalResult);
            AddResult("Accumulator value is " + accumulator.ToString());
        }
    }
    class Instruction
    {
        public string Command { get; set; }
        public int Operand { get; set; }
        public bool HasExecuted { get; set; }
        public Instruction(string command, int operand)
        {
            Command = command; Operand = operand; HasExecuted = false;
        }
    }
}
