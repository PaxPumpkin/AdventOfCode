using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2021
{
   class Day14_ExtendedPolymerization : AoCodeModule
   {
      public Day14_ExtendedPolymerization()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         //** > Result for Day14_ExtendedPolymerization part 1: Difference Between Most and Least is 2975 (Process: 0.5294 ms)
         //** > Result for Day14_ExtendedPolymerization part 2: Difference Between Most and Least is 3015383850689 (Process: 1.3194 ms)
         ResetProcessTimer(true);
         Polymerizer.SetInput(inputFile);
         Polymerizer.DoTransformation(10);
         AddResult("Difference Between Most and Least is " + Polymerizer.Answer);
         ResetProcessTimer(true);
         Polymerizer.DoTransformation(30); // for total 40 transforms as per puzzle instructions
         AddResult("Difference Between Most and Least is " + Polymerizer.Answer);
         ResetProcessTimer(true);
      }
   }
   public class Polymerizer
   {
      private static string originalPolymerTemplate = "";

      private static Dictionary<string, char> pairTransformationRules = new Dictionary<string, char>();
      private static Dictionary<string, long> pairBuckets = new Dictionary<string, long>();
      private static Dictionary<string, long> elementCounters = new Dictionary<string, long>();
      private static long MostCommon { get { return elementCounters.Max(kvp => kvp.Value); } }
      private static long LeastCommon { get { return elementCounters.Min(kvp => kvp.Value); } }
      public static string Answer { get { return (MostCommon - LeastCommon).ToString(); } }

      public Polymerizer()
      {
         //never instantiated
      }
      public static void SetInput(List<string> allInput)
      {
         Clear();
         allInput.ForEach(line => ParseInput(line));
         SetStartingConditions();
      }
      private static void ParseInput(string line)
      {
         if (line != "")
         {
            if (line.Contains("->"))
            {
               string[] rule = line.Split(new string[] { " -> " }, StringSplitOptions.RemoveEmptyEntries);
               if (pairTransformationRules.ContainsKey(rule[0])) throw new Exception("About to enter a duplicate rule!");
               // for the string pair in the first part of the split, set the first char from the second split string
               pairTransformationRules.Add(rule[0], rule[1][0]);
            }
            else
            {
               originalPolymerTemplate = line;
            }
         }
      }
      private static void Clear()
      {
         pairTransformationRules.Clear();
         pairBuckets.Clear();
         elementCounters.Clear();
      }
      private static void AddOrIncrement(Dictionary<string, long> dictionary, string key, long value = 1)
      {
         if (!dictionary.ContainsKey(key)) dictionary.Add(key, value);
         else dictionary[key] += value;
      }
      private static void AddOrIncrement(Dictionary<string, long> dictionary, char key, long value = 1)
      {
         AddOrIncrement(dictionary, key.ToString(), value);
      }
      private static void SetStartingConditions()
      {
         for (int x = 0; x < originalPolymerTemplate.Length - 1; x++) // will get everything but last single char
         {
            string pair = originalPolymerTemplate.Substring(x, 2);
            string element = originalPolymerTemplate.Substring(x, 1);
            AddOrIncrement(pairBuckets, pair);
            AddOrIncrement(elementCounters, element);

         }
         // count last single char
         AddOrIncrement(elementCounters, originalPolymerTemplate.Substring(originalPolymerTemplate.Length - 1));
      }
      public static void DoTransformation(int thisManyTimes)
      {
         for (int x = 0; x < thisManyTimes; x++)
            MagicTransform();
      }
      private static void MagicTransform() //PaxTech™ Powered
      {
         char transformationInsertion;
         string newFirstPair, newSecondPair;
         Dictionary<string, long> newPairBuckets = new Dictionary<string, long>();
         foreach (KeyValuePair<string, long> pairDefinition in pairBuckets)
         {
            transformationInsertion = pairTransformationRules[pairDefinition.Key]; // char to insert for this pair definition
            newFirstPair = new string(new char[] { pairDefinition.Key[0], transformationInsertion }); //first char in pair, inserted char
            newSecondPair = new string(new char[] { transformationInsertion, pairDefinition.Key[1] }); //inserted char, second char in pair
            AddOrIncrement(newPairBuckets, newFirstPair, pairDefinition.Value);
            AddOrIncrement(newPairBuckets, newSecondPair, pairDefinition.Value);
            AddOrIncrement(elementCounters, transformationInsertion, pairDefinition.Value);
         }
         pairBuckets = newPairBuckets;
      }
   }
}
