using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace BunnyHeadquarters
{
    class FancyFlag
    {
        public int woot = 0;
    }
    class DuetThreading : AoCodeModule
    {
        Dictionary<int, List<long>> Queue = new Dictionary<int, List<long>>(); // set this up for potential multi-multi threading.... but really only need two, and they could be explicit. 
        int Process1EnqueueCounter = 0; // part 2 answer is here.
        int Process0EnqueueCounter = 0; // just for funsies
        public Object thingy = new Object(); // lock object for thread safety on the queues.
        public DuetThreading()
        {

        }
        public override void DoProcess()
        {
            FancyFlag flag = new FancyFlag(); // a "Stall Flag" uses binary flags by process id. 
            Queue.Add(0, new List<long>()); // queues for each thread, empty
            Queue.Add(1, new List<long>());
            // added references to shared methods which act on local objects. Could have just passed in the references to both queues instead. 
            Thread process0 = new Thread(() => new Duet(1,Enqueue,Dequeue, flag).DoProcess()); // using IDs 1 and 2 for the binary math. If I had a 3rd one, the id would be 4. Shoulda written that better, but oh well. 
            Thread process1 = new Thread(() => new Duet(2,Enqueue,Dequeue, flag).DoProcess());
            process0.Start();
            process1.Start();

            while ((flag.woot & 3) != 3) // when "waiting" for something to read, the flag will be "OR"-d to have the bit set for the process ID waiting. When it equals 3, they're both waiting. 
            {
                Thread.Sleep(0);
            }
            process0.Abort();
            process1.Abort();
            process0 = null;
            process1 = null;
            Console.WriteLine("Exiting Main");
            FinalOutput.Add("Program 1 enqueued " + Process1EnqueueCounter.ToString() + " times. ");
            FinalOutput.Add("Program 0 enqueued " + Process0EnqueueCounter.ToString() + " times. ");
        }
        public void Enqueue(int processId, long value)
        {
            lock (thingy)// thread safety! If the other process is pulling something out of the queue, we should wait before adding to it. 
            {
                Queue[(processId == 2) ? 0 : 1].Add(value); // my ids are 1 and 2 for binary math in the flags, but they're 0 and 1 in the queue collection. Nurrrrr.
                if (processId == 2) { Process1EnqueueCounter++; } // answer for part 2! how many times did the second program add something to the queue?
                else { Process0EnqueueCounter++; } // just for funsies, how many times did the first do it?
            }
        }
        public Tuple<long,bool> Dequeue(int processId)
        {
            Tuple<long, bool> result = new Tuple<long, bool>(0,false); // init to no real returned value. The queue is empty!
            lock (thingy) // lock before removing from the queue so that the other one isn't trying to add to it at the same time. 
            {
                List<long> queue = Queue[processId == 1 ? 0 : 1]; // get the queue list based upon which process is asking for information. if the ID is 1 ( the first, because the other is 2), then get the first list of longs ( 0 ). Otherwise, get the other ( 1 ).
                if (queue.Count > 0) // if there is actually something to return... 
                {
                    result = new Tuple<long, bool>(queue[0], true); // we got a value, and the flag is true! successful read! ( need a separate flag since 0 is a legit read value. I could have used an object instead of a primitive and returned NULL for no read, but this works. 
                    queue.RemoveAt(0);// remove the value from the queue (at the top).
                }
            }// unlock!
            return result;
        }
    }



    class Duet : AoCodeModule
    {
        FancyFlag myFlagRef; // reference to the "waiting" flag. We'll binary OR and XOR to turn it on and off for our states. 
        Action<int, long> Enqueue; // reference to the enqueue method, instead of a reference to the queues... just a personal preference to see how to do it. I know how to do the other way. 
        Func<int, Tuple<long, bool>> Dequeue; // ref to the Dequeue method. Since it returns a value ( the tuple ), it is a "Func". The Enqueue is a VOID method, so it is an "Action"
        Dictionary<string, long> registers = new Dictionary<string, long>(); // my registers in this thread. 
        public Duet()
        {
            inputFileName = "duet3.txt"; // test data for part 2.
            base.GetInput();
        }
        public Duet(int processId, Action<int,long> QueueCallback, Func<int,Tuple<long,bool>>DeQueueCallback, FancyFlag flag)
        {
            inputFileName = "duet.txt";
            base.GetInput();
            this.ProcessId = processId;
            registers["p"] = processId - 1; // 0 or 1, as per problem setup specification. 
            this.Enqueue = QueueCallback; // method references
            this.Dequeue = DeQueueCallback;
            this.myFlagRef = flag;
        }
        public int ProcessId = -1;
        int rcvCount = 0;
        long lfp = 0;
        bool loopConditionSatisfied = false; // part 1 flag. 
        long lastFrequencyPlayed // not used for part 2. Kinda useless now that part two was coded. 
        {
            get { rcvCount++; if (rcvCount == 1) { 
                FinalOutput.Add("First RCV value: " + lfp.ToString());
                Console.WriteLine("Duet PID:" + ProcessId.ToString() + " first rcv value: " + lfp.ToString());
                loopConditionSatisfied = true; } return lfp; }
            set { lfp = value; }
        }
        public override void DoProcess()
        {
            Console.WriteLine(ProcessId.ToString() + " started");
            //Dictionary<string, long> registers = new Dictionary<string, long>();
            long nextInstruction = 0;
            long instructionActionValue=0; // using LONG ints because that multiplication will cause overflow and bad numbers for regular ints. 
            while (!loopConditionSatisfied && nextInstruction >= 0 && nextInstruction < inputFile.Count)
            {
                string[] instruction = inputFile[(int)nextInstruction].Split(new string[] { " " }, StringSplitOptions.None);
                //Console.WriteLine(ProcessId.ToString() + ": " + inputFile[(int)nextInstruction]);
                //FinalOutput.Add(inputFile[(int)nextInstruction]);
                if (!registers.ContainsKey(instruction[1])){
                    //FinalOutput.Add("We didn't have register " + instruction[1] + " - init to zero.");
                    int test;
                    if (!Int32.TryParse(instruction[1],out test)) // if it parses as a number, it's not a register name and we shouldn't add it as a register. 
                        registers.Add(instruction[1],0);
                }
                if (instruction.Length > 2) // if the instruction has a third part, we need to turn it into a value.
                {
                    if (!Int64.TryParse(instruction[2], out instructionActionValue))
                    {
                        if (!registers.ContainsKey(instruction[2])) // it wasn't a static number. Get the register value if we have it. Init the register if we don't. 
                        {
                            instructionActionValue = 0;
                            registers.Add(instruction[2], 0);
                        }
                        else
                        {
                            instructionActionValue = registers[instruction[2]];
                        }
                    }
                }
                else { instructionActionValue = 0; } // only two pieces, there is no value for the third part. 
                
                switch(instruction[0]){
                    case "snd":
                        int possibleNumber2 = 0;
                        Int32.TryParse(instruction[1], out possibleNumber2); // sending a static number instead of a register value

                        //lastFrequencyPlayed = registers[instruction[1]]; // part 1 stuff, part 2 doesn't use it.
                        Enqueue(ProcessId, (possibleNumber2 > 0) ? possibleNumber2 : registers[instruction[1]]); // send the number if we got one, otherwise send register value.
                        //FinalOutput.Add("Setting last frequency played to " + registers[instruction[1]]); // part 1 stuff. Part 2 doesn't use it this way. 
                        break;
                    case "set":
                        registers[instruction[1]] = instructionActionValue;
                        break;
                    case "add":
                        registers[instruction[1]] = registers[instruction[1]]+instructionActionValue;
                        break;
                    case "mul":
                        registers[instruction[1]] = registers[instruction[1]] * instructionActionValue;
                        break;
                    case "mod":
                        registers[instruction[1]] = (registers[instruction[1]] % instructionActionValue);
                        break;
                    case "rcv":
                        // the lastFrequencyPlayed thing is from part one, and has been essentially dismantled
                        //registers[instruction[1]] = (registers[instruction[1]]==0)?0:lastFrequencyPlayed;
                        //if (registers[instruction[1]] != 0) // this rule was only for part 1. For part two, do it whether the register value is zero or not.
                        {
                            
                            Tuple<long, bool> result;
                            result = Dequeue(ProcessId);
                            
                            this.myFlagRef.woot = this.myFlagRef.woot | ProcessId; // set my "I'm waiting to read" flag on. 
                            while (!result.Item2)// got nothing. 
                            {
                                Thread.Sleep(0);
                                result = Dequeue(ProcessId); // so long as I've got nothing to read in, yield and try to read again. 
                                
                            }
                            this.myFlagRef.woot = this.myFlagRef.woot ^ ProcessId; // XORd out, I'm "back" from trying to read. 
                            registers[instruction[1]] = result.Item1; // set the value. 
                        }
                        break;
                    case "jgz":
                        int possibleNumber = 0;
                        Int32.TryParse(instruction[1], out possibleNumber); // one of the instructions is jgz 1, which isn't a register name but a static value that should always equate to true. This is a debug hack. Should clean it up.
                        
                        if (possibleNumber>0 || registers[instruction[1]] > 0)
                        {
                            nextInstruction = nextInstruction + instructionActionValue;
                        }
                        else { nextInstruction++; }
                        break;
                    default: 
                        Console.WriteLine("instruction: " + instruction[0] + " is unknown.");
                        break;
                }
                if (!instruction[0].Equals("jgz")) nextInstruction++;
                Thread.Yield();// let the other thread play, in case I'm hogging resources.
            }
            Console.WriteLine("Done with Duet PID:" + ProcessId.ToString());
            myFlagRef.woot = myFlagRef.woot | ProcessId; // mark done ( waiting flag, same diff at the moment )
        }
    }
}
