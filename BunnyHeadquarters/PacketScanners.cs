using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class PacketScanners : AoCodeModule
    {
        public PacketScanners()
        {
            inputFileName = "packetscanners.txt";
            base.GetInput();
        }


        public override void DoProcess()
        {
            Layer.InitAllLayers(Int32.Parse(inputFile.Last().Split(new string[]{": "},StringSplitOptions.None)[0]));
            inputFile.ForEach(line => new Layer(line));
            int totalPicosecondDelay = -1; //gets incremented to zero for first loop;
            Layer.Severity = -1; // gets us 1 loop "free", the Reset puts it back to 0;
            DateTime lastCheckin = DateTime.Now;

            int delay = -1; // init since we want to start at zero, but increment at the beginning of the loop.
            bool didntGetCaughtThisTime = false; //loop entry condition.
            while (!didntGetCaughtThisTime) // so long as my flag says I got caught, let's try again.
            {
                delay++;
                didntGetCaughtThisTime = true; //this loop's flag. Let's assume it'll be all OK!
                for (int i=0;i<=Layer.MaxLayers;i++){ // for each layer, we need to calculate where the marker WOULD be at this number of iterations.
                    if (Layer.allLayers[i]==null) continue; // I set it up to have holes, so... if we are in a hole, it doesn't matter. Get the next layer in the loop.
                    int range = (Layer.allLayers[i].layerScanRange - 1); // the range was input as 1-based, we want zero-based for math and stuff.
                    // two times the total range is "there and back again" -- or, in other words, at some point this layer will return to zero when it cycles 2*range. 
                    // i, being the layer, is the number of steps I walked to get to that layer (from zero, and then add our "delay" iteration ). It would advance once for each of those things. 
                    // so, my steps(i) plus the number we're trying to figure out (delay), gives us a number where this scanner would be when I step in when divided by a full cycle. The leftover is the position and 0 is the bad place.
                    int positionAtThisIteration = (i + delay) % (2 * range); // and we take the leftovers because that would indicate a partial step in the range. 
                    if (positionAtThisIteration == 0) // if this layer would be zero when I step into it, OOPSIE! got caught!
                    {
                        didntGetCaughtThisTime = false; // I GOT CAUGHT, DAMMIT!
                        break; // no need to iterate through any more layers. 
                    }
                }
            }
            FinalOutput.Add("I didn't get caught when we delayed " + delay.ToString() + " picoseconds.");
            //return; // part two exit strategy.
            

            // Tried this brute-force looping. this is WAAAAAY NOT THE WAY TO GO! NO NO NO! The estimates to finish iteration each MILLION were almost a full day after 100,000 iterations. 
            // That would just continue to grow as additional preset delays were added. Since the answer was nearly 4 million, I could see this taking weeks to run. Wow.
            //while (Layer.Severity != 0) // part two BAD strategy loop.
            while (totalPicosecondDelay<0) // part one short-circuit....
            {
                Layer.ResetAllLayers(); // puts all pointers to 0, resets total severity to 0
                totalPicosecondDelay++; // how many loops to let the layer scanners move without us in the firewall.
                for (int x = 0; x < totalPicosecondDelay; x++) { Layer.MoveAllPointers(); }
                int layerPointer = 0;
                while (layerPointer <= Layer.MaxLayers)
                {
                    Layer enteredLayer = Layer.allLayers[layerPointer];
                    if (enteredLayer != null)
                    {
                        if (enteredLayer.layerScanPointer == 0)
                        {
                            enteredLayer.GotCaught();
                            // first entry exception. The condition is that the severity must be greater than zero, which the first layer only gives, so we have to keep going. 
                            // also, for the "first part" of the daily puzzle, we have to process all the layers and sum the entire severity  for no delay. 
                            // after solving that problem, we want to exit the layer loop as soon as we get caught. No reason to keep checking further, we already failed!
                            if (totalPicosecondDelay>0 && layerPointer != 0) { break; }// only prematurely exit without checking all  
                        }
                    }
                    Layer.MoveAllPointers();
                    layerPointer++;
                }
                // this is just so I get updates while it is processing and how long it expects to take. I saw references to that the answer could be in the millions! MILLIONS!
                // the time value to do each 10000 block will just keep going up since the initial conditions have to be set by artificially iterating ALL the scanners a certain number of times. This can't be right.
                if (totalPicosecondDelay % 10000== 0)
                {
                    DateTime thisOne = DateTime.Now;
                    double seconds =((thisOne - lastCheckin).TotalSeconds);
                    lastCheckin = thisOne;
                    double minutes = seconds / 60; //how long did this one take?
                    double estimatehours = ((seconds*100)/60)/ 60; // seconds for 10,000 times 100 would be seconds for a million. More or less. /60 to get minutes /60 to get hours. 
                    // output the "I'm still alive" message."
                    if (minutes < .01) // this will happen on initial launch. The message is wrong and confusing. 
                    {
                        Console.WriteLine(DateTime.Now.ToString() + ": Cranking this up!!");
                    }else 
                    Console.WriteLine(DateTime.Now.ToString() + ": 10000 more took " + minutes.ToString() + " minutes. Current Delay:" + totalPicosecondDelay.ToString() + " ps. Estimate per million: " + estimatehours.ToString());// + 
                }
                // part one output.
                if (totalPicosecondDelay == 0) { FinalOutput.Add("Total Severity for loop 1: " + Layer.Severity.ToString()); }
            } // part two loop. If short-circuit is in place, this is no longer true (totaldelay == 0 first [and only] time through)
            
            
            // nope. This is nonsense. We're not running that monster loop. see MATH answer at top. 
            //FinalOutput.Add("Exited without a severity value after: " + totalPicosecondDelay.ToString() + " ps delay"); // only valid when the part one short circuit isn't in place AND I've got weeks to let this run. 
        }
    }
    public class Layer
    {
        public static Layer[] allLayers;
        public int layerScanPointer = 0;
        public int layerScanRange = 0;
        public int layerDepth = 0;
        public int scanDirection = -1; // init to backwards, since the first MovePointer will reverse it immediately.
        public static int MaxLayers { get { return allLayers.GetUpperBound(0); } }
        public static int Severity = 0;
        public Layer(string definition)
        {
            string[] defs = definition.Split(new string[]{": "},StringSplitOptions.None);
            int layer=Int32.Parse(defs[0]);
            int range = Int32.Parse(defs[1]);
            this.layerScanRange = range;
            this.layerDepth = layer;
            allLayers[layer] = this;
        }
        public static void InitAllLayers(int layerCount)
        {
            allLayers = new Layer[layerCount + 1];
        }
        public static void ResetAllLayers()
        {
            allLayers.ToList().ForEach(x => { if (x != null) { x.layerScanPointer = 0; x.scanDirection = -1; } });
            Severity = 0;
        }
        public static void MoveAllPointers()
        {
            allLayers.ToList().ForEach(x =>{if (x != null) x.MovePointer();});
        }
        public void GotCaught()
        {
            Severity += this.layerDepth * this.layerScanRange;
        }
        public void MovePointer()
        {
            // if the current pointer is zero, we're moving forward again. if it is depth-1, we're now moving back down.
            scanDirection *= ((layerScanPointer==0 || layerScanPointer==(layerScanRange-1))?-1:1);  // multiply by -1 to reverse direction
            layerScanPointer+=scanDirection;
        }
    }
}
