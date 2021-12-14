using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AoC_2019
{
   class IntCodeComputer
   {
      List<long> program = new List<long>() { 99 };
      List<long> memory;
      long[] memoryHandle;
      int instructionPointer = 0;
      int relativeBase = 0;

      string lastCompyOutput = "Compy Hasn't Output Anything.";
      bool outputQualified = true;
      List<string> preparedInput = new List<string>();
      bool waitForInput = false;
      public string CompyName { get; set; }
      public int CompyIndex { get; set; }
      public int CompyOutputRoute { get; set; }
      public STATE CompyState { get; set; }
      public bool CompyIsWaitingForInput { get { return CompyState.HasFlag(STATE.WAITING_FOR_INPUT); } }
      public bool CompyHasOutputToRead { get { return CompyState.HasFlag(STATE.OUTPUT_WAITING); } }
      public bool CompyIsRunning { get { return CompyState.HasFlag(STATE.RUNNING); } }
      public bool CompyIsDone { get { return CompyState.HasFlag(STATE.HALTED); } }
      public bool CompyIsVerbose { get; set; }
      public bool CompyDisplayAllDeliveredOutput { get; set; }
      [Flags]
      public enum STATE { UNKNOWN=1, INITIALIZED=2, RUNNING=4, WAITING_FOR_INPUT=8, OUTPUT_WAITING=16, HALTED=32 };
      enum OPCODE
      {
         ADD = 1,
         MULTIPLY = 2,
         INPUT = 3,
         OUTPUT = 4,
         JUMPIFTRUE = 5,
         JUMPIFFALSE = 6,
         LESSTHAN = 7,
         EQUALS = 8,
         RELATIVE_BASE_OFFSET = 9,

         HALT = 99
      };
      // parameter counts include the OPCODE itself
      Dictionary<OPCODE, int> opCodeParameterCount = new Dictionary<OPCODE, int>() {
            { OPCODE.ADD, 4 },
            { OPCODE.MULTIPLY, 4 },
            { OPCODE.INPUT, 2 },
            { OPCODE.OUTPUT, 2 },
            { OPCODE.JUMPIFTRUE, 3 },
            { OPCODE.JUMPIFFALSE, 3 },
            { OPCODE.LESSTHAN, 4 },
            { OPCODE.EQUALS, 4 },
            { OPCODE.RELATIVE_BASE_OFFSET, 2 },
            { OPCODE.HALT, 1 }
        };
      public IntCodeComputer()
      {
         ClearProgram();
         ResetProgram();
         CompyState = STATE.UNKNOWN;
         CompyName = "Unnamed Sad Compy";
         CompyIsVerbose = false;
      }
      public IntCodeComputer(List<long> newProgram)
      {
         program = newProgram;
         instructionPointer = 0;
         relativeBase = 0;
         ResetProgram();
         CompyState = STATE.INITIALIZED;
         CompyName = "Unnamed Sad Compy";
         CompyIsVerbose = false;
      }
      public void SetOutputWithText(bool withText)
      {
         outputQualified = withText;
      }
      public void SetInputAutomaticWait(bool loopUntilInputExists)
      {
         waitForInput = loopUntilInputExists;
      }
      public string GetLastProgramOutput()
      {
         CompyState &= ~STATE.OUTPUT_WAITING;
         if (CompyIsVerbose) Console.WriteLine(CompyName + " has released awaiting output: " + lastCompyOutput);
         return lastCompyOutput;
      }
      public void AddPreparedProgramInput(string input)
      {
         if (CompyIsVerbose) Console.WriteLine(CompyName + " has had input stacked: " + input);
         preparedInput.Add(input);
      }
      public void ClearProgram()
      {
         program = new List<long>() { 99 };
         instructionPointer = 0;
         relativeBase = 0;
         ResetProgram();
         CompyState = STATE.INITIALIZED;
      }
      public void ResetProgram()
      {
         memoryHandle = new long[program.Count];
         program.CopyTo(memoryHandle);
         memory = memoryHandle.ToList();
         preparedInput = new List<string>();
         lastCompyOutput = "";
         CompyState = STATE.INITIALIZED;
      }
      public void LoadProgram(List<long> newProgram)
      {
         program = newProgram;
         instructionPointer = 0;
         relativeBase = 0;
         CompyState = STATE.INITIALIZED;
      }
      public void SetProgramInstruction(int pointer, long value)
      {
         if (pointer < memory.Count)
         {
            memory[pointer] = value;
         }
         else
         {
            throw new Exception("Cannot set value in program outside program instruction range.");
         }
      }
      public long GetProgramValue(int pointer)
      {
         if (pointer < memory.Count)
         {
            return memory[pointer];
         }
         else
         {
            throw new Exception("Cannot set value in program outside program instruction range.");
         }
      }
      public void ValidateProgram()
      {
         for (int x = 0; x < program.Count; x++)
         {
            if (program[x] != memory[x])
            {
               Console.WriteLine("Value mismatch at location " + x.ToString());
               Console.WriteLine("Value in memory is " + memory[x].ToString() + ", expected " + program[x].ToString());
            }
         }
      }
      public void RunProgram()
      {
         if (CompyIsVerbose) Console.WriteLine(CompyName + " is running the program.");
         CompyState = STATE.RUNNING;
         instructionPointer = 0;
         long opCode = memory.Get(instructionPointer);
         char[] specifiers = "00000".ToCharArray();
         int specifierPointer = 0;
         string specifiersPreParse = "";
         bool explicitInstructionPointerSet = false;
         if (opCode > 99) // it has parameter TYPE specifiers
         {
            // the input for these parameter types comes in right-to-left, but we are reversing it  so that the type specifier index matches the actual parameter index number later.
            //Notes for day 5 - parameters being written to are NEVER in "immediate mode" ("1"), so the default of 0 is assumed. It just stores it in the 0-based position.
            // Day 9 - new mode of "relative base". Refactor all that use specifier "0" to also include possibility of "2" - relative.
            specifiersPreParse = opCode.ToString().Substring(0, opCode.ToString().Length - 2);
            specifierPointer = 0;
            for (int x = specifiersPreParse.Length - 1; x >= 0; x--)
            {
               specifiers[specifierPointer] = specifiersPreParse[x];
               specifierPointer++;
            }
            opCode = long.Parse(opCode.ToString().Substring(opCode.ToString().Length - 2));
         }
         List<long> parameters;
         int targetAddress;
         long resolvedParam0, resolvedParam1;
         while (true)
         {
            if (CompyIsVerbose)
            {
               Console.WriteLine(((OPCODE)opCode).ToString() + "(" + memory.Get(instructionPointer).ToString() + ")");
            }
            Thread.Yield();
            if ((OPCODE)opCode == OPCODE.HALT) { break; }

            parameters = memory.GetRange(instructionPointer + 1, opCodeParameterCount[(OPCODE)opCode] - 1);
            if (CompyIsVerbose)
            {
               string paramsOut = "";
               parameters.ForEach(x => paramsOut += x.ToString() + " ");
               Console.WriteLine("\t\t" + paramsOut );
            }
            switch ((OPCODE)opCode)
            {
               case OPCODE.ADD:// add positions and store in 3rd
                  // refactor for parameter relative base mode
                  // 0 is relative mode from 0, 2 is relative from a moving base adjusted by opcode 9
                  // target address can never be 1 (immediate)
                  targetAddress = (int)parameters[2] + (specifiers[2] == '0' ? 0 : relativeBase);
                  // if specifier is 1(immediate, just the value. If not, pull from memory at either 0 + parameter(relative, mode == "0") or relativeBase + parameter(mode =="2")
                  resolvedParam0 = specifiers[0] == '1' ? parameters[0] : memory.Get(((int)parameters[0] + (specifiers[0] == '0' ? 0 : relativeBase)));
                  resolvedParam1 = specifiers[1] == '1' ? parameters[1] : memory.Get(((int)parameters[1] + (specifiers[1] == '0' ? 0 : relativeBase)));
                  if (CompyIsVerbose) Console.WriteLine("Adding " + resolvedParam0.ToString() + " and " + resolvedParam1 + " into " + targetAddress.ToString());
                  memory.Put(targetAddress, resolvedParam0 + resolvedParam1);
                  break;
               case OPCODE.MULTIPLY:// multiply positions and store in 3rd
                  // see "ADD" opcode, above for refactor notes
                  targetAddress = (int)parameters[2] + (specifiers[2] == '0' ? 0 : relativeBase);
                  resolvedParam0 = specifiers[0] == '1' ? parameters[0] : memory.Get(((int)parameters[0] + (specifiers[0] == '0' ? 0 : relativeBase)));
                  resolvedParam1 = specifiers[1] == '1' ? parameters[1] : memory.Get(((int)parameters[1] + (specifiers[1] == '0' ? 0 : relativeBase)));
                  if (CompyIsVerbose) Console.WriteLine("Multiplying " + resolvedParam0.ToString() + " and " + resolvedParam1 + " into " + targetAddress.ToString());
                  memory.Put(targetAddress, (resolvedParam0 * resolvedParam1));
                  break;
               case OPCODE.INPUT:// get input ( validate as integer or this blows up ), and store in parameter 1
                  long value = 0;
                  bool parsedValue = false;
                  if (waitForInput)
                  {
                     if (CompyIsVerbose) Console.WriteLine(CompyName + " is waiting for input.");
                     CompyState = CompyState | STATE.WAITING_FOR_INPUT;
                     while (preparedInput.Count == 0 || !long.TryParse(preparedInput[0], out value)) { Thread.Yield(); };
                     CompyState &= ~STATE.WAITING_FOR_INPUT;
                     if (CompyIsVerbose) Console.WriteLine(CompyName + " has received input and will continue.");
                  }
                  if (preparedInput.Count > 0)
                  {
                     if (CompyIsVerbose) Console.WriteLine("Prepared Input Available: " + preparedInput[0]);
                     parsedValue = long.TryParse(preparedInput[0], out value);
                     preparedInput.RemoveAt(0);
                  }
                  if (!parsedValue)
                  {
                     Console.WriteLine(CompyName + ": No Prepared Input or input was not parseable.");
                     Console.WriteLine("Enter a value:");
                     CompyState = CompyState | STATE.WAITING_FOR_INPUT;
                     string input = Console.ReadLine();

                     while (input.Trim() == "" || !long.TryParse(input.Trim(), out value))
                     {
                        input = Console.ReadLine();
                        Thread.Yield();
                     }
                     CompyState &= ~STATE.WAITING_FOR_INPUT;
                  }
                  targetAddress = (int)parameters[0] + (specifiers[0] == '0' ? 0 : relativeBase);
                  if (CompyIsVerbose) Console.WriteLine("Putting input  " + value.ToString() + " into " + targetAddress.ToString());
                  memory.Put(targetAddress, value);
                  break;
               case OPCODE.OUTPUT: // display value of memory address in parameter 1
                  // Adjustments for day 9 - relative base.
                  // specifiers 0, 1, 2 = 0 relative to ZERO memory base, 2 relative to shifting relativeBase, 1, specified output
                  if (specifiers[0] == '0')
                  {
                     lastCompyOutput = (outputQualified) ? "Value in memory address " + parameters[0].ToString() + " is: " + memory.Get((int)parameters[0]).ToString() : memory.Get((int)parameters[0]).ToString();
                  }
                  else if (specifiers[0] == '2')
                  {
                     lastCompyOutput = (outputQualified) ? "Value in relative memory address " + parameters[0].ToString() + " is: " + memory.Get(relativeBase + (int)parameters[0]).ToString() : memory.Get(relativeBase + (int)parameters[0]).ToString();
                  }
                  else
                  {
                     lastCompyOutput = (outputQualified) ? "Explicit value output: " + parameters[0].ToString() : parameters[0].ToString();
                  }
                  CompyState = CompyState | STATE.OUTPUT_WAITING;
                  if (CompyIsVerbose || CompyDisplayAllDeliveredOutput) Console.WriteLine(CompyName + " has set output to read: " + lastCompyOutput);
                  break;
               case OPCODE.JUMPIFTRUE:
                  // adjustments for day 9, specifier "2" pulls from memory address relative to moving base instead of always 0-based address reference.
                  if ((specifiers[0] == '1' ? parameters[0] : memory.Get((int)parameters[0] + (specifiers[0] == '0' ? 0 : relativeBase))) != 0)
                  {
                     instructionPointer = (int)((specifiers[1] == '1') ? parameters[1] : memory.Get((int)parameters[1] + (specifiers[1] == '0' ? 0 : relativeBase)));
                     explicitInstructionPointerSet = true;
                     if (CompyIsVerbose) Console.WriteLine("Jumping to " + instructionPointer.ToString() + " because " + ((specifiers[0] == '1' ? parameters[0] : memory.Get((int)parameters[0] + (specifiers[0] == '0' ? 0 : relativeBase)))) + " is not zero.");
                  }
                  break;
               case OPCODE.JUMPIFFALSE:
                  // adjustments for day 9, specifier "2" pulls from memory address relative to moving base instead of always 0-based address reference.
                  if ((specifiers[0] == '1' ? parameters[0] : memory.Get((int)parameters[0] + (specifiers[0] == '0' ? 0 : relativeBase))) == 0)
                  {
                     instructionPointer = (int)((specifiers[1] == '1') ? parameters[1] : memory.Get((int)parameters[1] + (specifiers[1] == '0' ? 0 : relativeBase)));
                     explicitInstructionPointerSet = true;
                     if (CompyIsVerbose) Console.WriteLine("Jumping to " + instructionPointer.ToString() + " because " + ((specifiers[0] == '1' ? parameters[0] : memory.Get((int)parameters[0] + (specifiers[0] == '0' ? 0 : relativeBase)))) + " is zero.");
                  }
                  break;
               case OPCODE.LESSTHAN:
                  //if the first parameter is less than the second parameter, it stores 1 in the position given by the third parameter.Otherwise, it stores 0.
                  // see "ADD" opcode, above
                  targetAddress = (int)parameters[2] + (specifiers[2] == '0' ? 0 : relativeBase);
                  resolvedParam0 = specifiers[0] == '1' ? parameters[0] : memory.Get(((int)parameters[0] + (specifiers[0] == '0' ? 0 : relativeBase)));
                  resolvedParam1 = specifiers[1] == '1' ? parameters[1] : memory.Get(((int)parameters[1] + (specifiers[1] == '0' ? 0 : relativeBase)));
                  memory.Put(targetAddress, (resolvedParam0 < resolvedParam1) ? 1 : 0);
                  break;
               case OPCODE.EQUALS:
                  //if the first parameter is equal to the second parameter, it stores 1 in the position given by the third parameter. Otherwise, it stores 0.
                  // see "ADD" opcode, above
                  targetAddress = (int)parameters[2] + (specifiers[2] == '0' ? 0 : relativeBase);
                  resolvedParam0 = specifiers[0] == '1' ? parameters[0] : memory.Get(((int)parameters[0] + (specifiers[0] == '0' ? 0 : relativeBase)));
                  resolvedParam1 = specifiers[1] == '1' ? parameters[1] : memory.Get(((int)parameters[1] + (specifiers[1] == '0' ? 0 : relativeBase)));
                  memory.Put(targetAddress, (resolvedParam0 == resolvedParam1) ? 1 : 0);
                  break;
               case OPCODE.RELATIVE_BASE_OFFSET:
                  relativeBase += (int)( specifiers[0] == '1' ? parameters[0] : memory.Get(((int)parameters[0] + (specifiers[0] == '0' ? 0 : relativeBase))));
                  break;
               default:
                  throw new Exception("Unknown Program instruction opcode encountered at position " + instructionPointer.ToString());

            }
            instructionPointer += (explicitInstructionPointerSet) ? 0 : opCodeParameterCount[(OPCODE)opCode];
            opCode = memory.Get(instructionPointer);
            specifiers = "00000".ToCharArray();
            specifierPointer = 0;
            specifiersPreParse = "";
            if (opCode > 99) // it has parameter TYPE specifiers
            {
               specifiersPreParse = opCode.ToString().Substring(0, opCode.ToString().Length - 2);
               specifierPointer = 0;
               for (int x = specifiersPreParse.Length - 1; x >= 0; x--)
               {
                  specifiers[specifierPointer] = specifiersPreParse[x];
                  specifierPointer++;
               }
               opCode = long.Parse(opCode.ToString().Substring(opCode.ToString().Length - 2));
            }
            explicitInstructionPointerSet = false;
         }
         CompyState &= ~STATE.RUNNING;
         CompyState = CompyState | STATE.HALTED;
         if(CompyIsVerbose) Console.WriteLine(CompyName + " has halted program execution.");
      }
   }
   public static class IntCodeCompyExtensions
   {
      public static List<long> Fill (this List<long> memoryBlock, int targetMemoryAddress)
      {
         if (targetMemoryAddress>= memoryBlock.Count)
         {
            List<long> newMemory = new List<long>(new long[targetMemoryAddress-memoryBlock.Count + 1]);
            memoryBlock.AddRange(newMemory);
         }
         return memoryBlock;
      }
      public static long Get(this List<long> memoryBlock, int targetMemoryAddress)
      {
         return memoryBlock.Fill(targetMemoryAddress)[targetMemoryAddress];
      }
      public static void Put(this List<long> memoryBlock, int targetMemoryAddress, long value)
      {
         memoryBlock.Fill(targetMemoryAddress)[targetMemoryAddress] = value;
      }
   }
}
