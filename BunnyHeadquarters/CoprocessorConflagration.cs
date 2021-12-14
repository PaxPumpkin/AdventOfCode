using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class CoprocessorConflagration : AoCodeModule
    {
        public CoprocessorConflagration()
        {
            inputFileName = "coprocessorconflagration.txt";
            base.GetInput();
        }
        Int64 multiplicationCounter = 0;
        Dictionary<char, Int64> registers = new Dictionary<char, long>();
        public override void DoProcess()
        {
            
            for (char x = 'a'; x <= 'h'; x++) { registers.Add(x, 0); }
            registers['a']=1;
            int nextInstruction = 0;
            while (nextInstruction >= 0 && nextInstruction < inputFile.Count)
            {
                nextInstruction+=ExecuteCommand(inputFile[nextInstruction]);
                //Console.WriteLine(registers['h'].ToString());
                if (registers['h']!=0){
                    Console.WriteLine(registers['h'].ToString());
                    Console.WriteLine("WHAT???");
                } 
                // short circuit to execute the core assembly routine manually
                nextInstruction = -1;
            }
            FinalOutput.Add("Total MUL executions: " + multiplicationCounter.ToString());


           
            int a = 1;// not in debug mode
            int b = 0;
            int c = 0;
            int d = 0;
            int e = 0;
            int f = 0;
            int g = 0;
            int h = 0;
            b = 93; // initial setup value, this controls EVERYTHING about the result. 
            c = b;

            if (a != 0) // debug on, debug off. Help deciper the result of this nonsense if we don't have to go through it a bazillion times. 
            {
                b *= 100;
                b += 100000;
                c = b;
                c += 17000;
            }
            do
            {
                f = 1;
                d = 2;
                e = 2;

                for (d = 2; d < b; d++)
                {
                    if (b % d == 0)
                    {
                        f = 0;
                        break;
                    }
                }
                if (f == 0)
                {
                    h++;
                }
                g = b - c;
                b += 17;
            } while (g != 0);
            FinalOutput.Add("H would be" + h);
            



            
        }
        public int ExecuteCommand(string command)
        {
            int stepOffset = 1;
            /*
            - set X Y sets register X to the value of Y.
            - sub X Y decreases register X by the value of Y.
            - mul X Y sets register X to the result of multiplying the value contained in register X by the value of Y.
            - jnz X Y jumps with an offset of the value of Y, but only if the value of X is not zero. (An offset of 2 skips the next instruction, an offset of -1 jumps to the previous instruction, and so on.)
             * */
            string[] pieces = command.Split(new char[] { ' ' });
            long registerValue = 0;
            char registerName='\0';
            long commandValue = 0;
            char commandName ='\0';
            if (!Int64.TryParse(pieces[1],out registerValue)){ registerName=(pieces[1].ToArray())[0]; registerValue=registers[registerName];}
            if (!Int64.TryParse(pieces[2],out commandValue)){ commandName=(pieces[2].ToArray())[0]; commandValue=registers[commandName];}
            if (command.Contains("sub h")){
                Console.WriteLine("Whataya Know.");
            }

            switch (pieces[0])
            {
                case "set":
                    registers[registerName] = commandValue;
                    break;
                case "sub":
                    registers[registerName] -= commandValue;
                    break;
                case "mul":
                    registers[registerName] = registerValue * commandValue;
                    multiplicationCounter++;
                    break;
                case "jnz":
                    stepOffset = (int)((registerValue!=0)?commandValue:stepOffset);
                    break;
                default:
                    Console.WriteLine("OOPS! weird command: " + pieces[0]);
                    break;
            }

            return stepOffset;
        }
    }
}
