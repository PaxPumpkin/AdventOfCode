using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace BunnyHeadquarters
{
    class Registers : AoCodeModule
    {
        public Registers()
        {
            inputFileName = "Registers.txt";
            base.GetInput();
        }
        static Dictionary<string, int> registers = new Dictionary<string, int>();
        public override void DoProcess()
        {
            int maxValueEver = 0;

            inputFile.ForEach(line => { Execute(line); maxValueEver = Math.Max(registers.Values.Max(), maxValueEver);});

            // Display Result
            FinalOutput.Add("---Registers");
            FinalOutput.Add("Largest Value in Any Register is " + registers.Values.Max().ToString());
            FinalOutput.Add("Largest Value ever held in any Register was " + maxValueEver.ToString());
        }
        private static void Execute(string commandStatement)
        {
            // this is a lot of code that could be brought down to succinctness, but would be much more difficult to follow. 

            //For comments, this is the sample input line. 
            //fdv inc 137 if ben >= -330     ( increase register "fdv" by positive 137 if register "ben" is greater than or equal to negative 330 )
            string[] pieces = commandStatement.Split(new string[] { " if " }, StringSplitOptions.None); // the "if" is now removed:  fdv inc 137, ben >= -330 
            string[] condition = pieces[1].Split(new char[] { ' ' }); //{ben,>=,-330}
            string[] instruction = pieces[0].Split(new char[] { ' ' });//{fdv,inc,137}

            string instructionRegister = instruction[0]; //fdv
            string conditionRegister = condition[0]; //ben

            // if we don't have the named registers in our collection yet, add them with the initial 0 value per specification.
            if (!registers.Keys.Contains(conditionRegister)) registers.Add(conditionRegister, 0); 
            if (!registers.Keys.Contains(instructionRegister)) registers.Add(instructionRegister, 0);


            // these variables are just for nicety and code-readability. 
            int amountToChange = Convert.ToInt32(instruction[2]) * ((instruction[1].Equals("inc")) ? 1 : -1); // 137*1   eg. xyz dec -450 would be -450*-1, or +450
            int comparedValue = Convert.ToInt32(condition[2]); // the number we're testing against: -330
            int comparedRegisterCurrentValue = registers[conditionRegister]; // the value in the register to test *right now* == registers["ben"].value
            string comparison = condition[1]; // the comparison to do... >=  in this example.

            // this is SLOW AS HELL, but it uses a weird runtime-software-compilation to evaluate the string instead of switch/case, so there is *some* elegance to it. It's just awful slow, though.
            //string statementToEval = comparedRegisterCurrentValue.ToString() + comparison + comparedValue; // "4<12"   or "30==30", etc...
            //registers[instructionRegister] += Cheater.BooleanEval(statementToEval) ? amountToChange : 0; // if it evaluates to true, make the indicated change. Otherwise add zero (no change).
            //return;

            // this is pretty darn fast, but ugly. 
            bool passesTest = false;
            switch (comparison)
            {
                case "==":
                    passesTest = (comparedRegisterCurrentValue == comparedValue);
                    break;
                case "!=":
                    passesTest = (comparedRegisterCurrentValue != comparedValue);
                    break;
                case ">=":
                    passesTest = (comparedRegisterCurrentValue >= comparedValue);
                    break;
                case "<=":
                    passesTest = (comparedRegisterCurrentValue <= comparedValue);
                    break;
                case ">":
                    passesTest = (comparedRegisterCurrentValue > comparedValue);
                    break;
                case "<":
                    passesTest = (comparedRegisterCurrentValue < comparedValue);
                    break;
                default:
                    throw new Exception("Unimplemented Instruction: " + comparison);
            }
            registers[instructionRegister] += (passesTest)?amountToChange:0;
        }

    }
}
