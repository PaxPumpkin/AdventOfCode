using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class MemoryReallocation
    {
        static void Main(string[] args)
        {
            List<MemoryConfiguration> establishedConfigurations = new List<MemoryConfiguration>();
            MemoryConfiguration live; 
            // init the memory configuration MemoryConfiguration({1,2,3,4,5,6....});
            //live = new MemoryConfiguration(new int[] { 0, 2, 7, 0 }); // test/example condition.
            live = new MemoryConfiguration(new int[]{0,5,10,0,11,14,13,4,11,8,8,7,1,4,12,11}); // remember, this is an object pointer and is mutable
            establishedConfigurations.Add(live.Snapshot()); // put our initial condition in the list; it's a new instance to avoid the fact that the object is a POINTER, and MUTABLE when we run redistribute on it.
            live.Redistribute(); // same object as initially, values are now shifted inside. 
            while (establishedConfigurations.Where(item => item.Equals(live)).Count() == 0) // after our redistribution, see if anything in our list matches this. While not matching, keep churning.
            {
                establishedConfigurations.Add(live.Snapshot()); // the live one didn't exist already, so add it to our list as a new object ( a copy )
                live.Redistribute();
            }
            int iterationWhereDuplicateHappenedFirstTime=establishedConfigurations.IndexOf(live);
            Console.WriteLine("Iterations until duplication: " + live.allocationCycles.ToString());
            Console.WriteLine("Index of original duplicate:" + iterationWhereDuplicateHappenedFirstTime.ToString());
            Console.WriteLine("continued iterations until that one comes up again: " + (live.allocationCycles - iterationWhereDuplicateHappenedFirstTime).ToString());
            Console.ReadLine();

        }
    }
    class MemoryConfiguration
    {
        public int[] allocation;
        public int allocationCycles = 0; // keep track of how many times this object has been "redistributed" 
        public MemoryConfiguration()
        {
            allocation = new int[16];// default is 16 for this implementation, 0-15. Primitive array, so all values are initialized as zero. 
        }
        public MemoryConfiguration(int[] input) // unspecified bounds. 4 elements for testing, 16 for running the problem
        {
            if (input.GetUpperBound(0) == 0) throw new Exception("The input array needs an least ONE element"); /// yeah, you're OUTTA HERE dude. 
            allocation = new int[input.GetUpperBound(0)+1]; // eg, a 16 element array for initialization would have an upper bound of 15, but allows for dynamic conditions.
            for (int x = 0; x <= input.GetUpperBound(0); x++) { allocation[x] = input[x]; } // copy values in. These ARE primitives, though, so probably not necessary to go through this. 
        }
        // returns a new object with a copy of the array for comparison later. Avoids the "object pointer in the list" issue. 
        public MemoryConfiguration Snapshot()
        {
            return new MemoryConfiguration(this.allocation); // clone this array of values. We're instantiating a whole new object instead of just storing arrays so we can leverage the list Linq stuff. 
        }
        public void Redistribute()
        {
            // init from first bank.
            int bankWithMax = 0; 
            int maxValue = allocation[0]; //safe since the object cannot be instantiated with a zero-element array. Must be at least one. 
            // find index with highest value in a bank, brute force style.
            for (int q = 1; q <= allocation.GetUpperBound(0); q++) // iterate from 2nd bank to end ( index 1==2nd value). It would fail the test if we initialized with a 1 element array, so no errors in here. 
            {
                int test = Math.Max(maxValue, allocation[q]); // WHICH ONE IS BIGGERER?
                if (test != maxValue) // if it DOES equal max value, we're still keeping the FIRST index we found with that value ( to break the tie, as per the problem spec. )
                {
                    bankWithMax = q; // ok, so now THIS bank is the index with the highest value.
                    maxValue = test; // and this is the value that is currently the highest.
                }
            }
            // initial pointer setup is one more from the maximum value's bank index. loop back to zero if the max was at the last index in the array
            int nextBankPointer = (bankWithMax == allocation.GetUpperBound(0)) ? 0 : (bankWithMax+1); 
            allocation[bankWithMax] = 0; // clear the bank we're redistributing
            while (maxValue > 0) // while we still have memory "blocks" to redistribute...
            {
                allocation[nextBankPointer]++; // add one to value in the bank we're pointing at
                // now move the pointer to the next bank. If we were at the end of the array, loop back to the beginning.
                nextBankPointer = (nextBankPointer == allocation.GetUpperBound(0)) ? 0 : (nextBankPointer + 1); 
                maxValue--; // decrement the total blocks left to redistribute.
            }
            allocationCycles++; // how many times has this object been redistributed since instantiation? One more...
        }
        // makes things easier. Since these are OBJECTs and not VALUEs, the equality of two different configurations is entirely based upon the ordered matching of the values inside their "banks"
        public override bool Equals(object obj)
        {
            MemoryConfiguration mc = (MemoryConfiguration)obj; // cheapo. We won't actually test equality against other object types, and no polymorphism in this situation, so...
            bool equals = true; // assume that we're all nice and equal.
            for (int x = 0; equals && x <= this.allocation.GetUpperBound(0); x++) // iteration through all of THIS object's array values SO LONG AS WE KEEP MATCHING index values for index value. As soon as a mismatch occurs, we'll terminate the loop by flag.
            {
                equals = equals && this.allocation[x] == mc.allocation[x]; // if the same indexed item in the compared object's array doesn't match, the equals flag will flip to false, otherwise stay true.
            }
            return equals;
        }
        // doing this only to get that silly warning to go away. This happened anyway. 
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
