using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AoC_2019
{
   class Day07_AmplificationCircuit : AoCodeModule
   {
      public Day07_AmplificationCircuit()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         // ** > Result for Day07_AmplificationCircuit part 1:Highest signal to send to the thrusters is: 880726(Process: 40 ms)
         // ** > Result for Day07_AmplificationCircuit part 2:Highest signal to send to the thrusters is: 4931744(Process: 17452 ms)
         //** > Result for Day07_AmplificationCircuit part 1: Highest signal to send to the thrusters is: 880726 (Process: 11.8262 ms)
         //** > Result for Day07_AmplificationCircuit part 2: Highest signal to send to the thrusters is: 4931744 (Process: 5517.3523 ms)
         // The long time result is from all the printing. But as noted below, the thread synch stuff gets wonky and I haven't tightened it up yet. 
         Print("This *usually* runs the first time, fine. Multiple times can be iffy if verbose mode is off.");
         Print("Unconstrained Threads are causing issues. If it takes longer than 10 seconds, you're stuck for certain.");
         Print("Also, the expected result for part 2 is 4931744 - bad threading sometimes causes it to come back different, too. ");
         ResetProcessTimer(true);
         List<long> intCodeProgram = new List<long>();
         foreach (string processingLine in inputFile)
         {
            string[] programNumbers = processingLine.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string pn in programNumbers)
            {
               intCodeProgram.Add(Convert.ToInt64(pn));
            }
         }
         long maxThrusterOutput = 0;
         List<IntCodeComputer> amplifiers = new List<IntCodeComputer>() { new IntCodeComputer(intCodeProgram), new IntCodeComputer(intCodeProgram), new IntCodeComputer(intCodeProgram), new IntCodeComputer(intCodeProgram), new IntCodeComputer(intCodeProgram) };

         // Range(starting Number, HowMany). 
         IEnumerable<IEnumerable<int>> phaseSetting = GetPermutations(Enumerable.Range(0, 5), 5);

         foreach (IEnumerable<int> setOfSettings in phaseSetting)
         {
            int y = 0;
            foreach (int setting in setOfSettings)
            {
               // y is simultaneously a pointer to the phase settings and the compy to which it goes. 
               amplifiers[y].ResetProgram(); // reloads program fresh, clears all inputs. 
               amplifiers[y].AddPreparedProgramInput(setting.ToString()); // the first input every compy wants is its phase setting, so it's safe to load those now. 
               amplifiers[y].SetOutputWithText(false); // default is for output to explain the value(memory location or "immediate" value, so.... turn that off);
               y++;
            }
            // the only second input we know to start is that the first compy starts with 0. All the others are derived. 
            string inputForNextAmplifier = "0";
            // then we run them one at a time and add the last compy's output as the next compy's input.
            for (int z = 0; z < amplifiers.Count; z++)
            {
               amplifiers[z].AddPreparedProgramInput(inputForNextAmplifier);
               amplifiers[z].RunProgram(); // this is not an async operation at this time, so we can just go without checking any conditions.
               inputForNextAmplifier = amplifiers[z].GetLastProgramOutput();
            }
            long finalOutput = long.Parse(inputForNextAmplifier);
            maxThrusterOutput = maxThrusterOutput > finalOutput ? maxThrusterOutput : finalOutput;
         }

         AddResult("Highest signal to send to the thrusters is: " + maxThrusterOutput.ToString());
         ResetProcessTimer(true);

         phaseSetting = GetPermutations(Enumerable.Range(5, 5), 5);
         maxThrusterOutput = 0;
         long feedbackFinalOutput = 0;
         int phaseCounter = 0;
         foreach (IEnumerable<int> setOfSettings in phaseSetting)
         {
            phaseCounter++;
            //Console.WriteLine("Starting next Phase " + phaseCounter.ToString() + " of " + phaseSetting.Count().ToString());
            amplifiers = new List<IntCodeComputer>() { new IntCodeComputer(intCodeProgram), new IntCodeComputer(intCodeProgram), new IntCodeComputer(intCodeProgram), new IntCodeComputer(intCodeProgram), new IntCodeComputer(intCodeProgram) };
            int y = 0;
            foreach (int setting in setOfSettings)
            {
               // y is simultaneously a pointer to the phase settings and the compy to which it goes. 
               amplifiers[y].ResetProgram(); // reloads program fresh, clears all inputs. 
               amplifiers[y].AddPreparedProgramInput(setting.ToString()); // the first input every compy wants is its phase setting, so it's safe to load those now. 
               amplifiers[y].SetOutputWithText(false); // default is for output to explain the value(memory location or "immediate" value, so.... turn that off);
               y++;
            }

            amplifiers[0].AddPreparedProgramInput("0"); // first amp starts with an initial value of zero. 
            List<Thread> amplifierThreads = new List<Thread>();
            for (int z = 0; z < amplifiers.Count; z++)
            {
               amplifiers[z].CompyName = "Amplifier " + ((char)('A' + z)).ToString();
               amplifiers[z].CompyIndex = z;
               amplifiers[z].CompyOutputRoute = z == amplifiers.Count - 1 ? 0 : z + 1;
               amplifiers[z].SetInputAutomaticWait(true); // instead of auto-assuming that because there is no input waiting, that the user should type it in. Just wait until it arrives.
               amplifiers[z].CompyIsVerbose = false; // setting to False can get synchronization errors. Until that is beefed up, these printing delays help.
               Thread ampThread = new Thread(new ThreadStart(amplifiers[z].RunProgram));
               amplifierThreads.Add(ampThread);
               ampThread.Start();
            }
            long checkingCounter = 0;
            feedbackFinalOutput = 0;
            while (amplifiers.Count(x => x.CompyIsRunning) > 0)
            {
               while (amplifiers.Count(x => x.CompyIsRunning) > 0 && amplifiers.Count(x => x.CompyHasOutputToRead) == 0 && checkingCounter<100001)
               {
                  checkingCounter++;
                  if (checkingCounter % 100000 == 0)
                  {
                     Console.WriteLine("Looped while waiting on a compy " + checkingCounter.ToString() + " times. This is probably BAD NEWS.");
                  }
                  Thread.Yield();
               }
               amplifiers.Where(x => x.CompyHasOutputToRead).OrderBy(x => x.CompyName).ToList().ForEach(x =>
               {
                  if (x.CompyIsDone && x.CompyIndex == amplifiers.Count - 1)
                  {
                     feedbackFinalOutput = long.Parse(x.GetLastProgramOutput());
                  }
                  else
                  {
                     amplifiers.Where(q => q.CompyIndex == x.CompyOutputRoute).First().AddPreparedProgramInput(x.GetLastProgramOutput());
                  }
               });
               checkingCounter++;
               if (checkingCounter % 1000000 == 0)
               {
                  Console.WriteLine("Looped in this segment " + checkingCounter.ToString() + " times. Abort at 1" +
                     " Million");
               }
               if (checkingCounter > 1000000)
               {
                  amplifiers.ForEach(x => x.CompyState = IntCodeComputer.STATE.UNKNOWN);
                  feedbackFinalOutput = -1;
                  Console.WriteLine("Aborting this segment. Hope it wasn't the good one.");
               }
            }
            if (feedbackFinalOutput == 0) feedbackFinalOutput = long.Parse(amplifiers.Last().GetLastProgramOutput());
            maxThrusterOutput = maxThrusterOutput > feedbackFinalOutput ? maxThrusterOutput : feedbackFinalOutput;
            amplifierThreads.ForEach(t => { try { t.Abort(); } catch (Exception ex) {
                  int breakpointLatch = 1;
               } });
            amplifierThreads.ForEach(t => t = null);
            amplifierThreads.Clear();
         }
         AddResult("Highest signal to send to the thrusters is: " + maxThrusterOutput.ToString());
         ResetProcessTimer(true);
      }
      static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
      {
         if (length == 1) return list.Select(t => new T[] { t });

         return GetPermutations(list, length - 1)
             .SelectMany(t => list.Where(e => !t.Contains(e)),
                 (t1, t2) => t1.Concat(new T[] { t2 }));
      }
   }
}
