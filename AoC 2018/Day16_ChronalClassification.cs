using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day16_ChronalClassification : AoCodeModule
    {
        public Day16_ChronalClassification()
        {

            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 

        }
        public override void DoProcess()
        {

            string finalResult = "Not Set";
            ResetProcessTimer(true);

            string[] opCodes = new string[] { "addr", "addi", "mulr", "muli", "banr", "bani", "borr", "bori", "setr", "seti", "gtir", "gtri", "gtrr", "eqir", "eqri", "eqrr" };

            // the first part of the tuple is the instruction integer, as a key. The second is a list of opcodes that matched the processing.
            List<(int, List<(string, int)>)> opCodeDetermination = new List<(int, List<(string, int)>)>();
            Queue<string> parsing = new Queue<string>();

            foreach (string processingLine in inputFile)
            {
                if (!processingLine.Trim().Equals("")) parsing.Enqueue(processingLine);
            }
            string before = parsing.Dequeue(), instruction = "", after = "";
            int sampleCounter = 0;
            while (before.Contains("Before"))
            {
                int opCodeBehaviorCounter = 0;
                before = before.Substring(9, 10); // looks like "0, 0, 2, 3"
                instruction = parsing.Dequeue(); // looks like "11 2 3 0"
                after = parsing.Dequeue().Substring(9, 10); // looks like 0, 0, 2, 3
                int sampleCode = int.Parse(instruction.Substring(0, instruction.IndexOf(' '))); // turns first instruction number into an int for List key
                opCodes.ToList().ForEach(opCode =>
                {
                    Engine.Process(before, opCode + instruction.Substring(instruction.IndexOf(' '))); // should cut off the first number
                    if (Engine.Affirm(after))
                    {
                        opCodeBehaviorCounter++;
                        if (opCodeDetermination.Count(x => x.Item1 == sampleCode) == 0) opCodeDetermination.Add((sampleCode, new List<(string, int)>()));
                        if (opCodeDetermination.Where(x => x.Item1 == sampleCode).First().Item2.Count(x => x.Item1 == opCode) == 0)
                        {
                            opCodeDetermination.Where(x => x.Item1 == sampleCode).First().Item2.Add((opCode, 1));
                        }
                        else
                        {
                            (string, int) foundOpCode = opCodeDetermination.Where(x => x.Item1 == sampleCode).First().Item2.Where(x => x.Item1 == opCode).First();
                            foundOpCode.Item2++;
                            opCodeDetermination.Where(x => x.Item1 == sampleCode).First().Item2.RemoveAll(x => x.Item1 == opCode);
                            opCodeDetermination.Where(x => x.Item1 == sampleCode).First().Item2.Add(foundOpCode);
                        }
                    }
                });
                if (opCodeBehaviorCounter >= 3) sampleCounter++;
                before = parsing.Dequeue();
            }
            finalResult = "Samples that match 3 or more opCodes: " + sampleCounter.ToString();
            AddResult(finalResult);
            ResetProcessTimer(true);
            List<(int, string)> opCodeFinal = new List<(int, string)>();
            List<(int, List<(string, int)>)> opCodeDeterminationSingles = opCodeDetermination.OrderBy(x => x.Item1).ToList().Where(x => x.Item2.Count(y => y.Item2 == x.Item2.Max(z => z.Item2)) == 1).ToList();
            while (opCodeDeterminationSingles.Count > 0)
            {
                opCodeDeterminationSingles.ForEach(x =>
                {
                    (string, int) foundMax = x.Item2.Where(y => y.Item2 == x.Item2.Max(z => z.Item2)).First();
                    opCodeFinal.Add((x.Item1, foundMax.Item1));
                    opCodeDetermination.ForEach(q => q.Item2.RemoveAll(r => r.Item1 == foundMax.Item1)); // remove found op code string from everyone's list. Hopefully this will cause the "singles" evaluation to continually operate until all are found.
                    opCodeDetermination.RemoveAll(q => q.Item1 == x.Item1); // this number has been correlated, remove it from the list, too. 
                });
                opCodeDeterminationSingles = opCodeDetermination.OrderBy(x => x.Item1).ToList().Where(x => x.Item2.Count(y => y.Item2 == x.Item2.Max(z => z.Item2)) == 1).ToList();
            }
            string opCodex = "";
            Engine.Reset();// all registers to zero
            while (before!="")
            {
                opCodex = opCodeFinal.Where(x => x.Item1 == int.Parse(before.Substring(0, before.IndexOf(' ')))).First().Item2; 
                Engine.Process(opCodex + before.Substring(before.IndexOf(' ')));
                before = (parsing.Count > 0) ? parsing.Dequeue() : "";
            }
            finalResult = "Value in register zero: " + Engine.registers[0].ToString();
            AddResult(finalResult);

        }
    }
    public class Engine
    {
        public static int[] registers = new int[] { 0, 0, 0, 0 };
        public Engine()
        {

        }
        public static void Reset()
        {
            registers = new int[] { 0, 0, 0, 0 };
        }
        public static bool Affirm(string regString)
        {
            string[] regis = regString.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            bool result = true;
            for (int x = 0; x <= 3; x++)
            {
                result = result && (registers[x] == int.Parse(regis[x]));
            }
            return result;
        }
        public static void Process(string init, string command)
        {
            string[] regis = init.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            registers = new int[] {int.Parse(regis[0]), int.Parse(regis[1]), int.Parse(regis[2]), int.Parse(regis[3]) };
            Process(command);
        }
        public static void Process(string command)
        {
            string[] parse = command.Split(new char[] { ' ' });
            string opCode = parse[0];
            int A = int.Parse(parse[1]), B = int.Parse(parse[2]), C = int.Parse(parse[3]);
            switch (opCode) {
                case "addr":
                    registers[C] = registers[A] + registers[B];
                    break;
                case "addi":
                    registers[C] = registers[A] + B;
                    break;
                case "mulr":
                    registers[C] = registers[A] * registers[B];
                    break;
                case "muli":
                    registers[C] = registers[A] * B;
                    break;
                case "banr":
                    registers[C] = registers[A] & registers[B];
                    break;
                case "bani":
                    registers[C] = registers[A] & B;
                    break;
                case "borr":
                    registers[C] = registers[A] | registers[B];
                    break;
                case "bori":
                    registers[C] = registers[A] | B;
                    break;
                case "setr":
                    registers[C] = registers[A];
                    break;
                case "seti":
                    registers[C] = A;
                    break;
                case "gtir":
                    registers[C] = (A>registers[B]) ? 1 : 0;
                    break;
                case "gtri":
                    registers[C] = (registers[A]>B) ? 1 : 0;
                    break;
                case "gtrr":
                    registers[C] = (registers[A]>registers[B]) ? 1 : 0;
                    break;
                case "eqir":
                    registers[C] = (A==registers[B]) ? 1 : 0;
                    break;
                case "eqri":
                    registers[C] = (registers[A]==B) ? 1 : 0;
                    break;
                case "eqrr":
                    registers[C] = (registers[A]==registers[B]) ? 1 : 0;
                    break;

            }
        }
    }
}
