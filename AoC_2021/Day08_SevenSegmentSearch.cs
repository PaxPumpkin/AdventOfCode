using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day08_SevenSegmentSearch : AoCodeModule
   {
      public Day08_SevenSegmentSearch()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         //** > Result for Day08_SevenSegmentSearch part 1: Instances of Unique Patterns 479 (Process: 0.8011 ms)
         //** > Result for Day08_SevenSegmentSearch part 2: Summation of decoded values: 1041746 (Process: 2.9267 ms)
         ResetProcessTimer(true);
         List<SevenSegmentDisplay> displays = new List<SevenSegmentDisplay>();
         foreach (string processingLine in inputFile)
         {
            string[] parsed = processingLine.Split(new char[] { ' ', '|' }, StringSplitOptions.RemoveEmptyEntries);
            string[] signalPatterns = new string[10];
            string[] outputValue = new string[4];
            Array.Copy(parsed, 0, signalPatterns, 0, 10);
            Array.Copy(parsed, 10, outputValue, 0, 4);
            displays.Add(new SevenSegmentDisplay(signalPatterns, outputValue));
         }
         AddResult("Instances of Unique Patterns " + displays.Sum(x => x.CountOfUniquePatterns).ToString());
         ResetProcessTimer(true);
         long result = 0;
         displays.ForEach(x => { x.Decode(); result += x.ResultNumber; });
         AddResult("Summation of decoded values: " + result.ToString());
         ResetProcessTimer(true);
      }
   }
   public class SevenSegmentDisplay
   {
      // patterns with segment counts that uniquely identify them.
      public static int[] UniquePatterns = new int[] { 2, 3, 4, 7 };
      string[] SignalPatterns { get; set; } // input of mixed up segment wiring for all 10 digits
      string[] OutputValue { get; set; } // input of values desired for output by segment wiring indicators
      public int CountOfUniquePatterns { get { return OutputValue.Count(x => UniquePatterns.Contains(x.Length)); } }
      public string DecodedResult { get; set; } // string holder for decoding the OutputValue list.
      public long ResultNumber { get { if (DecodedResult != "") return long.Parse(DecodedResult); else return 0; } } // long value of the decoded string of numbers
      string[] numberPatterns = new string[10]; // place to store the matched up pattern strings to represent digits 0-9
      public SevenSegmentDisplay(string[] patterns, string[] output)
      {
         SignalPatterns = patterns; OutputValue = output;
         // to avoid messy mix-match of patterns later, just set the patterns in alpha order to match up with results later.
         for (int x = 0; x < 4; x++)
         {
            OutputValue[x] = new string(OutputValue[x].OrderBy(y => y).ToArray());
         }
         DecodedResult = ""; //init
      }
      public void Decode()
      {
         // 1 has 2 segments, and is unique
         // 7 has 3 segments, and is unique
         // 4 has 4 segments, and is unique
         // 8 all 7 segments, and is unique
         // 0, 6 and 9 all have 6 segment numbers. so will need to be determined logically.
         // 2, 3, and 5 all have 5 segment numbers, and ditto.


         // set our unique patterns. The index in the array is also the digit the pattern would display.
         numberPatterns[1] = SignalPatterns.Where(x => x.Length == 2).First();
         numberPatterns[4] = SignalPatterns.Where(x => x.Length == 4).First();
         numberPatterns[7] = SignalPatterns.Where(x => x.Length == 3).First();
         numberPatterns[8] = SignalPatterns.Where(x => x.Length == 7).First();

         // Now we have to derive the patterns with 5 and 6 segments from what we can match/exclude.

         // we know 1 and 4's patterns. But can use them together to create a pattern that will pull out the 0 pattern from the mix that includes 6 and 9. 6 and 9 both use the center, but 0 does not.
         List<char> whittle = numberPatterns[4].ToList().Except(numberPatterns[1]).ToList();// leaves us with a pattern of topLeft and Center only now.
         //zero will be the only one in the list where the intersection count==1, 6 and 9 both will intersect at both topLeft and Center, and count would be 2
         numberPatterns[0] = SignalPatterns.Where(x => x.Length == 6 && x.ToList().Intersect(whittle).Count() == 1).First();
         //six will be the one that isn't zero and intersection count with the segments that make up "1" == 1. The nine would count()==2. Then, whichever is not 0 or 6 is 9
         numberPatterns[6] = SignalPatterns.Where(x => x.Length == 6 && x != numberPatterns[0] && x.ToList().Intersect(numberPatterns[1].ToList()).Count() == 1).First();
         numberPatterns[9] = SignalPatterns.Where(x => x.Length == 6 && x != numberPatterns[0] && x != numberPatterns[6]).First();
         //three will be the 5-segment display where the count of intersections with the segments for "1" is 2 (3 has a full right side. 2 and 5 only use 1 segment on the right side)
         numberPatterns[3] = SignalPatterns.Where(x => x.Length == 5 && x.ToList().Intersect(numberPatterns[1].ToList()).Count() == 2).First();
         //two is whichever of the 5 segment displays is not 3 and if we pull out everything that is also matching 3, should leave the bottom left segment (count==1). Then if we intersect that with the segments for "4", there should be no matches. The 5 would match one segment with 4.
         numberPatterns[2] = SignalPatterns.Where(x => x.Length == 5 && x != numberPatterns[3] && x.ToList().Except(numberPatterns[3].ToList()).Intersect(numberPatterns[4]).Count() == 0).First();
         //five is whatever is a 5-segment display pattern that isn't 2 or 3.
         numberPatterns[5] = SignalPatterns.Where(x => x.Length == 5 && x != numberPatterns[2] && x != numberPatterns[3]).First();
         for (int x = 0; x < 10; x++)// put the patterns in alpha order to make matching with the desired output super-simple.
         {
            numberPatterns[x] = new string(numberPatterns[x].OrderBy(y => y).ToArray());
         }
         DecodedResult = ""; // init result string
         List<string> patterns = numberPatterns.ToList(); // List allows list of "IndexOf", Array does not. Index happens to match our Digits. Array can be made to work by looping/breaking when found, but... why?
         for (int x = 0; x < 4; x++) // each display has 4 digits to match.
         {
            DecodedResult += patterns.IndexOf(OutputValue[x]).ToString(); // the index of the matched pattern is the digit that pattern refers to. Append to the string for a Display value.
         }
      }

   }
}
