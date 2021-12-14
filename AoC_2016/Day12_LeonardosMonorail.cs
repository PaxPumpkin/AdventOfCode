using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day12_LeonardosMonorail : AoCodeModule
    {
        public Day12_LeonardosMonorail()
        {
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
            //If Comma Delimited on a single input line
            //List<string> inputItems = inputFile[0].Split(new char[] { ',' }).ToList<string>();
            //string finalResult = "Not Set";
            ResetProcessTimer(true);// true also iterates the section marker
            AssembunnyProcessor processor = new AssembunnyProcessor();
            foreach (string processingLine in inputFile)
            {
                processor.AddInstruction(processingLine);
            }

            AddResult(processor.Run());

            ResetProcessTimer(true);
            // reset registers to starting values
            processor.InitializeRegister("a", 0);
            processor.InitializeRegister("b", 0);
            processor.InitializeRegister("c", 1); // puzzle two, c starts at 1
            processor.InitializeRegister("d", 0);
            AddResult(processor.Run());
        }
    }
    public class AssembunnyProcessor
    {

        Dictionary<string, int> registers = new Dictionary<string, int>();
        List<string> codeInstructions = new List<string>();
        int instructionPointer = 0;
        public AssembunnyProcessor()
        {
            registers.Add("a", 0);
            registers.Add("b", 0);
            registers.Add("c", 0);
            registers.Add("d", 0);
        }
        public void InitializeRegister(string register, int value)
        {
            registers[register] = value;
        }
        public void AddInstruction(string instruction)
        {
            codeInstructions.Add(instruction);
        }
        public string Run()
        {
            string result = "";
            instructionPointer = 0;
            while (instructionPointer < codeInstructions.Count)
            {
                instructionPointer += ExecuteInstruction(codeInstructions[instructionPointer]);
            }
            result =registers["a"].ToString(); // both puzzles want register a
            return result;
        }
        private int ExecuteInstruction(string instruction)
        {
            int nextCodeLineOffset = 1; // unless a JNZ happens, then we'll just go to the next line
            string[] parse = instruction.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            switch (parse[0]) {
                case "cpy":
                    int cpyValue = 0;
                    if (!int.TryParse(parse[1], out cpyValue)){
                        cpyValue = registers[parse[1]];
                    }
                    registers[parse[2]] = cpyValue;
                    break;
                case "inc":
                    registers[parse[1]]+=1;
                    break;
                case "dec":
                    registers[parse[1]] += -1;
                    break;
                case "jnz":
                    int jnzVal = 0;
                    if (!int.TryParse(parse[1], out jnzVal))
                    {
                        jnzVal = registers[parse[1]];
                    }
                    int jnzAmt = 0;
                    if (!int.TryParse(parse[2], out jnzAmt))
                    {
                        jnzAmt = registers[parse[2]];
                    }
                    nextCodeLineOffset = jnzVal != 0 ? jnzAmt : nextCodeLineOffset;
                    break;
                default:
                    break;
            }
            return nextCodeLineOffset;
        }
    }
}
