using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day19_GoWithTheFlow : AoCodeModule
    {
        public Day19_GoWithTheFlow()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        public override void DoProcess()
        {
            //#ip 4
            //addi 4 16 4
            //seti 1 3 5
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            List<string> commands = new List<string>();
            int ipRegister = 0;
            int ipValue = 0;
            foreach (string processingLine in inputFile)
            {
                if (processingLine.StartsWith("#ip"))
                {
                    ipRegister = Int32.Parse(processingLine.Last().ToString());
                }
                else
                {
                    commands.Add(processingLine);
                }
            }
            Engine.Reset();
            while (ipValue < commands.Count())
            {
                Engine.SetRegisterValue(ipRegister, ipValue);
                Engine.Process(commands[ipValue]);
                ipValue = Engine.GetRegisterValue(ipRegister);
                ipValue++;
            }

            AddResult("The value in register 0 after program halts is: " + Engine.GetRegisterValue(0).ToString());
            ResetProcessTimer(true);
            Engine.Reset();
            Engine.SetRegisterValue(0, 1);
            ipValue = 0;
            while (ipValue < commands.Count())
            {
                Engine.SetRegisterValue(ipRegister, ipValue);
                Engine.Process(commands[ipValue]);
                ipValue = Engine.GetRegisterValue(ipRegister);
                ipValue++;
            }
            AddResult("The value in register 0 after program halts with initial register zero value of 1 is: " + Engine.GetRegisterValue(0).ToString());
        }
        public class Engine
        {
            public static int[] registers = new int[] { 0, 0, 0, 0, 0, 0 };
            public Engine()
            {

            }
            public static int GetRegisterValue(int regNumber)
            {
                return registers[regNumber];
            }
            public static void SetRegisterValue(int regNumber, int value)
            {
                registers[regNumber] = value;
            }
            public static void Reset()
            {
                registers = new int[] { 0, 0, 0, 0, 0, 0 };
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
                registers = new int[] { int.Parse(regis[0]), int.Parse(regis[1]), int.Parse(regis[2]), int.Parse(regis[3]), int.Parse(regis[4]), int.Parse(regis[5]) };
                Process(command);
            }
            public static void Process(string command)
            {
                string[] parse = command.Split(new char[] { ' ' });
                string opCode = parse[0];
                int A = int.Parse(parse[1]), B = int.Parse(parse[2]), C = int.Parse(parse[3]);
                switch (opCode)
                {
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
                        registers[C] = (A > registers[B]) ? 1 : 0;
                        break;
                    case "gtri":
                        registers[C] = (registers[A] > B) ? 1 : 0;
                        break;
                    case "gtrr":
                        registers[C] = (registers[A] > registers[B]) ? 1 : 0;
                        break;
                    case "eqir":
                        registers[C] = (A == registers[B]) ? 1 : 0;
                        break;
                    case "eqri":
                        registers[C] = (registers[A] == B) ? 1 : 0;
                        break;
                    case "eqrr":
                        registers[C] = (registers[A] == registers[B]) ? 1 : 0;
                        break;

                }
            }
        }
    }

}
